using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpinAction : BaseAction
{
    private float totalSpinAmount;


    //Con el namespace de System, podemos hacer lo que hacemos en estas dos lineas en una sola.
    //public delegate void SpinCompleteDelegate();
    //private SpinCompleteDelegate onSpinComplete;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per fram
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        totalSpinAmount += spinAddAmount;
        if (totalSpinAmount >= 360f)
        {
            ActionComplete();
        }

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
   
        totalSpinAmount = 0f;
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }


    public override int GetActionPointsCost()
    {
        return 2;
    }

    //Puntuación que tiene el spin action
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
}
