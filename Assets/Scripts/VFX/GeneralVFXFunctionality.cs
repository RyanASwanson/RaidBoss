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
    [SerializeField] private List<ParticleSystem> _particleSystems;

    // Start is called before the first frame update
    void Start()
    {
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

    public void SetLoopOfParticleSystems(bool shouldLoop)
    {
        foreach(ParticleSystem ps in _particleSystems)
        {
            var psMain = ps.main;
            psMain.loop = shouldLoop;
        }
    }

    #region Getters
    public List<ParticleSystem> GetParticleSystems() => _particleSystems;
    #endregion
}
