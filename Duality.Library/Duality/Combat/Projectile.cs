using UnityEngine;

namespace Duality
{
    [AddComponentMenu("Duality/Projectile")]
    public class Projectile : MonoBehaviour, IWorldObject
    {
        public GunAsset Gun { get; set; }
        public bool IgnorePortals => false;

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
            var damagable = collision.gameObject.GetComponent<IDamagable>();
            if (damagable is null)
            {
                damagable = collision.gameObject.GetComponentInParent<IDamagable>();
            }
            
            // If the target can not be damage then just spawn impact and destory this.
            if (damagable is null)
            {
                Debug.Log($"Hit {collision.gameObject.name}");
                Instantiate(Gun.ImpactPrefab, transform.position, transform.rotation);
                Destroy(this.gameObject);
                return;
            }

            // If hit a damage, but no damage was done spawn the correct prefab.
            if (damagable.Damage(Gun.DamageAmount, Gun.DamageType))
            {
                Debug.Log("Did damage to the target");
                Instantiate(Gun.ImpactPrefab, transform.position, transform.rotation);
            }
            else
            {   
                Instantiate(Gun.NoDamageImpactPrefab, transform.position, transform.rotation);
            }

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