using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _extrasButton;
    [SerializeField] private Button _quitButton;
    
    [Space]
    [Header("Extras")]
    [SerializeField] private GameObject _controlsCanvas;

    private void Start()
    {
        PlayMainMenuMusic();
    }
    
    // Plays the music associated with the boss fight
    private void PlayMainMenuMusic()
    {
        AudioManager.Instance.PlayMusic(AudioManager.MAIN_MENU_MUSIC_ID, false);
    }
    
    #region Buttons
    public void PlayButtonPressed()
    {
        SceneLoadManager.Instance.LoadSelectionScene();
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }
    #endregion
}
