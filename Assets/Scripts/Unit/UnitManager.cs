using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitManager : MonoBehaviour, IDataPersistence
    //Esta clase es para tener control de todas las unidades del mapa
{
    //Diccionario, unidad - Dead 

    [SerializeField] private List<Unit> unitList;
    [SerializeField] private List<Unit> friendlyUnitList;
    [SerializeField] private SerializableDictionary<GridPosition, Unit> woundedUnitDic;
    [SerializeField] private List<Unit> enemyUnitList;
    [SerializeField] private List<Unit> enemyDeadUnitList;

    public event EventHandler onEnemyUnitDestroyed;


    [SerializeField] private Transform GER_AT_Prefab;
    [SerializeField] private Transform GER_INF_Prefab;
    [SerializeField] private Transform GER_MED_Prefab;
    [SerializeField] private Transform GER_SUPP_Prefab;
    [SerializeField] private Transform US_INF_Prefab;
    [SerializeField] private Transform US_AT_Prefab;
    [SerializeField] private Transform US_MED_Prefab;
    [SerializeField] private Transform US_OF_Prefab;
    [SerializeField] private Transform US_REC_Prefab;
    [SerializeField] private Transform US_SUPP_Prefab;
    [SerializeField] private Transform GER_OF_Prefab;

    public event EventHandler onAllAlliesDead; 

    public static UnitManager Instance { get; private set; }
    private void Awake()
    {

        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        woundedUnitDic = new SerializableDictionary<GridPosition, Unit>();
        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>(); 
    }


    private void Start()
    {
        HealthSystem.onAnyRevived += HealthSystem_OnAnyRevived;
        Unit.onAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.onAnyUnitDestroyed += Unit_OnAnyUnitDestroyed;
        Unit.onAnyUnitWounded += Unit_OnAnyUnitWounded;

    }

    private void HealthSystem_OnAnyRevived(object sender, Unit e)
    {
        woundedUnitDic.Remove(e.GetGridPosition());
        friendlyUnitList.Add(e);
       
        Collider collider = e.GetComponent<Collider>();
        collider.enabled = true;
    }

    public Unit GetWoundedUnitAtGridPosition(GridPosition gridPosition)
    {
        Unit woundedUnit;
        if (woundedUnitDic.TryGetValue(gridPosition, out woundedUnit))
        {
            Debug.Log(woundedUnit);
            return woundedUnit;
        }

        return null;
    }



    private void Unit_OnAnyUnitWounded(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        
        friendlyUnitList.Remove(unit);

        if (friendlyUnitList.Count == 0)
        {
            onAllAlliesDead?.Invoke(this, EventArgs.Empty);
        }

        if (woundedUnitDic.ContainsKey(unit.GetGridPosition()))
        {
            //Matamos a la unidad
            Collider collider = unit.GetComponent<Collider>();
            collider.enabled = false;
            friendlyUnitList.Remove(unit);
        }
        else
        {
            woundedUnitDic.Add(unit.GetGridPosition(), unit); //Guardo el herido aqui
        }
     
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
        {
            if(friendlyUnitList.Count > 0)
            {
                Unit randomUnit = GetFriendlyUnitAlive();
                UnitActionSystem.Instance.SetSelectedUnit(randomUnit);
            }
    
        }
    }
    public int GetWoundedUnitsCount()
    {
        return woundedUnitDic.Values.Count;
    }

    private Unit GetFriendlyUnitAlive()
    {
       
        int random = UnityEngine.Random.Range(0, friendlyUnitList.Count);
        return friendlyUnitList[random];
    }

    public int GetEnemiesAlive()
    {    
        return enemyUnitList.Count; 
    }


    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
 
        Unit unit = sender as Unit;
        unitList.Add(unit);
        if (unit.IsEnemy())
            enemyUnitList.Add(unit);
        else
        {
            if(unit.GetHealthSystem().GetUnitHealth() == 0)
            {
                woundedUnitDic.Add(unit.GetGridPosition(), unit);
            }
            else
            {
                friendlyUnitList.Add(unit);
            }
          
        }
            
    }

    private void Unit_OnAnyUnitDestroyed(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        if (unit.IsEnemy())
        {
            unitList.Remove(unit);
            enemyUnitList.Remove(unit);
            enemyDeadUnitList.Add(unit);
            onEnemyUnitDestroyed?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Collider collider = unit.GetComponent<Collider>();
            collider.enabled = false;
            friendlyUnitList.Remove(unit);

 


            if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
            {
                int random = UnityEngine.Random.Range(0, friendlyUnitList.Count);
                Unit randomUnit = GetFriendlyUnitAlive();
                UnitActionSystem.Instance.SetSelectedUnit(randomUnit);
            }

            
        }

            
    }



    public List<Unit> GetUnitList()
    {
        return unitList;
    }
    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }
    public List<Unit> GetEnemyUnitList()
    {
        return enemyUnitList;
    }

    public void RemoveUnit(List<Unit> list, Unit unit)
    {
        list.Remove(unit);
    }

    public void LoadData(GameData data)
    {
        DestroyInitialUnits(this.unitList);
        SquadUI.Instance.DestroyAllSquadUI();
        //Y aqui deberiamos resetar la escena
        TakeEnemyUnitsBack(data.enemyUnitsDataList);
        TakeAlliesUnitBack(data.friendlyUnitList);
        TakeWoundedUnitsBack(data.woundedUnitDic);
    }
    private void TakeWoundedUnitsBack(List<string> jsonList)
    {
        List<Unit> unitList = new List<Unit>();
        foreach (string json in jsonList)
        {
            UnitData unitD = JsonUtility.FromJson<UnitData>(json);
            Unit unit = SpawnAlliesUnit(unitD);
            unit.setWounded(true);
            //woundedUnitDic.Add(unit.GetGridPosition(), unit);
            SquadUI.Instance.UpdateUiOnLoad(unit);
            unit.UnitWoundedOnLoad();
            unit.GetHealthSystem().UnitWoundedOnLoad();
        }
    }
    private void TakeEnemyUnitsBack(List<string> jsonList)
    {
        foreach(string json in jsonList)
        {
            UnitData unitD = JsonUtility.FromJson<UnitData>(json);
            Unit unit = SpawnEnemyUnit(unitD);  
        }
    }

    private void TakeAlliesUnitBack(List<string> jsonList)
    {
        foreach (string json in jsonList)
        {
            UnitData unitD = JsonUtility.FromJson<UnitData>(json);
            Unit unit = SpawnAlliesUnit(unitD);
            UnitActionSystem.Instance.LoadSelectedUnit(unit);
            SquadUI.Instance.UpdateUiOnLoad(unit);
        }
    }

    private Unit SpawnEnemyUnit(UnitData unitData)
    {
 
        Transform soldierPrefab = null;
        switch (unitData.GetTypeOfSoldier())
        {
            case TypeOfSoldier.INF:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_GER_INF(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.MED:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_GER_MED(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.SUPP:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_GER_SUPP(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.AT:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_GER_AT(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.OF:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_GET_OF(), unitData.GetPosition(), Quaternion.identity);
                break; 

        }

        Unit unit = soldierPrefab.GetComponent<Unit>();
        unit.Setup(unitData, true);
        return unit;
    }


    private Unit SpawnAlliesUnit(UnitData unitData)
    {

        Transform soldierPrefab = null;
        switch (unitData.GetTypeOfSoldier())
        {
            case TypeOfSoldier.INF:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_US_INF(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.MED:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_US_MED(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.SUPP:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_US_SUPP(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.AT:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_US_AT(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.REC:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_US_REC(), unitData.GetPosition(), Quaternion.identity);
                break;
            case TypeOfSoldier.OF:
                soldierPrefab = Instantiate(UnitManager.Instance.Get_US_OF(), unitData.GetPosition(), Quaternion.identity);
                break;
        }

        Unit unit = soldierPrefab.GetComponent<Unit>();
        unit.Setup(unitData, false);
        return unit;
    }


    private void DestroyInitialUnits(List<Unit> unitList)
    {
        List<UnitData> units = new List<UnitData>();


        //Primero eliminamos los que haya y
        foreach (Unit unit in unitList)
        {
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(unit.transform.position);
            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, unit);   
            Destroy(unit.gameObject); 
        }

        foreach(Unit unit in enemyDeadUnitList)
        {
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(unit.transform.position);
            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, unit);
            Destroy(unit.gameObject);
        }
        this.enemyDeadUnitList.Clear(); 
        this.woundedUnitDic.Clear();
        this.unitList.Clear();
        this.enemyUnitList.Clear();
        this.friendlyUnitList.Clear();

         
    }

    public void SaveData(GameData data)
    {
        SaveEnemyUnits(data);
        SaveAlliesUnits(data);
        SaveWoundedUnits(data);
        SaveAllUnits(data);
    }
    private void SaveEnemyUnits(GameData data)
    {
        foreach (Unit unit in this.enemyUnitList)
        {
            UnitData unitD = TakeData(unit);
            var json = JsonUtility.ToJson(unitD, true);
            data.enemyUnitsDataList.Add(json);

        }
    }
    private void SaveAlliesUnits(GameData data)
    {
        foreach (Unit unit in this.friendlyUnitList)
        {
            UnitData unitD = TakeData(unit);
            var json = JsonUtility.ToJson(unitD, true);
            data.friendlyUnitList.Add(json);

            //UnitData unit2 = JsonUtility.FromJson<UnitData>(json);;
        }
    }
    private void SaveWoundedUnits(GameData data)
    {
        foreach (Unit unit in this.woundedUnitDic.Values)
        {
            UnitData unitD = TakeData(unit);
            var json = JsonUtility.ToJson(unitD, true);
            data.woundedUnitDic.Add(json);

            //UnitData unit2 = JsonUtility.FromJson<UnitData>(json);;
        }
    }
   private void SaveAllUnits(GameData data)
    {
        foreach (Unit unit in this.friendlyUnitList)
        {
            UnitData unitD = TakeData(unit);
            var json = JsonUtility.ToJson(unitD, true);
            data.unitList.Add(json);
        }
        foreach (Unit unit in this.enemyUnitList)
        {
            UnitData unitD = TakeData(unit);
            var json = JsonUtility.ToJson(unitD, true);
            data.unitList.Add(json);
        }
    }
    private UnitData TakeData(Unit unit)
    {
        int health = unit.GetHealthSystem().GetUnitHealth();
        int grenadesLeft = 0;
        if (unit.GetAction<GrenadeAction>() != null)
        {
            grenadesLeft = unit.GetAction<GrenadeAction>().GetGrenadesLeft();
        }
        int ammoLeft = 0;
        if (unit.GetAction<ShootAction>() != null)
        {
            ammoLeft = unit.GetAction<ShootAction>().GetRemainingBullets();
        }
        return new UnitData(health, grenadesLeft, ammoLeft, unit.GetTypeOfSoldier().ToString(), unit.GetActionPoints(), unit.GetWorldPosition(), unit.GetIsWounded());
    }


    public Transform Get_GET_OF()
    {
        return GER_OF_Prefab;
    }
    public Transform Get_GER_AT()
    {
        return GER_AT_Prefab;
    }

    public Transform Get_GER_INF()
    {
        return GER_INF_Prefab;
    }

    public Transform Get_GER_MED()
    {
        return GER_MED_Prefab;
    }

    public Transform Get_GER_SUPP()
    {
        return GER_SUPP_Prefab;
    }

    public Transform Get_US_INF()
    {
        return US_INF_Prefab;
    }
    public Transform Get_US_AT()
    {
        return US_AT_Prefab;
    }
    public Transform Get_US_MED()
    {
        return US_MED_Prefab;
    }
    public Transform Get_US_OF()
    {
        return US_OF_Prefab;
    }
    public Transform Get_US_REC()
    {
        return US_REC_Prefab;
    }
    public Transform Get_US_SUPP()
    {
        return US_SUPP_Prefab;
    }
}
