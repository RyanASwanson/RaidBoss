using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class GameUIChildrenFunctionality : MonoBehaviour
{
    public virtual void ChildFuncSetup()
    {
        SubscribeToEvents();
    }
    public abstract void SubscribeToEvents();

}
