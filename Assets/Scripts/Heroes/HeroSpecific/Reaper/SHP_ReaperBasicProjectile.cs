using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_ReaperBasicProjectile : HeroProjectileFramework 
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileYSpinSpeed;
    [SerializeField] private Vector2 _movementVariability;

    [Space]
    [SerializeField] private GameObject _childObject;

    [Space]
    [SerializeField] private Animator _hitAnimator;
    private const string _hitAnimTrigger = "HitEnemy";


    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup()
    {
        StartCoroutine(MoveProjectile(_projectileSpeed,_movementVariability));
        StartCoroutine(RotateProjectile(_projectileYSpinSpeed));
    }

    /// <summary>
    /// Moves the projectile in a figure eight pattern around the player
    /// </summary>
    /// <param name="projectileSpeed"></param>
    /// <param name="movementVariability"></param>
    /// <returns></returns>
    private IEnumerator MoveProjectile(float projectileSpeed, Vector2 movementVariability)
    {
        float time = 4.75f;
        float xPos, zPos;

        while (true)
        {
            transform.position = _ownerHeroBase.transform.position;

            time += Time.deltaTime;
            xPos = Mathf.Cos(time);
            zPos = Mathf.Sin(2 * time) / 2;
            _childObject.transform.localPosition = new Vector3((xPos * movementVariability.x),
                _ownerHeroBase.transform.position.y, (zPos * movementVariability.y));
            yield return null;
        }
    }

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
        _hitAnimator.SetTrigger(_hitAnimTrigger);
    }
}
