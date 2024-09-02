using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SB_TerraLord : SpecificBossFramework
{
    [Space]

    [Header("Unstable Precipice")]
    [SerializeField] private float _passiveTickRate;

    [SerializeField] private float _passiveMaxValue;

    [SerializeField] private float _zRotationMultiplier;

    [Space]

    [SerializeField] private List<float> _difficultyWeightMultiplier;
    private float _passiveHeroWeightMultiplier;

    private float _passiveCounterValue = 0;


    private HeroesManager _heroesManager;
    private Coroutine _passiveProcessCoroutine;

    //Invokes the passive counter value scaled from -1 to 1
    private UnityEvent<float> _passivePercentUpdated = new UnityEvent<float>();



    #region Passive

    private void StartPassiveProcess()
    {
        SetStartingPassiveWeightMultiplier();

        if (_passiveProcessCoroutine != null) return;

        _passiveProcessCoroutine = StartCoroutine(PassiveProcess());
    }

    private void SetStartingPassiveWeightMultiplier()
    {
        GameDifficulty selectedDifficulty = UniversalManagers.Instance.GetSelectionManager().GetSelectedDifficulty();

        _passiveHeroWeightMultiplier = _difficultyWeightMultiplier[(int)selectedDifficulty-1];
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

        RotateCameraBasedOnPassive(val);
        InvokePassivePercentUpdate();

        CheckPassiveMax();
    }


    
    private float GetPassiveCounterPercent()
    {
        return Mathf.Clamp(_passiveCounterValue / _passiveMaxValue, -_passiveMaxValue, _passiveMaxValue);
    }

    private void StopPassiveProcess()
    {
        if (_passiveProcessCoroutine == null) return;

        StopCoroutine(_passiveProcessCoroutine);
        _passiveProcessCoroutine = null;
    }

    private void RotateCameraBasedOnPassive(float passiveDifference)
    {
        GameplayManagers.Instance.GetCameraManager().StartRotateCinemachineCamera
            (passiveDifference * _zRotationMultiplier, _passiveTickRate);
    }

    private void CheckPassiveMax()
    {
        if (Mathf.Abs(_passiveCounterValue) >= _passiveMaxValue)
            PassiveMax();
    }

    private void PassiveMax()
    {
        //GameplayManagers.Instance.GetGameStateManager().SetGameplayState(GameplayStates.PostBattleLost);
        GameplayManagers.Instance.GetHeroesManager().KillAllHeroes();
    }
    #endregion

    #region Base Boss
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
    #endregion

    #region Events


    private void InvokePassivePercentUpdate()
    {
        _passivePercentUpdated?.Invoke(GetPassiveCounterPercent());
    }

    #endregion

    #region Getters
    public UnityEvent<float> GetPassivePercentUpdatedEvent() => _passivePercentUpdated;
    #endregion
}
