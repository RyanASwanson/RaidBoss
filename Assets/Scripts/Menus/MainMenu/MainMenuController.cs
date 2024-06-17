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
    private int a;

    [Space]
    [Header("Controls")]
    private int b;

    [Space]
    [Header("Credits")]
    private int c;

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

    }

    public void ControlsButtonPressed()
    {

    }

    public void CreditsButtonPressed()
    {

    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }

    #endregion
}
