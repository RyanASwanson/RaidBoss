using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroChildrenFunctionality : MonoBehaviour
{
    #region Events
    public virtual void ChildFuncSetup(HeroBase heroBase)
    {
        SubscribeToEvents(heroBase);
    }
    public abstract void SubscribeToEvents(HeroBase heroBase);
    #endregion
}
