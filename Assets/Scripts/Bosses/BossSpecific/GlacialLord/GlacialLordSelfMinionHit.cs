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

        Vector3 startCheck = Vector3.zero;
        Vector3 endCheck = Vector3.zero;

        foreach(GlacialLord_FrostFiend fiend in glacialLord.GetAllFrostFiends())
        {
            startCheck.Set(transform.position.x,0,transform.position.z);
            endCheck.Set(fiend.transform.position.x,0,fiend.transform.position.z);
            
            if (Vector3.Distance(startCheck, endCheck) < _distance)
            {
                return fiend.FreezeMinion();
            }
        }

        return false;
    }

    //TODO finish this
    private void OnTriggerEnter(Collider collider)
    {
        if (!_contactOnCollision)
        {
            return;
        }
    }
    
    
    #region Setters
    public void MultiplyHitDistance(float multiplier) => _distance *= multiplier;
    #endregion
}
