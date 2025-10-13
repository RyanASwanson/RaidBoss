using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificHeroUIFramework : MonoBehaviour
{
    protected HeroBase _myHeroBase;
    
    public virtual void SetUpHeroSpecificUIFunctionality(HeroBase heroBase)
    {
        _myHeroBase = heroBase;

        SubscribeToEvents();
    }
    
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    protected virtual void SubscribeToEvents()
    {
        
    }

    protected virtual void UnsubscribeToEvents()
    {
        
    }
}
