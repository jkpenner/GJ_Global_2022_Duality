using UnityEngine;

namespace Duality
{
    public class HealthPickup : MonoBehaviour, IHasSpawnPoint
    {
        [SerializeField] float healthAmount = 0f;
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

            Debug.Log($"Healed player for {healthAmount}");
            player.Health.Heal(healthAmount);
            
            Spawn?.RespawnAfter(this.gameObject, respawnTime);
        }        
    }
}