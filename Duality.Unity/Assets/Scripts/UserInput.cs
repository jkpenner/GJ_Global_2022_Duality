// GENERATED AUTOMATICALLY FROM 'Assets/UserInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Duality.Unity
{
    public class @UserInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @UserInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""UserInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""89d7acba-f99a-4e5a-97b7-b78e08d92f9c"",
            ""actions"": [
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""9f106bbe-9a4c-476b-baba-52ed42d4df4b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""a1c6f000-957b-4d12-84cd-26d9ef726ac6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookX"",
                    ""type"": ""Value"",
                    ""id"": ""b9553faf-3802-42fc-81fd-cd293eb54edd"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookY"",
                    ""type"": ""Value"",
                    ""id"": ""a4d57a40-bd91-4944-9c19-688d23043f25"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0fd44bb4-7663-4661-b518-f0d2a3dd56e6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a5f9c42a-d8ee-48da-9805-277102ae25c1"",
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
                    ""id"": ""549838e9-f0f6-43da-9a25-a4d3665a3b76"",
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
                    ""id"": ""7b9f18fc-b26d-4fc3-96df-7cbc34c3bbad"",
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
                    ""id"": ""2de5872a-ba2f-4d27-a726-4d1d5a190e13"",
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
                    ""id"": ""868fa772-8262-47aa-8938-7702fde02a73"",
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
                    ""id"": ""4d0c3757-792d-4170-b609-bb60032221ba"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea477eab-6a2d-49c5-a433-897829bbbcd2"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookY"",
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
            m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_LookX = m_Player.FindAction("LookX", throwIfNotFound: true);
            m_Player_LookY = m_Player.FindAction("LookY", throwIfNotFound: true);
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

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Fire;
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_LookX;
        private readonly InputAction m_Player_LookY;
        public struct PlayerActions
        {
            private @UserInput m_Wrapper;
            public PlayerActions(@UserInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Fire => m_Wrapper.m_Player_Fire;
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @LookX => m_Wrapper.m_Player_LookX;
            public InputAction @LookY => m_Wrapper.m_Player_LookY;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @Fire.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                    @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @LookX.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                    @LookX.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                    @LookX.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                    @LookY.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                    @LookY.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                    @LookY.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @LookX.started += instance.OnLookX;
                    @LookX.performed += instance.OnLookX;
                    @LookX.canceled += instance.OnLookX;
                    @LookY.started += instance.OnLookY;
                    @LookY.performed += instance.OnLookY;
                    @LookY.canceled += instance.OnLookY;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);
        public interface IPlayerActions
        {
            void OnFire(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnLookX(InputAction.CallbackContext context);
            void OnLookY(InputAction.CallbackContext context);
        }
    }
}
