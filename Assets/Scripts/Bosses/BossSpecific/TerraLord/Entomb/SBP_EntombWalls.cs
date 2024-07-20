using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_EntombWalls : BossProjectileFramework
{
    [SerializeField] private float _wallDuration;

    [Space]
    [SerializeField] private Animator _wallAnimator;
    private const string _wallEndTrigger = "WallRemoval";

    public override void SetUpProjectile(BossBase bossBase)
    {
        StartCoroutine(RemovalProcess());
        base.SetUpProjectile(bossBase);
    }

    private IEnumerator RemovalProcess()
    {
        yield return new WaitForSeconds(_wallDuration);
        StartRemovalAnimation();
    }

    private void StartRemovalAnimation()
    {
        _wallAnimator.SetTrigger(_wallEndTrigger);
    }
}
