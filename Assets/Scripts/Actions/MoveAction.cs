using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MoveAction : BaseAction
{
    

    [SerializeField] private int maxMoveDistance = 4;

    private bool hasOfficerBuff;
    public event EventHandler onStartMoving;
    public event EventHandler onStopMoving;
    //public event EventHandler onStartRoll;
    private List<Vector3> positionList;
    private int currentPositionIndex;

    private float moveSpeed = 6.5f;
    private const float normalMovespeed = 6.5f; 

    // Update is called once per frame
    void Update()
    {

        if (!isActive)
        {
            return;
        }
        Vector3 rotatio = new Vector3(0, transform.rotation.y, transform.rotation.z);
        transform.Rotate(rotatio);
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

   

        //Rotate
        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        //Move
        float stoppingDistance = .1f;
        //float stoppingDistanceForRoll = 1f;
        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
        {
            /*
             if(Vector3.Distance(targetPosition, transform.position) <= stoppingDistanceForRoll)
            {
                onStartRoll?.Invoke(this, EventArgs.Empty);          
            }
            */
            if (unit.GetIsAlarmed())
            {
                moveSpeed = normalMovespeed;
            }
            else
            {
                moveSpeed = 3f;
                
            }
         
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if(currentPositionIndex >= positionList.Count)
            {
                //No more positions to follow
                onStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
        
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        currentPositionIndex = 0;
        positionList = new List<Vector3>(); //Lista de posicion, pero son posicion del mundo, por tanto son vector3

        foreach(GridPosition pathgridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathgridPosition));
        }
       
        onStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }


    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        
        if(unit.GetActionPoints() > 0)
        {
            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                   
                    GridPosition offSetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offSetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                        || unitGridPosition == testGridPosition
                        || LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }
                   
                    //Hay que checkear si es Walkable o no
                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    //Si tiene algun camino para llegar alli
                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                    {
                        continue;
                    }
                    int pathFindingDistanceMultiplier = 10;
                    if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathFindingDistanceMultiplier)
                    {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }

        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Avanzar";
    }

    //Puntuación que tiene el move action para los enemigos
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        enemyAIAction.gridPosition = gridPosition;

        switch (diff)
        {
            case Difficult.Easy:
                //En facil y normal la IA no tendrá en cuenta que se encuentra sin cobertura
                enemyAIAction.actionValue = CalculateValueEasy(gridPosition);
                break;
            case Difficult.Medium:
                enemyAIAction.actionValue = CalculateValueMediumHard(gridPosition);
                break;
            case Difficult.Hard:
                enemyAIAction.actionValue = CalculateValueMediumHard(gridPosition);
                break;
        }

       // Debug.Log("MOVE: " + enemyAIAction);
        return enemyAIAction;
    }


    private int CalculateValueEasy(GridPosition gridPosition)
    {


        int targetCountAtGridPosition;
        int value = 0; 


        if (unit.GetAction<ShootAction>() != null)
            targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        else
            targetCountAtGridPosition = 0;

        

        if (targetCountAtGridPosition > 0 && unit.GetCoverType() == CoverType.None) //Si hay enemigos cerca
        {
            switch (LevelGrid.Instance.GetUnitCoverType(gridPosition))
            {
                case CoverType.None:
                    value = 20;
                    break; 
                case CoverType.Half:
                    value = 65;
                    break;
                case CoverType.Full:
                    value = 65;
                    break;
            }        
        }
        else //Si no hay enemigos cerca, nos movemos en su dirección
        { 
            Unit unidadMasCercana = getUnitWithLessDistance(gridPosition);
            int distance = GetDistanceWithEnemyUnit(gridPosition, unidadMasCercana.GetGridPosition());
            value = 200 / distance;
        }
        Debug.Log("Target en " + gridPosition + " es " + value);
        return value;
    }


    private Unit getUnitWithLessDistance(GridPosition gridPosition)
    {
        Unit returnUnit = null;
        int minorDistance = 100; 
        foreach(Unit unit in UnitManager.Instance.GetFriendlyUnitList())
        {
            int auxDistance = GetDistanceWithEnemyUnit(gridPosition, unit.GetGridPosition());
            if(minorDistance >= auxDistance)
            {
                returnUnit = unit;
            }
        }
        return returnUnit;
    }

    private int GetDistanceWithEnemyUnit(GridPosition positionCheck, GridPosition gridPosition)
    {
        GridPosition gridPositionDistance = positionCheck - gridPosition;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return Mathf.Min(xDistance, zDistance) + remaining;
    }



    private int CalculateValueMediumHard(GridPosition gridPosition)
    {
        int targetCountAtGridPosition;
        int value = 0;


        if (unit.GetAction<ShootAction>() != null)
            targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        else
            targetCountAtGridPosition = 0;



        if (targetCountAtGridPosition > 0 && unit.GetCoverType() == CoverType.None) //Si hay enemigos cerca
        {
            switch (LevelGrid.Instance.GetUnitCoverType(gridPosition))
            {
                case CoverType.None:
                    value = 20;
                    break;
                case CoverType.Half:
                    value =70;
                    break;
                case CoverType.Full:
                    value = 75;
                    break;
            }
        }
        else //Si no hay enemigos cerca, nos movemos en su dirección
        {
            Unit unidadMasCercana = getUnitWithLessDistance(gridPosition);
            int distance = GetDistanceWithEnemyUnit(gridPosition, unidadMasCercana.GetGridPosition());
            value = 200 / distance;
        }

        //Chequeamos si es medico, si hay sitio donde curar
        if (unit.GetAction<HealAction>() != null)
        {
            int healTargetCountAtGridPosition = unit.GetAction<HealAction>().GetTargetCountAtPosition(gridPosition);
            List<GridPosition> gridPositionList = unit.GetAction<HealAction>().GetWoundedGridPositionList(gridPosition);
            Unit moreWoundedUnit = null;
            //Saco el que tenga menor vida de los que haya encontrado 
            if (gridPositionList.Count >= 1)
            {
                moreWoundedUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPositionList[0]);
                for (int i = 0; i < gridPositionList.Count; i++)
                {
                    Unit aux = LevelGrid.Instance.GetUnitAtGridPosition(gridPositionList[i]);

                    if (aux.GetHealthSystem().GetUnitHealth() < moreWoundedUnit.GetHealthSystem().GetUnitHealth())
                    {
                        moreWoundedUnit = aux;
                    }
                }
            }
            if (healTargetCountAtGridPosition > 0)
            {
                value = 100 - moreWoundedUnit.GetHealthSystem().GetUnitHealth();
            }
        }
        return value;
    }
    public List<GridPosition> GetFriendlyUnitsAround(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        if (unit.GetActionPoints() > 0)
        {
            for (int x = unitGridPosition.x - 1; x <= unitGridPosition.x + 1; x++)
            {
                for (int z = unitGridPosition.z - 1; z <= unitGridPosition.z + 1; z++)
                {
                    GridPosition testGridPosition = new GridPosition(x, z);

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                        || !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    Unit unitInGridPosition;
                    HealthSystem testHealthSystem;
                    try
                    {
                        unitInGridPosition = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                        testHealthSystem = unitInGridPosition.GetHealthSystem();

                        if (testHealthSystem.GetUnitHealth() == testHealthSystem.GetMaxUnitHealth())
                        {
                            continue;
                        }
                        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                        if (targetUnit.IsEnemy() != unit.IsEnemy())//Queremos solos los territorios que hay una unidad aliada
                        {
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("ERROR: " + e);
                    }


                    validGridPositionList.Add(testGridPosition);

                }
            }
        }
        return validGridPositionList;
    }


    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float move)
    {
        this.moveSpeed = move; 
    }

    public float GetNormalMoveSpeed()
    {
        return normalMovespeed;
    }

    public int GetMaxMoveDistance()
    {
        return maxMoveDistance;
    }

    public void SetMaxMoveDistance(int max)
    {
        this.maxMoveDistance = max; 
    }
   }
