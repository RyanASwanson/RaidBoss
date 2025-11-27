using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPressAudio : MonoBehaviour
{
    [SerializeField] private int[] _buttonPressAudioID;

    private Button _associatedButton;
    
    // Start is called before the first frame update
    void Start()
    {
        _associatedButton = GetComponent<Button>();
        _associatedButton.onClick.AddListener(PlayButtonPressedSound);
    }

    void OnDestroy()
    {
        _associatedButton.onClick.RemoveListener(PlayButtonPressedSound);
    }

    public void PlayButtonPressedSound()
    {
        int audioID;
        if (_buttonPressAudioID.Length > 1)
        {
            audioID = Random.Range(0, _buttonPressAudioID.Length-1);
        }
        else
        {
            audioID = _buttonPressAudioID[0];
        }
            
        AudioManager.Instance.PlaySpecificAudio
            (AudioManager.Instance.UserInterfaceAudio.ButtonUserInterfaceAudio.ButtonPressedAudio[audioID]);
    }
}
