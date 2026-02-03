using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls the functionality of the visuals of the boss
/// </summary>
public class BossVisuals : BossChildrenFunctionality
{
    public static BossVisuals Instance;
    
    private float _rotateSpeed;
    private Coroutine _bossLookAtCoroutine;

    private GameObject _visualObjectBase;

    [Space]
    [SerializeField] private Animator _bossGeneralAnimator;

    private const string LEVEL_INTRO_ANIM_TRIGGER = "LevelIntroTrigger";

    private const string BOSS_DAMAGED_ANIM_TRIGGER = "BossDamaged";

    private Animator _bossSpecificAnimator;

    private const string SPECIFIC_BOSS_LEVEL_INTRO_ANIM_TRIGGER = "G_BossIntro";
    private const string SPECIFIC_BOSS_IDLE_ANIM_BOOL = "G_BossIdle";
    private const string BOSS_STAGGER_ANIM_TRIGGER = "G_BossStagger";
    private const string BOSS_DEATH_ANIM_TRIGGER = "G_BossDeath";

    [Space] 
    [SerializeField] private MoveBetween _introProgression;
    
    [Space]
    [SerializeField] private float _outlineWidth;
    [SerializeField] private Color _outlineColor;
    [SerializeField] private Outline.Mode _outlineMode;
    private Outline _addedOutline;

    #region Directional Look
    /// <summary>
    /// Causes the boss to start looking at a desired target location
    /// </summary>
    /// <param name="lookLocation"></param>
    public void BossLookAt(Vector3 lookLocation)
    {
        StopBossLookAt();
        _bossLookAtCoroutine = StartCoroutine(LookAtProcess(lookLocation));
    }

    public void BossLookAt(GameObject lookTarget, float duration)
    {
        StopBossLookAt();
        _bossLookAtCoroutine = StartCoroutine(LookAtProcess(lookTarget, duration));
    }

    /// <summary>
    /// Stop the process of the boss looking at a target
    /// </summary>
    public void StopBossLookAt()
    {
        if (!_bossLookAtCoroutine.IsUnityNull())
        {
            StopCoroutine(_bossLookAtCoroutine);
        }
    }
    
    /// <summary>
    /// Rotates the boss to look at a target location
    /// </summary>
    /// <param name="lookLocation"></param>
    /// <returns></returns>
    private IEnumerator LookAtProcess(Vector3 lookLocation)
    {
        float progress = 0;
        Quaternion startingRotation = _visualObjectBase.transform.rotation;

        //Converts the position of the look location and boss location into a direction
        Vector3 lookDir = lookLocation - _visualObjectBase.transform.position;

        //Converts the direction into a quaternion
        Quaternion toRotation = Quaternion.LookRotation(lookDir);


        while (progress < 1)
        {
            //Iterates the progress
            progress += Time.deltaTime * _rotateSpeed ;

            //Lerps the rotation from the start to the end
            _visualObjectBase.transform.rotation = Quaternion.Lerp
                (startingRotation, toRotation, progress);

            //Makes certain that only the Y euler angle was adjusted
            _visualObjectBase.transform.eulerAngles = new Vector3(0, _visualObjectBase.transform.eulerAngles.y, 0);

            yield return null;
        }
        
    }

    private IEnumerator LookAtProcess(GameObject lookTarget, float duration)
    {
        float progress = 0;
        Quaternion startingRotation = _visualObjectBase.transform.rotation;
        Vector3 lookTargetLocation = lookTarget.transform.position;
        
        while (progress < duration)
        {
            if (!lookTarget.IsUnityNull())
            {
                // Store the last valid location of the target
                lookTargetLocation = lookTarget.transform.position;
            }
            
            //Converts the position of the look location and boss location into a direction
            Vector3 lookDir = lookTargetLocation - _visualObjectBase.transform.position;

            //Converts the direction into a quaternion
            Quaternion toRotation = Quaternion.LookRotation(lookDir);
            
            _visualObjectBase.transform.rotation = Quaternion.Lerp
                (startingRotation, toRotation, progress);
            
            _visualObjectBase.transform.eulerAngles = new Vector3(0, _visualObjectBase.transform.eulerAngles.y, 0);
            
            //transform.LookAt(target.transform.position);
            progress += Time.deltaTime;
            yield return null;
        }
        
    }
    #endregion
    

    private void BossTookDamage(float damageTaken)
    {
        BossDamagedAnimation();
    }

    public void StartBossSpecificAnimationBool(string boolName, bool boolStatus)
    {
        if (boolName == string.Empty)
        {
            return;
        }

        _bossSpecificAnimator.SetBool(boolName, boolStatus);
    }

