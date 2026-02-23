using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedSurvivalMissionModifier : MissionModifierBase
{
    private void HeroDied(HeroBase hero)
    {
        HeroesManager.Instance.ForceKillAllHeroes();
    }
    
    #region BaseMissionModifier

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        HeroesManager.Instance.GetOnHeroDiedEvent().AddListener(HeroDied);
    }

    protected override void UnsubscribeFromEvents()
    {
        base.SubscribeToEvents();
        HeroesManager.Instance.GetOnHeroDiedEvent().RemoveListener(HeroDied);
    }
    #endregion
}
