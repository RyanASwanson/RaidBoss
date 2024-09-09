using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacialLordIceCrown : MonoBehaviour
{
    [SerializeField] private Vector3 _crownFollowOffset;

    [SerializeField] private Animator _crownAnimator;

    private HeroBase _heroOwner;

    private EnvironmentManager _environmentManager;

    public void Setup()
    {

        _environmentManager = GameplayManagers.Instance.GetEnvironmentManager();
    }

    private void HeroPickUpCrown(HeroBase newOwner)
    {
        _heroOwner = newOwner;

        StartCoroutine(FollowOwner());
    }

    private IEnumerator FollowOwner()
    {
        while (!_heroOwner.GetHeroStats().IsHeroDead())
        {
            transform.position = _heroOwner.transform.position + _crownFollowOffset;
            yield return null;
        }

        DropCrown();
    }

    private void DropCrown()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("TriggerEnter");
        if (DoesColliderBelongToHero(collision))
            HeroPickUpCrown(collision.GetComponentInParent<HeroBase>());

    }

    private bool DoesColliderBelongToHero(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetHeroHitboxTagName());
    }


    #region Getters
    public bool DoesCrownHaveOwner() => _heroOwner != null;

    public HeroBase GetCrownHeroOwner() => _heroOwner;

    public Vector3 GetClosestCrownLocationToFloor() => _environmentManager.GetClosestPointToFloor(transform.position);
    #endregion
}
