//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/InputFunction/InputActions.inputactions
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

public partial class @InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""9ac92310-a435-448a-acbd-8b2b0e063fbc"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""48a2bb34-c866-4331-a286-d5733c734c42"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""3c4f5a5d-1e1a-430f-bf8c-048c21e5cb41"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""aaeb3710-e89d-4b49-b568-0d13f6958f41"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""b3402934-0380-4d96-ad09-e80cdea03aa0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PutCancel"",
                    ""type"": ""Button"",
                    ""id"": ""c361c3d3-2372-4eb0-a8ae-0506f701aea0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""fb2f1340-02c1-4557-9fae-5b5b0e623eff"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""InventryShift"",
                    ""type"": ""Button"",
                    ""id"": ""b74225eb-81f0-4be5-86ea-bed75b1726b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChangeInventryPanel"",
                    ""type"": ""Button"",
                    ""id"": ""af13f772-c5e4-4184-bb24-3ec9367e84b2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""InventryViewScroll"",
                    ""type"": ""Value"",
                    ""id"": ""20402c51-e61d-4373-9fb3-6efa5a85a5df"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f94e7345-5908-45c4-8f2a-ae10751910f3"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""deefeff2-ff7b-445c-b608-a115e3bd9a06"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d84ca256-2a82-4a78-9437-9392c47e7313"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e759e9a6-1199-4571-a633-41cb76e9d222"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""473b62af-e8f3-4a4a-b303-c51a990b2398"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a780e82e-25aa-4e48-8837-a8ded53f7bd8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""15b337db-1c17-4a43-96da-d4ff2cbaa398"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b39037b5-458e-4f1c-bdef-18a1d31c13ef"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6293b64e-26df-4818-bdf7-b2ee809d754e"",
                    ""path"": ""<DualShockGamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de84844b-48e0-40a2-add5-2054ed93fcb3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5424dc28-c92d-4dfb-afd6-3c62861fbde9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PutCancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90914091-9685-48ad-b39a-803f3f531b35"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PutCancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f62815bd-5308-40c3-96e0-0a6ef0b14f1c"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec794177-bda7-47ab-a475-965bcf69eb33"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4698c921-dc35-4ce7-a03d-141a96887be9"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ef07bad-ee04-4c8f-beb2-68f2c2d9f3f2"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd6bef4a-c96e-4519-9995-e1c2cd1f15e6"",
                    ""path"": ""<DualShockGamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryShift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef7420b9-9dac-4d1e-bbc9-930b425f4be4"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryShift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab7aae9a-ce86-498e-b470-2a616d12bee7"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeInventryPanel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bdb42aaa-466b-4410-85db-5f22321ca8a4"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeInventryPanel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis Pad"",
                    ""id"": ""47c52ec9-f409-44d6-a9f2-b883034d7373"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryViewScroll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c0a85795-8df8-47b1-a259-9ec33e527e45"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryViewScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""c17db2ee-7e38-42a9-a1b7-062e95bf7630"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryViewScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis Keyboad"",
                    ""id"": ""589a93a0-6426-47d3-beb0-3321e0d01c33"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryViewScroll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""e03a5d3d-5790-4e9c-819a-a63bf9838578"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryViewScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""a1d4d71c-4167-4b41-b4f4-b5e7436b013c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InventryViewScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""InputSeparate"",
            ""id"": ""b34f6de4-19b4-40c9-84d1-0b291fb395aa"",
            ""actions"": [
                {
                    ""name"": ""InputKeyBoard"",
                    ""type"": ""Button"",
                    ""id"": ""070173f7-aeb0-46c4-bc28-9c01d756c646"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""InputGamePad"",
                    ""type"": ""Button"",
                    ""id"": ""1064e04c-30c3-4846-b9e4-1f6156ffc036"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""89267926-5d4f-4c3b-ab48-c9570b2e41bf"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InputKeyBoard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1da098ef-f895-4591-8d5e-d7adf80da714"",
                    ""path"": ""<Gamepad>/*"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InputGamePad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Run = m_Player.FindAction("Run", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_UseItem = m_Player.FindAction("UseItem", throwIfNotFound: true);
        m_Player_PutCancel = m_Player.FindAction("PutCancel", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_InventryShift = m_Player.FindAction("InventryShift", throwIfNotFound: true);
        m_Player_ChangeInventryPanel = m_Player.FindAction("ChangeInventryPanel", throwIfNotFound: true);
        m_Player_InventryViewScroll = m_Player.FindAction("InventryViewScroll", throwIfNotFound: true);
        // InputSeparate
        m_InputSeparate = asset.FindActionMap("InputSeparate", throwIfNotFound: true);
        m_InputSeparate_InputKeyBoard = m_InputSeparate.FindAction("InputKeyBoard", throwIfNotFound: true);
        m_InputSeparate_InputGamePad = m_InputSeparate.FindAction("InputGamePad", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Run;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_UseItem;
    private readonly InputAction m_Player_PutCancel;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_InventryShift;
    private readonly InputAction m_Player_ChangeInventryPanel;
    private readonly InputAction m_Player_InventryViewScroll;
    public struct PlayerActions
    {
        private @InputActions m_Wrapper;
        public PlayerActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Run => m_Wrapper.m_Player_Run;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @UseItem => m_Wrapper.m_Player_UseItem;
        public InputAction @PutCancel => m_Wrapper.m_Player_PutCancel;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputAction @InventryShift => m_Wrapper.m_Player_InventryShift;
        public InputAction @ChangeInventryPanel => m_Wrapper.m_Player_ChangeInventryPanel;
        public InputAction @InventryViewScroll => m_Wrapper.m_Player_InventryViewScroll;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
            @Attack.started += instance.OnAttack;
            @Attack.performed += instance.OnAttack;
            @Attack.canceled += instance.OnAttack;
            @UseItem.started += instance.OnUseItem;
            @UseItem.performed += instance.OnUseItem;
            @UseItem.canceled += instance.OnUseItem;
            @PutCancel.started += instance.OnPutCancel;
            @PutCancel.performed += instance.OnPutCancel;
            @PutCancel.canceled += instance.OnPutCancel;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
            @InventryShift.started += instance.OnInventryShift;
            @InventryShift.performed += instance.OnInventryShift;
            @InventryShift.canceled += instance.OnInventryShift;
            @ChangeInventryPanel.started += instance.OnChangeInventryPanel;
            @ChangeInventryPanel.performed += instance.OnChangeInventryPanel;
            @ChangeInventryPanel.canceled += instance.OnChangeInventryPanel;
            @InventryViewScroll.started += instance.OnInventryViewScroll;
            @InventryViewScroll.performed += instance.OnInventryViewScroll;
            @InventryViewScroll.canceled += instance.OnInventryViewScroll;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
            @Attack.started -= instance.OnAttack;
            @Attack.performed -= instance.OnAttack;
            @Attack.canceled -= instance.OnAttack;
            @UseItem.started -= instance.OnUseItem;
            @UseItem.performed -= instance.OnUseItem;
            @UseItem.canceled -= instance.OnUseItem;
            @PutCancel.started -= instance.OnPutCancel;
            @PutCancel.performed -= instance.OnPutCancel;
            @PutCancel.canceled -= instance.OnPutCancel;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
            @InventryShift.started -= instance.OnInventryShift;
            @InventryShift.performed -= instance.OnInventryShift;
            @InventryShift.canceled -= instance.OnInventryShift;
            @ChangeInventryPanel.started -= instance.OnChangeInventryPanel;
            @ChangeInventryPanel.performed -= instance.OnChangeInventryPanel;
            @ChangeInventryPanel.canceled -= instance.OnChangeInventryPanel;
            @InventryViewScroll.started -= instance.OnInventryViewScroll;
            @InventryViewScroll.performed -= instance.OnInventryViewScroll;
            @InventryViewScroll.canceled -= instance.OnInventryViewScroll;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // InputSeparate
    private readonly InputActionMap m_InputSeparate;
    private List<IInputSeparateActions> m_InputSeparateActionsCallbackInterfaces = new List<IInputSeparateActions>();
    private readonly InputAction m_InputSeparate_InputKeyBoard;
    private readonly InputAction m_InputSeparate_InputGamePad;
    public struct InputSeparateActions
    {
        private @InputActions m_Wrapper;
        public InputSeparateActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @InputKeyBoard => m_Wrapper.m_InputSeparate_InputKeyBoard;
        public InputAction @InputGamePad => m_Wrapper.m_InputSeparate_InputGamePad;
        public InputActionMap Get() { return m_Wrapper.m_InputSeparate; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InputSeparateActions set) { return set.Get(); }
        public void AddCallbacks(IInputSeparateActions instance)
        {
            if (instance == null || m_Wrapper.m_InputSeparateActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_InputSeparateActionsCallbackInterfaces.Add(instance);
            @InputKeyBoard.started += instance.OnInputKeyBoard;
            @InputKeyBoard.performed += instance.OnInputKeyBoard;
            @InputKeyBoard.canceled += instance.OnInputKeyBoard;
            @InputGamePad.started += instance.OnInputGamePad;
            @InputGamePad.performed += instance.OnInputGamePad;
            @InputGamePad.canceled += instance.OnInputGamePad;
        }

        private void UnregisterCallbacks(IInputSeparateActions instance)
        {
            @InputKeyBoard.started -= instance.OnInputKeyBoard;
            @InputKeyBoard.performed -= instance.OnInputKeyBoard;
            @InputKeyBoard.canceled -= instance.OnInputKeyBoard;
            @InputGamePad.started -= instance.OnInputGamePad;
            @InputGamePad.performed -= instance.OnInputGamePad;
            @InputGamePad.canceled -= instance.OnInputGamePad;
        }

        public void RemoveCallbacks(IInputSeparateActions instance)
        {
            if (m_Wrapper.m_InputSeparateActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IInputSeparateActions instance)
        {
            foreach (var item in m_Wrapper.m_InputSeparateActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_InputSeparateActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public InputSeparateActions @InputSeparate => new InputSeparateActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
        void OnPutCancel(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnInventryShift(InputAction.CallbackContext context);
        void OnChangeInventryPanel(InputAction.CallbackContext context);
        void OnInventryViewScroll(InputAction.CallbackContext context);
    }
    public interface IInputSeparateActions
    {
        void OnInputKeyBoard(InputAction.CallbackContext context);
        void OnInputGamePad(InputAction.CallbackContext context);
    }
}