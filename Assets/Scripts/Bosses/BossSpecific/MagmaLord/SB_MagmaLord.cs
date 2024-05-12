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
            _currentBossAbilities[0].ActivateAbility(DetermineAggroTarget().transform.position);
        }
    }

    public override void SubscribeToEvents()
    {

    }
}
