using UnityEngine;

namespace Duality
{
    [CreateAssetMenu(menuName = "Duality / Gun Asset")]
    public class GunAsset : ScriptableObject
    {
        [SerializeField] Projectile prefab = null;
        [SerializeField] GameObject impactPrefab = null;
        [SerializeField] float fireRate = 0.0f;
        [SerializeField] float lifetime = 0.0f;
        [SerializeField] float speedMultiplier = 1f;


        public Projectile Prefab => prefab;
        public GameObject ImpactPrefab => impactPrefab;
        public float FireRate => fireRate;
        public float Lifetime => lifetime;
        public float SpeedMultiplier => speedMultiplier;
    }
}