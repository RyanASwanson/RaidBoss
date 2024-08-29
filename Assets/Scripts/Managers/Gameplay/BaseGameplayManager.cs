using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameplayManager : MonoBehaviour
{
    public virtual void SetupGameplayManager()
    {
        GetManagers();
        SubscribeToEvents();
    }

    protected virtual void GetManagers()
    {

    }

    protected virtual void SubscribeToEvents()
    {

    }
}
