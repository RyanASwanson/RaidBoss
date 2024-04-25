using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameplayManager : MonoBehaviour
{
    public virtual void SetupGameplayManager()
    {
        SubscribeToEvents();
    }
    public abstract void SubscribeToEvents();
    public virtual void B()
    {
        return;
    }
}
