using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameModeDisplayToggle : MonoBehaviour
{
    [SerializeField] private EGameMode _associatedGameMode;

    [Space] 
    [SerializeField] private UnityEvent _playingAssociatedGameModeEvent;
    [SerializeField] private UnityEvent _notPlayingAssociatedGameModeEvent;
        
    // Start is called before the first frame update
    void Start()
    {
        if (SelectionManager.Instance.GetSelectedGameMode() == _associatedGameMode)
        {
            InvokePlayingAssociatedGameModeEvent();
        }
        else
        {
            InvokeNotPlayingAssociatedGameModeEvent();
        }
    }

    void OnDestroy()
    {
        _playingAssociatedGameModeEvent.RemoveAllListeners();
        _notPlayingAssociatedGameModeEvent.RemoveAllListeners();
    }
    
    #region Events

    private void InvokePlayingAssociatedGameModeEvent()
    {
        _playingAssociatedGameModeEvent?.Invoke();
    }

    private void InvokeNotPlayingAssociatedGameModeEvent()
    {
        _notPlayingAssociatedGameModeEvent?.Invoke();
    }
    #endregion
}
