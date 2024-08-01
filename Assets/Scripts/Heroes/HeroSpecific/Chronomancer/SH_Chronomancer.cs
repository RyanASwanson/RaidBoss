using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Chronomancer : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;
    [SerializeField] private Vector3 _attackRotationIncrease;

    private Vector3 _currentAttackDirection = new Vector3(0, 0, 1);

    [Space]
    [SerializeField] private float _rewindTimeAmount;
    private List<Queue<float>> _heroPastHealthValues = new List<Queue<float>>();
    private List<float> _heroPreviousCheckedHealth = new List<float>(5);
    
    [Space]
    [SerializeField] private float _passiveAbilityBasicCooldownReduction;

    /*private void Update()
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

    public override bool ConditionsToActivateBasicAbilities()
    {
        return !_myHeroBase.GetPathfinding().IsHeroMoving();
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
        IncreaseCurrentAttackRotation();
    }

    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, _myHeroBase.transform.position, Quaternion.identity);

        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(_myHeroBase);

        Physics.IgnoreCollision(_myHeroBase.GetHeroDamageCollider(), 
            spawnedProjectile.GetComponentInChildren<Collider>(), true);

        spawnedProjectile.GetComponent<SHP_ChronomancerBasicProjectile>().
            AdditionalSetup(_currentAttackDirection, _passiveAbilityBasicCooldownReduction);

        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        spawnedProjectile.GetComponent<GeneralHeroHealArea>().SetUpHealingArea(_myHeroBase);
    }

    private void IncreaseCurrentAttackRotation()
    {
        _currentAttackDirection = Quaternion.Euler(_attackRotationIncrease) * _currentAttackDirection;
    }


    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);


        int counter = 0;
        foreach (HeroBase heroBase in GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes())
        {
            RevertHeroHealth(counter, heroBase);
            heroBase.GetSpecificHeroScript().AddToBasicAbilityChargeTime(_passiveAbilityBasicCooldownReduction);
            counter++;
        }
    }

    private void RevertHeroHealth(int heroID, HeroBase heroBase)
    {
        float currentHighestCheckedHealth = 0;

        foreach(float healthValue in _heroPastHealthValues[heroID])
        {
            if (healthValue > currentHighestCheckedHealth)
                currentHighestCheckedHealth = healthValue;
        }

        float healthDifference = 0;

        if (currentHighestCheckedHealth > heroBase.GetHeroStats().GetCurrentHealth())
        {
            healthDifference = currentHighestCheckedHealth - heroBase.GetHeroStats().GetCurrentHealth();
        }

        if (healthDifference == 0) return;

        heroBase.GetHeroStats().HealHero(healthDifference);
    }

    private void AddPastHealthValue(int heroID, float healthValue)
    {
        _heroPastHealthValues[heroID].Enqueue(healthValue);
        StartCoroutine(RemovePastHealthValueProcess(heroID));
    }

    private void AddHeroHealthValue(int heroID)
    {
        AddPastHealthValue(heroID, GameplayManagers.Instance.GetHeroesManager()
            .GetCurrentHeroes()[heroID].GetHeroStats().GetPreviousHealth());
    }

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
        foreach(HeroBase heroBase in GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes())
        {
            if (heroBase == null) return;

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

    #endregion


    public override void ActivateHeroSpecificActivity()
    {
        base.ActivateHeroSpecificActivity();
    }

    public override void DeactivateHeroSpecificActivity()
    {
        base.DeactivateHeroSpecificActivity();
    }

    protected override void BattleStarted()
    {
        base.BattleStarted();
        AddStartingHealthValues();

        SubscribeToHeroesDamagedEvents();
    }

    protected override void HeroDied()
    {
        base.HeroDied();
        UnsubscribeToHeroesDamagedEvents();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    public void SubscribeToHeroesDamagedEvents()
    {
        //I could've gotten this done like an hour ago, but instead this code is *dynamic*

        /*        List<HeroBase> heroBases = GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes();
                heroBases[0]?.GetHeroHealthChangedEvent().AddListener( AddHeroOneHealthValue);*/

        //Iterate through every hero
        for(int i = 0; i < GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes().Count;i++)
        {
            int tempI = i;
            GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes()[i]
                .GetHeroHealthChangedEvent().AddListener(delegate { AddHeroHealthValue(tempI); });
        }

        //GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes()[0].GetHeroHealthChangedEvent().AddListener(delegate { AddHeroHealthValue(7);});
        //if (GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes()[1] != null) print("A");
        //GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes()[1].GetHeroHealthChangedEvent().AddListener(delegate { AddHeroHealthValue(2);});
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
}
