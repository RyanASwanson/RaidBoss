using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Provides the functionality for the Chronomancer hero
/// </summary>
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

    [SerializeField] private float _manualTimeVariationAmount;
    [SerializeField] private float _manualTimeVariationDuration;
    
    [SerializeField] private GameObject _manualProjectile;
    private WaitForSeconds _rewindWait;
    
    private List<Queue<float>> _heroPastHealthValues = new List<Queue<float>>();
    private float[] _highestPastHealthValues;

    private float _storedTotalRewindHealing;

    private UnityEvent<float> _onStoredHealingUpdated = new UnityEvent<float>();
    
    [Space]
    [SerializeField] private float _passiveAbilityBasicCooldownReduction;

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

        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase, EHeroAbilityType.Basic);

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
        while (!gameObject.IsUnityNull() && !_storedDirectionObj.IsUnityNull())
        {
            _storedDirectionObj.transform.position = _basicDirectionOrigin.transform.position;
            yield return null;
        }
    }


    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();
        
        TimeManager.Instance.AddNewTimeVariationForDuration(_manualTimeVariationAmount,_manualTimeVariationDuration);
        
        Instantiate(_manualProjectile, Vector3.zero, Quaternion.identity);
        
        int counter = 0;

        //TODO Change this to a for loop
        //Iterate through all heroes
        foreach (HeroBase heroBase in HeroesManager.Instance.GetCurrentHeroes())
        {
            RevertHeroHealth(counter, heroBase);

            counter++;
        }
    }

    /// <summary>
    /// Reverts the hero to their highest health value in the past few seconds
    /// Amount of time it can revert back is based on the manual rewind amount
    /// </summary>
    /// <param name="heroID"></param>
    /// <param name="targetHeroBase"></param>
    private void RevertHeroHealth(int heroID, HeroBase targetHeroBase)
    {
        float currentHighestCheckedHealth = 0;

        //Iterates through all values the heroes health has been in the rewind time
        foreach(float healthValue in _heroPastHealthValues[heroID])
        {
            //Finds the highest health value
            if (healthValue > currentHighestCheckedHealth)
            {
                currentHighestCheckedHealth = healthValue;
            }
        }

        float healthDifference = 0;

        //Determines the difference between the current health and the highest found health
        if (currentHighestCheckedHealth > targetHeroBase.GetHeroStats().GetCurrentHealth())
        {
            healthDifference = currentHighestCheckedHealth - targetHeroBase.GetHeroStats().GetCurrentHealth();
        }

        if (healthDifference == 0)
        {
            return;
        }

        //Heal the hero for the difference in health
        HealTargetHero(healthDifference, targetHeroBase);
        
        //Applies the Chronomancers passive to the target hero
        PassiveReduceBasicCooldownOfHero(targetHeroBase);
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
        
        CalculateManualHealOfHero(heroID);

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
        AddPastHealthValue(heroID, HeroesManager.Instance
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
        yield return _rewindWait;
        _heroPastHealthValues[heroID].Dequeue();
        
        CalculateManualHealOfHero(heroID);
    }

    /// <summary>
    /// Adds the starting values of each hero into the list of queues
    /// </summary>
    private void AddStartingHealthValues()
    {
        _highestPastHealthValues = new float[HeroesManager.Instance.GetCurrentHeroes().Count];

        for(int i = 0; i < _highestPastHealthValues.Length; i++)
        {
            _highestPastHealthValues[i] = 0;
        }
        
        int counter = 0;

        //Iterates through all heroes
        foreach(HeroBase heroBase in HeroesManager.Instance.GetCurrentHeroes())
        {
            //Check the hero exists
            if (heroBase.IsUnityNull())
            {
                return;
            }

            //Adds a new queue of floats to the list of previous health values
            _heroPastHealthValues.Add(new Queue<float>());

            AddPastHealthValue(counter, heroBase.GetHeroStats().GetMaxHealth());
            counter++;
        }
    }

    private void CalculateManualHealOfHero(int heroID)
    {
        HeroBase targetHeroBase = HeroesManager.Instance.GetCurrentHeroes()[heroID];

        if (targetHeroBase.IsUnityNull())
        {
            Debug.Log("Chronomancer could not find hero");
            return;
        }
        
        float highestHeal = 0;
        
        Queue<float> heroQueue = _heroPastHealthValues[heroID];

        foreach (float health in heroQueue)
        {
            float healthDifference = health - targetHeroBase.GetHeroStats().GetCurrentHealth();
            
            if (healthDifference > highestHeal)
            {
                highestHeal = healthDifference;
            }
        }
        
        _highestPastHealthValues[heroID] = highestHeal;
        UpdateManualTotalStoredHealing();
    }

    private void UpdateManualTotalStoredHealing()
    {
        float total = 0;
        for (int i = 0; i < _highestPastHealthValues.Length; i++)
        {
            total += _highestPastHealthValues[i];
        }
        InvokeOnStoredHealingUpdated(total);
        //Debug.Log("The total is " + total);
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
        for(int i = 0; i < HeroesManager.Instance.GetCurrentHeroes().Count;i++)
        {
            int tempI = i;

            HeroesManager.Instance.GetCurrentHeroes()[i]
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

        _rewindWait = new WaitForSeconds(_rewindTimeAmount);
        
        //Adds the current health of all heroes to the list of queues
        AddStartingHealthValues();
        //Creates the target direction object for the basic ability
        CreateInitialTargetDirectionObject();

        //Listens for all damage taken by heroes
        SubscribeToHeroesDamagedEvents();
    }

    public override void HeroSpecificUICreated(GameObject heroSpecificUI)
    {
        base.HeroSpecificUICreated(heroSpecificUI);
        SHUI_ChronomancerUI heroUI = heroSpecificUI.GetComponent<SHUI_ChronomancerUI>();
        
        heroUI.AdditionalSetUp(this);
        heroUI.SetUpHeroSpecificUIFunctionality(_myHeroBase);
    }

    protected override void HeroDied()
    {
        base.HeroDied();
        UnsubscribeToHeroesDamagedEvents();
        Destroy(_storedDirectionObj);
    }
    #endregion
    
    #region Events

    private void InvokeOnStoredHealingUpdated(float healing)
    {
        _onStoredHealingUpdated?.Invoke(healing);
    }
    #endregion
    
    #region Getters
    public UnityEvent<float> GetOnStoredHealingUpdated() => _onStoredHealingUpdated;
    #endregion
}
