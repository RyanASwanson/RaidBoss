using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functionality for the shaman manual projectile
/// </summary>
public class SHP_ShamanManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [Space]

    [SerializeField] private float _vfxLightningSpawnRate;
    [SerializeField] private float _vfxLightningDuration;
    [SerializeField] private GameObject _vfxLightning;
    [SerializeField] private Transform _vfxLightningSpawnPoint;
    //private List<GameObject> _spawnedVFXLightning


    private Queue<GameObject> _targetsNotGoneTo = new Queue<GameObject>();

    private SH_Shaman _ownerShaman;

    

    /// <summary>
    /// Determines the order to go between each hero
    /// </summary>
    private void DetermineTargetOrder(GameObject totem)
    {
        //Add all living heroes to the hero object list except for the shaman
        List<GameObject> travelToObjects = new List<GameObject>();
        
        foreach(HeroBase hb in GameplayManagers.Instance.GetHeroesManager().GetCurrentLivingHeroes())
        {
            if (hb.GetSpecificHeroScript() == _mySpecificHero) continue;

            travelToObjects.Add(hb.gameObject);
        }

        if (totem != null)
            travelToObjects.Add(totem.gameObject);

        //Goes through the list of living heroes to determine which is the next target
        //Remove the hero from the list of heroObjects after find the target
        Vector3 lastCheckedLocation = _mySpecificHero.gameObject.transform.position;
        while(travelToObjects.Count != 0)
        {
            GameObject newestAddition = gameObject;
            float furthestDist = 0;

            //Goes through the list of each hero and find the farthest one
            foreach (GameObject currentHeroObj in travelToObjects)
            {
                float newDist = Vector3.Distance(lastCheckedLocation, currentHeroObj.transform.position);
                if (newDist > furthestDist)
                {
                    furthestDist = newDist;
                    newestAddition = currentHeroObj;
                }
            }

            //Add the furthest hero to the queue
            _targetsNotGoneTo.Enqueue(newestAddition);
            travelToObjects.Remove(newestAddition);
            lastCheckedLocation = newestAddition.transform.position;
        }

        //Add the shaman to the end of the queue
        _targetsNotGoneTo.Enqueue(_mySpecificHero.gameObject);
    }

    /// <summary>
    /// Moves the projectile towards the next hero in the list of heroes to visit
    /// After reaching the target hero call ProjectileReachedTargetHero
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProjectile()
    {
        //Keep moving so long as we haven't reached the end
        while(_targetsNotGoneTo.Count > 0)
        {
            //Makes sure there is a next hero in the list
            if(_targetsNotGoneTo.Peek() != null)
            {
                //Moves the projectile towards the next hero so long as it isn't too close
                if(Vector3.Distance(gameObject.transform.position,_targetsNotGoneTo.Peek().transform.position) > .2f)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        _targetsNotGoneTo.Peek().transform.position, _projectileSpeed * Time.deltaTime);

                    yield return null;
                }
                else
                {
                    ProjectileReachedTargetHero();
                }
                
            }
        }
       
        _ownerShaman = (SH_Shaman)_mySpecificHero;
        _ownerShaman.ActivatePassiveAbilities();
        Destroy(gameObject);
    }

    private IEnumerator LightningVFXSpawnProcess()
    {
        while(true)
        {
            yield return new WaitForSeconds(_vfxLightningSpawnRate);

            GameObject newestLighting = Instantiate(_vfxLightning, _vfxLightningSpawnPoint.transform);

            Destroy(newestLighting,_vfxLightningDuration);

            newestLighting.transform.rotation = Random.rotation;
        }
    }

    /// <summary>
    /// Reenable the collider and remove the current hero from the queue of heroes not gone to
    /// </summary>
    private void ProjectileReachedTargetHero()
    {
        GetComponent<GeneralHeroDamageArea>().ToggleProjectileCollider(true);

        _targetsNotGoneTo.Dequeue();
    }


    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(GameObject totem)
    {
        DetermineTargetOrder(totem);

        StartCoroutine(MoveProjectile());
        StartCoroutine(LightningVFXSpawnProcess());
    }
    #endregion
}
