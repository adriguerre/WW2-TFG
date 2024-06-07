using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    public event EventHandler onGrenadeLaunched;

    

    [SerializeField] private Transform grenadeProjectilePrefab;
    private int maxThrowDistance = 7;
    private int grenadesLeft = 2; 
    private GridPosition actualGridPosition;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
       
       
    }

    public void SetGrenades(int aux)
    {
        this.grenadesLeft = aux;
    }

    public override string GetActionName()
    {
        return "Granada";
    }
    public override int GetActionPointsCost()
    {
        if (unit.GetActionPoints() == 0)
            return 1000;
        else
            return unit.GetActionPoints();
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        enemyAIAction.gridPosition = gridPosition;

        if (grenadesLeft > 0)
        {
            int enemyCount = GetEnemyCountHittedByGrenades(gridPosition);
            int v = 0;
            switch (diff)
            {
                case Difficult.Easy:
                    if (calculateChanceNoHelp(15f))
                    {
                       v = 0 + (enemyCount * 25);
                       enemyAIAction.actionValue = v;
                    }
                    break;
                case Difficult.Medium:
                    if (calculateChanceNoHelp(30f))
                    {
                        v = 0 + (enemyCount * 30);
                        enemyAIAction.actionValue = v;
                    }
                    break;
                case Difficult.Hard:
                    v = 0 + (enemyCount * 30);
                    enemyAIAction.actionValue = v;
                    //Aqui habría que tener en cuenta, si podemos tirar granada y ademas matar a alguien
                    break;
            }
        }
        else
        {
            enemyAIAction.actionValue = -100;
        }




        //Esta acción dependerá del numero de enemigos que se encuentren al alcance de granadas. 
        return enemyAIAction;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        if (unit.GetActionPoints() > 0 || grenadesLeft > 0)
        {
            for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
            {
                for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
                {
                    GridPosition offSetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offSetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxThrowDistance) //Esto es para hacer que no sea un cuadrado, en vez de X X es X   y asi progresivamente
                    {                                                                                       // X X    X X 
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);

                }
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        onGrenadeLaunched?.Invoke(this, EventArgs.Empty);
        float counter = 0;

        float waitTime = 8; 
        while(counter < waitTime)
        {
            Vector3 aimDirection = (LevelGrid.Instance.GetWorldPosition(gridPosition) - unit.GetWorldPosition()).normalized;
            float rotateSpeed = 5f;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);

            counter += Time.deltaTime;

        }
        grenadesLeft--;
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);
        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }
    public int GetEnemyCountHittedByGrenades(GridPosition gridPosition)
    {
        //Checkeamos a los objetos/enemigos que podría dar
        int count = 0;

        Vector3 hitBoxSize = new Vector3(3, 3, 3);
        Vector3 targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        
        Collider[] colliderArray = Physics.OverlapBox(targetPosition, hitBoxSize);//Para saber que enemigos hay dentro del radio de la granada. 
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<Unit>(out Unit targerUnit))
            {
                if(!targerUnit.IsEnemy())
                    count++;
            }
        }

        return count;
    }

    public int GetGrenadesLeft()
    {
        return grenadesLeft;
    }
}
