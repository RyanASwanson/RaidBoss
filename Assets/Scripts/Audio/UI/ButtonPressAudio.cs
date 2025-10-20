using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPressAudio : MonoBehaviour
{
    [SerializeField] private int _buttonPressAudioID;
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
        AudioManager.Instance.PlaySpecificAudio
            (AudioManager.Instance.UserInterfaceAudio.ButtonUserInterfaceAudio.ButtonPressedAudio[_buttonPressAudioID]);
    }
}
