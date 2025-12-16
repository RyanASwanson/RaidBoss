using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSceneTrigger : MonoBehaviour
{
    [SerializeField] private EMusicTracks _musicTrack;
    
    // Start is called before the first frame update
    void Awake()
    {
        AudioManager.Instance.PlayMusic(_musicTrack,false);
    }
}
