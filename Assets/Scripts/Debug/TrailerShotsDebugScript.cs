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


    [Header("Map Preview")]
    [SerializeField] private LevelSO[] _backgroundLevels;

    [Space] 
    [SerializeField] private GameObject[] _removalObjects;
    
    
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

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(MissionMapPreview());
        }

        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            RemoveObjects();
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

    private IEnumerator MissionMapPreview()
    {
        _coverTransparency.ForceSetCurveProgress(1);
        MapController.Instance.SetCameraLocation(-999);
        foreach (SelectableMission mission in MapController.Instance.GetAllSelectableMissions())
        {
            mission.CreateBossBanner();
            mission.ForceUnlockMission();
        }
        
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);

        MapController.Instance.SetCameraMoveSpeed(20);
        
        MapController.Instance.MoveCameraToTargetByIncrease(999);
        _coverTransparency.StartMovingDownOnCurve();
        

        int i = 0;
        foreach (SelectableMission mission in MapController.Instance.GetAllSelectableMissions())
        {
            i++;

            switch (i)
            {
                case 7:
                    MapController.Instance.UpdateBackground(_backgroundLevels[0]);
                    break;
                case 14:
                    MapController.Instance.UpdateBackground(_backgroundLevels[1]);
                    break;
                case 21:
                    MapController.Instance.UpdateBackground(_backgroundLevels[2]);
                    break;
                default:
                    break;
            }
            
            //mission.SelectMission();
            mission.ForceShowAllPreviews();
            yield return new WaitForSeconds(.16f);
            //mission.ShowAllPreviews();
        }

        yield return null;
    }

    private void RemoveObjects()
    {
        foreach (GameObject removal in _removalObjects)
        {
            removal.SetActive(false);
        }
    }
    #endif
}
