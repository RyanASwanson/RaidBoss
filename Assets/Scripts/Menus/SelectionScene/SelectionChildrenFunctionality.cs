using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a framework for the selection scene scripts
/// </summary>
public abstract class SelectionChildrenFunctionality : MonoBehaviour
{
    public virtual void ChildFuncSetup()
    {
        SubscribeToEvents();
    }
    public abstract void SubscribeToEvents();
}
