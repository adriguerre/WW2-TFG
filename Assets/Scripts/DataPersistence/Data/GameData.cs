using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{

    public List<string> unitList;
    public List<string> friendlyUnitList;
    public List<string> woundedUnitDic;
    public List<string> enemyUnitsDataList;
    public List<string> enemyUnitsDeadList; 

    public List<string> objetivesNotCompleted;
    public List<string> objetivesCompleted;
    public bool needToClearEnemies; 

    public int missionNumber;
    public GameData()
    {
        enemyUnitsDeadList = new List<string>();
        woundedUnitDic = new List<string>();
        enemyUnitsDataList = new List<string>();
        unitList = new List<string>();
        friendlyUnitList = new List<string>();
        missionNumber = 0; 
        objetivesCompleted = new List<string>();
        objetivesNotCompleted = new List<string>();
        needToClearEnemies = false;
    }
}
