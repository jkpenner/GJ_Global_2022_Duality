using Duality.Unity;
using UnityEngine;

namespace Duality
{
    [AddComponentMenu("Duality/Player Controller")]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] new Camera camera = null;
        [SerializeField] float moveSpeed = 10.0f;

        private CharacterController cc = null;
        private UserInput userInput = null;

        [SerializeField] float cameraMinAngleX = -45f;
        [SerializeField] float cameraMaxAngleX = 80f;
        private float cameraAngle = 0f;

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
            userInput = new UserInput();
        }

        private void OnEnable()
        {
            userInput.Enable();
        }

        private void OnDisable()
        {
            userInput.Disable();
        }

        private void Update()
        {
            cameraAngle -= userInput.Player.LookY.ReadValue<float>();
            cameraAngle = Mathf.Clamp(cameraAngle, cameraMinAngleX, cameraMaxAngleX);
            

            float lookX = userInput.Player.LookX.ReadValue<float>();

            transform.Rotate(Vector3.up * lookX);
            camera.transform.localRotation = Quaternion.Euler(cameraAngle, 0f, 0f);

            var moveInput = userInput.Player.Move.ReadValue<Vector2>();
            var movement = new Vector3(moveInput.x, 0f, moveInput.y);
            movement = cc.transform.TransformDirection(movement);
            cc.Move(movement * moveSpeed * Time.deltaTime);
        }
    }
}