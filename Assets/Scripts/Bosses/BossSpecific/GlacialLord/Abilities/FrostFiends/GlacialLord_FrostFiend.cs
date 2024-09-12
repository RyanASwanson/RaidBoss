using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacialLord_FrostFiend : BossMinionBase
{
    private bool _minionFrozen;
    private float _freezeDuration;

    public void AdditionalSetup(float freezeDuration)
    {
        _freezeDuration = freezeDuration;
    }

    public void FreezeMinion()
    {
        if (_minionFrozen) return;

        _minionFrozen = true;
        StartCoroutine(FreezeProcess());
    }

    private IEnumerator FreezeProcess()
    {
        yield return new WaitForSeconds(_freezeDuration);
        UnfreezeMinion();
    }

    private void UnfreezeMinion()
    {
        _minionFrozen = false;
    }

    #region Getters
    public bool IsMinionFrozen() => _minionFrozen;
    #endregion
}
