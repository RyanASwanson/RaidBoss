using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
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
    public void QuitButtonPressed()
    {
        Application.Quit();
    }
    #endregion
}
