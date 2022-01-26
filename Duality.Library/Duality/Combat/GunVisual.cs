using UnityEngine;

namespace Duality
{
    public class GunVisual : MonoBehaviour
    {
        [SerializeField] Animator animator = null;
        [SerializeField] string shootAnimName = "GunShoot";
        [SerializeField] Transform projectileSpawnPoint = null;

        public Transform ProjectileSpawnPoint => projectileSpawnPoint;

        public void PlayShoot()
        {
            if (animator is null)
            {
                return;
            }

            animator.Play(shootAnimName);
        }

        public void AimTowards(Vector3 position)
        {
            transform.rotation = Quaternion.LookRotation(position, transform.up);
        }
    }
}