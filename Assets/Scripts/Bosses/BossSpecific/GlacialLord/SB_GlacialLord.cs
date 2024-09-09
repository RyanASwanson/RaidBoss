using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_GlacialLord : SpecificBossFramework
{
    [Space]
    [Header("Ice Crown")]
    [SerializeField] private Vector3 _iceCrownSpawnLocation;
    [SerializeField] private GameObject _iceCrown;

    private GlacialLordIceCrown _currentIceCrown;

    protected override void StartFight()
    {
        CreateIceCrown();
        base.StartFight();
    }

    #region Ice Crown
    private void CreateIceCrown()
    {
        GameObject crown = Instantiate(_iceCrown, _iceCrownSpawnLocation, Quaternion.identity);

        _currentIceCrown = crown.GetComponent<GlacialLordIceCrown>();

        _currentIceCrown.Setup();
    }
    #endregion

    #region Passive Attack (NAME PENDING)
    
/*    private void TempSpawnAttack()
    {
        _currentPassiveAttack = Instantiate(_passiveAttack, transform.position, Quaternion.identity);

        StartCoroutine(TempLookAt());
    }

    private IEnumerator TempLookAt()
    {
        while(true)
        {
            *//*Vector3 lookLocation = CrownLocationClosestToFloor();
            lookLocation = new Vector3(-lookLocation.x, lookLocation.y, -lookLocation.z);*//*
            _currentPassiveAttack.transform.LookAt(CrownLocationClosestToFloor());
            _currentPassiveAttack.transform.eulerAngles = new Vector3(0, _currentPassiveAttack.transform.eulerAngles.y, 0);
            yield return null;
        }
        
    }*/
    #endregion


    #region Getters
    public Vector3 GetIceCrownDirection()
    {
        Vector3 crownPos = _currentIceCrown.transform.position;
        Vector3 bossPos = transform.position;

        crownPos = new Vector3(crownPos.x, 0, crownPos.z);
        bossPos = new Vector3(bossPos.x, 0, bossPos.z);

        return (crownPos - bossPos).normalized;
    }

    public bool DoesIceCrownHaveHeroOwner() =>_currentIceCrown.DoesCrownHaveOwner();

    public HeroBase GetIceCrownHeroOwner() => _currentIceCrown.GetCrownHeroOwner();


    public Vector3 CrownLocationClosestToFloor() => _currentIceCrown.GetClosestCrownLocationToFloor();

    #endregion
}
