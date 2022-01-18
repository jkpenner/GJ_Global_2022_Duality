using UnityEngine;

namespace Duality
{
    [AddComponentMenu("Duality/Projectile")]
    public class Projectile : MonoBehaviour, IWorldObject
    {
        public GunAsset Gun { get; set; }

        [SerializeField] float moveSpeed = 10f;
        [SerializeField] new Rigidbody rigidbody = null;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            // Destroy projectile after given lifetime, if one is set.
            if (Gun.Lifetime > 0.0f)
            {
                Destroy(this.gameObject, Gun.Lifetime);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Instantiate(Gun.ImpactPrefab, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }

        private void Update()
        {
            var speed = moveSpeed * Gun.SpeedMultiplier;
            var position = rigidbody.position + transform.forward * speed * Time.deltaTime;
            rigidbody.MovePosition(position);
        }

        public void SetWorld(World world)
        {
            
        }

        public void FlipWorld()
        {
            
        }

        public void WrapPosition(Vector3 position, Quaternion rotation)
        {
            Debug.Log($"Moved from {rigidbody.position} to {position}");

            // rigidbody.gameObject.SetActive(false);
            rigidbody.Sleep();
            rigidbody.position = position;
            rigidbody.rotation = rotation;
            rigidbody.WakeUp();
            // rigidbody.gameObject.SetActive(true);
        }
    }
}