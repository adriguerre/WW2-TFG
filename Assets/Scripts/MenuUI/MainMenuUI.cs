using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
public class MainMenuUI : MonoBehaviour
{

    [Header("Main Buttons")]
    [SerializeField] private GameObject startGameObject;
    private Button startButton;
    [SerializeField] private GameObject changeDiff;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject quitButton;    
    [SerializeField] private GameObject aboutButton;
    [Header("Difficult Buttons")]
    [SerializeField] private GameObject difficultSelector;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [Header("Difficult Text")]
    [SerializeField] private TextMeshProUGUI easyText;
    [SerializeField] private TextMeshProUGUI normalText;
    [SerializeField] private TextMeshProUGUI hardText;

    [SerializeField] private TextMeshProUGUI textoBack;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadScreen;
    [SerializeField] private Image loadingBarFill;
    [SerializeField] private Image backgroundLoadingBarFill;

    [SerializeField] private Image barra1; 
    [SerializeField] private Image barra2;
    [SerializeField] private Image barra3; 
    [SerializeField] private Image barra4;

    private bool selectorActivated;

    private TextMeshProUGUI textoStart; 
    private TextMeshProUGUI textoDiff;
    private TextMeshProUGUI textoAbout;
    private TextMeshProUGUI textoOptions;  
    private TextMeshProUGUI textoQuit;

    private string diffChosen;

    // Start is called before the first frame update
    void Start()
    {
        selectorActivated = false;
        difficultSelector.SetActive(false);
        startButton = startGameObject.GetComponentInChildren<Button>();

        

        textoStart = startGameObject.GetComponentInChildren<TextMeshProUGUI>();
        textoDiff = changeDiff.GetComponentInChildren<TextMeshProUGUI>();
        textoOptions = options.GetComponentInChildren<TextMeshProUGUI>();
        textoQuit = quitButton.GetComponentInChildren<TextMeshProUGUI>();
        textoAbout = aboutButton.GetComponentInChildren<TextMeshProUGUI>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void optionSelected(int number)
    {
        //0 es el circulo de start 
        //1 es el circulo de cambiar dificultad 
        //2 es el circulo de opciones 
        //3 es el circulo de quit 
        switch (number)
        {
            case 0:
                //startCirculoAnimator.SetBool("StartFilling", true);
                textoStart.color = Color.grey;
                break;
            case 1:
                //diffCirculoAnimator.SetBool("StartFilling", true);
                textoDiff.color = Color.grey;
               break;
            case 2:
                textoOptions.color = Color.grey;
                //optionsCirculoAnimator.SetBool("StartFilling", true);
                break;
            case 3:
                textoQuit.color = Color.grey;
                //quitCirculoAnimator.SetBool("StartFilling", true); 
                break;
            case 4:
                textoAbout.color = Color.grey;
                break; 
        }
    }

    public void optionExit(int number)
    {
        //0 es el circulo de start 
        //1 es el circulo de cambiar dificultad 
        //2 es el circulo de opciones 
        //3 es el circulo de quit 
        switch (number)
        {
            case 0:
                textoStart.color = Color.white;
                //startCirculoAnimator.SetBool("StartFilling", false);
                break;
            case 1:
                textoDiff.color = Color.white;
                //diffCirculoAnimator.SetBool("StartFilling", false);
                break;
            case 2:
                textoOptions.color = Color.white;
                //optionsCirculoAnimator.SetBool("StartFilling", false);
                break;
            case 3:
                textoQuit.color = Color.white;
                //quitCirculoAnimator.SetBool("StartFilling", false);
                break;
            case 4:
                textoAbout.color = Color.white;
                break; 
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void PlayGame()
    {
        StartCoroutine(LoadSceneAsync("Mission_1"));
    }

    public void Check()
    {
        SceneManager.LoadScene("MisionSelector");
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
        startGameObject.SetActive(false);
        changeDiff.SetActive(false);
        options.SetActive(false);
        aboutButton.SetActive(false);
        barra1.enabled = false;
        barra2.enabled = false;
        barra3.enabled = false;
        barra4.enabled = false;

        quitButton.SetActive(false);
    }

    public void ShowDifficultSelector()
    {
        if (!selectorActivated)
        {
           startGameObject.SetActive(false);
            options.SetActive(false);
            aboutButton.SetActive(false);
            quitButton.SetActive(false);
            changeDiff.SetActive(false);
            selectorActivated = true;
            difficultSelector.SetActive(true);
        }
        else
        {
            aboutButton.SetActive(true);
           
            startGameObject.SetActive(true);
            options.SetActive(true);
            quitButton.SetActive(true);
            changeDiff.SetActive(true);
            selectorActivated = false;
            difficultSelector.SetActive(false);
        }
    }

    public void ClickedInImage()
    {
        if (selectorActivated)
        {
            startButton.interactable = true;
            selectorActivated = false;
            difficultSelector.SetActive(false);
        }
    }

    public void SetDifficultyEasy()
    {
        EnemyDifficulty.Instance.SetDifficult(Difficult.Easy);

        easyText.color = Color.gray;
        normalText.color = Color.white;
        hardText.color = Color.white;
        diffChosen = "easy";
    }
    public void SetDifficultyNormal()
    {
        EnemyDifficulty.Instance.SetDifficult(Difficult.Medium);
        easyText.color = Color.white;
        normalText.color = Color.gray;
        hardText.color = Color.white;
        diffChosen = "normal";
    }
    public void SetDifficultyHard()
    {
        EnemyDifficulty.Instance.SetDifficult(Difficult.Hard);
        easyText.color = Color.white;
        normalText.color = Color.white;
        hardText.color = Color.white;
        diffChosen = "hard";
    }

    public void optionDifficultySelected(int number)
    {
        
        switch (number)
        {
            case 0:
                
                easyText.color = Color.grey;
                break;
            case 1:
                normalText.color = Color.grey;
                break;
            case 2:
                hardText.color = Color.grey;
                break;
            case 3:
                textoBack.color = Color.grey;
                break;
        }
    }

    public void optionDifficultyExit(int number)
    {
        //0 es el circulo de start 
        //1 es el circulo de cambiar dificultad 
        //2 es el circulo de opciones 
        //3 es el circulo de quit 
        switch (number)
        {
            case 0:
                if (diffChosen != "easy")
                    easyText.color = Color.white;
                break;
            case 1:
                if (diffChosen != "normal")
                    normalText.color = Color.white;
                break;
            case 2:
                if (diffChosen != "hard")
                    hardText.color = Color.white;
                break;
            case 3:
                textoBack.color = Color.white; 
                break; 
        
        }
    }

}
