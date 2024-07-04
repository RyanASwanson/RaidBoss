using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_FallingMeteor : BossProjectileFramework
{
    [SerializeField] private GameObject _particlesToRemove;

    public void AdditionalSetup(GameObject target, float duration)
    {
        StartCoroutine(LookAtTarget(target));
        StartCoroutine(RemoveParticles(duration));
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

    private IEnumerator RemoveParticles(float duration)
    {
        yield return new WaitForSeconds(duration-.1f);

        _particlesToRemove.transform.SetParent(null);
    }
}
