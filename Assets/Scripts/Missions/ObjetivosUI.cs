using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ObjetivosUI : MonoBehaviour
{
    public static ObjetivosUI Instance { get; private set; }

    [SerializeField] private List<TextMeshProUGUI> listaTexto;
    [SerializeField] private TextMeshProUGUI textoClearEnemies;
    [SerializeField] private List<ObjetivoInteractuar> objetivos;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one ObjetivosUI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
 
    void Start()
    {
        UnitManager.Instance.onEnemyUnitDestroyed += UnitManager_onEnemyUnitDestroyed;
        objetivos = MissionManager.Instance.GetObjetivos();
        WriteObjetives(false, 0);
        MissionManager.onAnyObjectiveCompleted += missionManager_onAnyObjectiveCompleted;
    }

    private void UnitManager_onEnemyUnitDestroyed(object sender, EventArgs e)
    {
        if (MissionManager.Instance.getNeedToClearenemies())
        {
            UpdateCountOfEnemies(UnitManager.Instance.GetEnemiesAlive());
        }
    }

    public void UpdateCountOfEnemies(int enemiesRemaining)
    {
      
        if (enemiesRemaining == 0)
        {
            textoClearEnemies.color = Color.gray;
            textoClearEnemies.text = "- Objetivos eliminados ";
            
        }
        else
        {
            textoClearEnemies.text = "- Elimina a los enemigos: " + enemiesRemaining.ToString();
        }
       
    }

    public void ChangeTextOnLoad(int enemiesRemaining)
    {
        WriteObjetives(true, enemiesRemaining);
    }


    private void missionManager_onAnyObjectiveCompleted(object sender, ObjetivoInteractuar e)
    {
        UpdateObjective(e);
    }

    private void UpdateObjective(ObjetivoInteractuar obj)
    {
        for (int i = 0; i < objetivos.Count; i++)
        {
            if (obj.Equals(objetivos[i]))
            {
                listaTexto[i].color = Color.gray;
            }
           
        }
    }

    private void WriteObjetives(bool isLoading, int enemiesLoading)
    {
        for(int i = 0; i < objetivos.Count; i++) 
        {
            if (objetivos[i].getIsCompleted())
            {
                listaTexto[i].color = Color.gray;
            }
            else { 
                listaTexto[i].color = Color.white; 
            }
            listaTexto[i].text = objetivos[i].GetObjetiveText();
        }
        if (MissionManager.Instance.getNeedToClearenemies())
        {
            if (isLoading)
            {
                UpdateCountOfEnemies(enemiesLoading);
            }
            else
            {
                UpdateCountOfEnemies(UnitManager.Instance.GetEnemyUnitList().Count);
            }
        }

        
       
    }
}
