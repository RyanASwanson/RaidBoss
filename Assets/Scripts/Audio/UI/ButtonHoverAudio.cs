using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverAudio : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private int[] _buttonHoverEnterAudioID;
    
    // Called when the mouse pointer enters the UI element area
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayButtonHoverSound();
    }

    public void PlayButtonHoverSound()
    {
        int audioID;
        if (_buttonHoverEnterAudioID.Length > 1)
        {
            audioID = Random.Range(0, _buttonHoverEnterAudioID.Length-1);
        }
        else
        {
            audioID = _buttonHoverEnterAudioID[0];
        }
            
        AudioManager.Instance.PlaySpecificAudio
            (AudioManager.Instance.UserInterfaceAudio.ButtonUserInterfaceAudio.ButtonHoverOverAudio[audioID]);
    }
}
