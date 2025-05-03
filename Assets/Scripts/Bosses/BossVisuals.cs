using System.Collections;
using UnityEngine;

public class BossVisuals : BossChildrenFunctionality
{
    private float _rotateSpeed;

    private GameObject _visualObjectBase;

    [Space]
    [SerializeField] private Animator _bossGeneralAnimator;

    private const string LEVEL_INTRO_ANIM_TRIGGER = "LevelIntroTrigger";

    private const string BOSS_DAMAGED_ANIM_TRIGGER = "BossDamaged";

    private Animator _bossSpecificAnimator;

    private const string SPECIFIC_BOSS_LEVEL_INTRO_ANIM_TRIGGER = "G_BossIntro";
    private const string BOSS_STAGGER_ANIM_TRIGGER = "G_BossStagger";
    private const string BOSS_DEATH_ANIM_TRIGGER = "G_BossDeath";


    /// <summary>
    /// Causes the boss to start looking at a desired target location
    /// </summary>
    /// <param name="lookLocation"></param>
    public void BossLookAt(Vector3 lookLocation)
    {
        StartCoroutine(LookAtProcess(lookLocation));
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

    private void BossTookDamage(float damageTaken)
    {
        BossDamagedAnimation();
    }


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

    public void StartBossSpecificAnimationTrigger(string triggerName)
    {
        if (triggerName == string.Empty) return;

        _bossSpecificAnimator.SetTrigger(triggerName);
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
    }

    private void BattleLost()
    {

    }

    public override void ChildFuncSetup(BossBase bossBase)
    {
        base.ChildFuncSetup(bossBase);

        SetVisualObjectBase(bossBase.GetSpecificBossScript().GetBossVisualBase());
        _visualObjectBase.transform.eulerAngles = new Vector3(0, 180, 0);

        BossLevelIntroAnimation();
    }

    private void SetFromSO(BossSO bossSO)
    {
        _rotateSpeed = _myBossBase.GetBossSO().GetBossRotationSpeed();

        _bossSpecificAnimator = _myBossBase.GetSpecificBossScript().GetBossSpecificAnimator();

        BossSpecificLevelIntroTrigger();
    }

    public override void SubscribeToEvents()
    {
        _myBossBase.GetSOSetEvent().AddListener(SetFromSO);

        _myBossBase.GetBossDamagedEvent().AddListener(BossTookDamage);

        _myBossBase.GetBossStaggeredEvent().AddListener(BossFullyStaggered);

        GameplayManagers.Instance.GetGameStateManager().GetBattleWonEvent().AddListener(BattleWon);
    }

    #region Getters

    #endregion

    #region Setters

    public void SetVisualObjectBase(GameObject newBase)
    {
        _visualObjectBase = newBase;
    }

    #endregion
}
