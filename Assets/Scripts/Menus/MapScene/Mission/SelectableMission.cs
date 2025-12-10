using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableMission : MonoBehaviour
{
    [SerializeField] private MissionSO _associatedMission;

    [Space] 
    [SerializeField] private float _timeBetweenCharacterSpawns;
    private WaitForSeconds _waitTimeBetweenCharacterSpawns;
    private Coroutine _characterSpawnCoroutine;
    
    [Space]
    [SerializeField] private CharacterPreviewLocation _bossPreviewHolder;
    [SerializeField] private CharacterPreviewLocation[] _heroPreviewHolders;

    private void Start()
    {
        _waitTimeBetweenCharacterSpawns = new WaitForSeconds(_timeBetweenCharacterSpawns);
    }
    
    public void SelectMission()
    {
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
}
