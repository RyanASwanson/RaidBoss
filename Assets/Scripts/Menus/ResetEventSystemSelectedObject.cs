using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetEventSystemSelectedObject : MonoBehaviour
{
    [SerializeField] private bool _doesOnDestroyResetSelectedEventObject;
    
    public void ResetSelectedEventSystemObject()
    {
        if (EventSystem.current.IsUnityNull())
        {
            return;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDestroy()
    {
        if (_doesOnDestroyResetSelectedEventObject)
        {
            ResetSelectedEventSystemObject();
        }
    }
}
