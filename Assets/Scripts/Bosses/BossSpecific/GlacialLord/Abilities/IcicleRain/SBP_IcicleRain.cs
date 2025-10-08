using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;

public class SBP_IcicleRain : BossProjectileFramework
{
    [SerializeField] private float _iciclesSeperationTime;
    [SerializeField] private int _iciclesPerSet;

    [SerializeField] private List<GameObject> _icicles;
    
    [Space] 
    [SerializeField] private float _impactAudioDelay;
    [Space] 
    [SerializeField] private float _impactAudioPitchIncrease;

    private IEnumerator SpikeSpawningProcess()
    {
        int setCounter = 1;
        int icicleSetsCompleted = 0;
        for (int i = 0; i <= _icicles.Count - 1; i++)
        {
            SpawnIcicle(_icicles[i]);

            setCounter++;

            if(setCounter > _iciclesPerSet)
            {
                setCounter = 1;
                icicleSetsCompleted++;
                StartCoroutine(PlayIcicleImpactSfx(icicleSetsCompleted-1));
                yield return new WaitForSeconds(_iciclesSeperationTime);
            }
            
        }
    }

    private void SpawnIcicle(GameObject icicle)
    {
        icicle.GetComponent<SBP_Icicle>().SetUpProjectile(_myBossBase, _abilityID);
        icicle.SetActive(true);
    }

    private IEnumerator PlayIcicleImpactSfx(int icicleSet)
    {
        yield return new WaitForSeconds(_impactAudioDelay);
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_IcicleRain.ICICLE_RAIN_IMPACT_AUDIO_ID], out EventInstance eventInstance);
        
        eventInstance.getPitch(out float pitch);
        eventInstance.setPitch(pitch + (_impactAudioPitchIncrease * icicleSet));
    }

    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        StartCoroutine(SpikeSpawningProcess());
    }
    #endregion
}
