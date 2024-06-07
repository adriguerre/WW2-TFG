using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ReloadRocketAction : BaseAction
{

    public event EventHandler onStartReloading; //Animaciones para curar
    public event EventHandler onStopReloading;
    protected Unit targetUnit;


    private State state;
    private float stateTimer;



    private enum State
    {
        Waiting,
        Loading,
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
            case State.Waiting:
                break;
            case State.Loading:
                Reload();
                break;
            case State.Finished:
                break;
        }

        if (stateTimer <= 0)
        {
            NextState();
        }

    }

    private void Reload()
    {
        //Sumar las balas, si lo hago con el evento, primero actualiza y luego suma la bala
        onStartReloading?.Invoke(this, EventArgs.Empty);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Waiting:
                state = State.Loading;
                float rotatingStateTime = 0.1f;
                stateTimer = rotatingStateTime;
                break;
            case State.Loading:
                state = State.Finished;
                float coolOffStateTime = 2f;
                stateTimer = coolOffStateTime;
                break;
            case State.Finished:
                onStopReloading?.Invoke(this, EventArgs.Empty);
                state = State.Finished;
                ActionComplete();
                break;
        }
    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.Waiting;
        float waitingStateTime = 1f;
        stateTimer = waitingStateTime;
        onStartReloading?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }
    public override string GetActionName()
    {
        return "Recargar";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        enemyAIAction.gridPosition = gridPosition;

        int maxDistance = unit.GetAction<RocketAction>().GetMaxDistance();
        bool isLoaded = unit.GetAction<RocketAction>().GetIsLoaded();

        if (!isLoaded)
        {
            int enemyCount = SpotEnemiesNearBy(gridPosition, maxDistance);
            int v = 100 - (enemyCount * 40);
            enemyAIAction.actionValue = v;
        }
        else
        {
            enemyAIAction.actionValue = -100;
        }


        //Esta acción dependerá del numero de enemigos que se encuentren al alcance, si hay muchos enemigos nos renta mas movernos en vez de recargar
        return enemyAIAction;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition>
        {
            unitGridPosition,
        };
    }

    public int SpotEnemiesNearBy(GridPosition unitGridPosition, int maxDistance)
    {
        int count = 0;
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        if (unit.GetActionPoints() > 0)
        {
            for (int x = -maxDistance; x <= maxDistance; x++)
            {
                for (int z = -maxDistance; z <= maxDistance; z++)
                {
                    GridPosition offSetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offSetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                        || !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxDistance) //Esto es para hacer que no sea un cuadrado, en vez de X X es X   y asi progresivamente
                    {                                                                                       // X X    X X 
                        continue;
                    }


                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                    if (targetUnit.IsEnemy() == unit.IsEnemy())
                    {
                        continue; //Vamos a skipear si el target.isEnemy es el mismo que el unit.isEnemy, es decir si estan en equipos diferentes
                    }
                    count++;

                }
            }
        }

        return count;
    }
}
