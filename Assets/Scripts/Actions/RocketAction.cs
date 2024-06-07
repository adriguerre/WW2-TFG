using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RocketAction : BaseAction
{

    public event EventHandler onRocketLaunched;

    [SerializeField] private Transform rocketProjectilePrefab;
    private int maxThrowDistance = 10;
    private bool isLoaded = true;
    private GridPosition actualGridPosition;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }


    }
    public override int GetActionPointsCost()
    {
        if (unit.GetActionPoints() == 0)
            return 1000;
        else
            return unit.GetActionPoints();
    }
    private void Start()
    {
       

        if (TryGetComponent<ReloadRocketAction>(out ReloadRocketAction reloadRocketAction))
        {
            reloadRocketAction.onStartReloading += reloadRocketAction_OnStartReloading;
        }


    }

    private void reloadRocketAction_OnStartReloading(object sender, EventArgs e)
    {
        isLoaded = true;
    }

    public override string GetActionName()
    {
        return "Cohete";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        enemyAIAction.gridPosition = gridPosition;

        //No habrá unidades de cohetes en dificultades faciles 
        if (isLoaded)
        {
            int enemyCount = GetEnemyCountHittedByRocket(gridPosition);
            int v = 0 + (enemyCount * 25);
            enemyAIAction.actionValue = v;
        }
        else
        {
            enemyAIAction.actionValue = -100;
        }


        //Esta acción dependerá del numero de enemigos que se encuentren al alcance de granadas. 
        return enemyAIAction;
    }


    public int GetEnemyCountHittedByRocket(GridPosition gridPosition)
    {
        //Checkeamos a los objetos/enemigos que podría dar
        int count = 0;

        Vector3 hitBoxSize = new Vector3(3, 3, 3);
        Vector3 targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        Collider[] colliderArray = Physics.OverlapBox(targetPosition, hitBoxSize);//Para saber que enemigos hay dentro del radio del misil 
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<Unit>(out Unit targerUnit))
            {
                if (!targerUnit.IsEnemy())
                    count++;
            }
        }
        return count;
    }



    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        if (unit.GetActionPoints() > 0 && isLoaded)
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
        onRocketLaunched?.Invoke(this, EventArgs.Empty);
        float counter = 0;
        Vector3 aimDirection = (LevelGrid.Instance.GetWorldPosition(gridPosition) - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 5f;
        float waitTime = 8;
        while (counter < waitTime)
        {
            aimDirection = (LevelGrid.Instance.GetWorldPosition(gridPosition) - unit.GetWorldPosition()).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
            counter += Time.deltaTime;
        }
        isLoaded = false;
        Quaternion quaternion = Quaternion.LookRotation((LevelGrid.Instance.GetWorldPosition(gridPosition) - unit.GetWorldPosition()).normalized);
        Transform rocketProjectileTransform = Instantiate(rocketProjectilePrefab, unit.GetWorldPosition(), quaternion);
        RocketProjectile rocketProjectile = rocketProjectileTransform.GetComponent<RocketProjectile>();
        rocketProjectile.Setup(gridPosition, OnRocketBehaviourComplete);
        ActionStart(onActionComplete);
    }

    private void OnRocketBehaviourComplete()
    {
        ActionComplete();
    }

    public bool GetIsLoaded()
    {
        return isLoaded;
    }

    public int GetMaxDistance()
    {
        return maxThrowDistance;
    }
}
