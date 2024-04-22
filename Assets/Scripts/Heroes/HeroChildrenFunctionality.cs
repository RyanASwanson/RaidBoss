using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroChildrenFunctionality : MonoBehaviour
{
    public HeroBase myHeroBase;
    #region Events

    public virtual void ChildFuncSetup(HeroBase heroBase)
    {
        myHeroBase = heroBase;
        SubscribeToEvents();
    }
    public abstract void SubscribeToEvents();
    #endregion
}
