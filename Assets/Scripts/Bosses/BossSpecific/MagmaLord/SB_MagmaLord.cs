using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_MagmaLord : SpecificBossFramework
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            Debug.Log(DetermineAggroTarget().GetHeroSO().GetHeroName());
    }

    public override void SubscribeToEvents()
    {

    }
}
