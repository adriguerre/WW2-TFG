
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAction : BaseAction
{


    public event EventHandler onStartPlacing; //Animaciones para poner la bomba
    public event EventHandler onStopPlacing;
    [SerializeField] private Transform tntPrefab;
    protected Unit targetUnit;
    private Vector3 targetPosition;
    private GridPosition targetGridPosition; //Aliado al que curar
    private State state;
    private float stateTimer;
    public bool bombPlaced = false;
    private bool notExploded = true;
    private TNTObject bomb; 
    private enum State
    {
        Rotating,
        Placing,
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
              
                Vector3 moveDirection = (targetPosition - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Placing:

                if (notExploded)
                {
                    if (bombPlaced)
                    {
                        //Explota
                        ExplodeBomb();
                        bombPlaced = false;
                    }
                    else
                    {
                        //Colocamos la bomba
                        PlaceBomb(targetGridPosition);
                        bombPlaced = true;
                    }
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
                state = State.Placing;
                float rotatingStateTime = 0.1f;
                stateTimer = rotatingStateTime;
                break;
            case State.Placing:
                state = State.Finished;
                float coolOffStateTime = 2f;
                stateTimer = coolOffStateTime;
                break;
            case State.Finished:
                onStopPlacing?.Invoke(this, EventArgs.Empty);
                state = State.Finished;
                ActionComplete();
                break;
        }
    }


    private void PlaceBomb(GridPosition gridPosition)
    {
        Debug.Log("Bomba Colocada");
        notExploded = false;
        onStartPlacing?.Invoke(this, EventArgs.Empty);
    }

    private void ExplodeBomb()
    {
        bomb.Explode();

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetGridPosition = gridPosition;
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        state = State.Rotating;
        float rotatingStateTime = 1f;
        stateTimer = rotatingStateTime;

        Quaternion quaternion = Quaternion.Euler(-90, 0, 0);
        Transform tntPrefabTransform = Instantiate(tntPrefab, targetPosition, quaternion);
        bomb = tntPrefabTransform.GetComponent<TNTObject>();
        bomb.Setup(gridPosition, OnTNTBehaviourComplete);


        ActionStart(onActionComplete);
    }


    private void OnTNTBehaviourComplete()
    {
        ActionComplete();
    }

    public override string GetActionName()
    {
            return "Explosive";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
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

                    validGridPositionList.Add(testGridPosition);

                }
            }
        }
        return validGridPositionList;
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
