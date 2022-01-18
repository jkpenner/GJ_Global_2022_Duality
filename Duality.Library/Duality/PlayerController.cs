using System;
using Duality.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Duality
{
    public interface IWorldObject
    {
        void SetWorld(World world);
        void FlipWorld();
        void WrapPosition(Vector3 position, Quaternion rotation);
    }

    public enum World {
        One, Two,
    }

    [AddComponentMenu("Duality/Player Controller")]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IWorldObject
    {
        [SerializeField] Shoot shoot = null;

        [SerializeField] float moveSpeed = 10.0f;

        private CharacterController cc = null;
        private UserInput userInput = null;

        [SerializeField] float cameraMinAngleX = -45f;
        [SerializeField] float cameraMaxAngleX = 80f;
        private float cameraAngle = 0f;

        private bool isFiring = false;

        [Header("Camera Settings")]
        [SerializeField] Transform cameras = null;
        [SerializeField] Camera cameraWorldOne = null;
        [SerializeField] Camera cameraWorldTwo = null;

        [SerializeField] World activeWorld = World.One;

        public World ActiveWorld => activeWorld;

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
            userInput = new UserInput();

            SetWorld(activeWorld);
        }

        private void OnEnable()
        {
            userInput.Enable();
            userInput.Player.Fire.started += OnFireStart;
            userInput.Player.Fire.canceled += OnFireCanceled;
        }

        private void OnFireStart(InputAction.CallbackContext obj)
        {
            isFiring = true;
        }

        private void OnFireCanceled(InputAction.CallbackContext obj)
        {
            isFiring = false;
        }

        private void OnDisable()
        {
            userInput.Disable();
        }

        public void FlipWorld()
        {
            if (activeWorld == World.One)
            {
                SetWorld(World.Two);
            }
            else
            {
                SetWorld(World.One);
            }
        }

        public void SetWorld(World world)
        {
            activeWorld = world;
            cameraWorldOne.gameObject.SetActive(world == World.One);
            cameraWorldTwo.gameObject.SetActive(world == World.Two);
        }

        public void WrapPosition(Vector3 position, Quaternion rotation)
        {
            cc.enabled = false;
            transform.position = position;
            transform.rotation = rotation;
            cc.enabled = true;
        }

        private void Update()
        {
            cameraAngle -= userInput.Player.LookY.ReadValue<float>();
            cameraAngle = Mathf.Clamp(cameraAngle, cameraMinAngleX, cameraMaxAngleX);


            float lookX = userInput.Player.LookX.ReadValue<float>();

            transform.Rotate(Vector3.up * lookX);
            cameras.transform.localRotation = Quaternion.Euler(cameraAngle, 0f, 0f);

            var moveInput = userInput.Player.Move.ReadValue<Vector2>();
            var movement = new Vector3(moveInput.x, 0f, moveInput.y);
            movement = cc.transform.TransformDirection(movement);
            movement += Vector3.down * 8f;

            cc.Move(movement * moveSpeed * Time.deltaTime);

            if (isFiring)
            {
                shoot?.Fire();
            }
        }
    }
}