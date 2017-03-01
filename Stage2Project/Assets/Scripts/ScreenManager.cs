using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public delegate void GameEvent();
    public static event GameEvent OnNewGame;
    public static event GameEvent OnStartGame;
    public static event GameEvent OnExitGame;

    public enum Screens {
        TitleScreen,

        GameScreen,

        ScoresScreen,

        MainMenuScreen,
        JoinGameScreen,
        JoiningScreen,

        CreateErrorScreen,
        JoinErrorScreen,
        DisconnectErrorScreen,

        NumScreens }

    private Canvas [] mScreens;
    private Screens mCurrentScreen;
    private NetworkManagerHUDCustom mNetHUD;

    [SerializeField]
    private Text[] ScoreTextElements;

    // Special menu elements
    // All of these exist in the GameScreen, but they have to be handled depending on the situation
    // Several screens could be created, but then it would be difficult to have elements that are common to all of them.

    [SerializeField]
    private GameObject EscapeMenu;

    [SerializeField]
    private GameObject LobbyHostMenu;

    [SerializeField]
    private GameObject LobbyClientMenu;

    [SerializeField]
    private GameObject DefeatPanel;

    [SerializeField]
    private GameObject TimerBar;

    void Awake()
    {
        mNetHUD = GetComponent<NetworkManagerHUDCustom>();

        GameManager.OnGameOver += GameManager_OnGameOver;
        GameManager.OnDefeat += GameManager_OnDefeat;
        GameManager.OnStart += GameManager_OnStart;
        GameManager.OnGameTimeLeftChange += GameManager_OnGameTimeLeftChange;

        mScreens = new Canvas[(int)Screens.NumScreens];
        Canvas[] screens = GetComponentsInChildren<Canvas>();
        for (int count = 0; count < screens.Length; ++count)
        {
            for (int slot = 0; slot < mScreens.Length; ++slot)
            {
                if (mScreens[slot] == null && ((Screens)slot).ToString() == screens[count].name)
                {
                    mScreens[slot] = screens[count];
                    break;
                }
            }
        }

        for (int screen = 1; screen < mScreens.Length; ++screen)
        {
            mScreens[screen].enabled = false;
        }

        mCurrentScreen = Screens.TitleScreen;
    }

    void Update()
    {
        if (mCurrentScreen == Screens.GameScreen && Input.GetKeyDown(KeyCode.Escape)) // Toggle in-game menu
        {
            EscapeMenu.SetActive(!EscapeMenu.activeSelf);
        }
    }

    public void OpenMainMenu()
    {
        TransitionTo(Screens.MainMenuScreen);
    }

    public void StartGame()
    {
        if (mNetHUD.StartHost())
        {
            if (OnNewGame != null)
            {
                OnNewGame();
            }

            TransitionTo(Screens.GameScreen);
            LobbyHostMenu.SetActive(true);
            LobbyClientMenu.SetActive(false);
            DefeatPanel.SetActive(false);
            TimerBar.transform.parent.gameObject.SetActive(false);
            EscapeMenu.SetActive(false);
        }
        else
        {
            TransitionTo(Screens.CreateErrorScreen);
        }
    }

    public void LobbyStartGame()
    {
        if (OnStartGame != null)
        {
            TimerBar.transform.parent.gameObject.SetActive(true);
            LobbyHostMenu.SetActive(false);

            OnStartGame();
        }
    }

    public void EndGame()
    {
        if (OnExitGame != null)
        {
            OnExitGame();
        }

        TransitionTo(Screens.MainMenuScreen);

        mNetHUD.StopGame();
    }

    public void SelectHost()
    {
        TransitionTo(Screens.JoinGameScreen);
    }

    public void JoinGame(InputField inputField)
    {
        TransitionTo(Screens.JoiningScreen);
        mNetHUD.StartClient(inputField.text.Length == 0 ? "localhost" : inputField.text);
    }

    public void OnGameJoined()
    {
        if (OnNewGame != null)
        {
            OnNewGame();
        }

        TransitionTo(Screens.GameScreen);
        LobbyHostMenu.SetActive(false);
        LobbyClientMenu.SetActive(true);
        DefeatPanel.SetActive(false);
        TimerBar.transform.parent.gameObject.SetActive(false);
        EscapeMenu.SetActive(false);
    }

    public void CancelJoin()
    {
        mNetHUD.OnCancelJoin();
        TransitionTo(Screens.MainMenuScreen);
    }

    public void OnJoinError()
    {
        TransitionTo(Screens.JoinErrorScreen);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnDisconnectError()
    {
        TransitionTo(Screens.DisconnectErrorScreen);
    }

    private void TransitionTo(Screens screen)
    {
        mScreens[(int)mCurrentScreen].enabled = false;
        mScreens[(int)screen].enabled = true;
        mCurrentScreen = screen;
    }

    private void GameManager_OnGameOver(int[] scores)
    {
        for (int i = 0; i < ScoreTextElements.Length; i++)
        {
            if (ScoreTextElements[i] != null)
            {
                ScoreTextElements[i].text = "-";

                if (i < scores.Length && scores[i] >= 0)
                {
                    ScoreTextElements[i].text = "" + scores[i];
                }
            }
        }

        TransitionTo(Screens.ScoresScreen);
    }

    private void GameManager_OnDefeat()
    {
        DefeatPanel.SetActive(true);
    }

    private void GameManager_OnStart()
    {
        LobbyHostMenu.SetActive(false);
        LobbyClientMenu.SetActive(false);
        TimerBar.transform.parent.gameObject.SetActive(true);
    }

    private void GameManager_OnGameTimeLeftChange(float ratio)
    {
        Vector3 barScale = TimerBar.transform.localScale;
        barScale.x = ratio;
        TimerBar.transform.localScale = barScale;
    }
}
