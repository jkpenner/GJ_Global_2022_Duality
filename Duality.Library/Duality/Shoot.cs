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

        private void Update()
        {
            counter -= Time.deltaTime;
        }

        public void Fire()
        {
            Fire(asset);
        }

        public void Fire(GunAsset gun) {
            // Still waiting on a cooldown
            if (IsOnCooldown || spawnPoints.Length == 0)
            {
                return;
            }

            // Get the current spawn point and update to next index
            var spawn = spawnPoints[spawnIndex];
            spawnIndex = (spawnIndex + 1) % spawnPoints.Length;

            var proj = Instantiate(gun.Prefab, spawn.position, spawn.rotation);
            proj.Gun = gun;

            // Reset the fire counter
            counter = gun.FireRate;
        }
    }
}