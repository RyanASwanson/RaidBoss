using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_TerraLord : SpecificBossFramework
{
    [Space]

    [Header("Unstable Precipice")]
    [SerializeField] private float _passiveTickRate;

    private int _passiveCounterValue = 0;
    private const int _passiveMaxValue = 100;

    [Space]
    [SerializeField] private GameObject _passiveUI;

    private Coroutine _passiveProcessCoroutine;

    #region Passive
    private void StartPassiveProcess()
    {
        if (_passiveProcessCoroutine != null) return;

        _passiveProcessCoroutine = StartCoroutine(PassiveProcess());
    }

    private IEnumerator PassiveProcess()
    {
        yield return new WaitForSeconds(_passiveTickRate);
    }

    private void PassiveTick()
    {

    }

    private void ChangePassiveCounterValue(int val)
    {
        _passiveCounterValue += val;
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
