using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _howToPlayButton;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitButton;

    [Space]
    [Header("HowToPlay")]
    [SerializeField] private GameObject _howToPlayCanvas;

    [Space]
    [SerializeField] private GameObject _universalSublayer;

    [SerializeField] private GameObject _selectionSublayer;
    [SerializeField] private GameObject _bossMechanicsSublayer;
    [SerializeField] private GameObject _heroMechanicsSublayer;
    [SerializeField] private GameObject _generalSublayer;
    private GameObject _currentSublayer;

    [Space]
    [Header("Controls")]
    [SerializeField] private GameObject _controlsCanvas;

    [Space]
    [Header("Options")]
    [SerializeField] private GameObject _optionsCanvas;

    [SerializeField] private OptionsMenu _optionsMenu;

    [Space]
    [Header("Credits")]
    [SerializeField] private GameObject _creditsCanvas;

    private const int MAIN_MENU_MUSIC_ID = 0;

    private void Start()
    {
        PlayMainMenuMusic();
    }
    
    // Plays the music associated with the boss fight
    private void PlayMainMenuMusic()
    {
        AudioManager.Instance.PlayMusic(MAIN_MENU_MUSIC_ID, false);
    }
    
    #region Buttons
    public void PlayButtonPressed()
    {
        SceneLoadManager.Instance.LoadSelectionScene();
    }

    #region How To Play
    public void HowToPlayButtonPressed()
    {
        _howToPlayCanvas.SetActive(true);
    }

    public void HideHowToPlayButton()
    {
        _howToPlayCanvas.SetActive(false);
    }

    #region How To Play Sublayers

    public void ShowHowToPlaySublayer(GameObject sublayer)
    {
        sublayer.SetActive(true);
        _universalSublayer.SetActive(true);

        _currentSublayer = sublayer;
    }

    public void CloseHowToPlaySublayer()
    {
        if (_currentSublayer.IsUnityNull())
        {
            return;
        }

        _currentSublayer.SetActive(false);
        _universalSublayer.SetActive(false);
    }
    #endregion

    #endregion

    #region Controls
    public void ControlsButtonPressed()
    {
        _controlsCanvas.SetActive(true);
    }

    public void HideControlsButton()
    {
        _controlsCanvas.SetActive(false);
    }
    #endregion

    #region Options
    public void OptionsButtonPressed()
    {
        _optionsCanvas.SetActive(true);

        _optionsMenu.OptionsMenuOpened();
    }

    public void HideOptionsButton()
    {
        _optionsCanvas.SetActive(false);
    }

    /// <summary>
    /// Informs the save manager to reset all save data
    /// </summary>
    public void ResetSaveData()
    {
        SaveManager.Instance.ResetSaveData();
    }
    #endregion

    #region Credits
    public void CreditsButtonPressed()
    {
        _creditsCanvas.SetActive(true);
    }

    public void HideCreditsButton()
    {
        _creditsCanvas.SetActive(false);
    }
    #endregion

    public void QuitButtonPressed()
    {
        Application.Quit();
    }
    #endregion
}
