using UnityEngine;

namespace Duality
{
    public class ShootTester : MonoBehaviour
    {
        [SerializeField] Shoot shoot = null;

        private void Update()
        {
            shoot.Fire();
        }
    }
}