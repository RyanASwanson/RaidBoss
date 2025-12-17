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
    private WaitForSeconds _waitTimeBetweenCharacterSpawns;
    private Coroutine _characterSpawnCoroutine;

    [Space] 
    [SerializeField] private Button _selectButton;

    [Space] 
    [SerializeField] private MeshRenderer[] _missionPlatformRenderers;
    
    [Space]
    [SerializeField] private Material _lockedMissionMaterial;

    [Space] 
    [SerializeField] private MeshRenderer _missionSelectCenter;
    
    [Space]
    [SerializeField] private CharacterPreviewLocation _bossPreviewHolder;
    [SerializeField] private CharacterPreviewLocation[] _heroPreviewHolders;

    private bool _isMissionUnlocked;

    private void Start()
    {
        UnlockMissionBasedOnSave();

        PerformMissionVisualsSetUp();
        
        _waitTimeBetweenCharacterSpawns = new WaitForSeconds(_timeBetweenCharacterSpawns);
    }

    private void PerformMissionVisualsSetUp()
    {
        if (_isMissionUnlocked)
        {
            SetMaterialOfPlatforms(_associatedMission.GetAssociatedLevel().GetLevelBoss().GetMiniFloorMaterial());
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

    private void UnlockMissionBasedOnSave()
    {
        SetMissionLockStatus(SaveManager.Instance.IsMissionUnlocked(_associatedMission));
    }
    
    private void SetMissionLockStatus(bool interactable)
    {
        _isMissionUnlocked = interactable;
        
        _selectButton.interactable = interactable;
        
        _missionSelectCenter.gameObject.SetActive(interactable);
        //gameObject.SetActive(interactable);
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

        ShowAllPreviews();
    }

    public void DeselectMission()
    {
        SelectionManager.Instance.ResetSelectionData();

        RemoveAllPreviews();
    }
    
    
    #region Previews

    private void ShowAllPreviews()
    {
        StopShowAllPreviews();
        _characterSpawnCoroutine = StartCoroutine(ShowAllPreviewsProcess());
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
        _bossPreviewHolder.PreviewCharacter(SelectionManager.Instance.GetSelectedBoss());
        yield return _waitTimeBetweenCharacterSpawns;
        
        List<HeroSO> heroes = SelectionManager.Instance.GetAllSelectedHeroes();
        for (int i = 0; i < heroes.Count; i++)
        {
            _heroPreviewHolders[i].PreviewCharacter(heroes[i]);
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
