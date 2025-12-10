using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [SerializeField] private LayerMask _missionLayerMask;
    
    private Coroutine _mouseCheckProcess;
    private const float PLAYER_CLICK_RANGE = 50;
    
    [Space]
    [Header("Mission Selection Pop Up")]
    [SerializeField] private MissionSelectionPopUp _missionSelectionPopUp;
    
    private SelectableMission _currentlySelectedMission;
    private SelectableMission _previousSelectedMission;

    private SelectableMission _currentlyHoveredOverMission;

    [Space]
    [Header("Camera")]
    [SerializeField] private float _minimumCameraLocation;
    [SerializeField] private float _maximumCameraLocation;

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

    private const float CAMERA_MISSION_OFFSET = -1f;
    private Camera _mainCam;
    
    private UniversalPlayerInputActions _universalPlayerInputActions;

    private bool _isSubscribedToInput = false;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        Instance = this;
        SubscribeToEvents();
        
        SelectionManager.Instance.SetSelectedGameMode(EGameMode.Missions);
        
        CameraStart();
        StartMouseChecks();
        SubscribeToPlayerInput();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        UnsubscribeToPlayerInput();
    }
    
    #region MouseChecks

    private void StartMouseChecks()
    {
        StopMouseChecks();
        _mouseCheckProcess = StartCoroutine(CheckMouseLocationProcess());
    }

    private void StopMouseChecks()
    {
        if (!_mouseCheckProcess.IsUnityNull())
        {
            StopCoroutine(_mouseCheckProcess);
            _mouseCheckProcess = null;
        }
    }

    private IEnumerator CheckMouseLocationProcess()
    {
        while (true)
        {
            PerformMouseChecks();
            yield return null;
        }
    }

    private void PerformMouseChecks()
    {
        if (MouseOnPoint(_missionLayerMask, out RaycastHit hit))
        {
            MissionHoveredOver(hit.collider.gameObject);
        }
        else if (!_currentlyHoveredOverMission.IsUnityNull())
        {
            MissionNoLongerHoveredOver();
        }
    }

    private void MissionHoveredOver(GameObject missionObject)
    {
        // If there was no previous mission hovered over or the current mission is 
        if (!_currentlyHoveredOverMission.IsUnityNull() && missionObject == _currentlyHoveredOverMission.gameObject)
        {
            //We are already hovering over this mission
            return;
        }
        
        if (missionObject.TryGetComponent(out SelectableMission mission))
        {
            NewMissionHoveredOver(mission);
        }
    }

    private void NewMissionHoveredOver(SelectableMission mission)
    {
        _currentlyHoveredOverMission = mission;
    }

    private void MissionNoLongerHoveredOver()
    {
        _currentlyHoveredOverMission = null;
    }

    private void SelectHoveredMission()
    {
        if (_currentlySelectedMission != _currentlyHoveredOverMission)
        {
            SelectNewHoveredMission();
        }
        
    }

    private void SelectNewHoveredMission()
    {
        DeselectSelectedMission();
        
        _currentlySelectedMission = _currentlyHoveredOverMission;
        _previousSelectedMission = _currentlySelectedMission;
        
        MoveCameraToTarget(_currentlySelectedMission.transform.position.x + CAMERA_MISSION_OFFSET);
        
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
    
    /// <summary>
    /// Finds the object in 3D space at the location in which you clicked
    /// </summary>
    /// <param name="detectionLayerMask"> The layer/layers that can be clicked on </param> 
    /// <param name="rayHit"> Out variable for the object the ray hit </param> 
    /// <returns> Returns if something was clicked </returns> 
    private bool MouseOnPoint(LayerMask detectionLayerMask, out RaycastHit rayHit)
    {
        Ray clickRay = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(clickRay, out rayHit, PLAYER_CLICK_RANGE,detectionLayerMask))
        {
            return true;
        }
        return false;
    }
    
    #endregion

    #region MissionSelection
    
    public void SelectMission(SelectableMission mission)
    {
        
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
    
    
    #region CameraMovement

    private void CameraStart()
    {
        _mainCam = Camera.main;
        SetCameraLocation(_minimumCameraLocation);
    }

    private void Update()
    {

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
        
        //ebug.Log("starting target direction " + _cameraTargetDirection + "    target " + _cameraTargetLocation + " compared to " + _cameraHolder.transform.position.x);
        
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
    private void PlayerLeftClickClicked(InputAction.CallbackContext context)
    {
        if (_currentlyHoveredOverMission.IsUnityNull())
        {
            return;
        }

        SelectHoveredMission();
    }

    private void PlayerRightClicked(InputAction.CallbackContext context)
    {
        DeselectSelectedMission();
    }
    
    private void SubscribeToPlayerInput()
    {
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.GameplayActions.Enable();

        _universalPlayerInputActions.GameplayActions.SelectClick.started += PlayerLeftClickClicked;
        _universalPlayerInputActions.GameplayActions.DirectClick.started += PlayerRightClicked;

        _isSubscribedToInput = true;
    }

    private void UnsubscribeToPlayerInput()
    {
        if (!_isSubscribedToInput) return;

        _universalPlayerInputActions.GameplayActions.SelectClick.started -= PlayerLeftClickClicked;
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
