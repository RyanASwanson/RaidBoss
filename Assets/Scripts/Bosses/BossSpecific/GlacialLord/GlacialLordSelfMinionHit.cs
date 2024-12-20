using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacialLordSelfMinionHit : MonoBehaviour
{
    [SerializeField] private bool _contactOnCollision;

    [Space]
    [SerializeField] private float _distance;

    public bool MinionContactFromDistance()
    {
        SB_GlacialLord glacialLord =(SB_GlacialLord)GameplayManagers.Instance.GetBossManager().GetBossBase().GetSpecificBossScript();

        foreach(GlacialLord_FrostFiend fiend in glacialLord.GetAllFrostFiends())
        {
            if (Vector3.Distance(transform.position, fiend.transform.position) < _distance)
            {
                //if (fiend.IsMinionFrozen()) return false;

                fiend.FreezeMinion();
                return true;
            }
                
        }

        return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!_contactOnCollision)
            return;
    }
}
