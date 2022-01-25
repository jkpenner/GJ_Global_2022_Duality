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

            if (other.GetComponent<Projectile>() != null)
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
            
            // // Transform offset and forward direction relative to the in portal.
            // var offset = transform.InverseTransformPoint(other.transform.position);
            // var forward = transform.InverseTransformDirection(other.transform.forward);


            // // Transform offset and forward direction relative to the out portal.
            // var newOffset = connectedPortal.transform.TransformPoint(offset);
            // var newForward = connectedPortal.transform.TransformDirection(forward);
            // newForward.x *= -1f;
            // newForward.z *= -1f;

            // var newRotation = Quaternion.LookRotation(newForward, connectedPortal.transform.up);

            var position = other.transform.position;
            var rotation = other.transform.rotation;

            Teleport(ref position, ref rotation);

            Debug.DrawRay(transform.position, transform.forward, Color.red, 1f);
            Debug.DrawRay(transform.position, rotation * Vector3.forward, Color.red, 1f);

            worldObject.FlipWorld();
            worldObject.WrapPosition(position, rotation);

            connectedPortal.ReceiveObject(other.gameObject);
        }

        public void Teleport(ref Vector3 position, ref Quaternion rotation)
        {
            var start = position;

            // Transform offset and forward direction relative to the in portal
            var positionLocal = transform.InverseTransformPoint(position);
            var forwardLocal = transform.InverseTransformDirection(rotation * Vector3.forward);

            // forwardLocal = Quaternion.Euler(0f, 180f, 0f) * forwardLocal;
            // forwardLocal.x *= -1f;
            forwardLocal.z *= -1f;

            // Transform offset and forward direction relative to the out portal.
            position = connectedPortal.transform.TransformPoint(positionLocal);
            var newForward = connectedPortal.transform.TransformDirection(forwardLocal);
            rotation = Quaternion.LookRotation(newForward, connectedPortal.transform.up);

            Debug.Log($"Teleported from {name}:{start} to {connectedPortal.name}:{position}");
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