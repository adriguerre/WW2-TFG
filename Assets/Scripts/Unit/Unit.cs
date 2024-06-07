using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Unit : MonoBehaviour
{

    [SerializeField] private bool isEnemy;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler onAnyUnitSpawned;
  
    public static event EventHandler onAnyUnitWounded;
    public static event EventHandler onAnyUnitDestroyed;
    public event EventHandler onUnitDestroyed;
    public static event EventHandler onAnyUnitAlerted; 
    public event EventHandler onSuppresed;
    public event EventHandler onEndedSuppression;
    public event EventHandler onCoverChanged;
    public event EventHandler<OnFlankShoot> onGetHitPercent; //Evento para enseñar los porcentajes de disparo
    public event EventHandler onActiveUnit;
    private GridPosition gridPosition;
    private BaseAction[] baseActionArray;
    private const int ACTION_POINTS_MAX = 2;
    public CoverType coverType;
    private bool isSupressed = false;
    private bool isWounded = false;
    private bool isAlerted = false;
    private int hitHelper = 0;
    private int shieldHelper = 0;
    [SerializeField] private TypeOfSoldier typeOfSoldier;

    [SerializeField] private UnitWorldUI UI; 

    int turnNumber;
    private HealthSystem healthSystem;
    [SerializeField] private Sprite avatar;
    [SerializeField] private string role;
    private int actionPoints = ACTION_POINTS_MAX;


    public class OnFlankShoot : EventArgs
    {
        public bool flanked;
        public float chance;
    }

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>(); //Cogemos todos los componentes que hereden de BaseAction (Por tanto los scripts de action), y asi podemos saber que acciones tiene
    }

   

    public void Setup(UnitData unitData, bool isEnemy)
    {
        //gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
       // LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        this.healthSystem.SetHealth(unitData.GetHealth());
        this.typeOfSoldier = unitData.GetTypeOfSoldier();
        this.actionPoints = unitData.GetActionPoints();
        if (GetAction<ShootAction>() != null)
        {
            this.GetAction<ShootAction>().SetBullets(unitData.GetAmmo());
        }
        if (GetAction<GrenadeAction>() != null)
        {
            this.GetAction<GrenadeAction>().SetGrenades(unitData.GetGrenades());
        }
        this.isEnemy = isEnemy;
        if (!isEnemy && unitData.GetHealth() == 0)
        {
            //onAnyUnitWounded?.Invoke(this, EventArgs.Empty);
        }
        //onAnyUnitSpawned?.Invoke(this, EventArgs.Empty);


    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.onAnyUnitAlerted += Unit_OnAnyUnitAlerted;
        healthSystem.onDead += HealthSystem_OnDead;
        healthSystem.onWounded += HealthSystem_OnWounded;
        UpdateCoverType();
        onAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
        if (!isEnemy)
        {
            isAlerted = true;
            onActiveUnit?.Invoke(this, EventArgs.Empty);
        }
           

    }


    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);

            //Actualizamos coverType
            UpdateCoverType();
            CheckForEnemies();
        }

        if (isEnemy)
        {
            CheckForEnemies();
        }

        //Por ahora vamos a poner que mantenga el Suprimido por 1 turno 
        
        //Pongo 2, ya que es el actual mas el siguiente
        if(TurnSystem.Instance.GetTurnNumber() == turnNumber + 2)
        {
            EliminateAllDebuffs();
        }

    }

    private void Unit_OnAnyUnitAlerted(object sender, EventArgs e)
    {
        this.isAlerted = true;
    }
    private void CheckForEnemies()
    {

        int targetCountAtGridPosition;

        if (GetAction<ShootAction>() != null)
            targetCountAtGridPosition = GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        else
            targetCountAtGridPosition = 0;


        if (targetCountAtGridPosition > 0)
        {
            //Alerted 
            isAlerted = true;
            onAnyUnitAlerted?.Invoke(this, EventArgs.Empty);
        }
    }

    public TypeOfSoldier GetTypeOfSoldier()
    {
        return typeOfSoldier;
    }
    private void EliminateAllDebuffs()
    {
        SetIsSupressed(false);
        onEndedSuppression?.Invoke(this, EventArgs.Empty);
        SetShieldHelper(0);
    }

    public void StartRecordTurn()
    {
        turnNumber = TurnSystem.Instance.GetTurnNumber();
    }

    public void UpdateCoverType()
    {
        
        coverType = LevelGrid.Instance.GetUnitCoverType(gridPosition);

        onCoverChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!isEnemy && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
       }
    }

    private void HealthSystem_OnWounded(object sender, EventArgs e)
    {
        
        isWounded = true;
        onAnyUnitWounded?.Invoke(this, EventArgs.Empty);
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
    }


    public void setWounded(bool wounded)
    {
        this.isWounded = wounded;
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this); //En caso de querer poder reanimar, tendría que añadirlo a otro array, que sea unidades muertas, y luego recogerlo de ahi
        //Destroy(gameObject);
        onUnitDestroyed?.Invoke(this, EventArgs.Empty);
        onAnyUnitDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public T GetAction<T>() where T : BaseAction //Metodo para devolver el baseAction sin necesidad de tener que poner todas las acciones por separado
    {
        foreach(BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;

    }
    

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointsCost();
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UnitWoundedOnLoad()
    {
        Debug.Log("HERIDO DE MUERTE MORTAL");
        isWounded = true;
        UI.OnWoundedLoad();
        onAnyUnitWounded?.Invoke(this, EventArgs.Empty);
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    public void ShowHitPercent(float hitPercent, bool flank)
    {
        onGetHitPercent?.Invoke(this, new OnFlankShoot { flanked = flank, chance = hitPercent * 100 });
     
    } 

    public void AddActionPoints(int actionAdded)
    {
        SetActionPoints(GetActionPoints() + actionAdded);
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(int damageAmount)
    {

        healthSystem.Damage(damageAmount, this);
    }
    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void SetActionPoints(int action)
    {
        this.actionPoints = action;
    }

    public int GetMaxActionPoints()
    {
        return ACTION_POINTS_MAX;
    }

    public string GetRole()
    {
        return role;
    }

    public bool GetIsWounded()
    {
        return isWounded;
    }

   
    public bool IsEnemy()
    {
        return isEnemy;
    }

    public bool GetIsAlarmed()
    {
        return isAlerted;
    }
    public Vector3 GetWorldPosition()
    {
        if(this != null)
        {
            return transform.position;
        }
        else { return new Vector3(0,0,0); }
        
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    public Sprite GetAvatar()
    {
        return avatar;
    }
    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public CoverType GetCoverType()
    {
        return coverType;
    }

    public void SetIsSupressed(bool supressed)
    {
        this.isSupressed = supressed;
    }

    public void SetSupressedUnit()
    {
        onSuppresed?.Invoke(this, EventArgs.Empty);
        //Aqui habria que llamar al turnSystem para que tuviese constancia, aunque a lo mejor se puede hacer con los get y los set, por decidir y mirar
        this.SetIsSupressed(true);
        turnNumber = TurnSystem.Instance.GetTurnNumber();
    }
    public bool GetIsSupressed()
    {
        return isSupressed;
    }

    public void SetHitHelper(int help)
    {
        this.hitHelper = help; 
    }

    public int GetHitHelper()
    {
        return this.hitHelper;
    }
    public void SetShieldHelper(int help)
    {
        this.shieldHelper = help;
    }

    public int GetShieldHelper()
    {
        return this.shieldHelper;
    }
    public void SetCoverType(CoverType coverType)
    {
        this.coverType = coverType;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

 
    public void SetGridPosition(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }
}
