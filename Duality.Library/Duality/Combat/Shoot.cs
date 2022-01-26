using System.Collections.Generic;
using UnityEngine;

namespace Duality
{

    [AddComponentMenu("Duality/Shoot")]
    public class Shoot : MonoBehaviour
    {
        [SerializeField] List<Transform> spawnPoints = new List<Transform>();
        [SerializeField] GunAsset asset = null;

        private List<GunVisual> visuals = new List<GunVisual>();
        private int spawnIndex = 0;
        private float counter = 0f;

        public bool IsOnCooldown => counter > 0f;

        public Transform CurrentSpawnPoint => spawnPoints[spawnIndex];
        public GunAsset GunAsset => asset;

        private void Update()
        {
            counter -= Time.deltaTime;
        }

        public void SetGun(GunAsset gunAsset)
        {
            asset = gunAsset;
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
            if (IsOnCooldown || spawnPoints.Count == 0)
            {
                return;
            }

            // Get the current spawn point and update to next index
            var spawn = spawnPoints[spawnIndex];
            spawnIndex = (spawnIndex + 1) % spawnPoints.Count;

            var forward = (targetPosition - spawn.position).normalized;
            var rotation = Quaternion.LookRotation(forward, spawn.up);

            Instantiate(gun.SpawnPrefab, spawn.position, rotation);

            if (visuals.Count > spawnIndex)
            {
                visuals[spawnIndex].PlayShoot();
            }

            var proj = Instantiate(gun.Prefab, spawn.position, rotation);
            proj.Gun = gun;

            // Reset the fire counter
            counter = gun.FireRate;
        }

        public void ClearSpawnPoints()
        {
            this.visuals.Clear();
            this.spawnPoints.Clear();
            this.spawnIndex = 0;
        }

        public void AddSpawnPoint(Transform spawnPoint)
        {
            this.spawnPoints.Add(spawnPoint);
        }

        public void AddGunVisual(GunVisual visual)
        {
            this.visuals.Add(visual);
        }
    }
}