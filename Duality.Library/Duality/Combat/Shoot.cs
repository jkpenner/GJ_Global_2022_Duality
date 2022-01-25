using UnityEngine;

namespace Duality
{

    [AddComponentMenu("Duality/Shoot")]
    public class Shoot : MonoBehaviour
    {
        [SerializeField] Transform[] spawnPoints = null;
        [SerializeField] GunAsset asset = null;

        private int spawnIndex = 0;
        private float counter = 0f;

        public bool IsOnCooldown => counter > 0f;

        public Transform CurrentSpawnPoint => spawnPoints[spawnIndex];

        private void Update()
        {
            counter -= Time.deltaTime;
        }

        public void Fire()
        {
            Fire(transform.position + transform.forward * 100f, asset);
        }

        public void Fire(Vector3 targetPosition)
        {
            Fire(targetPosition, asset);
        }

        public void Fire(Vector3 targetPosition, GunAsset gun) {
            // Still waiting on a cooldown
            if (IsOnCooldown || spawnPoints.Length == 0)
            {
                return;
            }

            // Get the current spawn point and update to next index
            var spawn = spawnPoints[spawnIndex];
            spawnIndex = (spawnIndex + 1) % spawnPoints.Length;

            var forward = (targetPosition - spawn.position).normalized;
            var rotation = Quaternion.LookRotation(forward, spawn.up);

            var proj = Instantiate(gun.Prefab, spawn.position, rotation);
            proj.Gun = gun;

            // Reset the fire counter
            counter = gun.FireRate;
        }
    }
}