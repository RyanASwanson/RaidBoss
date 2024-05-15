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

        if(Input.GetKeyDown(KeyCode.I))
        {
            HeroBase newTarget = DetermineAggroTarget();

            _currentBossAbilities[1].ActivateAbility(Vector3.zero, newTarget);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            _currentBossAbilities[2].ActivateAbility(Vector3.zero, null);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _currentBossAbilities[3].ActivateAbility(Vector3.zero, null);
        }
    }

    public override void SubscribeToEvents()
    {

    }
}
