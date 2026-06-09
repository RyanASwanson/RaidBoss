using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSceneTrigger : MonoBehaviour
{
    [SerializeField] private EMusicTracks _musicTrack;
    
    // Start is called before the first frame update
    void Awake()
    {
        // Wait until after the middle of the scene load
        SceneLoadManager.Instance.GetOnStartMusicOnSceneLoad().AddListener(PlayMusic);
    }

    public void PlayMusic()
    {
        SceneLoadManager.Instance.GetOnStartMusicOnSceneLoad().RemoveListener(PlayMusic);
        AudioManager.Instance.PlayMusic(_musicTrack,false);
    }
}
