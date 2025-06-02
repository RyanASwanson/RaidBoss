using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functionality for the Glacial Lord Minions to get frozen by boss attacks
/// </summary>
public class GlacialLordSelfMinionHit : MonoBehaviour
{
    [SerializeField] private bool _contactOnCollision;

    [Space]
    [SerializeField] private float _distance;

    public bool MinionContactFromDistance()
    {
        //TODO Rework this line by instancing the Glacial Lord
        SB_GlacialLord glacialLord =(SB_GlacialLord)BossBase.Instance.GetSpecificBossScript();

        foreach(GlacialLord_FrostFiend fiend in glacialLord.GetAllFrostFiends())
        {
            if (Vector3.Distance(transform.position, fiend.transform.position) < _distance)
            {
                fiend.FreezeMinion();
                return true;
            }
        }

        return false;
    }

    //TODO finish this
    private void OnTriggerEnter(Collider collider)
    {
        if (!_contactOnCollision)
            return;
    }
}
