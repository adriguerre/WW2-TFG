using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class DataPersistenceManager : MonoBehaviour
{
    //Encargada de controlar del estado actual de los datos

    [Header("File Storage Config")]
    [SerializeField] private string fileName;


    private bool isLoading = false;
    private bool canLoad = true;
    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObject; //Tener todos los objetos que tenemos que guardar
    public static DataPersistenceManager Instance { get; private set; }

    public event EventHandler onLoad;


    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one GameManager");
        }
        Instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObject = FindAllDataPersistenceObjects();
    }


    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        isLoading = true;
        this.gameData = dataHandler.Load();

        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObject)
        {
           
            if(this.gameData.missionNumber == MissionManager.Instance.getMissionNumber())
            {
                canLoad = true;
                Debug.Log("ESTAMOS EN LA MISION");
                dataPersistenceObj.LoadData(gameData);
            }
            else
            {
                canLoad = false; 
                Debug.Log("False");
            }
        }

        isLoading = false;
        onLoad?.Invoke(this, EventArgs.Empty);
    }

    public bool getIsLoading()
    {
        return isLoading; 
    }

    public bool MissionValid()
    {
        return canLoad; 
    }


    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.Log("No data was found, Initializing data to defaults");
            NewGame();
        }
        this.gameData = new GameData();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObject)
        {
            dataPersistenceObj.SaveData(gameData);
            Debug.Log("Saved: " + dataPersistenceObj);
        }
        dataHandler.Save(gameData);
    }


    private void OnApplicationQuit()
    {
       // SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistencesObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistencesObjects);
    }

}
