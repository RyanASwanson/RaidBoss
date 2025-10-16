using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_FallingMeteor : BossProjectileFramework
{
    [SerializeField] private GameObject _contactParticles;
    
    [Space]
    [SerializeField] private Animator _meteorAnimator;
    
    private const string REMOVE_PROJECTILE_ANIM_TRIGGER = "RemoveMeteor";
    
    private bool _hasMeteorBeenStopped = false;
    
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

    public void StopFallingMeteor()
    {
        _hasMeteorBeenStopped = true;
        _meteorAnimator.SetTrigger(REMOVE_PROJECTILE_ANIM_TRIGGER);
    }

    #region Base Ability

    public void AdditionalSetUp(GameObject target)
    {
        StartCoroutine(LookAtTarget(target));
    }
    #endregion
}
