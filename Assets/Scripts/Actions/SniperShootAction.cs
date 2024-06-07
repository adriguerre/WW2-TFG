using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SniperShootAction : BaseAction
{

    [SerializeField] private LayerMask obstacleLayerMask;

    private float totalSpinAmount;
    private int maxShootDistance = 10;
    private int minDamageFromSniper = 39;
    private int maxDamageFromSniper = 50;
    private Unit targetUnit;
    private bool canShootBullet;

    public static event EventHandler<OnShootEventArgs> onAnyShoot;
    public event EventHandler<OnShootEventArgs> onSniperShoot;

    public event EventHandler<Unit> onAnyMissShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }


    private float stateTimer;
    private State state;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 5f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShootBullet)
                {

                    float hitChance = GetHitChance(targetUnit);

                    onSniperShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit });
                    onAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit });

                    if (calculateChance(hitChance))
                    {
                        unit.SetHitHelper(0);
                        if (unit.IsEnemy())
                        {
                            switch (EnemyAI.Instance.GetDifficult())
                            {
                                case Difficult.Easy:
                                    targetUnit.SetShieldHelper(targetUnit.GetShieldHelper() + 10);
                                    targetUnit.StartRecordTurn();
                                    break;
                                case Difficult.Medium:
                                    targetUnit.SetShieldHelper(targetUnit.GetShieldHelper() + 5);
                                    targetUnit.StartRecordTurn();
                                    break;
                            }
                        }
                        Shoot();
                    }
                    else
                    {
                        if (!unit.IsEnemy())
                        {
                            switch (EnemyAI.Instance.GetDifficult())
                            {
                                case Difficult.Easy:
                                    unit.SetHitHelper(unit.GetHitHelper() + 10);
                                    break;
                                case Difficult.Medium:
                                    unit.SetHitHelper(unit.GetHitHelper() + 5);
                                    break;
                            }
                        }
                        //Instanciamos el missPrefab
                        onAnyMissShoot?.Invoke(this, targetUnit);
                    }
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:

                break;
        }
        if (stateTimer <= 0f)
        {
            NextState();
        }

    }

    private bool calculateChance(float hitChance)
    {
        float chance = UnityEngine.Random.Range(0f, 1f);

        return chance > hitChance;
    }

    private float GetHitChance(Unit targetUnit)
    {
        int distance = CalculateDistance(targetUnit.GetGridPosition());
        float totalPercent = UnityEngine.Random.Range(0.90f, 1f);

        float noneCoverIncrease = 0.28f;
        float halfCoverReduction = 0.1523f;
        float suppressedReduction = 0.176f;
        float fullCoverReduction = 0.331f;

        //El francotirador no tiene desventaja por distancia

        //Disminuimos porcentaje por cobertura 
        switch (targetUnit.GetCoverType())
        {
            case CoverType.None:
                totalPercent += noneCoverIncrease;
                break;
            case CoverType.Half:
                totalPercent -= halfCoverReduction;
                break;
            case CoverType.Full:
                totalPercent -= fullCoverReduction;
                break;
        }

        if (unit.GetIsSupressed())
            totalPercent -= suppressedReduction;



        return totalPercent;
    }

    private int CalculateDistance(GridPosition targetGridPosition)
    {
        //Usamos el mismo calculo que en el pathfinding
        GridPosition gridPositionDistance = unit.GetGridPosition() - targetGridPosition;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return Mathf.Min(xDistance, zDistance) + remaining;
    }

    private void Shoot()
    {
        
        int damage = UnityEngine.Random.Range(39, 55);
        targetUnit.Damage(damage);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                state = State.Cooloff;
                ActionComplete();
                break;
        }


    }


    public override string GetActionName()
    {
        return "Francotirador";
    }


    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }


    //Este segundo metodo de GetValid es para poder ver los target disponibles desde una posicion concreta
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offSetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offSetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                    || !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance) //Esto es para hacer que no sea un cuadrado, en vez de X X es X   y asi progresivamente
                {                                                                                       // X X    X X 
                    continue;
                }


                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue; //Vamos a skipear si el target.isEnemy es el mismo que el unit.isEnemy, es decir si estan en equipos diferentes
                }

                //Validation para no disparar a traves de las paredes

                float unitShoulderHeight = 2f;
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                if (Physics.Raycast(
                  unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstacleLayerMask))
                {
                    continue;
                }
                validGridPositionList.Add(testGridPosition);

            }
        }


        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {


        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;
        canShootBullet = true;

        ActionStart(onActionComplete);

    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    //Puntuación que tiene el spin action
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition, Difficult diff)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        enemyAIAction.gridPosition = gridPosition;
        int targetCountAtGridPosition = 0;
        int friendlyUnitsAround = unit.GetAction<MoveAction>().GetFriendlyUnitsAround(gridPosition).Count;
        int minDamageNormalShoot = 0;
        int maxDamageNormalShoot = 0;

        if (unit.GetAction<ShootAction>() != null)
        {
            targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
            minDamageNormalShoot = unit.GetAction<ShootAction>().GetMinDamageFromShoot();
            maxDamageNormalShoot = unit.GetAction<ShootAction>().GetMaxDamageFromShoot();

        }

        int divider = 200;

        switch (diff)
        {
            case Difficult.Easy:
                //En facil y normal la IA no tendrá en cuenta que se encuentra sin cobertura
                if (calculateChanceNoHelp(20f))
                    enemyAIAction.actionValue = divider / targetCountAtGridPosition;  //Calculo aun por hacer
                break;
            case Difficult.Medium:
                if (calculateChanceNoHelp(30f))
                    enemyAIAction.actionValue = Mathf.RoundToInt(GetHitChance(targetUnit) * 100);
                break;
            case Difficult.Hard:
                if (unit.GetCoverType() == CoverType.None) //Si el personaje que dispara no esta en cobertura
                {
                    enemyAIAction.actionValue -= 20;
                }

                //Ahora hay que comprobar los limites
                //Si hay muchos aliados cerca, deberiamos tener en cuenta la supresión
                enemyAIAction.actionValue += 20 * friendlyUnitsAround;
                //Si el enemigo esta muy cerca cerca del personaje, le activamos la supresión 
                enemyAIAction.actionValue += 20 * CalculateDistance(gridPosition);

                //Y ahora hay que tener en cuenta si podemos matar al personaje
                int targetHealth = targetUnit.GetHealthSystem().GetUnitHealth();
                if ((targetHealth > maxDamageFromSniper && targetHealth < minDamageFromSniper)
                    || (targetHealth > minDamageNormalShoot && targetHealth < maxDamageNormalShoot))
                //En este caso no renta suprimir, es mejor disparar normal que hace mas daño y lo podemos matar
                {
                    enemyAIAction.actionValue = 0;
                }
                break;
        }

        return enemyAIAction;
    }


    public override int GetActionPointsCost()
    {
        if (unit.GetActionPoints() == 0)
            return 1000;
        else
            return unit.GetActionPoints();
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
