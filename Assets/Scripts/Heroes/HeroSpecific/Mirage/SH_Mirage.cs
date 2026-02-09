using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Mirage hero
/// </summary>
public class SH_Mirage : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [SerializeField] private GameObject _basicTargetZone;
    private GameObject _currentBasicTargetZone;
    private const float _targetZoneYOffset = -.5f;

    [SerializeField] private GameObject _basicAbilityCastVFX;
    
    [SerializeField] private float _cloneBasicAudioPitchOffset;
    private static Vector3 CLONE_BASIC_PROJECTILE_EULER_OFFSET = new Vector3(0, 90, 0);
    
    private const int CLONE_BASIC_AUDIO_ID = 0;
    
    [Space]
    [SerializeField] private GameObject _manualAbilityVFX;

    [Space]
    [SerializeField] private float _cloneSpawnDelay;
    [Space]
    [SerializeField] private HeroSO _cloneSO;
    [SerializeField] private GameObject _manualClone;
    [Space]
    [SerializeField] private GameObject _cloneDirectIcon;
    
    private const float _cloneSpawnOffset = -2;

    private HeroBase _cloneBase;
    private MirageClone _cloneFunc;

    #region Basic Abilities
    /// <summary>
    /// Creates the zone that shows where the basic ability will be cast at
    /// </summary>
    private void CreateBasicTargetZone()
    {
        _currentBasicTargetZone = Instantiate(_basicTargetZone, FindHeroCloneMidpoint(), Quaternion.identity);

        StartCoroutine(MoveBasicTargetZone());
    }

    /// <summary>
    /// Moves the location of the target zone to be in the midpoint between 
    /// the Mirage and the clone
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveBasicTargetZone()
    {
        while(!this.IsUnityNull() && !_currentBasicTargetZone.IsUnityNull())
        {
            _currentBasicTargetZone.transform.position = FindHeroCloneMidpoint();

            _currentBasicTargetZone.transform.LookAt(transform.position);
            _currentBasicTargetZone.transform.eulerAngles = new Vector3
                (0,_currentBasicTargetZone.transform.eulerAngles.y, 0);
            yield return null;
        }
    }


    /// <summary>
    /// Finds the point at the middle in between the Mirage and the clone
    /// </summary>
    /// <returns></returns>
    private Vector3 FindHeroCloneMidpoint()
    {
        //Finds the midpoint by lerping half way between the 2 locations
        Vector3 tempVector = Vector3.Lerp(transform.position, _cloneBase.transform.position, .5f);
        tempVector = new Vector3(tempVector.x, tempVector.y + _targetZoneYOffset, tempVector.z);
        return tempVector;
    }

    public override void ActivateBasicAbilities()
    {
        CreateBasicAbilityProjectile(true);
        CreateBasicAbilityCastVFX(gameObject);
        base.ActivateBasicAbilities();
    }

    /// <summary>
    /// Spawns and sets up the projectile for the basic ability
    /// </summary>
    public void CreateBasicAbilityProjectile(bool castByHero)
    {
        //Spawns the projectile at the location of the midpoint
        GameObject newestProjectile = Instantiate(_basicProjectile, 
            _currentBasicTargetZone.transform.position, _currentBasicTargetZone.transform.rotation);

        if (!castByHero)
        {
            newestProjectile.transform.localEulerAngles += CLONE_BASIC_PROJECTILE_EULER_OFFSET;
        }

        //Performs the set up for the damage area so that it knows it's owner
        newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
    }

    private void CreateBasicAbilityCastVFX(GameObject caster)
    {
        GameObject spawnedVFX = Instantiate(_basicAbilityCastVFX, caster.transform.position, Quaternion.identity);
        spawnedVFX.transform.LookAt(FindHeroCloneMidpoint());
        spawnedVFX.transform.eulerAngles = new Vector3(0,spawnedVFX.transform.eulerAngles.y, 0);
    }

    private void PlayMirageBasicAbilityAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()]
                .MiscellaneousHeroAudio[CLONE_BASIC_AUDIO_ID]);
    }
    #endregion

    #region Manual Abilities
    /// <summary>
    /// Spawns the clone and sets them up with the needed functionality
    /// Called at the start of the battle
    /// </summary>
    private void CreateClone()
    {
        Vector3 spawnLocation = _myHeroBase.transform.position + (_myHeroBase.transform.forward * _cloneSpawnOffset);

        _cloneBase = HeroesManager.Instance.CreateHeroBase(spawnLocation,
            _myHeroBase.transform.rotation, _cloneSO);

        _cloneFunc = ((MirageClone)_cloneBase.GetSpecificHeroScript());
        _cloneFunc.AdditionalSetUp(this);
    }

    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();
        Instantiate(_manualAbilityVFX, transform.position, Quaternion.identity);
        Instantiate(_manualAbilityVFX, _cloneBase.transform.position, Quaternion.identity);
        //This doesn't do any override as the animation has a trigger on it which calls CloneSwap
    }

    public void CloneSwap()
    {
        Vector3 storedCloneLocation = _cloneFunc.transform.position;
        Quaternion storedCloneRotation = _cloneFunc.transform.rotation;
        _cloneFunc.CloneSwap(_myHeroBase.transform);

        _myHeroBase.transform.position = storedCloneLocation;
        _myHeroBase.transform.rotation = storedCloneRotation;

        _myHeroBase.GetPathfinding().BriefStopCurrentMovement();
    }

    /*/// <summary>
    /// Directs the pathfinding of the clone to move to the target location
    /// </summary>
    /// <param name="moveLocation"></param>
    private void MoveClone(Vector3 moveLocation)
    {
        _cloneBase.GetPathfinding().DirectNavigationTo(moveLocation);
        CreateCloneDirectIcon(moveLocation);
    }

    /// <summary>
    /// Creates a direct icon similar to the one created when moving a hero to a location
    /// </summary>
    /// <param name="location"></param>
    private void CreateCloneDirectIcon(Vector3 location)
    {
        location = GameplayManagers.Instance.GetPlayerInputManager().CalculateDirectIconLocation(location);
        Instantiate(_cloneDirectIcon, location, Quaternion.identity);
    }*/


    /// <summary>
    /// The version of the basic ability cast by the clone
    /// </summary>
    public void CloneBasicAbility()
    {
        PlayMirageBasicAbilityAudio();
        CreateBasicAbilityProjectile(false);
        CreateBasicAbilityCastVFX(_cloneBase.gameObject);
    }

    /// <summary>
    /// Kills the clone when the Mirage dies
    /// </summary>
    private void CloneDeath()
    {
        _cloneBase.GetHeroStats().ForceKillHero();
    }
    #endregion

    #region Passive Abilities
    //Passive is handled by the clone
    #endregion

    #region Base Hero
    /// <summary>
    /// Performs the set up for the Mirage
    /// </summary>
    /// <param name="heroBase"> The hero base associated with the Mirage </param>
    /// <param name="heroSO"> The scriptable object of the Mirage </param>
    public override void SetUpSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        base.SetUpSpecificHero(heroBase, heroSO);

        //Spawn the clone at a delay
        Invoke(nameof(CreateClone), _cloneSpawnDelay);
    }

    /// <summary>
    /// Starts the fight by spawning the target zone for the basic ability
    /// </summary>
    protected override void BattleStarted()
    {
        CreateBasicTargetZone();
        base.BattleStarted();
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        _myHeroBase.GetHeroDiedEvent().AddListener(CloneDeath);
    }
    #endregion

}
