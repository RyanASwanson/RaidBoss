using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUIManager : GameUIChildrenFunctionality
{
    private HeroBase _associatedHeroBase;


    public void AssignSpecificHero(HeroBase heroBase)
    {
        _associatedHeroBase = heroBase;

        SubscribeToEvents();
    }


    public override void SubscribeToEvents()
    {
        
    }
}
