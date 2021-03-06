using UnityEngine;

namespace Duality
{
    [CreateAssetMenu(menuName = "Duality / Gun Asset")]
    public class GunAsset : ScriptableObject
    {
        [SerializeField] Projectile prefab = null;
        [SerializeField] GameObject spawnPrefab = null;
        [SerializeField] GameObject impactPrefab = null;
        [SerializeField] GameObject noDamageImpactPrefab = null;
        [SerializeField] GunVisual gunVisualPrefab = null;

        [Header("Handling")]
        [SerializeField] float fireRate = 0.0f;
        [SerializeField] float lifetime = 0.0f;
        [SerializeField] float speedMultiplier = 1f;

        [Header("Damage")]
        [SerializeField] float damageAmount = 0f;
        [SerializeField] DamageTypes damageType = DamageTypes.None;
        
        [Header("Portal Flip")]
        [SerializeField] GunAsset portalFlipAsset = null;



        public Projectile Prefab => prefab;
        public GameObject SpawnPrefab => spawnPrefab;
        public GameObject ImpactPrefab => impactPrefab;
        public GameObject NoDamageImpactPrefab => noDamageImpactPrefab;
        public GunVisual GunVisualPrefab => gunVisualPrefab;

        public float FireRate => fireRate;
        public float Lifetime => lifetime;
        public float SpeedMultiplier => speedMultiplier;

        public float DamageAmount => damageAmount;
        public DamageTypes DamageType => damageType;

        public GunAsset PortalFlipAsset => portalFlipAsset;
    }
}