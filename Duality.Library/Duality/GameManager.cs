using UnityEngine;

namespace Duality
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] PortalRenderer portalRenderer = null;


        public void SetMainCamera(Camera camera)
        {
            if (portalRenderer != null)
            {
                portalRenderer.SetMainCamera(camera);
            }
        }
    }
}