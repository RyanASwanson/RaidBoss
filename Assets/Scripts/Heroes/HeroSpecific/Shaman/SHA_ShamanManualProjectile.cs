using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA_ShamanManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _attackDamageCooldown;

    private Queue<GameObject> _heroesNotGoneTo = new Queue<GameObject>();

    private Collider _projCollider;
    

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        _projCollider = GetComponentInChildren<Collider>();
    }

    public void AdditionalSetup(Vector3 initialTargetLocation)
    {
        DetermineTargetOrder();

        StartCoroutine(MoveProjectile());
    }

    private void DetermineTargetOrder()
    {
        List<GameObject> heroObjects = new List<GameObject>();
        
        foreach(HeroBase hb in GameplayManagers.Instance.GetHeroesManager().GetCurrentLivingHeroes())
        {
            if (hb.GetSpecificHeroScript() == _mySpecificHero) continue;

            heroObjects.Add(hb.gameObject);
        }

        Vector3 lastCheckedLocation = _mySpecificHero.gameObject.transform.position;
        while(heroObjects.Count != 0)
        {
            GameObject newestAddition = gameObject;
            float furthestDist = 0;

            foreach (GameObject currentHeroObj in heroObjects)
            {
                float newDist = Vector3.Distance(lastCheckedLocation, currentHeroObj.transform.position);
                if (newDist > furthestDist)
                {
                    furthestDist = newDist;
                    newestAddition = currentHeroObj;
                }
            }

            _heroesNotGoneTo.Enqueue(newestAddition);
            heroObjects.Remove(newestAddition);
            lastCheckedLocation = newestAddition.transform.position;
        }

        _heroesNotGoneTo.Enqueue(_mySpecificHero.gameObject);
    }

    private IEnumerator MoveProjectile()
    {
        while(_heroesNotGoneTo.Count > 0)
        {
            if(_heroesNotGoneTo.Peek() != null)
            {
                if(Vector3.Distance(gameObject.transform.position,_heroesNotGoneTo.Peek().transform.position) > .2f)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        _heroesNotGoneTo.Peek().transform.position, _projectileSpeed * Time.deltaTime);

                    yield return null;
                }
                else
                {
                    _heroesNotGoneTo.Dequeue();
                }
                
            }
        }

        Destroy(gameObject);
    }

    private void StartDamageCooldown()
    {
        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        _projCollider.enabled = false;
        yield return new WaitForSeconds(_attackDamageCooldown);
        _projCollider.enabled = true;
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName()))
        {
            StartDamageCooldown();

            _ownerHeroBase.GetSpecificHeroScript().DamageBoss(_mySpecificHero.GetBasicAbilityStrength());
            _ownerHeroBase.GetSpecificHeroScript().StaggerBoss(_mySpecificHero.GetBasicAbilityStagger());
        }
    }
}
