using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Chronomancer : SpecificHeroFramework
{
    [Space]
    [SerializeField] private Vector3 _attackRotationIncrease;
    [SerializeField] private GameObject _basicProjectile;
    [Space]
    [SerializeField] private GameObject _basicDirectionOrigin;
    [SerializeField] private GameObject _basicDirectionObj;
    private GameObject _storedDirectionObj;
    

    private Vector3 _currentAttackDirection = new Vector3(0, 0, 1);

    [Space]
    [SerializeField] private float _rewindTimeAmount;
    private List<Queue<float>> _heroPastHealthValues = new List<Queue<float>>();
    private List<float> _heroPreviousCheckedHealth = new List<float>(5);
    
    [Space]
    [SerializeField] private float _passiveAbilityBasicCooldownReduction;


    private HeroesManager _heroesManager;

    /* Code is here to test the storing of data for the manual ability
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            print("Start");
            int heroID = 0;
            foreach(HeroBase hb in GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes())
            {
                string tempString = "";
                foreach (float a in _heroPastHealthValues[heroID])
                {
                    tempString += a;
                    tempString += " ";
                }
                print(hb.GetHeroSO().GetHeroName() + " " + heroID + " " + tempString);
                heroID++;
            }
        }
    }*/


    #region Basic Abilities

    /// <summary>
    /// Creates the target direction object for the basic ability
    /// </summary>
    private void CreateInitialTargetDirectionObject()
    {
        _storedDirectionObj = Instantiate(_basicDirectionObj, _basicDirectionOrigin.transform.position, Quaternion.identity);
        StartCoroutine(AttackDirectionFollow());
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
        IncreaseCurrentAttackRotation();
    }

    protected void CreateBasicAttackProjectiles()
    {
        //Spawns the projectile
        GameObject spawnedProjectile = Instantiate(_basicProjectile, _myHeroBase.transform.position, Quaternion.identity);

        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        Physics.IgnoreCollision(_myHeroBase.GetHeroDamageCollider(), 
            spawnedProjectile.GetComponentInChildren<Collider>(), true);

        spawnedProjectile.GetComponent<SHP_ChronomancerBasicProjectile>().
            AdditionalSetup(_currentAttackDirection, _passiveAbilityBasicCooldownReduction, this);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        //Performs the setup for the healing area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroHealArea>().SetUpHealingArea(_myHeroBase);
    }

    /// <summary>
    /// Rotates the direction at which the chronomancer is attacking
    /// Attacks in a clockwise pattern
    /// </summary>
    private void IncreaseCurrentAttackRotation()
    {
        _currentAttackDirection = Quaternion.Euler(_attackRotationIncrease) * _currentAttackDirection;

        //Rotates the target direction object for the basic ability
        _storedDirectionObj.transform.localEulerAngles += _attackRotationIncrease;
    }

    /// <summary>
    /// Makes the attack direction always follow the exact location of its owner
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackDirectionFollow()
    {
        while (gameObject != null && _storedDirectionObj != null)
        {
            _storedDirectionObj.transform.position = _basicDirectionOrigin.transform.position;
            yield return null;
        }
    }


    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);


        int counter = 0;

        //Iterate through all heroes
        foreach (HeroBase heroBase in GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes())
        {
            RevertHeroHealth(counter, heroBase);

            //Applies the chronomancer's passive to each hero
            PassiveReduceBasicCooldownOfHero(heroBase);

            counter++;
        }
    }

    /// <summary>
    /// Reverts the hero to their highest health value in the past few seconds
    /// Amount of time it can revert back is based on the manual rewind amount
    /// </summary>
    /// <param name="heroID"></param>
    /// <param name="heroBase"></param>
    private void RevertHeroHealth(int heroID, HeroBase heroBase)
    {
        float currentHighestCheckedHealth = 0;

        //Iterates through all values the heroes health has been in the rewind time
        foreach(float healthValue in _heroPastHealthValues[heroID])
        {
            //Finds the highest health value
            if (healthValue > currentHighestCheckedHealth)
                currentHighestCheckedHealth = healthValue;
        }

        float healthDifference = 0;

        //Determines the difference between the current health and the highest found health
        if (currentHighestCheckedHealth > heroBase.GetHeroStats().GetCurrentHealth())
        {
            healthDifference = currentHighestCheckedHealth - heroBase.GetHeroStats().GetCurrentHealth();
        }

        if (healthDifference == 0) return;

        //Heal the hero for the difference in health
        heroBase.GetHeroStats().HealHero(healthDifference);
    }

    /// <summary>
    /// Adds a health value into the list of queues at the specific hero id
    /// </summary>
    /// <param name="heroID"></param>
    /// <param name="healthValue"></param>
    private void AddPastHealthValue(int heroID, float healthValue)
    {
        //At the position of the hero id the health value is added to the end of the queue
        _heroPastHealthValues[heroID].Enqueue(healthValue);
        //Starts the process of removing that value from the queue
        StartCoroutine(RemovePastHealthValueProcess(heroID));
    }

    /// <summary>
    /// Listens for when all heroes take damage
    /// When a hero takes damage store what their health was before the damage was taken
    /// </summary>
    /// <param name="heroID"></param>
    private void AddHeroHealthValue(int heroID)
    {
        AddPastHealthValue(heroID, _heroesManager
            .GetCurrentHeroes()[heroID].GetHeroStats().GetPreviousHealth());
    }

    /// <summary>
    /// Removes a value that was added into the queue after a set amount of time
    /// Time determine by how much the manual ability rewinds by
    /// </summary>
    /// <param name="heroID"></param>
    /// <returns></returns>
    private IEnumerator RemovePastHealthValueProcess(int heroID)
    {
        yield return new WaitForSeconds(_rewindTimeAmount);
        _heroPastHealthValues[heroID].Dequeue();
    }

    /// <summary>
    /// Adds the starting values of each hero into the list of queues
    /// </summary>
    private void AddStartingHealthValues()
    {
        int counter = 0;

        //Iterates through all heroes
        foreach(HeroBase heroBase in GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes())
        {
            //Check the hero exists
            if (heroBase == null) return;

            //Adds a new queue of floats to the list of previous health values
            _heroPastHealthValues.Add(new Queue<float>());

            AddPastHealthValue(counter, heroBase.GetHeroStats().GetMaxHealth());
            _heroPreviousCheckedHealth.Add(heroBase.GetHeroStats().GetMaxHealth());
            counter++;
        }
    }

    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        
    }

    /// <summary>
    /// Makes the heroes use their basic attack sooner when affected by the
    ///     Chronomancer's passive
    /// Called when a hero is affected by either the Chronomancer's basic or manual ability
    /// </summary>
    /// <param name="heroBase"></param>
    public void PassiveReduceBasicCooldownOfHero(HeroBase heroBase)
    {
        heroBase.GetSpecificHeroScript().AddToBasicAbilityChargeTime(_passiveAbilityBasicCooldownReduction);
    }
    #endregion


    
    
    /// <summary>
    /// Goes through every hero and listens for them taking damage
    /// Each hero has an associated number which is aligned with their position
    ///     in the list of queues of health values
    /// </summary>
    public void SubscribeToHeroesDamagedEvents()
    {
        //I could've gotten this done like an hour ago, but instead this code is *dynamic*

        //Iterate through every hero
        for(int i = 0; i < _heroesManager.GetCurrentHeroes().Count;i++)
        {
            int tempI = i;
            /*GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes()[i]
                .GetHeroHealthChangedEvent().AddListener(delegate { AddHeroHealthValue(tempI); });*/

            _heroesManager.GetCurrentHeroes()[i]
                .GetHeroHealthChangedEvent().AddListener(delegate { AddHeroHealthValue(tempI); });
        }

        
    }

    private void UnsubscribeToHeroesDamagedEvents()
    {
        /*for (int i = 0; i < GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes().Count; i++)
        {
            int tempI = i;
            GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes()[i]
                .GetHeroHealthChangedEvent().RemoveListener(AddHeroHealthValue(1));
        }*/

    }

    #region Base Hero
    protected override void BattleStarted()
    {
        base.BattleStarted();
        //Adds the current health of all heroes to the list of queues
        AddStartingHealthValues();
        //Creates the target direction object for the basic ability
        CreateInitialTargetDirectionObject();

        //Listens for all damage taken by heroes
        SubscribeToHeroesDamagedEvents();
    }

    protected override void HeroDied()
    {
        base.HeroDied();
        UnsubscribeToHeroesDamagedEvents();
        Destroy(_storedDirectionObj);
    }

    protected override void GetManagers()
    {
        _heroesManager = GameplayManagers.Instance.GetHeroesManager();
        base.GetManagers();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }
    #endregion
}
