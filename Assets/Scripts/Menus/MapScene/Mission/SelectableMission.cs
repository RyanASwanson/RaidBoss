using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectableMission : MonoBehaviour
{
    [SerializeField] private MissionSO _associatedMission;

    [Space] 
    [SerializeField] private float _timeBetweenCharacterSpawns;
    private static WaitForSeconds _waitTimeBetweenCharacterSpawns;
    private Coroutine _characterSpawnCoroutine;

    [Space] 
    [SerializeField] private Button _selectButton;

    [Space] 
    [SerializeField] private MeshRenderer[] _missionPlatformRenderers;
    
    [Space]
    [SerializeField] private Material _lockedMissionMaterial;

    [Space]
    [SerializeField] private Transform _missionSpecificStandardParent;

    [SerializeField] private CurveProgression _missionStandardScaleCurve;
    private GameObject _currentMissionStandard;


    [SerializeField] private MaterialSetCustomProperty _missionGlowMaterialProperty;
    [SerializeField] private CurveProgression _missionHoverGlowCurve;
    
    [SerializeField] private MeshRenderer[] _missionGlowRenderers;

    [Space] 
    [SerializeField] private GameObject _missionSelectCenter;
    
    [Space]
    [SerializeField] private CharacterPreviewLocation _bossPreviewHolder;
    [SerializeField] private CharacterPreviewLocation[] _heroPreviewHolders;

    private bool _isMissionUnlocked;

    private void Start()
    {
        UnlockMissionBasedOnSave();

        PerformMissionVisualsSetUp();

        ChangeStandard();

        if (_waitTimeBetweenCharacterSpawns.IsUnityNull())
        {
            _waitTimeBetweenCharacterSpawns = new WaitForSeconds(_timeBetweenCharacterSpawns);
        }
    }

    private void PerformMissionVisualsSetUp()
    {
        if (_isMissionUnlocked)
        {
            SetMaterialOfPlatforms(_associatedMission.GetAssociatedLevel().GetLevelBoss().GetMiniFloorMaterial());
            SetMaterialOfGlow(_associatedMission.GetAssociatedLevel().GetLevelBoss().GetMissionSelectionGlowMaterial());
            _missionGlowMaterialProperty.SetUp();
        }
        else
        {
            SetMaterialOfPlatforms(_associatedMission.GetAssociatedLevel().GetLevelBoss().GetMiniFloorLockedMaterial());
        }
    }

    private void SetMaterialOfPlatforms(Material newMaterial)
    {
        foreach (MeshRenderer renderer in _missionPlatformRenderers)
        {
            renderer.material = newMaterial;
        }
    }

    private void SetMaterialOfGlow(Material newMaterial)
    {
        foreach (MeshRenderer glowRenderer in _missionGlowRenderers)
        {
            glowRenderer.material = newMaterial;
        }
    }

    private void UnlockMissionBasedOnSave()
    {
        SetMissionLockStatus(SaveManager.Instance.IsMissionUnlocked(_associatedMission));
    }
    
    private void SetMissionLockStatus(bool interactable)
    {
        _isMissionUnlocked = interactable;
        
        _selectButton.interactable = interactable;
        
        _missionSelectCenter.gameObject.SetActive(interactable);
    }

    public void ForceUnlockMission()
    {
        SetMissionLockStatus(true);
        
        SetMaterialOfPlatforms(_associatedMission.GetAssociatedLevel().GetLevelBoss().GetMiniFloorMaterial());
        SetMaterialOfGlow(_associatedMission.GetAssociatedLevel().GetLevelBoss().GetMissionSelectionGlowMaterial());
        _missionGlowMaterialProperty.SetUp();
    }

    private void ChangeStandard()
    {
        if (SaveManager.Instance.IsMissionCompleted(_associatedMission))
        {
            CreateBanner(MapController.Instance.GetVictoryStandard());
        }
        else
        {
            CreateBossBanner();
        }
    }

    public void CreateBossBanner()
    {
        CreateBanner(_associatedMission.GetAssociatedLevel().GetLevelBoss().GetBossStandard());
    }

    private void CreateBanner(GameObject banner)
    {
        if (!_currentMissionStandard.IsUnityNull())
        {
            Destroy(_currentMissionStandard);
        }
        
        _currentMissionStandard = Instantiate(banner, _missionSpecificStandardParent);
    }

    public void MissionHoveredStarted()
    {
        if (!_isMissionUnlocked)
        {
            return;
        }
        
        _missionStandardScaleCurve.StartMovingUpOnCurve();
        _missionHoverGlowCurve.StartMovingUpOnCurve();
    }

    public void MissionHoverEnded()
    {
        if (!_isMissionUnlocked)
        {
            return;
        }
        
        _missionStandardScaleCurve.StartMovingDownOnCurve();
        _missionHoverGlowCurve.StartMovingDownOnCurve();
    }

    public void InformControllerOfSelection()
    {
        MapController.Instance.SelectMission(this);
    }
    
    public void SelectMission()
    {
        SelectionManager.Instance.SetSelectedMission(_associatedMission);
        
        SelectionManager.Instance.SetSelectedDifficulty(_associatedMission.GetAssociatedDifficulty());
        SelectionManager.Instance.SetSelectedLevelAndBoss(_associatedMission.GetAssociatedLevel());
        SelectionManager.Instance.SetSelectedHeroes(_associatedMission.GetAssociatedHeroes());
        SelectionManager.Instance.SetSelectedModifiers(_associatedMission.GetMissionModifiers());

        ShowAllPreviews();
    }

    public void DeselectMission()
    {
        SelectionManager.Instance.ResetSelectionData(true);

        RemoveAllPreviews();
    }
    
    
    #region Previews

    private void ShowAllPreviews()
    {
        StopShowAllPreviews();
        _characterSpawnCoroutine = StartCoroutine(ShowAllPreviewsProcess());
    }

    /// <summary>
    /// Used for debug purposes only
    /// </summary>
    public void ForceShowAllPreviews()
    {
        SelectionManager.Instance.SetSelectedLevelAndBoss(_associatedMission.GetAssociatedLevel());
        SelectionManager.Instance.SetSelectedHeroes(_associatedMission.GetAssociatedHeroes());
        
        _bossPreviewHolder.PreviewCharacterAndPerformAnimations(_associatedMission.GetAssociatedLevel().GetLevelBoss(),true,true);
        
        HeroSO[] heroes = _associatedMission.GetAssociatedHeroes();
        for (int i = 0; i < heroes.Length; i++)
        {
            _heroPreviewHolders[i].PreviewCharacterAndPerformAnimations(heroes[i],true,true);
        }
    }

    private void StopShowAllPreviews()
    {
        if (!_characterSpawnCoroutine.IsUnityNull())
        {
            StopCoroutine(_characterSpawnCoroutine);
        }
    }

    private IEnumerator ShowAllPreviewsProcess()
    {
        _bossPreviewHolder.PreviewCharacterAndPerformAnimations(SelectionManager.Instance.GetSelectedBoss(),true,true);
        
        yield return _waitTimeBetweenCharacterSpawns;
        
        List<HeroSO> heroes = SelectionManager.Instance.GetAllSelectedHeroes();
        for (int i = 0; i < heroes.Count; i++)
        {
            _heroPreviewHolders[i].PreviewCharacterAndPerformAnimations(heroes[i],true,true);
            
            yield return _waitTimeBetweenCharacterSpawns;
        }
    }

    private void RemoveAllPreviews()
    {
        StopShowAllPreviews();
        _bossPreviewHolder.RemoveCharacterPreview();

        for (int i = 0; i < _heroPreviewHolders.Length; i++)
        {
            _heroPreviewHolders[i].RemoveCharacterPreview();
        }
    }
    
    #endregion
    
    #region Getters
    public MissionSO GetAssociatedMission() => _associatedMission;
    #endregion 
    
    #region Setters

    public void SetAssociatedMission(MissionSO associatedMission)
    {
        _associatedMission = associatedMission;
    }
    #endregion
}
