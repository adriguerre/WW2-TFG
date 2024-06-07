using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : BaseAction { 


    public event EventHandler onStartHealing; //Animaciones para curar
    public event EventHandler onStopHealing;
    protected Unit targetUnit;
    private int healAmount = 40;
    private Vector3 targetPosition; //Aliado al que curar
    private State state;
    private float stateTimer;
    private bool canHeal;


    private enum State
    {
        Rotating, 
        Healing, 
        Finished,
    }
    private void Update()
    {

        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime; 

        switch(state)
        {
            case State.Rotating:
                Vector3 moveDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

                break;
            case State.Healing:
                if (canHeal)
                {
                    HealthSystem targetHealthSystem = targetUnit.GetHealthSystem();
                    Heal(targetHealthSystem, healAmount);
                    canHeal = false;
                }
      
                break;
            case State.Finished:
                break;
        }

        if (stateTimer <= 0)
        {
            NextState();
        }

    }

    private void NextState()
    {
        switch (state)
        {
            case State.Rotating:
                state = State.Healing;
                float rotatingStateTime = 0.1f;
                stateTimer = rotatingStateTime;
                break;
            case State.Healing:
                state = State.Finished;
                float coolOffStateTime = 2f;
                stateTimer = coolOffStateTime;
                break;
            case State.Finished:
                onStopHealing?.Invoke(this, EventArgs.Empty);
                state = State.Finished;
                ActionComplete();
                break;
        }
    }


    private void Heal(HealthSystem targetHealthSystem, int healAmount)
    {
        Debug.Log("Time");
        targetHealthSystem.Heal(healAmount);        
        onStartHealing?.Invoke(this, EventArgs.Empty);

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.Rotating;
        float rotatingStateTime = 1f;
        canHeal = true;
        stateTimer = rotatingStateTime;
        ActionStart(onActionComplete);
    }
    public override string GetActionName()
    {
        return "Heal";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        if (unit.GetActionPoints() > 0)
        {
            for (int x = unitGridPosition.x - 1; x <= unitGridPosition.x + 1; x++)
            {
                for (int z = unitGridPosition.z - 1; z <= unitGridPosition.z + 1; z++)
                {
                    GridPosition testGridPosition = new GridPosition(x, z);

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                        || !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    Unit unitInGridPosition;
                    HealthSystem testHealthSystem;
                    try
                    {
                        unitInGridPosition = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                        testHealthSystem = unitInGridPosition.GetHealthSystem();

                        if (testHealthSystem.GetUnitHealth() == testHealthSystem.GetMaxUnitHealth())
                        {
                            continue;
                        }
                        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                        targetPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);

                        if (targetUnit.IsEnemy() != unit.IsEnemy())//Queremos solos los territorios que hay una unidad aliada
                        {
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("ERROR: " + e);
                    }
 

                    validGridPositionList.Add(testGridPosition);

                }
            }
        }
        return validGridPositionList;
    }



    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        enemyAIAction.gridPosition = gridPosition;

        int healTargetCountAtGridPosition = unit.GetAction<HealAction>().GetTargetCountAtPosition(gridPosition);
        List<GridPosition> gridPositionList = unit.GetAction<HealAction>().GetWoundedGridPositionList(gridPosition);
        Unit moreWoundedUnit = null;
        //Saco el que tenga menor vida de los que haya encontrado 
        if (gridPositionList.Count >= 1)
        {
            moreWoundedUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPositionList[0]);
            for (int i = 0; i < gridPositionList.Count; i++)
            {
                Unit aux = LevelGrid.Instance.GetUnitAtGridPosition(gridPositionList[i]);

                if (aux.GetHealthSystem().GetUnitHealth() < moreWoundedUnit.GetHealthSystem().GetUnitHealth())
                {
                    moreWoundedUnit = aux;
                }
            }
        }

        if (healTargetCountAtGridPosition > 0)
        {
            enemyAIAction.actionValue = 100 - moreWoundedUnit.GetHealthSystem().GetUnitHealth() + 5;
        }
    
        return enemyAIAction;
    }
    public int GetTargetCountAtPosition(GridPosition gridPosition) //Devuelve el numero total de enemigos a la "vista"
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

    public List<GridPosition> GetWoundedGridPositionList(GridPosition gridPosition) 
    {
        return GetValidActionGridPositionList(gridPosition);
    }
}
