using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ReviveAction : BaseAction
{

    public event EventHandler onStartReviving; //Animaciones para curar
    public event EventHandler onStopReviving;
    protected Unit targetUnit;
    private int healAmount = 20;
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

        switch (state)
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
                    Revive(targetHealthSystem, targetUnit);
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
                onStopReviving?.Invoke(this, EventArgs.Empty);
                state = State.Finished;
                ActionComplete();
                break;
        }
    }


    private void Revive(HealthSystem targetHealthSystem, Unit unit)
    {
        
        targetHealthSystem.Revive(healAmount, unit);
        onStartReviving?.Invoke(this, EventArgs.Empty);

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = UnitManager.Instance.GetWoundedUnitAtGridPosition(gridPosition);
        state = State.Rotating;
        float rotatingStateTime = 1f;
        canHeal = true;
        stateTimer = rotatingStateTime;
        ActionStart(onActionComplete);
    }
    public override string GetActionName()
    {
        return "Revivir";
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }


    //Este segundo metodo de GetValid es para poder ver los target disponibles desde una posicion concreta
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

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    try
                    {
                        //En este caso lo que tenemos que hacer es con la posicion testGridPosition, comprobar si hay una unidad en el diccionario de heridos
                        //if no hay muerto 
                        // continue
                       
                        if(UnitManager.Instance.GetWoundedUnitAtGridPosition(testGridPosition) == null)
                        {
                            continue;
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.LogError("ERROR: " + e);
                    }
                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                    targetPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);

                    
                    validGridPositionList.Add(testGridPosition);

                }
            }
        }
        return validGridPositionList;
    }

    public int GetReviveCountAtGridPosition(GridPosition gridPosition) //Devuelve el numero total de enemigos a la "vista"
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {

        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 50,
        };
    }
}
