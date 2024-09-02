using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_FallingMeteor : BossProjectileFramework
{
    [SerializeField] private GameObject _contactParticles;

    public void AdditionalSetup(GameObject target)
    {
        StartCoroutine(LookAtTarget(target));
    }

    private IEnumerator LookAtTarget(GameObject target)
    {
        while (true)
        {
            transform.LookAt(target.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    public void FloorContact()
    {
        Instantiate(_contactParticles, transform.position, Quaternion.identity);
    }

    #region Base Ability

    #endregion
}
