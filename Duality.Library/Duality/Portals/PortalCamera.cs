using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Duality
{
    [AddComponentMenu("Duality/Portals/Portal Camera")]
    public class PortalCamera : MonoBehaviour
    {
        [SerializeField] Portal[] portals = new Portal[2];

        [SerializeField] Camera portalCamera = null;
        [SerializeField] int iterations = 7;

        private RenderTexture[] tempTextures = new RenderTexture[2];

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            for (int i = 0; i < 2; i++)
            {
                tempTextures[i] = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            }
        }

        private void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                portals[i].Renderer.material.mainTexture = tempTextures[i];
            }
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += UpdateCamera;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= UpdateCamera;
        }

        private void UpdateCamera(ScriptableRenderContext src, Camera camera)
        {
            if (portals.Any(p => !p.IsPlaced))
            {
                return;
            }

            if (portals[0].Renderer.isVisible)
            {
                portalCamera.targetTexture = tempTextures[0];

                for (int i = iterations - 1; i >= 0; --i)
                {
                    RenderCamera(portals[0], portals[1], i, src);
                }
            }

            if (portals[1].Renderer.isVisible)
            {
                portalCamera.targetTexture = tempTextures[1];

                for (int i = iterations - 1; i >= 0; --i)
                {
                    RenderCamera(portals[1], portals[0], i, src);
                }
            }
        }

        private void RenderCamera(Portal inPortal, Portal outPortal, int iterID, ScriptableRenderContext src)
        {
            Transform inTransform = inPortal.transform;
            Transform outTransform = outPortal.transform;

            Transform cameraTransform = portalCamera.transform;
            cameraTransform.position = transform.position;
            cameraTransform.rotation = transform.rotation;

            for (int i = 0; i <= iterID; i++)
            {
                // Position the camera behind the other portal
                Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                cameraTransform.position = outTransform.TransformPoint(relativePos);

                // Rotate the camera to look through the other portal
                Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
                relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
                cameraTransform.rotation = outTransform.rotation * relativeRot;
            }

            // Set the camera's oblique view frustum.
            Plane p = new Plane(-outTransform.forward, outTransform.position);
            Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            portalCamera.projectionMatrix = newMatrix;

            // Render the camera to its render target.
            UniversalRenderPipeline.RenderSingleCamera(src, portalCamera);
        }
    }
}