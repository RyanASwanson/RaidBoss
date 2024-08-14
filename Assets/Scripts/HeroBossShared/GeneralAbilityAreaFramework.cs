using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class GeneralAbilityAreaFramework : MonoBehaviour
{
    [SerializeField] private List<Collider> _areaColliders;

    [SerializeField] private bool _hasLifetime;
    [SerializeField] private float _lifetime;

    [SerializeField] private UnityEvent _lifeTimeEndEvent;

    // Start is called before the first frame update

    protected virtual void Start()
    {
        StartLifeTime();
    }

    /// <summary>
    /// Remove the area after its life time if it has one
    /// </summary>
    protected virtual void StartLifeTime()
    {
        if (_hasLifetime)
            StartCoroutine(LifeTimeDestruction());
    }

    protected virtual IEnumerator LifeTimeDestruction()
    {
        yield return new WaitForSeconds(_lifetime);
        _lifeTimeEndEvent?.Invoke();
        Destroy(gameObject);
    }


    public void ToggleProjectileCollider(bool colliderEnabled)
    {
        foreach (Collider col in _areaColliders)
            col.enabled = colliderEnabled;
    }

    protected virtual IEnumerator DisableColliderForDuration(float duration)
    {
        ToggleProjectileCollider(false);
        yield return new WaitForSeconds(duration);
        ToggleProjectileCollider(true);
    }

    public void ForceDestroyProjectileWithoutVFX()
    {
        Destroy(gameObject);
    }
}
