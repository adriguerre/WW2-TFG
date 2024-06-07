using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
public class MissionSelectorUI : MonoBehaviour
{

    [SerializeField] private Image mainImage;

    [SerializeField] private TextMeshProUGUI missionName;
    [SerializeField] private Image missionImage;
    [SerializeField] private Button startMission;
    [SerializeField] private Image backgroundMissionImage;
    [Header("Imagen Misiones")]
    [SerializeField] private Image mission1Image;
    [SerializeField] private Image mission2Image;
    [SerializeField] private Image mission3Image;
    [SerializeField] private Image mission4Image; 
    [SerializeField] private Image mission5Image;
    [Header("Game objects to hide")]
    [SerializeField] private GameObject hideItems;
   
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadScreen;
    [SerializeField] private Image loadingBarFill;
    [SerializeField] private Image backgroundLoadingBarFill;


    private int missionChosed = -1;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ButtonBackToMenu();
        }

        if (missionChosed != -1)
        {
            backgroundMissionImage.enabled = true;
            missionImage.enabled = true;
            missionName.enabled = true;
            startMission.enabled = true;
            startMission.interactable = true;
        }
        else
            startMission.interactable = false;
    }

    public void StartMission()
    {
        switch (missionChosed)
        {
            case 0:
                StartCoroutine(LoadSceneAsync("Mission_1"));
                break;
            case 1:
                StartCoroutine(LoadSceneAsync("Mission_2"));
                break;
        }
    }
  
    IEnumerator LoadSceneAsync(string nameScene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nameScene);
        ActivateLoadingScreen();
        //Desactivar todo los botones

        while (!operation.isDone)
        {
            float progessValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBarFill.fillAmount = progessValue;

            yield return null;
        }

    }
    private void ActivateLoadingScreen()
    {
        loadScreen.SetActive(true);
        loadingBarFill.enabled = true;
        backgroundLoadingBarFill.enabled = true;
        hideItems.SetActive(false);
    }



    public void ButtonBackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ShowInfoMission(int mission)
    {
        
        /*
         * 0 - Sainte-Mere-Eglise
         * 1 - Sainte-Marie-Du-Mont
         * 2 - Caen
         * 3 - Paris 
         * 4 - Berlin
         */
        Debug.Log("EEE");
        switch (mission)
        {
            case 0:
                Debug.Log(mission); 
                missionChosed = 0;
                // missionImage.sprite = mission1Image;
                //mainImage.sprite = mission1Image;
                missionName.text = "Sainte-Mére-Eglise";
                ResetColors();
                mission1Image.color = Color.gray;
                break;
            case 1:
                missionChosed = 1;
                // missionImage.sprite = mission2Image;
                //mainImage.sprite = mission2Image;
                missionName.text = "Sainte-Marie-du-Mont";
                ResetColors();
                mission2Image.color = Color.gray;
                break;
            case 2:
                missionChosed = 2;
                //missionImage.sprite = mission3Image;
                //mainImage.sprite = mission3Image;
                missionName.text = "Caen";
                ResetColors();
                mission3Image.color = Color.gray;
                break;
        }
    }

    private void ResetColors()
    {
        mission1Image.color = Color.white;
        mission2Image.color = Color.white;
    }
}
