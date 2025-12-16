using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [Header("Missions")]
    [SerializeField] private GameObject _mission;
    [SerializeField] private GameObject _missionHolder;

    [Space] 
    [SerializeField] private float _missionCreationXDefault;
    
    [Space]
    [SerializeField] private float _missionCreationXIncrease;
    [SerializeField] private float _missionCreationYValue;
    
    private List<SelectableMission> _createdMissions = new List<SelectableMission>();
    
    [Space]
    [Header("Mission Selection Pop Up")]
    [SerializeField] private MissionSelectionPopUp _missionSelectionPopUp;
    
    private SelectableMission _currentlySelectedMission;
    private SelectableMission _previousSelectedMission;

    private SelectableMission _currentlyHoveredOverMission;
    
    [Space]
    [Header("Backgrounds")]
    [SerializeField] private CurveProgression[] _backgroundCurveProgressions;
    private CurveProgression _currentBackgroundCurveProgression;
    
    [SerializeField] private GeneralVFXFunctionality[] _backgroundParticles;
    private GeneralVFXFunctionality _currentBackgroundParticles;

    [Space]
    [Header("Camera")]
    [SerializeField] private float _minimumCameraLocation;
    [SerializeField] private float _maximumCameraLocation;
    
    [SerializeField] private  float _cameraMissionOffSet;

    [Space] 
    [SerializeField] private float _cameraButtonMoveDistance;
    
    [Space] 
    [SerializeField] private float _cameraMoveSpeed;
    
    [Space]
    [SerializeField] private float _cameraAccelerationTime;
    [SerializeField] private float _cameraDeccelerationTime;
    
    [SerializeField] private AnimationCurve _cameraAccelerationCurve;

    [Space] 
    [SerializeField] private float _cameraDeccelerationDistance;
    
    [Space]
    [SerializeField] private GameObject _cameraHolder;

    private float _cameraTargetLocation;
    private float _cameraTargetDirection;
    private float _cameraCurrentDirection;
    
    private float _cameraAccelerationProgress = 0;
    private float _cameraAccelerationCurveValue = 0;
    private float _cameraVelocity;
    
    private Coroutine _cameraMovementCoroutine;
    
    private UniversalPlayerInputActions _universalPlayerInputActions;

    private bool _isSubscribedToInput = false;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        Instance = this;
        SubscribeToEvents();
        
        SelectionManager.Instance.SetSelectedGameMode(EGameMode.Missions);

        HideAllBackgroundParticles();
        // REMOVE THIS IF YOU ADD FUNCTIONALITY FOR STARTING ON A DIFFERENT MISSION THAN THE FIRST
        ShowStartingBackgroundParticles();

        CreateMissions();
        SelectStartingMission();
        
        CameraStart();
        SubscribeToPlayerInput();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        UnsubscribeToPlayerInput();
    }
    
    
    #region MissionCreation

    private void CreateMissions()
    {
        MissionSO[] allMisions = SaveManager.Instance.GetMissionsInGame();
        Vector3 spawnLocation = Vector3.zero;

        for (int i = 0; i < allMisions.Length; i++)
        {
            spawnLocation.Set((_missionCreationXIncrease * i) + _missionCreationXDefault
                ,_missionHolder.transform.position.y, i % 2 == 0 ? _missionCreationYValue : -_missionCreationYValue);
            
            CreateMission(allMisions[i],spawnLocation);
        }
    }

    private void CreateMission(MissionSO mission, Vector3 location)
    {
        Instantiate(_mission, _missionHolder.transform).TryGetComponent(out SelectableMission selectableMission);
        selectableMission.gameObject.transform.position = location;
        selectableMission.SetAssociatedMission(mission);
        
        _createdMissions.Add(selectableMission);
    }

    private void SelectStartingMission()
    {
        //SelectMission(_createdMissions[0]);
    }
    #endregion
    
    #region MissionSelection
    
    public void SelectMission(SelectableMission mission)
    {
        if (mission == _currentlySelectedMission)
        {
            return;
        }
        NewMissionSelected(mission);
    }

    private void NewMissionSelected(SelectableMission mission)
    {
        DeselectSelectedMission();
        
        _currentlySelectedMission = mission;
        _previousSelectedMission = _currentlySelectedMission;

        UpdateBackground(mission.GetAssociatedMission());
        MoveCameraToTarget(_currentlySelectedMission.transform.position.x + _cameraMissionOffSet);
        
        _currentlySelectedMission.SelectMission();
        ShowMissionSelectionPopUp();
    }
    
    private void DeselectSelectedMission()
    {
        if (_currentlySelectedMission.IsUnityNull())
        {
            return;
        }
        
        _currentlySelectedMission.DeselectMission();
        HideMissionSelectionPopUp();
        _currentlySelectedMission = null;
    }

    public void PlayMission(SelectableMission mission)
    {
        
    }
    
    #endregion
    
    #region MissionSelectionPopUp

    private void SetMissionSelectionPopUpLocationToCurrentMission()
    {
        //_currentlyHoveredOverMission
    }

    private void ShowMissionSelectionPopUp()
    {
        _missionSelectionPopUp.MissionSelected();
    }

    private void HideMissionSelectionPopUp()
    {
        _missionSelectionPopUp.MissionDeselected();
    }
    #endregion
    
    #region Background

    private void UpdateBackground(MissionSO mission)
    {
        RemoveCurrentBackground();
        RemoveCurrentBackgroundParticles();
        ShowBackground(mission);
        ShowBackgroundParticles(mission);
    }

    private void ShowBackground(MissionSO mission)
    {
        _currentBackgroundCurveProgression = _backgroundCurveProgressions[mission.GetAssociatedLevel().GetLevelNumber()];
        
        if (_currentBackgroundCurveProgression.IsUnityNull())
        {
            return;
        }
        
        _currentBackgroundCurveProgression.StartMovingUpOnCurve();
    }

    private void RemoveCurrentBackground()
    {
        if (_currentBackgroundCurveProgression.IsUnityNull())
        {
            return;
        }
        _currentBackgroundCurveProgression.StartMovingDownOnCurve();

    }

    private void ShowBackgroundParticles(MissionSO mission)
    {
        _currentBackgroundParticles = _backgroundParticles[mission.GetAssociatedLevel().GetLevelNumber()];
        _currentBackgroundParticles.gameObject.SetActive(true);
    }

    private void RemoveCurrentBackgroundParticles()
    {
        if (_currentBackgroundParticles.IsUnityNull())
        {
            return;
        }
        _currentBackgroundParticles.gameObject.SetActive(false);
    }

    private void HideAllBackgroundParticles()
    {
        foreach (GeneralVFXFunctionality particle in _backgroundParticles)
        {
            particle.gameObject.SetActive(false);
        }
    }

    private void ShowStartingBackgroundParticles()
    {
        ShowBackgroundParticles(SaveManager.Instance.GetMissionsInGame()[0]);
    }
    
    #endregion
    
    #region CameraMovement

    private void CameraStart()
    {
        SetCameraLocation(_minimumCameraLocation);
    }
    
    public void CameraLeftButton()
    {
        DeselectSelectedMission();
        MoveCameraToTargetByIncrease(-_cameraButtonMoveDistance);
    }

    public void CameraRightButton()
    {
        DeselectSelectedMission();
        MoveCameraToTargetByIncrease(_cameraButtonMoveDistance);
    }
    
    private void MoveCameraToTargetByIncrease(float xIncrease)
    {
        MoveCameraToTarget(_cameraHolder.transform.position.x + xIncrease);
    }

    private void MoveCameraToTarget(float xLocation)
    {
        StopCameraMoveProcess();
        
        xLocation = ClampLocationWithinLimits(xLocation);
        _cameraTargetLocation = xLocation;

        if (_cameraTargetLocation > _cameraHolder.transform.position.x)
        {
            _cameraTargetDirection = 1;
        }
        else
        {
            _cameraTargetDirection = -1;
        }
        
        _cameraMovementCoroutine = StartCoroutine(CameraMoveProcess());
    }

    private void StopCameraMoveProcess()
    {
        if (!_cameraMovementCoroutine.IsUnityNull())
        {
            StopCoroutine(_cameraMovementCoroutine);
        }
    }

    private void IncreaseCameraLocation(float xIncrease)
    {
        SetCameraLocation(_cameraHolder.transform.position.x + xIncrease);
    }

    private void SetCameraLocation(float xLocation)
    {
        xLocation = ClampLocationWithinLimits(xLocation);
        _cameraHolder.transform.position = new Vector3(xLocation,_cameraHolder.transform.position.y,_cameraHolder.transform.position.z);
    }

    private float ClampLocationWithinLimits(float clampValue)
    {
        return Mathf.Clamp(clampValue, _minimumCameraLocation, _maximumCameraLocation);
    }

    private IEnumerator CameraMoveProcess()
    {
        // Decelerate if moving in opposite direction
        if (_cameraVelocity > 0 != _cameraTargetDirection > 0)
        {
            _cameraCurrentDirection = -_cameraTargetDirection;
            while (_cameraAccelerationProgress > 0)
            {
                _cameraAccelerationProgress -= Time.deltaTime / _cameraDeccelerationTime;
                CalculateCameraVelocityAndMove();
                yield return null;
            }

            _cameraAccelerationProgress = 0;
        }
        _cameraCurrentDirection = _cameraTargetDirection;
        

        while (Mathf.Abs(_cameraTargetLocation - _cameraHolder.transform.position.x) > _cameraDeccelerationDistance)
        {
            if ( _cameraAccelerationProgress < 1)
            {
                _cameraAccelerationProgress += Time.deltaTime / _cameraAccelerationTime;

                if (_cameraAccelerationProgress > 1)
                {
                    _cameraAccelerationProgress = 1;
                }
            }
            
            CalculateCameraVelocityAndMove();
            yield return null;
        }

        //Decelerate at the end to stop
        while (_cameraAccelerationProgress > 0)
        {
            _cameraAccelerationProgress -= Time.deltaTime / _cameraDeccelerationTime;
            CalculateCameraVelocityAndMove();
            yield return null;
        }
    }
    
    private void CalculateCameraVelocityAndMove()
    {
        EvaluateAccelerationCurve();
        CalculateCameraVelocity();
        MoveCameraWithVelocity();
    }

    private void EvaluateAccelerationCurve()
    {
        _cameraAccelerationCurveValue = _cameraAccelerationCurve.Evaluate(_cameraAccelerationProgress);
    }

    private void CalculateCameraVelocity()
    {
        _cameraVelocity = _cameraMoveSpeed * _cameraCurrentDirection * _cameraAccelerationCurveValue;
    }

    private void MoveCameraWithVelocity()
    {
        IncreaseCameraLocation(_cameraVelocity * Time.deltaTime);
    }
    
    #endregion
   
    #region General
    public void BackToMainMenu()
    {
        SceneLoadManager.Instance.LoadMainMenuScene();
    }
    #endregion
    
    #region InputActions

    private void PlayerRightClicked(InputAction.CallbackContext context)
    {
        DeselectSelectedMission();
    }
    
    private void SubscribeToPlayerInput()
    {
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();
        
        _universalPlayerInputActions.GameplayActions.DirectClick.started += PlayerRightClicked;

        _isSubscribedToInput = true;
    }

    private void UnsubscribeToPlayerInput()
    {
        if (!_isSubscribedToInput) return;
        
        _universalPlayerInputActions.GameplayActions.DirectClick.started -= PlayerRightClicked;
        
        _isSubscribedToInput = false;
    }

    #endregion

    private void SubscribeToEvents()
    {
    }

    private void UnsubscribeFromEvents()
    {
    }
    
    #region Getters

    public SelectableMission GetSelectedMission() => _currentlySelectedMission;

    #endregion
}
