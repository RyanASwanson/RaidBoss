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

    public void BossLookAt(Vector3 lookLocation)
    {
        /*transform.LookAt(lookLocation);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);*/
        StartCoroutine(LookAtProcess(lookLocation));
    }

    private IEnumerator LookAtProcess(Vector3 lookLocation)
    {
        float progress = 0;
        Quaternion startingRotation = _visualObjectBase.transform.rotation;
        while (progress < 1)
        {
            progress += Time.deltaTime * _rotateSpeed ;

            Vector3 lookDir = lookLocation - _visualObjectBase.transform.position;

            //Quaternion toRotation = Quaternion.FromToRotation(_visualObjectBase.transform.forward, lookDir);
            Quaternion toRotation = Quaternion.LookRotation(lookDir);

            _visualObjectBase.transform.rotation = Quaternion.Lerp
                (startingRotation, toRotation, progress);
            /*_visualObjectBase.transform.eulerAngles = new Vector3(0, 
                Mathf.Lerp(_visualObjectBase.transform.eulerAngles.y,toRotation.eulerAngles.y,progress), 0);*/

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
        UniversalManagers.Instance.GetTimeManager().BossStaggeredTimeSlow();
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
        //_bossSpecificAnimator.SetTrigger(_bossDeathTriggerAnim);
    }

    private void BattleWon()
    {
        UniversalManagers.Instance.GetTimeManager().BossDiedTimeSlow();
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
        _rotateSpeed = myBossBase.GetBossSO().GetBossRotationSpeed();

        _bossSpecificAnimator = myBossBase.GetSpecificBossScript().GetBossSpecificAnimator();

        BossSpecificLevelIntroTrigger();
    }

    public override void SubscribeToEvents()
    {
        myBossBase.GetSOSetEvent().AddListener(SetFromSO);

        myBossBase.GetBossDamagedEvent().AddListener(BossTookDamage);

        myBossBase.GetBossStaggeredEvent().AddListener(BossFullyStaggered);
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
