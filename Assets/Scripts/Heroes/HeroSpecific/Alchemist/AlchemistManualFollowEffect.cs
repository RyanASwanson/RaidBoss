using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AlchemistManualFollowEffect : MonoBehaviour
{
    [SerializeField] private float _potionLifetimeWarningTime;
    private WaitForSeconds _potionLifetimeWarningWait;
    
    [Space]
    [SerializeField] private MeshRenderer _potionRenderer;
    [SerializeField] private FollowObject _followObject;
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private MaterialSetCustomProperty _materialSetCustomProperty;
    [SerializeField] private CurveProgression _lifeTimeWarningCurve;
    
    public void SetUp(GameObject followHero, Material potionMaterial, float buffDuration)
    {
        _potionRenderer.material = potionMaterial;
        
        _followObject.StartFollowingObject(followHero);
        
        _scaleCurve.SetDecreaseDelay(buffDuration - (_scaleCurve.GetCurveIncreaseTime() + _scaleCurve.GetCurveDecreaseTime()));
        _scaleCurve.StartMovingUpOnCurve();
        
        _materialSetCustomProperty.SetUp();
        StartCoroutine(PotionRemoveWarningTimer(buffDuration));
    }
    
    private IEnumerator PotionRemoveWarningTimer(float buffDuration)
    {
        yield return new WaitForSeconds(buffDuration - _potionLifetimeWarningTime);
        PotionRemoveWarning();
    }

    private void PotionRemoveWarning()
    {
        _lifeTimeWarningCurve.StartMovingUpOnCurve();
    }

    public void FollowHeroMissing()
    {
        _scaleCurve.SetHasDecreaseDelay(false);
        _scaleCurve.StartMovingDownOnCurve();
    }

    public void DestroyFollowEffect()
    {
        Destroy(gameObject);
    }
}
