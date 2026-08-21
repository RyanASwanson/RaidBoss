using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BossSpecificLayerHit))]
public class VoidLordSelfRayHit : MonoBehaviour
{
    [SerializeField] private BossSpecificLayerHit _specificLayerHit;

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void HitSpecificBossLayer(GameObject hitObject)
    {
        BossSpecificLayerObject specificLayerObject = hitObject.GetComponent<BossSpecificLayerObject>();

        if (specificLayerObject.IsUnityNull())
        {
            return;
        }
        specificLayerObject.InvokeOnHit();
    }

    private void SubscribeToEvents()
    {
        _specificLayerHit.GetOnEnterBossSpecificLayer().AddListener(HitSpecificBossLayer);
    }

    private void UnsubscribeFromEvents()
    {
        _specificLayerHit.GetOnEnterBossSpecificLayer().RemoveListener(HitSpecificBossLayer);
    }
}
