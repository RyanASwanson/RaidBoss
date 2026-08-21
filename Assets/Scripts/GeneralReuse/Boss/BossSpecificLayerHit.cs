using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BossSpecificLayerHit : MonoBehaviour
{
    [SerializeField] private UnityEvent<GameObject> _onEnterBossSpecificLayer;
    [SerializeField] private UnityEvent<GameObject> _onExitBossSpecificLayer;
    [SerializeField] private UnityEvent<GameObject> _onDestructionOfBossSpecificLayer;

    private HashSet<GameObject> _hitSpecificLayers = new();
    
    private void OnTriggerEnter(Collider collider)
    {
        if (DoesColliderBelongToBossSpecificObject(collider))
        {
            // If the object hasn't been hit by this script yet   
            if (!_hitSpecificLayers.Contains(collider.gameObject))
            {
                // Check if the object already has a GeneralOnDestroy
                GeneralOnDestroy generalDestroy = collider.GetComponent<GeneralOnDestroy>();
                if (generalDestroy.IsUnityNull())
                {
                    // Add the script if the object doesn't have it
                    generalDestroy = collider.gameObject.AddComponent<GeneralOnDestroy>();
                }
                
                // Listen for the destruction of the object
                generalDestroy.GetOnDestroy().AddListener(BossSpecificLayerDestroyed);
                // Add it to the HashSet of contacted objects
                _hitSpecificLayers.Add(collider.gameObject);
            }

            HitBossSpecificLayer(_onEnterBossSpecificLayer, collider.gameObject);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (DoesColliderBelongToBossSpecificObject(collider))
        {
            HitBossSpecificLayer(_onExitBossSpecificLayer,collider.gameObject);
        }
    }

    private bool DoesColliderBelongToBossSpecificObject(Collider collider)
    {
        return TagStringData.DoesColliderBelongToBossSpecificObject(collider);
    }

    private void HitBossSpecificLayer(UnityEvent<GameObject> _bossLayerEvent, GameObject collider)
    {
        _bossLayerEvent?.Invoke(collider);
    }

    private void BossSpecificLayerDestroyed(GameObject specificLayer)
    {
        _onDestructionOfBossSpecificLayer?.Invoke(specificLayer);
    }
    
    #region Getters
    public UnityEvent<GameObject> GetOnEnterBossSpecificLayer() => _onEnterBossSpecificLayer;
    public UnityEvent<GameObject> GetOnExitBossSpecificLayer() => _onExitBossSpecificLayer;
    public UnityEvent<GameObject> GetOnDestructionOfBossSpecificLayer() => _onDestructionOfBossSpecificLayer;
    
    #endregion
}