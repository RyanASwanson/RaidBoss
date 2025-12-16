using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreePlayButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        SubscribeToEvents();
        DetermineFreePlayUnlocked();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void DetermineFreePlayUnlocked()
    {
        SetButtonInteractability(SaveManager.Instance.IsFreePlayUnlocked());
    }

    private void SaveDataReset()
    {
        SetButtonInteractability(false);
    }

    private void SetButtonInteractability(bool interactable)
    {
        if (TryGetComponent(out Button freePlayButton))
        {
            freePlayButton.interactable = interactable;
        }
    }

    private void SubscribeToEvents()
    {
        SaveManager.Instance.GetOnGameplaySaveDataReset().AddListener(SaveDataReset);
    }

    private void UnsubscribeFromEvents()
    {
        SaveManager.Instance.GetOnGameplaySaveDataReset().RemoveListener(SaveDataReset);
    }
}
