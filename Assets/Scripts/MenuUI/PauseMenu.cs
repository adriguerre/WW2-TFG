using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image loadingBarFill;
    [SerializeField] private Image backgroundLoadingBarFill;

    [SerializeField] private GameObject savePanel;
    [SerializeField] private TextMeshProUGUI text;


    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
 
    private void OnEnable()
    {
        Time.timeScale = 0f;
        savePanel.SetActive(false);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Start()
    {
        LoadValues(1, 1);
    }

    public void SaveGame()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            text.text = "Se ha guardado correctamente la partida";
            savePanel.SetActive(true);
            DataPersistenceManager.Instance.SaveGame();
        }
        else
        {
            text.text = "Espere a que termine el turno enemigo";
            savePanel.SetActive(true);
        }
   

    }

    public void LoadGame()
    {
        


        if (DataPersistenceManager.Instance.MissionValid())
        {
            if (TurnSystem.Instance.IsPlayerTurn())
            {
                DataPersistenceManager.Instance.LoadGame();
                text.text = "Se ha cargado correctamente la partida";
                savePanel.SetActive(true);
            }
            else
            {
                text.text = "Espere a que termine el turno enemigo";
                savePanel.SetActive(true);
            }
        }
        else
        {
            text.text = "Debe de iniciar la mision que desea cargar primero";
            savePanel.SetActive(true);
        }

        // SceneManager.LoadScene("MainGameScene");
    }

    public void NewGame()
    {
        DataPersistenceManager.Instance.NewGame();
    }

    IEnumerator LoadSceneAsync(string nameScene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nameScene);
        loadingBarFill.enabled = true;
        backgroundLoadingBarFill.enabled = true;

        //Desactivar todo los botones

        while (!operation.isDone)
        {
            float progessValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBarFill.fillAmount = progessValue;

            yield return null;
        }  
    }


    public void ApplyVolumeChanges()
    {

        float musicVolume = musicVolumeSlider.value;
        float effectVolume = effectsVolumeSlider.value;


        LoadValues(musicVolume, effectVolume);
    }

    public void LoadValues(float musicValue, float effectValue)
    {
        AudioSource musicSource = GameManager.Instance.GetAudioSource();

        musicSource.volume = musicValue;
        musicVolumeSlider.value = musicValue; 


        foreach(Unit unit in UnitManager.Instance.GetUnitList())
        {
            AudioSource unitSource = unit.GetComponent<AudioSource>();
            unitSource.volume = effectValue;
        
        }
        effectsVolumeSlider.value = effectValue;
    }

}
