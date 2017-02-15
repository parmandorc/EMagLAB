using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public delegate void GameEvent();
    public static event GameEvent OnNewGame;
    public static event GameEvent OnExitGame;

    public enum Screens { TitleScreen, GameScreen, VictoryScreen, DefeatScreen, MainMenuScreen, NumScreens }

    private Canvas [] mScreens;
    private Screens mCurrentScreen;
    private NetworkManagerHUDCustom mNetHUD;

    void Awake()
    {
        mNetHUD = GetComponent<NetworkManagerHUDCustom>();
        mNetHUD.enabled = false;

        GameManager.OnVictory += GameManager_OnVictory;
        GameManager.OnDefeat += GameManager_OnDefeat;

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

    public void OpenMainMenu()
    {
        TransitionTo(Screens.MainMenuScreen);
    }

    public void StartGame()
    {
        if(OnNewGame != null)
        {
            OnNewGame();
        }

        TransitionTo(Screens.GameScreen);
    }

    public void EndGame()
    {
        if (OnExitGame != null)
        {
            OnExitGame();
        }

        TransitionTo(Screens.MainMenuScreen);
    }

    private void TransitionTo(Screens screen)
    {
        mScreens[(int)mCurrentScreen].enabled = false;
        mScreens[(int)screen].enabled = true;
        mCurrentScreen = screen;
        mNetHUD.enabled = screen != Screens.TitleScreen;
    }

    private void GameManager_OnVictory()
    {
        TransitionTo(Screens.VictoryScreen);
    }

    private void GameManager_OnDefeat()
    {
        TransitionTo(Screens.DefeatScreen);
    }
}
