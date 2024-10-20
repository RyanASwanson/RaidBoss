//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/Input/UniversalPlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @UniversalPlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @UniversalPlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""UniversalPlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""GameplayActions"",
            ""id"": ""3ae50ac2-4521-4057-aa4f-006fc48c3e4c"",
            ""actions"": [
                {
                    ""name"": ""SelectClick"",
                    ""type"": ""Button"",
                    ""id"": ""01c42468-c092-4759-8f6f-c6c28ad79dac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DirectClick"",
                    ""type"": ""Button"",
                    ""id"": ""cc85b0e9-97ac-4eed-94a4-d954cb339d4a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ActiveAbility"",
                    ""type"": ""Button"",
                    ""id"": ""696ed6af-6859-48fe-8e6f-bd9f9fcd19b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""NumberPress"",
                    ""type"": ""Button"",
                    ""id"": ""4e9de618-9d76-403b-a6e3-9b72161e14a1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SpecificAbilityPress"",
                    ""type"": ""Button"",
                    ""id"": ""bcf30fa4-425b-4033-915b-f69b1318430e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""EscapePress"",
                    ""type"": ""Button"",
                    ""id"": ""01be1bbf-92a0-455d-93da-39b2762a69da"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseScroll"",
                    ""type"": ""Value"",
                    ""id"": ""2ed982a0-9da7-4e89-97e8-16fb56e89c60"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fdd5eb47-4925-46ff-8606-93db7063e2c3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b115e33-bce1-4eb0-b76d-2ed961cf178a"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DirectClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62765004-a9a1-416f-8313-600031bffbe3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ActiveAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a08058e0-54cb-4e1b-90f0-e297bb312204"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ActiveAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d32b115-6bd4-484d-8644-af4226731896"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0029bc7e-3744-4f76-978a-921f807d52c2"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8ce6dd19-ad5a-487c-8def-946c6f0ceec9"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""33ea0831-8bef-468b-b38e-903183111ef8"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c7f46d9-8846-478a-ac8a-b21ae7fb5566"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=4)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39e9ed2e-c5f7-4c43-8337-cb1e5f15ace8"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapePress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dbe9e1b4-3313-4e18-aea4-35af5f6828c6"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db8bf44d-ee8b-4f61-9e59-893d1682e75d"",
                    ""path"": ""<Keyboard>/numpad2"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d0ae4ed3-6791-45e0-a0c5-53903e3c34eb"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7f067f1-4d52-421f-b8dd-759ed96d5761"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""275d9f77-54c9-46a0-abf0-7bf62b4fbcef"",
                    ""path"": ""<Keyboard>/numpad5"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=4)"",
                    ""groups"": """",
                    ""action"": ""NumberPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a81ab5ce-27d2-4120-b0cf-5b9e9741b1e5"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""MouseScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""67de583d-8d8d-44eb-8aa7-698deb483179"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0)"",
                    ""groups"": """",
                    ""action"": ""SpecificAbilityPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d91b3f3b-b089-4ee0-bf53-fecfca4d29ad"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""SpecificAbilityPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0783fd6d-670e-40d5-9957-4e5fff86d066"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": """",
                    ""action"": ""SpecificAbilityPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e12c24ba-3fb3-4c48-acfd-d49a9c951cdc"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": """",
                    ""action"": ""SpecificAbilityPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b69b4889-646d-4a09-a2e6-d90fd4555c30"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=4)"",
                    ""groups"": """",
                    ""action"": ""SpecificAbilityPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GameplayActions
        m_GameplayActions = asset.FindActionMap("GameplayActions", throwIfNotFound: true);
        m_GameplayActions_SelectClick = m_GameplayActions.FindAction("SelectClick", throwIfNotFound: true);
        m_GameplayActions_DirectClick = m_GameplayActions.FindAction("DirectClick", throwIfNotFound: true);
        m_GameplayActions_ActiveAbility = m_GameplayActions.FindAction("ActiveAbility", throwIfNotFound: true);
        m_GameplayActions_NumberPress = m_GameplayActions.FindAction("NumberPress", throwIfNotFound: true);
        m_GameplayActions_SpecificAbilityPress = m_GameplayActions.FindAction("SpecificAbilityPress", throwIfNotFound: true);
        m_GameplayActions_EscapePress = m_GameplayActions.FindAction("EscapePress", throwIfNotFound: true);
        m_GameplayActions_MouseScroll = m_GameplayActions.FindAction("MouseScroll", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // GameplayActions
    private readonly InputActionMap m_GameplayActions;
    private List<IGameplayActionsActions> m_GameplayActionsActionsCallbackInterfaces = new List<IGameplayActionsActions>();
    private readonly InputAction m_GameplayActions_SelectClick;
    private readonly InputAction m_GameplayActions_DirectClick;
    private readonly InputAction m_GameplayActions_ActiveAbility;
    private readonly InputAction m_GameplayActions_NumberPress;
    private readonly InputAction m_GameplayActions_SpecificAbilityPress;
    private readonly InputAction m_GameplayActions_EscapePress;
    private readonly InputAction m_GameplayActions_MouseScroll;
    public struct GameplayActionsActions
    {
        private @UniversalPlayerInputActions m_Wrapper;
        public GameplayActionsActions(@UniversalPlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @SelectClick => m_Wrapper.m_GameplayActions_SelectClick;
        public InputAction @DirectClick => m_Wrapper.m_GameplayActions_DirectClick;
        public InputAction @ActiveAbility => m_Wrapper.m_GameplayActions_ActiveAbility;
        public InputAction @NumberPress => m_Wrapper.m_GameplayActions_NumberPress;
        public InputAction @SpecificAbilityPress => m_Wrapper.m_GameplayActions_SpecificAbilityPress;
        public InputAction @EscapePress => m_Wrapper.m_GameplayActions_EscapePress;
        public InputAction @MouseScroll => m_Wrapper.m_GameplayActions_MouseScroll;
        public InputActionMap Get() { return m_Wrapper.m_GameplayActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActionsActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsActionsCallbackInterfaces.Add(instance);
            @SelectClick.started += instance.OnSelectClick;
            @SelectClick.performed += instance.OnSelectClick;
            @SelectClick.canceled += instance.OnSelectClick;
            @DirectClick.started += instance.OnDirectClick;
            @DirectClick.performed += instance.OnDirectClick;
            @DirectClick.canceled += instance.OnDirectClick;
            @ActiveAbility.started += instance.OnActiveAbility;
            @ActiveAbility.performed += instance.OnActiveAbility;
            @ActiveAbility.canceled += instance.OnActiveAbility;
            @NumberPress.started += instance.OnNumberPress;
            @NumberPress.performed += instance.OnNumberPress;
            @NumberPress.canceled += instance.OnNumberPress;
            @SpecificAbilityPress.started += instance.OnSpecificAbilityPress;
            @SpecificAbilityPress.performed += instance.OnSpecificAbilityPress;
            @SpecificAbilityPress.canceled += instance.OnSpecificAbilityPress;
            @EscapePress.started += instance.OnEscapePress;
            @EscapePress.performed += instance.OnEscapePress;
            @EscapePress.canceled += instance.OnEscapePress;
            @MouseScroll.started += instance.OnMouseScroll;
            @MouseScroll.performed += instance.OnMouseScroll;
            @MouseScroll.canceled += instance.OnMouseScroll;
        }

        private void UnregisterCallbacks(IGameplayActionsActions instance)
        {
            @SelectClick.started -= instance.OnSelectClick;
            @SelectClick.performed -= instance.OnSelectClick;
            @SelectClick.canceled -= instance.OnSelectClick;
            @DirectClick.started -= instance.OnDirectClick;
            @DirectClick.performed -= instance.OnDirectClick;
            @DirectClick.canceled -= instance.OnDirectClick;
            @ActiveAbility.started -= instance.OnActiveAbility;
            @ActiveAbility.performed -= instance.OnActiveAbility;
            @ActiveAbility.canceled -= instance.OnActiveAbility;
            @NumberPress.started -= instance.OnNumberPress;
            @NumberPress.performed -= instance.OnNumberPress;
            @NumberPress.canceled -= instance.OnNumberPress;
            @SpecificAbilityPress.started -= instance.OnSpecificAbilityPress;
            @SpecificAbilityPress.performed -= instance.OnSpecificAbilityPress;
            @SpecificAbilityPress.canceled -= instance.OnSpecificAbilityPress;
            @EscapePress.started -= instance.OnEscapePress;
            @EscapePress.performed -= instance.OnEscapePress;
            @EscapePress.canceled -= instance.OnEscapePress;
            @MouseScroll.started -= instance.OnMouseScroll;
            @MouseScroll.performed -= instance.OnMouseScroll;
            @MouseScroll.canceled -= instance.OnMouseScroll;
        }

        public void RemoveCallbacks(IGameplayActionsActions instance)
        {
            if (m_Wrapper.m_GameplayActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActionsActions @GameplayActions => new GameplayActionsActions(this);
    public interface IGameplayActionsActions
    {
        void OnSelectClick(InputAction.CallbackContext context);
        void OnDirectClick(InputAction.CallbackContext context);
        void OnActiveAbility(InputAction.CallbackContext context);
        void OnNumberPress(InputAction.CallbackContext context);
        void OnSpecificAbilityPress(InputAction.CallbackContext context);
        void OnEscapePress(InputAction.CallbackContext context);
        void OnMouseScroll(InputAction.CallbackContext context);
    }
}
