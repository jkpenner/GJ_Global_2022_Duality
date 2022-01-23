using UnityEngine;

namespace Duality
{
    public class EnemyVisual : MonoBehaviour
    {
        [SerializeField] Shoot shoot = null;

        public Shoot Shoot => shoot;
    }
}