    public void StartBossSpecificAnimationTrigger(string triggerName)
    {
        if (triggerName == string.Empty)
        {
            return;
        }

        _bossSpecificAnimator.SetTrigger(triggerName);
    }
    
    /*private void PlayBossIntro()
    {
        _introProgression.StartMoveProcessWithCurveProgression(Vector3.zero);
    }*/
    
    /// <summary>
    /// Starts the general boss level intro animation
    /// </summary>
    public void BossLevelIntroAnimation()
    {
        _bossGeneralAnimator.SetTrigger(LEVEL_INTRO_ANIM_TRIGGER);
    }

    /// <summary>
    /// Starts the general boss damaged animation which causes them to briefly shrink
    /// </summary>
    private void BossDamagedAnimation()
    {
        _bossGeneralAnimator.SetTrigger(BOSS_DAMAGED_ANIM_TRIGGER);
    }

    private void BossFullyStaggered()
    {
        TimeManager.Instance.BossStaggeredTimeSlow();
        BossSpecificStaggerAnimTrigger();
    }

    

    private void BossSpecificLevelIntroTrigger()
    {
        StartBossSpecificAnimationTrigger(SPECIFIC_BOSS_LEVEL_INTRO_ANIM_TRIGGER);
    }

    private void BossSpecificIdleAnimation()
    {
        StartBossSpecificAnimationBool(SPECIFIC_BOSS_IDLE_ANIM_BOOL, true);
    }

    private void BossSpecificStaggerAnimTrigger()
    {
        StartBossSpecificAnimationTrigger(BOSS_STAGGER_ANIM_TRIGGER);
    }

    private void BossSpecificDeathAnimTrigger()
    {
        StartBossSpecificAnimationTrigger(BOSS_DEATH_ANIM_TRIGGER);
    }

    private void BattleWon()
    {
        BossSpecificDeathAnimTrigger();
        //OutlineToggle(true);
    }

    private void BattleLost()
    {

    }
    
    #region BossOutline
    public void AddOutlineToBoss()
    {
        _addedOutline = _myBossBase.GetAssociatedBossObject().AddComponent<Outline>();

        _addedOutline.OutlineWidth = _outlineWidth;
        _addedOutline.OutlineColor = _outlineColor;
        _addedOutline.OutlineMode = _outlineMode;

        OutlineToggle(false);
    }

    private void OutlineToggle(bool isOutlineOn)
    {
        _addedOutline.enabled = isOutlineOn;
    }
    #endregion

    public override void ChildFuncSetUp(BossBase bossBase)
    {
        base.ChildFuncSetUp(bossBase);

        SetVisualObjectBase(bossBase.GetSpecificBossScript().GetBossVisualBase());
        _visualObjectBase.transform.eulerAngles = new Vector3(0, 180, 0);

        BossLevelIntroAnimation();
        // TODO Switch to PlayBossIntro which uses a curve progression instead of animation
        //PlayBossIntro();
        
        //AddOutlineToBoss();
    }

    private void SetFromSO(BossSO bossSO)
    {
        _rotateSpeed = _myBossBase.GetBossSO().GetBossRotationSpeed();

        _bossSpecificAnimator = _myBossBase.GetSpecificBossScript().GetBossSpecificAnimator();

        BossSpecificLevelIntroTrigger();
        
        BossSpecificIdleAnimation();
    }

    #region Base Children Functionality

    /// <summary>
    /// Establishes the instance for the Boss Visuals
    /// </summary>
    protected override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    
    public override void SubscribeToEvents()
    {
        _myBossBase.GetSOSetEvent().AddListener(SetFromSO);

        _myBossBase.GetBossDamagedEvent().AddListener(BossTookDamage);

        _myBossBase.GetBossStaggeredEvent().AddListener(BossFullyStaggered);

        GameStateManager.Instance.GetBattleWonEvent().AddListener(BattleWon);
        
        GameStateManager.Instance.GetBattleLostEvent().AddListener(BattleLost);
    }
    #endregion

    #region Getters

    #endregion

    #region Setters

    public void SetVisualObjectBase(GameObject newBase)
    {
        _visualObjectBase = newBase;
    }

    public void SetOutlineWidth(float newOutlineWidth)
    {
        if (_addedOutline.IsUnityNull())
        {
            return;
        }
        
        _addedOutline.OutlineWidth = newOutlineWidth;
    }

    public void SetOutLineColor(Color newOutLineColor)
    {
        if (_addedOutline.IsUnityNull())
        {
            return;
        }
        
        _addedOutline.OutlineColor = newOutLineColor;
    }

    public void SetOutlineMode(Outline.Mode newOutlineMode)
    {
        if (_addedOutline.IsUnityNull())
        {
            return;
        }
        
        _addedOutline.OutlineMode = newOutlineMode;
    }

    #endregion
}
