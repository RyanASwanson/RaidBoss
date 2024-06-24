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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            print("U");
            foreach (float a in _heroPastHealthValues[0])
            {
                Debug.Log(a);
            }
        }

    }


    #region Basic Abilities

    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
        IncreaseCurrentAttackRotation();
    }

    protected void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, myHeroBase.transform.position, Quaternion.identity);
        spawnedProjectile.GetComponent<HeroProjectileFramework>().SetUpProjectile(myHeroBase);

        Physics.IgnoreCollision(myHeroBase.GetHeroDamageCollider(), 
            spawnedProjectile.GetComponentInChildren<Collider>(), true);
        spawnedProjectile.GetComponent<SHP_ChronoDamageProjectile>().AdditionalSetup(_currentAttackDirection);
    }

    private void IncreaseCurrentAttackRotation()
    {
        _currentAttackDirection = Quaternion.Euler(_attackRotationIncrease) * _currentAttackDirection;
    }


    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        Debug.Log("start manual chrono");
        base.ActivateManualAbilities(attackLocation);

        Debug.Log("Activate manual chrono");

        int counter = 0;
        foreach (HeroBase heroBase in GameplayManagers.Instance.GetHeroesManager().GetCurrentHeroes())
        {
            RevertHeroHealth(counter, heroBase);
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

        heroBase.GetHeroStats().HealHero(healthDifference);
    }

    private void AddPastHealthValue(int heroID, float healthValue)
    {
        _heroPastHealthValues[heroID].Enqueue(healthValue);
        StartCoroutine(RemovePastHealthValueProcess(heroID));
    }

    private void AddHeroHealthValue(float heroID)
    {
        //print("h ID " + heroID);
        AddPastHealthValue(0, GameplayManagers.Instance.GetHeroesManager()
            .GetCurrentHeroes()[(int)heroID].GetHeroStats().GetPreviousHealth());
    }
/*    private void AddHeroTwoHealthValue(float healthVal)
    {
        AddPastHealthValue(1, healthVal);
    }
    private void AddHeroThreeHealthValue(float healthVal)
    {
        AddPastHealthValue(2, healthVal);
    }
    private void AddHeroFourHealthValue(float healthVal)
    {
        AddPastHealthValue(3, healthVal);
    }
    private void AddHeroFiveHealthValue(float healthVal)
    {
        AddPastHealthValue(4, healthVal);
    }*/

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

    public override void ActivatePassiveAbilities()
    {
        throw new System.NotImplementedException();
    }


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
}
