using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    public virtual void SetupManager()
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
