using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public abstract class BaseAction : MonoBehaviour
{

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public virtual Material GetActionImage()
    {
        Material material = Resources.Load<Material>("./Materials/Calavera");
        Debug.Log(material);
        return material;
    }

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual int GetActionPointsCost() //Se podría hacer abstract, y que cada action tuviese su coste
    {
        //Con el virtual, lo que hacemos es que todo gasta 1, pero si haces override gasta lo que pongas en el metodo que crees
        return 1;
    }

    protected virtual bool calculateChanceNoHelp(float hitChance)
    {
        float chance = UnityEngine.Random.Range(0f, 1f);
        return chance < hitChance;
    }

    protected void ActionStart(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        isActive = true;
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public virtual bool IsActive()
    {
        return isActive;
    }
    public EnemyAIAction GetBestEnemyAIAction(Difficult diff)
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach(GridPosition gridPosition in validActionGridPositionList) //Buscamos la mejor opción entre todas las posibles posiciones, y por cada posicion vemos el puntuaje que tenga
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition, diff);
            enemyAIActionList.Add(enemyAIAction);
        }

        if(enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue); //Para coger el mejor
            return enemyAIActionList[0];
        }
        else
        {
            
            //No possible AI Actions
            return null;
        }

    }


    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff);
}
