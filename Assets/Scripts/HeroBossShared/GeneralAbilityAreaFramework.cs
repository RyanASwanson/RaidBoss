using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class GeneralAbilityAreaFramework : MonoBehaviour
{
    [SerializeField] private List<Collider> _areaColliders;

    [SerializeField] private bool _hasLifetime;
    [SerializeField] private float _lifetime;

    [Space]

    [SerializeField] private bool _hasColliderLifetime;
    [SerializeField] private float _colliderLifetime;

    [Space]
    [SerializeField] private GameObject _hitCenteredVFX;

    [SerializeField] private UnityEvent _lifetimeEndEvent;

    [SerializeField] private UnityEvent _colliderLifetimeEndEvent;

    // Start is called before the first frame update

    protected virtual void Start()
    {
        StartLifeTimes();
    }

    /// <summary>
    /// Remove the area after its life time if it has one
    /// </summary>
    protected virtual void StartLifeTimes()
    {
        if (_hasLifetime)
        {
            StartCoroutine(LifetimeDestruction());
        }

        if (_hasColliderLifetime)
        {
            StartCoroutine(ColliderLifetime());
        }

    }

    protected virtual IEnumerator LifetimeDestruction()
    {
        yield return new WaitForSeconds(_lifetime);
        InvokeDestructionEvent();
        Destroy(gameObject);
    }

    protected virtual IEnumerator ColliderLifetime()
    {
        yield return new WaitForSeconds(_colliderLifetime);
        InvokeColliderLifetimeEvent();
        ToggleProjectileCollider(false);
    }


    public void ToggleProjectileCollider(bool colliderEnabled)
    {
        foreach (Collider col in _areaColliders)
        {
            col.enabled = colliderEnabled;
        }
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

    public void CreateDestructionVFX()
    {
        if (_hitCenteredVFX.IsUnityNull())
        {
            return;
        }

        Instantiate(_hitCenteredVFX, transform.position, Quaternion.identity);
    }

    public void DestroyProjectile()
    {
        CreateDestructionVFX();
        Destroy(gameObject);
    }

    #region Events
    private void InvokeDestructionEvent()
    {
        _lifetimeEndEvent?.Invoke();
    }
    private void InvokeColliderLifetimeEvent()
    {
        _colliderLifetimeEndEvent?.Invoke();
    }
    #endregion

    #region Getters
    public UnityEvent GetLifetimeEndEvent() => _lifetimeEndEvent;
    public UnityEvent GetColliderLifetimeEvent() => _colliderLifetimeEndEvent;
    #endregion
}
