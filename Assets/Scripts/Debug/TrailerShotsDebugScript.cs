using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerShotsDebugScript : MonoBehaviour
{
    [Header("Intro Spiral Boss")] 
    [SerializeField] private CurveProgression _coverTransparency;

    [SerializeField] private CurveProgression _cameraRotationCurve;
    [SerializeField] private GeneralRotation _cameraRotation;

    [SerializeField] private Animator _guardianAnimator;

    [SerializeField] private CurveProgression _magmaLordMovementCurve;
    [SerializeField] private Animator _magmaLordAnimator;
    [SerializeField] private Animator _terraLordAnimator;
    [SerializeField] private Animator _glacialLordAnimator;
    [SerializeField] private Animator _thunderLordAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine(IntroSpiralBossProcess());
        }
    }


    private IEnumerator IntroSpiralBossProcess()
    {
        _coverTransparency.ForceSetCurveProgress(1);
        _cameraRotation.ResetToDefaultRotation();
        _magmaLordMovementCurve.ForceSetCurveProgress(0);

        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        
        _terraLordAnimator.SetBool("G_BossIdle", true);
        _terraLordAnimator.SetTrigger("EncirclingVinesUsed");
        
        _glacialLordAnimator.SetBool("G_BossIdle", true);
        _glacialLordAnimator.SetTrigger("FrostBiteUsed");
        
        _thunderLordAnimator.SetBool("G_BossIdle", true);
        _thunderLordAnimator.SetTrigger("ThunderboltUsed");
        
        yield return new WaitForSeconds(.5f);

        _coverTransparency.StartMovingDownOnCurve();
        
        _cameraRotationCurve.ForceSetCurveProgress(0);
        _cameraRotationCurve.StartMovingUpOnCurve();
        //_cameraRotation.BeginRotation();

        _guardianAnimator.SetBool("G_HeroIdle", true);

        _magmaLordAnimator.SetBool("G_BossIdle", true);
        
        
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        _magmaLordMovementCurve.StartMovingUpOnCurve();
        _magmaLordAnimator.SetTrigger("ScorchAbility");
        
        
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.85f);
        _guardianAnimator.SetTrigger("G_HeroBasic");
        _guardianAnimator.speed = .85f;
        //_guardianAnimator.SetTrigger("G_HeroManual");
    }
    #endif
}
