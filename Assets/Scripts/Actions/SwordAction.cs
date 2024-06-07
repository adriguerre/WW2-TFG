using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;
    public static event EventHandler onAnySwordHit;


    private int maxSwordDistance = 1;
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private enum State
    {
        SwingingSwordBeforeHit, 
        SwingingSwordAfterHit,
    }



    private void Update()
    {
        if (!isActive)
        {
            return; 
        }

        stateTimer -= Time.deltaTime; //Tiempo del giro
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 5f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:

                break;
        }

        if (stateTimer <= 0f) //Cuando se acabe el tiempo, cambiamos de estado
        {
            NextState();
        }
    }


    private void NextState()
    {
        switch (state)
        {
            case State.SwingingSwordBeforeHit:  //En este estado, ponemos un tiempo y hacemos el daño
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.1f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                onAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }


    public override string GetActionName()
    {
        return "Cuchillo";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        return new EnemyAIAction{
            gridPosition = gridPosition, 
            actionValue = 200, //Siempre que pueda acuchillar, lo hará 
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();


        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offSetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offSetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                     || !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue; //Vamos a skipear si el target.isEnemy es el mismo que el unit.isEnemy, es decir si estan en equipos diferentes
                }
                validGridPositionList.Add(testGridPosition);

            }
        }


        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }


    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }
}
