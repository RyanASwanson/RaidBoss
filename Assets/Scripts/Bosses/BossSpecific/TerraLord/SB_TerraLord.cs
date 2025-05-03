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

    /// <summary>
    /// At the start of the fight or after a stagger ends the boss passive is activated
    /// </summary>
    private void StartPassiveProcess()
    {
        SetStartingPassiveWeightMultiplier();

        if (_passiveProcessCoroutine != null) return;

        _passiveProcessCoroutine = StartCoroutine(PassiveProcess());
    }

    /// <summary>
    /// Sets up the initial values for the passive to function
    /// </summary>
    private void SetStartingPassiveWeightMultiplier()
    {
        //Gets the difficulty
        EGameDifficulty selectedDifficulty = SelectionManager.Instance.GetSelectedDifficulty();
        //Scales the speed of the passive based on the difficulty
        _passiveHeroWeightMultiplier = _difficultyWeightMultiplier[(int)selectedDifficulty-1];
    }

    /// <summary>
    /// The process at which the passive ticks every few set amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator PassiveProcess()
    {
        while(_heroesManager.GetCurrentLivingHeroes().Count > 0)
        {
            yield return new WaitForSeconds(_passiveTickRate);
            PassiveTick();
        }
    }

    /// <summary>
    /// Each individual tick of the passive
    /// </summary>
    private void PassiveTick()
    {
        ChangePassiveCounterValue(CalculatePassiveHeroWeight());
    }

    /// <summary>
    /// Calculates how much to increase or decrease the passive value by each tick
    /// </summary>
    /// <returns></returns>
    private float CalculatePassiveHeroWeight()
    {
        float weightCounter = 0;

        //Determines the center of mass based on how far each hero is from the center in the X
        foreach (HeroBase heroBase in _heroesManager.GetCurrentLivingHeroes())
        {
            weightCounter += heroBase.transform.position.x * _passiveHeroWeightMultiplier;
        }

        //Scales the speed of the passive with how many heroes are alive
        weightCounter /= _heroesManager.GetCurrentLivingHeroes().Count;

        return weightCounter;
    }

    /// <summary>
    /// Changes the passive value based on the input float and calls and needed functionality
    /// </summary>
    /// <param name="val"></param>
    private void ChangePassiveCounterValue(float val)
    {
        _passiveCounterValue += val;

        //Rotates the camera to demonstrate the imbalance of the arena
        RotateCameraBasedOnPassive(val);
        InvokePassivePercentUpdate();

        //Checks if the passive value is too far in either direction
        CheckPassiveMax();
    }

    /// <summary>
    /// Gets a value from -1 to 1 of how far the balance is in either direction
    /// </summary>
    /// <returns></returns>
    private float GetPassiveCounterPercent()
    {
        return Mathf.Clamp(_passiveCounterValue / _passiveMaxValue, -_passiveMaxValue, _passiveMaxValue);
    }

    /// <summary>
    /// Stops the passive from ticking
    /// </summary>
    private void StopPassiveProcess()
    {
        if (_passiveProcessCoroutine == null) return;

        StopCoroutine(_passiveProcessCoroutine);
        _passiveProcessCoroutine = null;
    }

    /// <summary>
    /// Rotates the camera to demonstrate the imbalance of the arena
    /// </summary>
    /// <param name="passiveDifference"></param>
    private void RotateCameraBasedOnPassive(float passiveDifference)
    {
        GameplayManagers.Instance.GetCameraManager().StartRotateCinemachineCamera
            (passiveDifference * _zRotationMultiplier, _passiveTickRate);
    }

    /// <summary>
    /// Checks if the passive counter value is too far in either direction
    /// </summary>
    private void CheckPassiveMax()
    {
        if (Mathf.Abs(_passiveCounterValue) >= _passiveMaxValue)
        {
            PassiveMax();
        }
    }

    /// <summary>
    /// If the passive goes too far in either direction all heroes are killed
    /// </summary>
    private void PassiveMax()
    {
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

    /// <summary>
    /// Stops the passive when the boss is staggered
    /// </summary>
    protected override void BossStaggerOccured()
    {
        base.BossStaggerOccured();

        StopPassiveProcess();
    }

    /// <summary>
    /// Resumes the passive when the boss is no longer staggered
    /// </summary>
    protected override void BossNoLongerStaggeredOccured()
    {
        base.BossNoLongerStaggeredOccured();

        StartPassiveProcess();
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
