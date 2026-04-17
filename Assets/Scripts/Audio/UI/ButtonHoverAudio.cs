using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverAudio : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private bool _requiresButtonInteractable;
    private Button _associatedButton;
    
    [SerializeField] private int[] _buttonHoverEnterAudioID;

    private bool _canPlayHoverAudio = true;

    private void Start()
    {
        if (_requiresButtonInteractable)
        {
            _associatedButton = GetComponent<Button>();
        }
    }
    
    // Called when the mouse pointer enters the UI element area
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayButtonHoverSound();
    }

    public void PlayButtonHoverSound()
    {
        if (_requiresButtonInteractable && !_associatedButton.interactable)
        {
            return;
        }
        
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
    
    #region Setters

    public void SetCanPlayHoverAudio(bool canPlayHoverAudio)
    {
        _canPlayHoverAudio = canPlayHoverAudio;
    }
    #endregion
}
