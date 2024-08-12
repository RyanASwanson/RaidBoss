using System.Collections;
using System.Collections.Generic;
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
    [Header("Controls")]
    [SerializeField] private GameObject _controlsCanvas;

    [Space]
    [Header("Options")]
    [SerializeField] private GameObject _optionsCanvas;

    [SerializeField] private OptionsMenu _optionsMenu;

    [Space]
    [Header("Credits")]
    [SerializeField] private GameObject _creditsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region Buttons
    public void PlayButtonPressed()
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadSelectionScene();
    }

    public void HowToPlayButtonPressed()
    {
        _howToPlayCanvas.SetActive(true);
    }

    public void HideHowToPlayButton()
    {
        _howToPlayCanvas.SetActive(false);
    }

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

    public void ResetSaveData()
    {
        UniversalManagers.Instance.GetSaveManager().ResetSaveData();
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
