using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MissionManager : MonoBehaviour, IDataPersistence
{
    public static MissionManager Instance { get; private set; }
    [SerializeField] private int missionNumber;
    [SerializeField] private List<ObjetivoInteractuar> listaObjetivosInteractuar;
    [SerializeField] private bool needToClearEnemies;
    private bool enemiesCleared;

    [SerializeField] private List<ObjetivoInteractuar> listaObjetivosInteractuarCompletados;

    [SerializeField] private Transform MorteroPRefab;
    [SerializeField] private Transform DocumentosPrefab;
    [SerializeField] private Transform camionPrefab;


    [SerializeField] private ObjetivoInteractuable mesaDocumentos;
    [SerializeField] private ObjetivoInteractuable table_Cafe;
    [SerializeField] private ObjetivoInteractuable Mortero;
    [SerializeField] private ObjetivoInteractuable camion;

    public static event EventHandler<ObjetivoInteractuar> onAnyObjectiveCompleted;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one MissionManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
      
        listaObjetivosInteractuarCompletados = new List<ObjetivoInteractuar>();
        UnitManager.Instance.onAllAlliesDead += UnitManager_OnAllAlliesDead;

        ObjetivoInteractuar.onObjetivoCompletado += onObjetivoCompletado;
    }

 

    private void onObjetivoCompletado(object sender, EventArgs e)
    {
        ObjetivoInteractuar objetivo = sender as ObjetivoInteractuar;
        listaObjetivosInteractuarCompletados.Add(objetivo);
        objetivo.SetIsCompleted(true);
        onAnyObjectiveCompleted?.Invoke(this, objetivo);
    }

    // Update is called once per frame
    void Update()
    {

        if (listaObjetivosInteractuar.Count == listaObjetivosInteractuarCompletados.Count 
            && (!needToClearEnemies || UnitManager.Instance.GetEnemiesAlive() == 0) && !DataPersistenceManager.Instance.getIsLoading())
        {
            StartCoroutine(GameManager.Instance.FinishGame("MISION COMPLETADA"));
        }
            
        

        

       
    }

    private void UnitManager_OnAllAlliesDead(object sender, EventArgs e)
    {
        StartCoroutine(GameManager.Instance.FinishGame("MISION FALLIDA"));
    }

    public List<ObjetivoInteractuar> GetObjetivos()
    {
        return listaObjetivosInteractuar;
    }
    public List<ObjetivoInteractuar> GetObjetivosCompletados()
    {
        return listaObjetivosInteractuarCompletados;
    }

    public bool getNeedToClearenemies()
    {
        return needToClearEnemies;
    }

    public void LoadData(GameData data)
    {
    
        ClearObjectives();
        LoadObjectivesNotCompleted(data.objetivesNotCompleted);

        if (needToClearEnemies)
        {
            ObjetivosUI.Instance.UpdateCountOfEnemies(data.enemyUnitsDataList.Count);
        }
        ObjetivosUI.Instance.ChangeTextOnLoad(data.enemyUnitsDataList.Count);

    }



    private void ClearObjectives()
    {
       // this.listaObjetivosInteractuar.Clear();
        this.listaObjetivosInteractuarCompletados.Clear();
    }

    private void LoadObjectivesNotCompleted(List<string> data)
    {
        foreach (string json in data)
        {
       
            ObjetiveData objectivoData = JsonUtility.FromJson<ObjetiveData>(json);
            ObjetivoInteractuar obj = null;
            switch (objectivoData.GetObjectiveName())
            {
                case "Mortero":
                    foreach (ObjetivoInteractuar objetivo in listaObjetivosInteractuar)
                    {
                        if (objetivo.GetObjectiveName() == "Mortero")
                        {
                            objetivo.SetIsCompleted(objectivoData.isCompleted);
                            SetStar(Mortero, objetivo);
                            obj = objetivo;
                        }
                    }
                    break;
                case "Documentos":

                    foreach (ObjetivoInteractuar objetivo in listaObjetivosInteractuar)
                    {
                        if (objetivo.GetObjectiveName() == "Documentos")
                        {
                            objetivo.SetIsCompleted(objectivoData.isCompleted);
                            SetStar(mesaDocumentos, objetivo);
                            obj = objetivo;
                        }
                    }

                    break;
                case "Camion":
                 
                    foreach(ObjetivoInteractuar objetivo in listaObjetivosInteractuar)
                    {
                        if(objetivo.GetObjectiveName() == "Camion")
                        {
                            objetivo.SetIsCompleted(objectivoData.isCompleted);
                            SetStar(camion, objetivo);

                            obj = objetivo;
                        }
                    }
                    break;
            }
            if (obj.getIsCompleted())
            {
                this.listaObjetivosInteractuarCompletados.Add(obj);
            }
        }
    }

    private void SetStar(ObjetivoInteractuable inte, ObjetivoInteractuar obj)
    {

        if (obj.getIsCompleted())
            inte.GetStar().SetActive(false);
        else
            inte.GetStar().SetActive(true);
    }

    public void SaveData(GameData data)
    {
        data.needToClearEnemies = needToClearEnemies;
        SaveObjectivesNotCompleted(data);
        SaveObjectivesCompleted(data);
        data.missionNumber = this.missionNumber;
    }

    private void SaveObjectivesNotCompleted(GameData data)
    {
        foreach (ObjetivoInteractuar objetivo in this.listaObjetivosInteractuar)
        {
            ObjetiveData objData = new ObjetiveData(objetivo.GetObjetiveText(), objetivo.getIsCompleted(), objetivo.GetObjectiveName());
            var json = JsonUtility.ToJson(objData, true);
            data.objetivesNotCompleted.Add(json);
        }
    }
    private void SaveObjectivesCompleted(GameData data)
    {
        foreach (ObjetivoInteractuar objetivo in this.listaObjetivosInteractuarCompletados)
        {
            ObjetiveData objData = new ObjetiveData(objetivo.GetObjetiveText(), objetivo.getIsCompleted(), objetivo.GetObjectiveName());
            Debug.Log(objData.GetObjectiveText());
            var json = JsonUtility.ToJson(objData, true);
            data.objetivesCompleted.Add(json);
        }
    }




    public int getMissionNumber()
    {
        return missionNumber;
    }
}
