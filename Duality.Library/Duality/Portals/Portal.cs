using System.Collections.Generic;
using UnityEngine;

namespace Duality
{
    [AddComponentMenu("Duality/Portals/Portal")]
    public class Portal : MonoBehaviour
    {
        [SerializeField] Portal connectedPortal = null;
        [SerializeField] World world = World.White;
        [SerializeField] Renderer portalRenderer = null;

        [SerializeField] BoxCollider portalTrigger = null;

        public bool IsPlaced { get; private set; } = true;

        private List<GameObject> objects = new List<GameObject>();

        public Portal ConnectedPortal => connectedPortal;
        public World World => world;

        
        public Renderer Renderer => portalRenderer;
        public RenderTexture RenderTexture { get; set; } = null;
        public BoxCollider PortalTrigger => portalTrigger;

        // private RenderTexture[] renderTextures = new RenderTexture[4];
        

        private void Awake()
        {
            // this.Renderer = GetComponent<Renderer>();

            GenerateRenderTextures();
        }

        void GenerateRenderTextures()
        {
            
        }

        void OnTriggerEnter(Collider other)
        {
            if (objects.Contains(other.gameObject))
            {
                return;
            }

            // Flip objects current world if a world object
            var worldObject = other.GetComponent<IWorldObject>();
            if (worldObject is null || worldObject.IgnorePortals)
            {
                return;
            }

            // Debug.Log($"Teleported {other.name}");
            
            // Transform offset and forward direction relative to the in portal.
            var offset = transform.InverseTransformPoint(other.transform.position);
            var forward = transform.InverseTransformDirection(other.transform.forward);


            // Transform offset and forward direction relative to the out portal.
            var newOffset = connectedPortal.transform.TransformPoint(offset);
            var newForward = connectedPortal.transform.TransformDirection(forward);
            newForward.x *= -1f;
            newForward.z *= -1f;

            var newRotation = Quaternion.LookRotation(newForward, connectedPortal.transform.up);

            Debug.DrawRay(transform.position, transform.forward, Color.red, 1f);
            Debug.DrawRay(transform.position, newForward, Color.red, 1f);

            worldObject.FlipWorld();
            worldObject.WrapPosition(newOffset, newRotation);

            connectedPortal.ReceiveObject(other.gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            objects.Remove(other.gameObject);
        }

        public void ReceiveObject(GameObject other)
        {
            this.objects.Add(other);
        }
    }
}