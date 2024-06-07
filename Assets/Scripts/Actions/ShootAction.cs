using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShootAction : BaseAction
{

    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private LayerMask coverLayerMask;
    private float totalSpinAmount;
    private int maxShootDistance = 7;
    private int minDamage = 20;
    private int maxDamage = 45;
    public int bulletsRemaining;
    private const int MAX_BULLETS = 4;
    private Unit targetUnit;
    private bool canShootBullet;
    private LinkedList<Unit> unitsFlanked;
    //private Dictionary<Unit, bool> unitsFlanked; 

    public static event EventHandler<OnShootEventArgs> onAnyShoot;
    public event EventHandler<OnShootEventArgs> onShoot;


    public event EventHandler<Unit> onAnyMissShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }


    private void Start()
    {
        UnitActionSystem.onAnySelectedChangedSoHidePercent += UnitSelectedVisual_onAnySelectedChangedSoHidePercent;

        if (TryGetComponent<ReloadAction>(out ReloadAction reloadAction))
        {
            reloadAction.onStartReloading += reloadAction_OnStartReloading;
        }

        bulletsRemaining = MAX_BULLETS;
        unitsFlanked = new LinkedList<Unit>();
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

                    onShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit });
                    onAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit });

                    if (calculateChance(hitChance))
                    {
                        //Acierto 
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

                    EmptyFlankedList();
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
    private void reloadAction_OnStartReloading(object sender, EventArgs e)
    {
        ReloadAllBullets();

    }

    private void ReloadAllBullets()
    {
        bulletsRemaining = MAX_BULLETS;
    }

    private bool calculateChance(float hitChance)
    {
        float chance = UnityEngine.Random.Range(0f, 1f);

        //Dependiendo de la dificultad, añadimos un multiplicador
        Difficult difficulty = EnemyAI.Instance.GetDifficult();
        float easyDiff = 1.15f;
        float mediumDiff = 1.075f;
     
        if (targetUnit.IsEnemy())
        {
            switch (difficulty)
            {
                case Difficult.Easy:
                    hitChance *= easyDiff;
                    break;
                case Difficult.Medium:
                    hitChance *= mediumDiff;
                    break;
            }
        }
        return chance < hitChance; 
    }



    private float GetHitChance(Unit targetUnit)
    {
        int distance = CalculateDistance(targetUnit.GetGridPosition());
        float totalPercent = 0.92f;

        float distanceReduction = 0.0843f;
        float noneCoverIncrease = 0.1342f;
        float halfCoverReduction = 0.152f;
        float suppressedReduction = 0.134f;
        float fullCoverReduction = 0.278f;
        float flankedIncrease = 0.324f;

        if (unit.IsEnemy())
        {
            //Disminuimos porcentaje por distancia a los enemigos en facil y medio
            switch (EnemyAI.Instance.GetDifficult())
            {
                case Difficult.Easy:
                    distanceReduction = 0.12f;
                    break;
                case Difficult.Medium:
                    distanceReduction = 0.10f;
                    break;
            }
            
        }
        totalPercent -= distance * distanceReduction;

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

        //Ahora comprobamos si esta rodeado 

        if (unitsFlanked.Contains(targetUnit))
        {
            totalPercent += flankedIncrease; 
        }

        if(unit.GetHitHelper() > 0)
        {
            
            totalPercent += unit.GetHitHelper() / 100;
        }

        if(targetUnit.GetShieldHelper() > 0)
        {
            totalPercent -= targetUnit.GetShieldHelper() / 100;
        }

        if (totalPercent > 1f)
            return 1;
        else
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
        int damage = UnityEngine.Random.Range(minDamage, maxDamage);
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
        return "Shoot";
    }

    
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition, true);
    }


    //Este segundo metodo de GetValid es para poder ver los target disponibles desde una posicion concreta
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition, bool GetPercent)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        if (unit.GetActionPoints() > 0)
        {
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
                    if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDir, Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstacleLayerMask))
                    {
                        continue;
                    }
                    //Show enemy hit % 
                    //Este if, es para poder hacer el alerta, sin necesidad de mostrar los porcentajes
                    if (GetPercent)
                    {
                        if (!Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                           shootDir, Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                           coverLayerMask) && targetUnit.GetCoverType() != CoverType.None)
                        {
                            if (targetUnit.IsEnemy())
                            {
                                AddUnitToFlankedUnits(targetUnit);
                                targetUnit.ShowHitPercent(GetHitChance(targetUnit), true);
                            }
                        }
                        else
                        {
                            if (targetUnit.IsEnemy())
                                targetUnit.ShowHitPercent(GetHitChance(targetUnit), false);
                        }
                    }
                       
                    validGridPositionList.Add(testGridPosition);

                }
            }
        }

        return validGridPositionList;
    }

    private void UnitSelectedVisual_onAnySelectedChangedSoHidePercent(object sender, EventArgs e)
    {
        EmptyFlankedList();
    }


    public void EmptyFlankedList()
    {
        unitsFlanked.Clear();
        
    }

    private void AddUnitToFlankedUnits(Unit unit)
    {
        unitsFlanked.AddLast(unit);
    }



    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
       

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        bulletsRemaining--;
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

        switch (diff)
        { 
            case Difficult.Easy:
                //En facil y normal la IA no tendrá en cuenta que se encuentra sin cobertura
                //En fácil, habrá un 50% de que dispare a un soldado que tenga menos porcentaje 
                if (calculateChanceNoHelp(50f))
                    //Enemigo con menos porcentaje 
                    enemyAIAction.actionValue = (100 / Mathf.RoundToInt(GetHitChance(targetUnit) * 100)) * 50;
                else
                    //Disparo normal, con los escudos y ayudas de estar en facil
                    enemyAIAction.actionValue = Mathf.RoundToInt(GetHitChance(targetUnit) * 100);
                break;
            case Difficult.Medium:
                if (calculateChanceNoHelp(20f))
                    //Enemigo con menos porcentaje 
                    enemyAIAction.actionValue = (100 / Mathf.RoundToInt(GetHitChance(targetUnit) * 100)) * 50;
                else
                    //Disparo normal, con los escudos y ayudas de estar en facil
                    enemyAIAction.actionValue = Mathf.RoundToInt(GetHitChance(targetUnit) * 100);
                break;
            case Difficult.Hard:
                if (unit.GetCoverType() == CoverType.None) //Si el personaje que dispara no esta en cobertura
                {
                    enemyAIAction.actionValue -= 50;
                }
                //Comprobar, si se puede matar o no
                //Y ahora hay que tener en cuenta si podemos matar al personaje
                int targetHealth = targetUnit.GetHealthSystem().GetUnitHealth();
                if ((targetHealth < minDamage) || (targetHealth > minDamage && targetHealth < maxDamage))
                //En este caso no renta suprimir, es mejor disparar normal que hace mas daño y lo podemos matar
                {
                    enemyAIAction.actionValue = (Mathf.RoundToInt(GetHitChance(targetUnit) * 100)) + (100 - targetHealth);
                }
                else //No podemos matarlo
                {
                    enemyAIAction.actionValue = Mathf.RoundToInt(GetHitChance(targetUnit) * 100);
                }

                break; 
        }

        //enemyAIAction.actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f);
        return enemyAIAction;
    }


    public override int GetActionPointsCost()
    {
        if(unit.GetActionPoints() == 0)
            return 1000;   
        else
            return unit.GetActionPoints();
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) //Devuelve el numero total de enemigos a la "vista"
    {
        return GetValidActionGridPositionList(gridPosition, false).Count;
    }

    public int GetRemainingBullets()
    {
        return bulletsRemaining;
    }

    public void SetBullets(int aux)
    {
        this.bulletsRemaining = aux;
    }
    public int GetMaxDamageFromShoot()
    {
        return maxDamage; 
    }
    public int GetMinDamageFromShoot()
    {
        return minDamage;
    }

}
