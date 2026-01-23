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
    [SerializeField] private bool _hasColliderActivationDelay;
    [SerializeField] private float _colliderActivationDelay;
    
    [Space]
    [SerializeField] private bool _hasColliderLifetime;
    [SerializeField] private float _colliderLifetime;

    [Space] 
    [SerializeField] private bool _doesHitCenteredVFXCopyRotation;
    [SerializeField] private Transform _hitCenteredVFXCopyTarget;
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
            StartLifeTime();
        }

        if (_hasColliderActivationDelay)
        {
            StartColliderActivationDelay();
        }

        if (_hasColliderLifetime)
        {
            StartColliderLifetime();
        }

    }

    public void StartLifeTime()
    {
        StartCoroutine(LifetimeDestruction());
    }

    public void StartColliderActivationDelay()
    {
        ToggleProjectileCollider(false);
        StartCoroutine(ColliderActivationDelay());
    }

    public void StartColliderLifetime()
    {
        StartCoroutine(ColliderLifetime());
    }

    protected virtual IEnumerator LifetimeDestruction()
    {
        yield return new WaitForSeconds(_lifetime);
        InvokeDestructionEvent();
        Destroy(gameObject);
    }

    protected virtual IEnumerator ColliderActivationDelay()
    {
        yield return new WaitForSeconds(_colliderActivationDelay);
        ToggleProjectileCollider(true);
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

    public void StartDisableColliderForDuration(float duration)
    {
        StartCoroutine(DisableColliderForDuration(new WaitForSeconds(duration)));
    }

    public void StartDisableColliderForDuration(WaitForSeconds waitDuration)
    {
        StartCoroutine(DisableColliderForDuration(waitDuration));
    }

    protected virtual IEnumerator DisableColliderForDuration(WaitForSeconds waitDuration)
    {
        ToggleProjectileCollider(false);
        yield return waitDuration;
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

        GameObject destructionVfx = Instantiate(_hitCenteredVFX, transform.position, Quaternion.identity);
        if (_doesHitCenteredVFXCopyRotation)
        {
            if (!_hitCenteredVFXCopyTarget.IsUnityNull())
            {
                destructionVfx.transform.localEulerAngles = _hitCenteredVFXCopyTarget.localEulerAngles;
            }
            else
            {
                destructionVfx.transform.localEulerAngles = transform.localEulerAngles;
            }
            
        }
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
