using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVisuals : BossChildrenFunctionality
{
    private float _rotateSpeed;

    private GameObject _visualObjectBase;

    [Space]
    [SerializeField] private Animator _bossGeneralAnimator;

    private const string _levelIntroTriggerAnim = "LevelIntroTrigger";

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

    }


    public void HeroLevelIntroAnimation()
    {
        _bossGeneralAnimator.SetTrigger(_levelIntroTriggerAnim);
    }


    public override void ChildFuncSetup(BossBase bossBase)
    {
        base.ChildFuncSetup(bossBase);

        SetVisualObjectBase(bossBase.GetSpecificBossScript().GetBossVisualBase());
        _visualObjectBase.transform.eulerAngles = new Vector3(0, 180, 0);

        HeroLevelIntroAnimation();
    }

    private void SetFromSO(BossSO bossSO)
    {
        _rotateSpeed = myBossBase.GetBossSO().GetBossRotationSpeed();
    }

    public override void SubscribeToEvents()
    {
        myBossBase.GetSOSetEvent().AddListener(SetFromSO);

        myBossBase.GetBossDamagedEvent().AddListener(BossTookDamage);
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
