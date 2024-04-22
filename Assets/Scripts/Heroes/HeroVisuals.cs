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

    private void HeroSOAssigned(HeroSO heroSO)
    {
        //Debug.Log("Hero Assigned SO Event");
        //_meshAgent.speed = heroSO.GetMoveSpeed();

    }

    #region Events
    public override void SubscribeToEvents()
    {
        myHeroBase.GetSOSetEvent().AddListener(HeroSOAssigned);
        myHeroBase.GetPathfinding().HeroStartedMovingEvent().AddListener(HeroStartedMoving);
        myHeroBase.GetPathfinding().HeroStoppedMovingEvent().AddListener(HeroStoppedMoving);
    }
    private void HeroControlStart()
    {

    }

    private void HeroControlStop()
    {

    }

    private void HeroStartedMoving()
    {
        
    }

    private void HeroStoppedMoving()
    {

    }
    #endregion

}
