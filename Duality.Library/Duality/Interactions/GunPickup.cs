using UnityEngine;

namespace Duality
{
    public class GunPickup : MonoBehaviour, IHasSpawnPoint
    {
        [SerializeField] GunAsset gunAsset = null;
        [SerializeField] float respawnTime = 0f;

        public ObjectSpawn Spawn { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player is null)
            {
                player = other.GetComponentInParent<PlayerController>();
            }

            if (player == null)
            {
                return;
            }

            player.SetGun(gunAsset);
            
            Spawn?.RespawnAfter(this.gameObject, respawnTime);
        }        
    }
}