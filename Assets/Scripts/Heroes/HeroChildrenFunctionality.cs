using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the framework for specific functionality of heroes
/// </summary>
public abstract class HeroChildrenFunctionality : MonoBehaviour
{
    protected HeroBase _myHeroBase;

    public virtual void ChildFuncSetup(HeroBase heroBase)
    {
        _myHeroBase = heroBase;
        SubscribeToEvents();
    }
    public virtual void SubscribeToEvents()
    {
        _myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
    }

    protected virtual void HeroSOAssigned(HeroSO heroSO)
    {

    }

}
