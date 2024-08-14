using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUniversalManager : MonoBehaviour
{
    public virtual void SetupUniversalManager()
    {
        SubscribeToEvents();
    }
    public virtual void SubscribeToEvents()
    {

    }
}
