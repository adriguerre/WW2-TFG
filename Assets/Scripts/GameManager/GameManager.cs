using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{



    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameState gameState;
    [SerializeField] private GameState previousGameState;
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [SerializeField] private CanvasGroup canvasGroup;


    [SerializeField] private List<AudioClip> backgroundSongs;
    private int actualSong; 
    private AudioSource audioSource; 

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GameManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    private void Start()
    {
        int mission = MissionManager.Instance.getMissionNumber();

        if(mission == 1)
        {
            StartCoroutine(DisplayLevelText("06/06/1944 | 08:49hrs - Normandy"));
        }
        else
        {
            StartCoroutine(DisplayLevelText("10/06/1944 | 14:42hrs - Normandy"));
        }
        
        audioSource = GetComponent<AudioSource>();
    }


    private void NextSong()
    {
        if(actualSong == backgroundSongs.Count)
        {
            actualSong = 0; 
        }
        audioSource.clip =backgroundSongs[actualSong];
        audioSource.Play();
        actualSong++;
    }

    private void Update()
    {

        if (!audioSource.isPlaying)
        {
            NextSong();
        }


        //Comprobacion para abrir el menu de pausa 
        if(TryGetComponent<PauseMenu>(out PauseMenu pausa))
        {
            pauseMenu = pausa.gameObject;
        }
        switch (gameState)
        {
            case GameState.onMission:
                if (InputManager.Instance.IsEscapeKeyButtonDown())
                {
                    PauseGameMenu(); 
                }
                break;
            case GameState.restartGame:
                break;
            case GameState.gamePaused:
                if (InputManager.Instance.IsEscapeKeyButtonDown())
                {
                    PauseGameMenu();
                }
                break;
            case GameState.gameWon:
                if (InputManager.Instance.IsEscapeKeyButtonDown())
                {
                    PauseGameMenu();
                }
                break;
            case GameState.gameLost:
                if (InputManager.Instance.IsEscapeKeyButtonDown())
                {
                    PauseGameMenu();
                }
                break;
        }
    }


    public void PauseGameMenu()
    {
        if(gameState != GameState.gamePaused)
        {
            pauseMenu.SetActive(true);
            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }else if(gameState == GameState.gamePaused)
        {
            pauseMenu.SetActive(false);
            //Reactivar juego
            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
        }


    }


    public AudioSource GetAudioSource()
    {
        return GetComponent<AudioSource>();
    }


    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    //Cuando se pierda o se gane, podemos usar esto
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;
        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
    }


    public IEnumerator DisplayLevelText(string message)
    {
        //Set screen to black 
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        string messageText = message;

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        //Fade in 
        yield return StartCoroutine(Fade(1f, 0f, 1.5f, Color.black));
    }

    public IEnumerator FinishGame(string message)
    {
 
        string messageText = message;

         messageTextTMP.SetText(message);
         messageTextTMP.color = Color.white;

        //Fade in 
        yield return StartCoroutine(Fade(0f, 1f, 1.5f, Color.black));
        float time = 0;
        while (time <= 2f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("MainMenuScene");
    }
    public IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        messageTextTMP.SetText("");
    }

}
