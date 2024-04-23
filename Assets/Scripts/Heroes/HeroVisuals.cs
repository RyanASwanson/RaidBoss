using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroVisuals : HeroChildrenFunctionality
{
    [SerializeField] private MeshRenderer _controlIcon;
    // Start is called before the first frame update
    void Start()
    {

    }

    

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
        myHeroBase.GetHeroControlledBeginEvent().AddListener(HeroControlStart);
        myHeroBase.GetHeroControlledEndEvent().AddListener(HeroControlStop);
        myHeroBase.GetPathfinding().HeroStartedMovingEvent().AddListener(HeroStartedMoving);
        myHeroBase.GetPathfinding().HeroStoppedMovingEvent().AddListener(HeroStoppedMoving);
    }
    private void HeroSOAssigned(HeroSO heroSO)
    {
        //Debug.Log("Hero Assigned SO Event");
        //_meshAgent.speed = heroSO.GetMoveSpeed();

    }
    private void HeroControlStart()
    {
        _controlIcon.enabled = true;
    }

    private void HeroControlStop()
    {
        _controlIcon.enabled = false;
    }

    private void HeroStartedMoving()
    {
        
    }

    private void HeroStoppedMoving()
    {

    }
    #endregion

}
