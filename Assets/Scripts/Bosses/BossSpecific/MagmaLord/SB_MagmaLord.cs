using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_MagmaLord : SpecificBossFramework
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            Debug.Log(DetermineAggroTarget().GetHeroSO().GetHeroName());

        if(Input.GetKeyDown(KeyCode.U))
        {
            HeroBase newTarget = DetermineAggroTarget();

            _currentBossAbilities[0].ActivateAbility(
                ClosestFloorSpaceOfHeroTarget(newTarget),newTarget);
        }
    }

    public override void SubscribeToEvents()
    {

    }
}
