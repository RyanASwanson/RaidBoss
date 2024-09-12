using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacialLordSelfMinionHit : MonoBehaviour
{
    [SerializeField] private bool _contactOnCollision;

    [Space]
    [SerializeField] private Vector3 _halfExtents;
    [SerializeField] private float _detectionDistance;


    [SerializeField] private LayerMask _minionLayer;



    public bool MinionContactInRay()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit rayHit, _detectionDistance, _minionLayer))
        {
            rayHit.collider.GetComponentInParent<GlacialLord_FrostFiend>().FreezeMinion();
            return true;
        }
        return false;
    }


    public bool MinionContactInSquare()
    {


        if (Physics.BoxCast(transform.position, _halfExtents, transform.forward, out RaycastHit rayHit,
            Quaternion.identity, _detectionDistance, _minionLayer))
        {
            print("hit");
            rayHit.collider.GetComponentInParent<GlacialLord_FrostFiend>().FreezeMinion();
            print("hitminion");
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!_contactOnCollision)
            return;
    }
}
