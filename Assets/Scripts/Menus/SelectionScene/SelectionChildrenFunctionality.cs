using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectionChildrenFunctionality : MonoBehaviour
{
    public virtual void ChildFuncSetup()
    {
        SubscribeToEvents();
    }
    public abstract void SubscribeToEvents();
}
