using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TrailerShotsDebugScript : MonoBehaviour
{
    /*
     * THIS SCRIPT IS ENTIRELY FOR TESTING AND SETTING UP TRAILER SHOTS
     * IT IS NOT INTENDED FOR ANY OTHER PURPOSE AND SHOULD NEVER AFFECT
     * GAMEPLAY OF AN ACTUAL BUILD
     * 
     * LEVELS OF POLISH AND QUALITY OF CODE ARE LOWER AS THIS IS
     * NOT INTENDED TO BE AFFECT THE EXPERIENCE OF THE END USER
     */
    
    public static bool IS_SHOOTING_TRAILER = false;

    public static bool IS_HIDING_HERO_FOLLOW_CANVAS = false;
    public static bool IS_HIDING_DAMAGE_NUMBERS = false;
    public static bool IS_PREVENTING_HERO_SELECTION_ON_BATTLE_START = false;
    public static bool IS_PERFORMING_METEOR_ZOOM_IN = false;
    public static bool IS_USING_CINEMATIC_GAMEPLAY_CAPTURES = false;

    public static TrailerShotsDebugScript Instance;

    [SerializeField] private CurveProgression _coverTransparency;

    [Header("Intro Spiral Boss")] [SerializeField]
    private CurveProgression _cameraRotationCurve;

    [SerializeField] private GeneralRotation _cameraRotation;

    [SerializeField] private Animator _guardianAnimator;

    [SerializeField] private CurveProgression _magmaLordMovementCurve;
    [SerializeField] private Animator _magmaLordAnimator;
    [SerializeField] private Animator _terraLordAnimator;
    [SerializeField] private Animator _glacialLordAnimator;
    [SerializeField] private Animator _thunderLordAnimator;


    [Header("Map Preview")] [SerializeField]
    private LevelSO[] _backgroundLevels;

    [Header("Level Gameplay")] [SerializeField]
    private GameObject _cameraHolder;

    [SerializeField] private GameObject _fightBackground;

    [Header("Selection Preview")] [SerializeField]
    private SelectBossLevelButton[] _bossButtons;

    [SerializeField] private DifficultyDropdown _difficultyDropdown;
    [SerializeField] private SelectHeroButton[] _heroButtons;
    [SerializeField] private MissionModifierSO[] _missionModifiers;
    [SerializeField] private SelectionPlayButton _playButton;

    [Space] [Header("All Heroes Intro")] [SerializeField]
    private Animator[] _heroAnimators;

    [SerializeField] private string[] _heroAnimations;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [Space] [Header("Title")] [SerializeField]
    private CurveProgression[] _titleTrailerShotCurves;

    [Space] [SerializeField] private float _cameraZoomInTime;
    [SerializeField] private AnimationCurve _cameraZoomInCurve;

    [Space] [SerializeField] private float _cameraMoveTime;

    [SerializeField] private Vector3 _cameraTargetLoc;
    [SerializeField] private AnimationCurve _cameraMoveCurve;

    private int _bossValue = 0;
    private int _heroValue = 0;
    private int _missionModifierValue = 0;

    private int _difficultyValue = 1;


    [Space] [SerializeField] private UnityEvent _testEvent;

    [SerializeField] private CurveProgression _textCurve;

    [SerializeField] private GameObject[] _removalObjects;

    [Space] [SerializeField] private GameObject _testObject;

    [SerializeField] private Animator _testAnimator;
    [SerializeField] private string _testAnimatorString;

    [SerializeField] private int _sceneLoadDebug;

    [SerializeField] private float _timeAdjustSpeed;
    [SerializeField] private AnimationCurve _timeAdjustCurve;
    
    private static bool _shouldHiedUI = false;


    #region TrailerShots

    private void Start()
    {
        Instance = this;
        
        if (IS_USING_CINEMATIC_GAMEPLAY_CAPTURES && SceneManager.GetActiveScene().buildIndex == 3)
        {
            GameplayCaptureSettings();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Failsafe in case I forget to remove the script
        if (!IS_SHOOTING_TRAILER)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine(IntroSpiralBossProcess());
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(MissionMapPreview());
        }

        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.PageDown))
        {
            RemoveObjects();

            if (!PlayerInputGameplayManager.Instance.IsUnityNull())
            {
                PlayerInputGameplayManager.Instance.ClearControlledHeroes();
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            StartCoroutine(SelectionScenePreview());
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            StartCoroutine(AllHeroTrailerShot());
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            StartCoroutine(StartHeroMoveIn());
        }

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            _testEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            TurnOffCursor();
        }

        if (Input.GetKeyDown(KeyCode.Keypad9))
        {

            StartCoroutine(LevelGameplaySpiral());
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ToggleShowUI();
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            StartCoroutine(GeneralShot());
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _virtualCamera.m_Lens.FieldOfView += .1f;
            _virtualCamera.m_Lens.OrthographicSize += .1f;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _virtualCamera.m_Lens.FieldOfView -= .1f;
            _virtualCamera.m_Lens.OrthographicSize -= .1f;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            SceneLoadManager.Instance.LoadSceneByID(_sceneLoadDebug);
        }

        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            FindObjectOfType<SelectionPlayButton>().PlayLevel();
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            // Not actually for the text, just reusing the variable
            //_textCurve.StartMovingUpOnCurve();

            FindObjectOfType<HeroVisuals>().HideOutline();

            FindObjectOfType<HeroBase>().transform.position = new Vector3(0, -0.222500086f, -3.5f);
            FindObjectOfType<HeroBase>().transform.rotation = Quaternion.identity;
        }



        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
        {
            SaveManager.Instance.UnlockNextMissions();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SaveManager.Instance.UnlockAllCharacters();

            SaveManager.Instance.UnlockAllMissionModifiers();
        }
    }

    private void GameplayCaptureSettings()
    {
        _virtualCamera.m_Lens.OrthographicSize = 4.5f;
        RemoveObjects();
        TurnOffCursor();
        RemoveHeroOverheadUI();
    }


    private IEnumerator IntroSpiralBossProcess()
    {
        _coverTransparency.ForceSetCurveProgress(1);
        _cameraRotation.ResetToDefaultRotation();
        _magmaLordMovementCurve.ForceSetCurveProgress(0);

        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);
        yield return new WaitForSeconds(.5f);

        _terraLordAnimator.speed = .85f;

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
        //_coverTransparency.ForceSetCurveProgress(1);
        MapController.Instance.SetCameraLocation(-999);
        foreach (SelectableMission mission in MapController.Instance.GetAllSelectableMissions())
        {
            mission.CreateBossBanner();
            mission.ForceUnlockMission();
        }

        yield return new WaitForSeconds(1);

        //_textCurve.StartMovingUpOnCurve();

        //yield return new WaitForSeconds(1.85f);

        MapController.Instance.SetCameraMoveSpeed(20);

        MapController.Instance.MoveCameraToTargetByIncrease(999);
        //_coverTransparency.StartMovingDownOnCurve();


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

    private IEnumerator LevelGameplaySpiral()
    {
        _cameraHolder.AddComponent<GeneralRotation>();
        GeneralRotation rotation = _cameraHolder.GetComponent<GeneralRotation>();
        
        if (!SB_TerraLord.Instance.IsUnityNull())
        {
            SB_TerraLord.Instance.StopPassiveProcess();

            yield return new WaitForSeconds(1f);
            _virtualCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        //_coverTransparency.ForceSetCurveProgress(1);

        _fightBackground.gameObject.transform.SetParent(_cameraHolder.transform);

        foreach (HeroBase hero in HeroesManager.Instance.GetCurrentHeroes())
        {
            hero.GetHeroVisuals().HideHeroOverheadCanvas();
        }

        yield return new WaitForSeconds(1.1f);

        rotation.SetRotationsPerSecond(new Vector3(0, -30f, 0));
        rotation.BeginRotation();

        
    }

    private IEnumerator SelectionScenePreview()
    {
        _bossValue = 0;
        _heroValue = 0;
        _difficultyValue = 0;

        WaitForSeconds displayWait = new WaitForSeconds(.4f);

        //_coverTransparency.ForceSetCurveProgress(1);

        //_difficultyDropdown.ForceSetDifficulty(0);
        IncreaseDifficulty();

        yield return new WaitForSeconds(1);

        //_textCurve.StartMovingUpOnCurve();

        //yield return new WaitForSeconds(1.85f);
        
        Time.timeScale = 1.6f;

        //_coverTransparency.StartMovingDownOnCurve();

        yield return displayWait;

        // START CHARACTER DISPLAY

        // Magma Lord
        ShowBossSelection();

        yield return displayWait;

        // Guardian
        ShowHeroSelection();

        yield return displayWait;

        // Samurai
        ShowHeroSelection();

        yield return displayWait;

        // Reaper
        HideBossSelection();
        ShowHeroSelection();

        yield return displayWait;

        // Terra Lord
        ShowBossSelection();

        ShowMissionModifierSelection();

        yield return displayWait;

        // Shaman
        IncreaseDifficulty();
        ShowHeroSelection();

        yield return displayWait;

        // Chronomancer
        HideBossSelection();
        IncreaseDifficulty();
        ShowHeroSelection();

        yield return displayWait;

        // Glacial Lord
        ShowBossSelection();

        yield return displayWait;

        // Vampire
        HideHeroSelection();
        ShowHeroSelection();

        ShowMissionModifierSelection();

        yield return displayWait;

        // Fae
        IncreaseDifficulty();
        HideHeroSelection();
        ShowHeroSelection();

        yield return displayWait;

        // Astromancer
        HideBossSelection();
        HideHeroSelection();
        ShowHeroSelection();

        yield return displayWait;

        // Thunder Lord
        ShowBossSelection();

        yield return displayWait;

        // Mirage
        HideHeroSelection();
        ShowHeroSelection();

        ShowMissionModifierSelection();

        yield return displayWait;

        // Alchemist
        HideHeroSelection();
        ShowHeroSelection();

        yield return displayWait;
        yield return new WaitForSeconds(.1f);

        Time.timeScale = 1;

        _playButton.PlayLevel();
    }

    private void ShowBossSelection()
    {
        
        _bossButtons[_bossValue].SelectBossButtonHoverBegin();
        _bossButtons[_bossValue].SelectBossLevelLeftClicked();
        _bossValue++;
        
        //FindObjectOfType<BossPillar>().MovePillar(false);

    }

    private void HideBossSelection()
    {
        _bossButtons[_bossValue-1].SelectBossLevelLeftClicked();
        _bossButtons[_bossValue-1].SelectBossButtonHoverEnd();
    }

    private void ShowHeroSelection()
    {
        _heroButtons[_heroValue].SelectHeroButtonHoverBegin();
        _heroButtons[_heroValue].SelectHeroButtonLeftClicked();
        _heroValue++;
    }

    private void HideHeroSelection()
    {
        _heroButtons[_heroValue - 5].SelectHeroButtonLeftClicked();
    }

    private void ShowMissionModifierSelection()
    {
        SelectionManager.Instance.AddMissionModifier(_missionModifiers[_missionModifierValue]);
        _missionModifierValue++;

    }

    private void IncreaseDifficulty()
    {
        _difficultyDropdown.ForceSetDifficulty(_difficultyValue);
        _difficultyValue++;
    }

    public void StartHeroMoveInCoroutine()
    {
        StartCoroutine(StartHeroMoveIn());
    }

    public IEnumerator StartHeroMoveIn()
    {
        /*while (!GameStateManager.Instance.GetHasFightBegun())
        {
            yield return null;
        }*/

        foreach (HeroBase hero in HeroesManager.Instance.GetCurrentLivingHeroes())
        {
            hero.GetPathfinding().OverrideHeroMovement(1.2f, 80, 5f);
            hero.GetPathfinding().AdjustNavMeshSize(.3f);
            hero.GetHeroStats().AdjustHeroHitboxSize(.25f);
        }
        HeroesManager.Instance.GetCurrentLivingHeroes()[0].GetPathfinding().OverrideHeroMovement(1.5f, 80, 5f);
        HeroesManager.Instance.GetCurrentLivingHeroes()[4].GetPathfinding().OverrideHeroMovement(1.5f, 80, 5f);
        

        //PlayerInputGameplayManager.Instance.HeroMoveIn();

        StartCoroutine(ZoomInCamera(.5f,_cameraZoomInTime));
        StartCoroutine(MoveCamera(_cameraTargetLoc,_cameraMoveTime));
        _coverTransparency.StartMovingUpOnCurve();
        yield return null;
    }

    private IEnumerator ZoomInCamera(float targetZoom, float zoomTime)
    {
        float progress = 0;
        float startZoom = _virtualCamera.m_Lens.OrthographicSize;

        while (true)
        {
            progress += Time.deltaTime / zoomTime;
            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, _cameraZoomInCurve.Evaluate(progress));
            yield return null;
        }
    }

    private IEnumerator MoveCamera(Vector3 target, float moveTime)
    {
        float progress = 0;
        Vector3 startLoc = _virtualCamera.transform.position;
        target += startLoc;

        while (progress < 1)
        {
            progress += Time.deltaTime / moveTime;
            _virtualCamera.transform.position = Vector3.Lerp(startLoc, target, _cameraMoveCurve.Evaluate(progress));
            yield return null;
        }
    }

    public void MeteorImpactShot()
    {
        StartCoroutine(MeteorImpactProcess());
    }

    private IEnumerator MeteorImpactProcess()
    {
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(ZoomInCamera(.5f,.8f));
        StartCoroutine(MoveCamera(new Vector3(0,-1.8f,0),.8f));

        StartCoroutine(AdjustTimeSpeed());
    }
    
    private IEnumerator AllHeroTrailerShot()
    {
        _coverTransparency.ForceSetCurveProgress(0);

        RunTestAnimationTrigger();

        //yield return new WaitForSeconds(3.2f);
        //yield return new WaitForSeconds(2.33f);
        yield return new WaitForSeconds(2.2f);
        //yield return new WaitForSeconds(2f);

        for (int i = 0; i < _heroAnimators.Length; i++)
        {
            _heroAnimators[i].SetBool("G_HeroIdle", true);
            _heroAnimators[i].SetTrigger(_heroAnimations[i]);

            yield return new WaitForSeconds(.083f);
        }

        yield return new WaitForSeconds(.5f);
        _coverTransparency.StartMovingUpOnCurve();
        _textCurve.StartMovingUpOnCurve();
    }

    private IEnumerator GeneralShot()
    {
        _testEvent?.Invoke();

        yield return null;
    }

    private IEnumerator AdjustTimeSpeed()
    {
        float progress = 0;

        while (progress < 1)
        {
            progress += Time.deltaTime / _timeAdjustSpeed;
            Time.timeScale = _timeAdjustCurve.Evaluate(progress);
            yield return null;
        }
    }

    public void ToggleShowUI()
    {
        _shouldHiedUI = !_shouldHiedUI;
    }

    public void AttemptHideUI()
    {
        if (_shouldHiedUI)
        {
            RemoveObjects();
        }
    }

    public void AttemptStopCameraSway()
    {

    }

    public void GameplayLoaded()
    {
        AttemptHideUI();
        AttemptStopCameraSway();
    }

    private void TurnOffCursor()
    {
        Cursor.visible = false;
    }

    private void RemoveObjects()
    {
        foreach (GameObject removal in _removalObjects)
        {
            removal.SetActive(false);
        }
    }

    private void RemoveHeroOverheadUI()
    {
        HeroCanvasFollow[] canvasFollows = FindObjectsOfType<HeroCanvasFollow>();
        foreach (HeroCanvasFollow canvas in canvasFollows)
        {
            if (canvas.IsUnityNull())
            {
                continue;
            }
            
            canvas.gameObject.SetActive(false);
        }
    }

    private void RunTestAnimationTrigger()
    {
        _testAnimator.SetTrigger(_testAnimatorString);
    }
    
    #endregion
    
}
