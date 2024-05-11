using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Provides a framework for all specific UI managers for gameplay
/// HeroUIManager inherits from this
/// BossUIManager inherits from this
/// </summary>
public abstract class GameUIChildrenFunctionality : MonoBehaviour
{
    public virtual void ChildFuncSetup()
    {
        SubscribeToEvents();
    }
    public abstract void SubscribeToEvents();

}
