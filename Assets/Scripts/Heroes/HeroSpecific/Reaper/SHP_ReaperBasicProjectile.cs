using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the basic projectile create by
///     the Reaper
/// </summary>
public class SHP_ReaperBasicProjectile : HeroProjectileFramework 
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileYSpinSpeed;
    [SerializeField] private Vector2 _movementVariability;

    [Space]
    [SerializeField] private GameObject _childObject;

    [Space]
    [SerializeField] private Animator _hitAnimator;

    private const string HIT_ANIM_TRIGGER = "HitEnemy";
    private const string OUTRO_ANIM_TRIGGER = "Outro";

    /// <summary>
    /// Sets the position of the projectile's main object to be at the associated
    ///     hero location
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowHero()
    {
        while(!_myHeroBase.IsUnityNull())
        {
            transform.position = _myHeroBase.transform.position;
            yield return null;
        }

        StartOutroAnimation();
    }

    /// <summary>
    /// Moves the projectile in a figure eight pattern around the player.
    /// Moves in local space. The main object is not moved.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProjectile()
    {
        float time = 4.75f;
        float xPos, zPos;

        while (true)
        {
            time += Time.deltaTime * _projectileSpeed;
            xPos = Mathf.Cos(time);
            zPos = Mathf.Sin(2 * time) / 2;
            _childObject.transform.localPosition = new Vector3((xPos * _movementVariability.x),
                transform.position.y, (zPos * _movementVariability.y));
            yield return null;
        }
    }

    /// <summary>
    /// Rotates the visuals of the projectile.
    /// </summary>
    /// <param name="rotateSpeed"></param>
    /// <returns></returns>
    private IEnumerator RotateProjectile(float rotateSpeed)
    {
        while(true)
        {
            _childObject.transform.eulerAngles += new Vector3(0, rotateSpeed, 0) * Time.deltaTime;
            yield return null;
        }
    }

    public void TriggerHitVFX()
    {
        _hitAnimator.SetTrigger(HIT_ANIM_TRIGGER);
    }

    private void StartOutroAnimation()
    {
        _hitAnimator.SetTrigger(OUTRO_ANIM_TRIGGER);
    }

    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);

        StartCoroutine(FollowHero());
        StartCoroutine(MoveProjectile());
        StartCoroutine(RotateProjectile(_projectileYSpinSpeed));
    }
    #endregion
}
