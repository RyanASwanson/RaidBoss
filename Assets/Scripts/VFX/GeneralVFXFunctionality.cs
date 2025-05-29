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
    [SerializeField] private bool _hasDetachTime;
    [SerializeField] private float _detachTime;

    [Space]
    [SerializeField] private List<ParticleSystem> _particleSystems;

    // Start is called before the first frame update
    void Start()
    {
        if (_hasLifeTime)
            DestroyOverLifeTime();

        if (_hasDetachTime)
        {
            if (_detachTime == 0)
                Detach();
            else
                StartCoroutine(DetachProcess());
        }
            
    }

    private IEnumerator DetachProcess()
    {
        yield return new WaitForSeconds(_detachTime);
        Detach();
    }

    public void Detach()
    {
        transform.SetParent(null);
    }


    public void StartDelayedLifetime()
    {
        DestroyOverLifeTime();
    }

    private void DestroyOverLifeTime()
    {
        Destroy(gameObject, _lifeTime);
    }

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
