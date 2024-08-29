using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUniversalManager : MonoBehaviour
{
    public virtual void SetupUniversalManager()
    {
        GetManagers();
        SubscribeToEvents();
    }

    public virtual void GetManagers()
    {

    }

    public virtual void SubscribeToEvents()
    {

    }
}
