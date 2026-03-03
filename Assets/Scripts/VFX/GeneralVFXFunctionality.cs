using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains general functionality for visual effects to use
/// </summary>
public class GeneralVFXFunctionality : MonoBehaviour
{
    [SerializeField] private bool _hasLifeTime;
    [SerializeField] private float _lifeTime;

    [Space]
    [Tooltip("If the visual effect detaches from its parent")]
    [SerializeField] private bool _hasDetachTime;
    [Tooltip("The time before the visual effect detaches from its parent")]
    [SerializeField] private float _detachTime;

    [Space] 
    [SerializeField] private bool _hasCustomEmmisionRateCurve;
    [SerializeField] private float _customEmissionCurveMultiplier;
    [SerializeField] private AnimationCurve _customEmissionRateCurve;
    private float[] _startingParticleSystemsEmissionRates;

    [Space]
    [SerializeField] private List<ParticleSystem> _particleSystems;

    // Start is called before the first frame update
    void Start()
    {
        if (_hasCustomEmmisionRateCurve)
        {
            _startingParticleSystemsEmissionRates = new float[_particleSystems.Count];
            for (int i = 0; i < _particleSystems.Count; i++)
            {
                _startingParticleSystemsEmissionRates[i] = _particleSystems[i].emission.rateOverTimeMultiplier;
            }
        }
        
        if (_hasLifeTime)
        {
            DestroyOverLifeTime();
        }

        if (_hasDetachTime)
        {
            if (_detachTime == 0)
            {
                DetachVisualEffect();
            }
            else
            {
                StartCoroutine(DetachProcess());
            }
        }
    }

    #region Visual Effect Destruction

    public void StartDelayedLifetime()
    {
        DestroyOverLifeTime();
    }

    private void DestroyOverLifeTime()
    {
        Destroy(gameObject, _lifeTime);
    }

    #endregion

    #region Visual Effect Detaching
    /// <summary>
    /// Waits before detaching the visual effect from its parent
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetachProcess()
    {
        yield return new WaitForSeconds(_detachTime);
        DetachVisualEffect();
    }

    /// <summary>
    /// Detaches the visual effect from its parent
    /// </summary>
    public void DetachVisualEffect()
    {
        transform.SetParent(null);
    }
    
    #endregion

    public void PlayAllParticleSystems()
    {
        foreach (ParticleSystem ps in _particleSystems)
        {
            ps.Play();
        }
    }
    
    public void SetLoopOfParticleSystems(bool shouldLoop)
    {
        foreach(ParticleSystem ps in _particleSystems)
        {
            var psMain = ps.main;
            psMain.loop = shouldLoop;
        }
    }

    public void SetEmissionRateMultiplierWithCurve(float emissionRateMultiplier)
    {
        emissionRateMultiplier = _customEmissionRateCurve.Evaluate(emissionRateMultiplier) * _customEmissionCurveMultiplier;

        SetEmissionRateMultiplier(emissionRateMultiplier);
        
    }

    public void SetEmissionRateMultiplier(float emissionRateMultiplier)
    {
        for (int i = 0; i < _particleSystems.Count; i++)
        {
            ParticleSystem.EmissionModule emissionModule = _particleSystems[i].emission;
            emissionModule.rateOverTimeMultiplier = emissionRateMultiplier * _startingParticleSystemsEmissionRates[i];
        }
    }

    #region Getters
    public List<ParticleSystem> GetParticleSystems() => _particleSystems;
    #endregion
}
