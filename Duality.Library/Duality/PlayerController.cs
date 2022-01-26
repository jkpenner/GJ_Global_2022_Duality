using System;
using System.Collections.Generic;
using Duality.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Duality
{
    public interface IWorldObject
    {
        bool IgnorePortals { get; }
        void SetWorld(World world);
        void FlipWorld();
        void WrapPosition(Vector3 position, Quaternion rotation);
    }

    public enum World {
        White, Black,
    }

    [AddComponentMenu("Duality/Player Controller")]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour, IWorldObject, IHasSpawnPoint
    {
        [SerializeField] Shoot shoot = null;

        [SerializeField] float moveSpeed = 10.0f;

        [Header("AI Targeting")]
        [SerializeField] Transform aiTargetingTransform = null;

        private CharacterController cc = null;
        private UserInput userInput = null;

        [SerializeField] float cameraMinAngleX = -45f;
        [SerializeField] float cameraMaxAngleX = 80f;
        private float cameraAngle = 0f;

        private bool isFiring = false;

        [Header("Camera Settings")]
        [SerializeField] Transform cameras = null;
        [SerializeField] new Camera camera = null;

        [SerializeField] World activeWorld = World.White;

        public World ActiveWorld => activeWorld;

        public ObjectSpawn Spawn { get; set; }
        public bool IgnorePortals => false;

        [SerializeField] List<Material> worldMaterials = new List<Material>();

        public Transform AITargetingTransform => aiTargetingTransform ?? transform;

        public Health Health { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            Health.Killed += OnKilled;

            cc = GetComponent<CharacterController>();
            userInput = new UserInput();

            SetWorld(activeWorld);

            var gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.SetMainCamera(camera);
            }
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

        public void SetGun(GunAsset gunAsset)
        {
            Debug.Log($"Set gun to {gunAsset.name}");
            shoot.SetGun(gunAsset);
        }

        public void FlipWorld()
        {
            if (activeWorld == World.White)
            {
                SetWorld(World.Black);
            }
            else
            {
                SetWorld(World.White);
            }
        }

        public void SetWorld(World world)
        {
            activeWorld = world;
            // cameraWorldOne.gameObject.SetActive(world == World.White);
            // cameraWorldTwo.gameObject.SetActive(world == World.Two);
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
            if (!Health.IsAlive)
            {
                return;
            }

            cameraAngle -= userInput.Player.LookY.ReadValue<float>();
            cameraAngle = Mathf.Clamp(cameraAngle, cameraMinAngleX, cameraMaxAngleX);


            float lookX = userInput.Player.LookX.ReadValue<float>();

            transform.Rotate(Vector3.up * lookX);
            cameras.transform.localRotation = Quaternion.Euler(cameraAngle, 0f, 0f);

            var moveInput = userInput.Player.Move.ReadValue<Vector2>();
            var movement = new Vector3(moveInput.x, 0f, moveInput.y);
            movement = cc.transform.TransformDirection(movement);
            movement += -transform.up * 2f;

            cc.Move(movement * moveSpeed * Time.deltaTime);

            if (isFiring)
            {
                if (camera != null)
                {
                    var ray = camera.ScreenPointToRay(new Vector3(
                        Screen.width / 2f, (Screen.height / 10f) * 4f, 0f
                    ));
                    Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

                    Vector3 targetPosition = ray.GetPoint(100f);
                    if (Physics.Raycast(ray.origin, ray.direction, out var hit, 100f, int.MaxValue, QueryTriggerInteraction.Collide))
                    {
                        if (hit.collider.TryGetComponent(out Portal portal))
                        {
                            var hitDistance = hit.distance;

                            var startPosition = hit.point;
                            var startRotation = Quaternion.LookRotation(ray.direction, transform.up);

                            portal.Teleport(ref startPosition, ref startRotation);

                            if (Physics.Raycast(startPosition, startRotation * Vector3.forward, out var portalHit, 100f, int.MaxValue))
                            {
                                hitDistance += portalHit.distance;
                                targetPosition = ray.GetPoint(hitDistance);
                            }
                        }
                        else
                        {
                            targetPosition = hit.point;
                        }
                    }

                    shoot?.Fire(targetPosition);
                }
                else
                {
                    shoot?.Fire();
                }
            }

            foreach(var mat in worldMaterials)
            {
                mat.SetVector("WorldPosition", transform.position);
            }
        }

        public void OnKilled()
        {
            if (Spawn is null)
            {
                Debug.Log("Player Killed, but no spawn assigned.");
                return;
            }

            cc.enabled = false;
            Spawn.Respawn(this.gameObject);
            Health.Reset();
            cc.enabled = true;
        }
    }
}