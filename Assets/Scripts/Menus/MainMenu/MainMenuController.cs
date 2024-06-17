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
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitButton;

    [Space]
    [Header("HowToPlay")]
    [SerializeField] private GameObject _howToPlayCanvas;

    [Space]
    [Header("Controls")]
    [SerializeField] private GameObject _controlsCanvas;

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

    public void ControlsButtonPressed()
    {
        _controlsCanvas.SetActive(true);
    }

    public void HideControlsButton()
    {
        _controlsCanvas.SetActive(false);
    }

    public void CreditsButtonPressed()
    {
        _creditsCanvas.SetActive(true);
    }

    public void HideCreditsButton()
    {
        _creditsCanvas.SetActive(false);
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }

    #endregion
}
