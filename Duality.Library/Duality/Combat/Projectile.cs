using UnityEngine;

namespace Duality
{
    [AddComponentMenu("Duality/Projectile")]
    public class Projectile : MonoBehaviour, IWorldObject
    {
        public GunAsset Gun { get; set; }
        public bool IgnorePortals => false;

        [SerializeField] float moveSpeed = 10f;
        // [SerializeField] new Rigidbody rigidbody = null;

        private void Start()
        {
            // rigidbody = GetComponent<Rigidbody>();

            // Destroy projectile after given lifetime, if one is set.
            if (Gun.Lifetime > 0.0f)
            {
                Destroy(this.gameObject, Gun.Lifetime);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (TryGetDamagableFromTargetOrParent(collision.gameObject, out IDamagable damagable))
            {
                HandleHitDamagable(damagable, collision.contacts[0].point);  
            }
            else
            {
                // Debug.Log($"Hit {collision.gameObject.name}");
                Instantiate(Gun.ImpactPrefab, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }
        }

        private void Update()
        {
            // transform.position += transform.forward * speed * Time.deltaTime;

            var start = transform.position;
            var speed = moveSpeed * Gun.SpeedMultiplier;
            var movement = transform.forward * speed * Time.deltaTime;

            if (Physics.Raycast(start, movement.normalized, out var hit, movement.magnitude, Physics.AllLayers, QueryTriggerInteraction.Collide))
            {
                transform.position = hit.point;

                // Check to see if we hit a portal
                if (hit.collider.TryGetComponent(out Portal portal))
                {
                    HandleHitPortal(portal, hit.point);
                }
                else if (TryGetDamagableFromTargetOrParent(hit.collider.gameObject, out IDamagable damagable))
                {
                    
                    HandleHitDamagable(damagable, hit.point);
                }
                else
                {
                    // Debug.Log($"Hit {collision.gameObject.name}");
                    Instantiate(Gun.ImpactPrefab, transform.position, transform.rotation);
                    Destroy(gameObject);
                }
            }
            else
            {
                transform.position += movement;
            }


            
        }

        private void HandleHitPortal(Portal portal, Vector3 hitPosition)
        {
            var position = hitPosition;
            var rotation = transform.rotation;

            portal.Teleport(ref position, ref rotation);

            if (Gun.PortalFlipAsset is null)
            {
                WrapPosition(position, rotation);
            }
            else
            {
                Projectile projectile = Instantiate(Gun.PortalFlipAsset.Prefab, position, rotation);
                projectile.Gun = Gun.PortalFlipAsset;

                Instantiate(Gun.PortalFlipAsset.SpawnPrefab, position, rotation);
                Destroy(this.gameObject);
            }
        }

        private bool TryGetDamagableFromTargetOrParent(GameObject gameObject, out IDamagable damagable)
        {
            damagable = gameObject.GetComponent<IDamagable>();
            if (damagable is null)
            {
                damagable = gameObject.GetComponentInParent<IDamagable>();
            }
            return damagable != null;
        }

        private void HandleHitDamagable(IDamagable damagable, Vector3 hitPosition)
        {
            // If hit a damage, but no damage was done spawn the correct prefab.
            if (damagable.Damage(Gun.DamageAmount, Gun.DamageType))
            {
                Debug.Log($"Hit object, applying {Gun.DamageAmount} damage of type {Gun.DamageType}");
                Instantiate(Gun.ImpactPrefab, transform.position, transform.rotation);
            }
            else
            {
                Instantiate(Gun.NoDamageImpactPrefab, transform.position, transform.rotation);
            }

            Destroy(this.gameObject);
        }

        public void SetWorld(World world)
        {

        }

        public void FlipWorld()
        {

        }

        public void WrapPosition(Vector3 position, Quaternion rotation)
        {
            // Debug.Log($"Moved from {rigidbody.position} to {position}");

            // var interpolation = rigidbody.interpolation;

            // rigidbody.gameObject.SetActive(false);
            // rigidbody.interpolation = RigidbodyInterpolation.None;
            // rigidbody.Sleep();
            transform.position = position;
            transform.rotation = rotation;
            // rigidbody.WakeUp();
            // rigidbody.gameObject.SetActive(true);
            // rigidbody.interpolation = interpolation;
        }
    }
}