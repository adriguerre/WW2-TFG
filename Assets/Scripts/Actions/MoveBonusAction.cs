using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MoveBonusAction : BaseAction
{

    private int maxBonusDistance = 6;
    public event EventHandler onStartBonus; 
    public event EventHandler onStopBonus;
    protected Unit targetUnit;
    private Vector3 targetPosition; 
    private State state;
    private float stateTimer;
    private bool canBonus;

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
                if (canBonus)
                {
                    Bonus(targetUnit);
                    canBonus = false;
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
                float coolOffStateTime = 1f;
                stateTimer = coolOffStateTime;
                break;
            case State.Finished:
                onStopBonus?.Invoke(this, EventArgs.Empty);
                state = State.Finished;
                ActionComplete();
                break;
        }
    }

    private void Bonus(Unit unit)
    {
        int actionAdded = 2;
        unit.AddActionPoints(actionAdded);
        onStartBonus?.Invoke(this, EventArgs.Empty);
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.Rotating;
        float rotatingStateTime = 1f;
        canBonus = true;
        stateTimer = rotatingStateTime;
        ActionStart(onActionComplete);
    }
    public override string GetActionName()
    {
        return "Movimiento extra";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxBonusDistance; x <= maxBonusDistance; x++)
        {
            for (int z = -maxBonusDistance; z <= maxBonusDistance; z++)
            {
                GridPosition offSetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offSetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                    || !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxBonusDistance) //Esto es para hacer que no sea un cuadrado, en vez de X X es X   y asi progresivamente
                {                                                                                       // X X    X X 
                    continue;
                }


                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                targetPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                if (targetUnit.IsEnemy() != unit.IsEnemy())
                {
                    continue; //Vamos a skipear si el target.isEnemy es el mismo que el unit.isEnemy, es decir si estan en equipos diferentes
                }

                validGridPositionList.Add(testGridPosition);

            }
        }


        return validGridPositionList;
    }


    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        enemyAIAction.gridPosition = gridPosition;
        int targetCountAtGridPosition = 0;
        int friendlyUnitsAround = unit.GetAction<MoveAction>().GetFriendlyUnitsAround(gridPosition).Count;


        if (unit.GetAction<ShootAction>() != null)
        {
            targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        }

        switch (diff)
        {
            case Difficult.Easy:
                //En facil y normal la IA no tendrá en cuenta que se encuentra sin cobertura
                if (calculateChanceNoHelp(20f))
                    enemyAIAction.actionValue = 30*targetCountAtGridPosition;  //Calculo aun por hacer
                break;
            case Difficult.Medium:
                if (calculateChanceNoHelp(30f))
                    enemyAIAction.actionValue = 30 * targetCountAtGridPosition;
                break;
            case Difficult.Hard:
                enemyAIAction.actionValue = 30 * targetCountAtGridPosition;
                break;
        }

        return enemyAIAction;
    }

    public int GetMaxBonusDistance()
    {
        return maxBonusDistance;
    }
}

