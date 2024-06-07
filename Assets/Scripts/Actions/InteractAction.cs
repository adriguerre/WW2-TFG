using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{

    private int maxInteractDistance = 1;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

    }
    public override string GetActionName()
    {
        return "Interactuar";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 50, 
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();


        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {

            
                GridPosition offSetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offSetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }


                IInteractable interactable  = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if(interactable == null)
                {
                    //No interactable on this gridPosition 
                    continue;
                }
                ObjetivoInteractuable objec = (ObjetivoInteractuable)LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);

                if (objec.getIsCompleted())
                {
                    Debug.Log(objec);
                    continue;
                }

                validGridPositionList.Add(testGridPosition);

            }
        }


        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractComplete);
        ActionStart(onActionComplete);
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }
}
