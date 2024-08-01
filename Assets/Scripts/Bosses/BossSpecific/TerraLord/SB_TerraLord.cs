using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_TerraLord : SpecificBossFramework
{
    [Space]

    [Header("Unstable Precipice")]
    [SerializeField] private float _passiveTickRate;

    [SerializeField] private float _passiveHeroWeightMultiplier;

    [SerializeField] private float _passiveMaxValue;

    [Space]

    [SerializeField] private List<float> _difficultyWeightMultiplier;

    private float _passiveCounterValue = 0;

    private HeroesManager _heroesManager;
    private Coroutine _passiveProcessCoroutine;

    #region Passive
    private void StartPassiveProcess()
    {
        if (_passiveProcessCoroutine != null) return;

        _passiveProcessCoroutine = StartCoroutine(PassiveProcess());
    }

    private IEnumerator PassiveProcess()
    {
        while(_heroesManager.GetCurrentLivingHeroes().Count > 0)
        {
            yield return new WaitForSeconds(_passiveTickRate);
            PassiveTick();
        }
        
    }

    private void PassiveTick()
    {
        ChangePassiveCounterValue(CalculatePassiveHeroWeight());
    }

    private float CalculatePassiveHeroWeight()
    {
        float weightCounter = 0;
        foreach (HeroBase heroBase in _heroesManager.GetCurrentLivingHeroes())
        {
            
            weightCounter += heroBase.transform.position.x * _passiveHeroWeightMultiplier;

        }

        weightCounter /= _heroesManager.GetCurrentLivingHeroes().Count;

        return weightCounter;
    }

    private void ChangePassiveCounterValue(float val)
    {
        _passiveCounterValue += val;
        //Debug.Log(_passiveCounterValue);
    }

    private void StopPassiveProcess()
    {
        if (_passiveProcessCoroutine == null) return;

        StopCoroutine(_passiveProcessCoroutine);
        _passiveProcessCoroutine = null;
    }
    #endregion

    protected override void StartFight()
    {
        base.StartFight();

        _heroesManager = GameplayManagers.Instance.GetHeroesManager();
        StartPassiveProcess();
    }

    protected override void BossStaggerOccured()
    {
        base.BossStaggerOccured();

        StopPassiveProcess();
    }

    protected override void BossNoLongerStaggeredOccured()
    {
        base.BossNoLongerStaggeredOccured();

        StartPassiveProcess();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        
    }
}
