using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Duality
{
    public class PortalRenderer : MonoBehaviour
    {
        [SerializeField] Camera mainCamera = null;
        [SerializeField] Camera portalCamera = null;

        // List of all active portals in the scene
        [SerializeField] List<Portal> portals = new List<Portal>();

        private int debugRenderCount = 0;

        private void Start()
        {
            foreach (var portal in portals)
            {
                portal.RenderTexture = new RenderTexture(
                    Screen.width / 4, Screen.height / 4, 24, RenderTextureFormat.ARGB32
                );

                portal.Renderer.material.mainTexture = portal.RenderTexture;
                portal.Renderer.material.SetTexture("PortalTexture", portal.RenderTexture);
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

        private void LateUpdate()
        {
            // Debug.Log($"Rendered {debugRenderCount} portals");
            debugRenderCount = 0;
        }

        public void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }

        private void UpdateCamera(ScriptableRenderContext src, Camera camera)
        {
            if (mainCamera is null || portalCamera is null)
            {
                return;
            }

            foreach (var portal in portals)
            {
                if (!portal.Renderer.isVisible)
                {
                    continue;
                }

                RenderPortal(portal, src);
                debugRenderCount ++;
            }
        }

        private void RenderPortal(Portal portal, ScriptableRenderContext src)
        {
            Transform inTransform = portal.transform;
            Transform outTransform = portal.ConnectedPortal.transform;

            // Set the target camera's transform to the main camera's transform.
            Transform cameraTransform = portalCamera.transform;
            cameraTransform.position = mainCamera.transform.position;
            cameraTransform.rotation = mainCamera.transform.rotation;

            // Position the camera behind the other portal
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;

            // Set the camera's oblique view frustum.
            Plane p = new Plane(outTransform.forward, outTransform.position);
            Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;
            // Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(portalCamera.cameraToWorldMatrix) * clipPlaneWorldSpace;

            // Set the portalCamera projection matrix based on the oblique matrix
            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            portalCamera.projectionMatrix = newMatrix;

            // Render the camera to its render target.
            portalCamera.targetTexture = portal.RenderTexture;
            UniversalRenderPipeline.RenderSingleCamera(src, portalCamera);
        }
    }
}