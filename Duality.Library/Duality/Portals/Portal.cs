using UnityEngine;

namespace Duality
{
    [AddComponentMenu("Duality/Portals/Portal")]
    public class Portal : MonoBehaviour
    {
        public bool IsPlaced { get; private set; }
        public Renderer Renderer { get; private set; }
    }
}