using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ReloadAction : BaseAction
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



        //Si no tiene balas tiene que recargar si o si
        if(unit.GetComponent<ShootAction>().GetRemainingBullets() == 0)
            enemyAIAction.actionValue = 150;
        else
            enemyAIAction.actionValue = 0;


        //Esta acción dependerá del numero de enemigos que se encuentren al alcance de granadas. 
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
}
