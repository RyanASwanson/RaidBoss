using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_ShamanManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;


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

    /// <summary>
    /// Determines the order to go between each hero
    /// </summary>
    private void DetermineTargetOrder()
    {
        //Add all living heroes to the hero object list except for the shaman
        List<GameObject> heroObjects = new List<GameObject>();
        
        foreach(HeroBase hb in GameplayManagers.Instance.GetHeroesManager().GetCurrentLivingHeroes())
        {
            if (hb.GetSpecificHeroScript() == _mySpecificHero) continue;

            heroObjects.Add(hb.gameObject);
        }

        //Goes through the list of living heroes to determine which is the next target
        //Remove the hero from the list of heroObjects after find the target
        Vector3 lastCheckedLocation = _mySpecificHero.gameObject.transform.position;
        while(heroObjects.Count != 0)
        {
            GameObject newestAddition = gameObject;
            float furthestDist = 0;

            //Goes through the list of each hero and find the farthest one
            foreach (GameObject currentHeroObj in heroObjects)
            {
                float newDist = Vector3.Distance(lastCheckedLocation, currentHeroObj.transform.position);
                if (newDist > furthestDist)
                {
                    furthestDist = newDist;
                    newestAddition = currentHeroObj;
                }
            }

            //Add the furthest hero to the queue
            _heroesNotGoneTo.Enqueue(newestAddition);
            heroObjects.Remove(newestAddition);
            lastCheckedLocation = newestAddition.transform.position;
        }

        //Add the shaman to the end of the queue
        _heroesNotGoneTo.Enqueue(_mySpecificHero.gameObject);
    }

    /// <summary>
    /// Moves the projectile towards the next hero in the list of heroes to visit
    /// After reaching the target hero call ProjectileReachedTargetHero
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProjectile()
    {
        //Keep moving so long as we haven't reached the end
        while(_heroesNotGoneTo.Count > 0)
        {
            //Makes sure there is a next hero in the list
            if(_heroesNotGoneTo.Peek() != null)
            {
                //Moves the projectile towards the next hero so long as it isn't too close
                if(Vector3.Distance(gameObject.transform.position,_heroesNotGoneTo.Peek().transform.position) > .2f)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        _heroesNotGoneTo.Peek().transform.position, _projectileSpeed * Time.deltaTime);

                    yield return null;
                }
                else
                {
                    ProjectileReachedTargetHero();
                }
                
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Reenable the collider and remove the current hero from the queue of heroes not gone to
    /// </summary>
    private void ProjectileReachedTargetHero()
    {
        _projCollider.enabled = true;

        _heroesNotGoneTo.Dequeue();
    }

    /// <summary>
    /// Handles collision with the boss
    /// Disables its collider on contact
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName()))
        {
            _projCollider.enabled = false;

            _ownerHeroBase.GetSpecificHeroScript().DamageBoss(_mySpecificHero.GetManualAbilityStrength());
            _ownerHeroBase.GetSpecificHeroScript().StaggerBoss(_mySpecificHero.GetManualAbilityStagger());
        }
    }
}
