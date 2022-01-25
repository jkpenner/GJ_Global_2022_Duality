using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Duality
{
    public static class NavUtility
    {
        public static IEnumerator HandleTeleport(NavMeshAgent agent, Action onComplete)
        {
            if (!agent.isOnOffMeshLink)
            {
                yield break;
            }
            // Debug.Log("On OffMesh link");
            var data = agent.currentOffMeshLinkData;
            var oml = data.offMeshLink;

            // Debug.Log($"Start: {oml.startTransform.name}, End: {oml.endTransform.name}");
            // Debug.Log($"Start Position: {data.startPos}, End Position: {data.endPos}");
            // Debug.Log($"Start: {oml.startTransform.position}, End: {oml.endTransform.position}");

            // * The link's start / end positions do not always align with the 
            // * startTransform / endTransform positions.
            var startDistance = Vector3.Distance(oml.startTransform.position, data.startPos);
            var endDistance = Vector3.Distance(oml.endTransform.position, data.startPos);

            Transform start = null, end = null;
            if (startDistance < endDistance)
            {
                start = oml.startTransform;
                end = oml.endTransform;
            }
            else
            {
                start = oml.endTransform;
                end = oml.startTransform;
            }

            // Debug.Log($"Start Portal {start}, End Portal {end}");

            var oldForward = start.InverseTransformDirection(agent.transform.forward);
            var newForward = end.TransformDirection(oldForward);

            agent.enabled = false;
            agent.transform.position = data.endPos;
            agent.transform.rotation = Quaternion.LookRotation(newForward, end.up);
            // Debug.Log($"Up {end.up}");
            agent.enabled = true;
            agent.CompleteOffMeshLink();

            onComplete?.Invoke();
        }

        /// <summary> Attempt to get the closest point on any NavMesh. </summary>
        public static bool TryGetClosetPoint(Vector3 position, out Vector3 outPosition)
        {
            var maxDistance = GameConfig.NAV_MESH_SAMPLE_DISTANCE;
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, maxDistance, int.MaxValue))
            {
                outPosition = hit.position;
                return true;
            }
            outPosition = position;
            return false;
        }

        /// <summary> Attempt to get the closest point on the World One NavMesh. </summary>
        public static bool TryGetClosetPointOnWorldOne(Vector3 position, out Vector3 outPosition)
        {
            var maxDistance = GameConfig.NAV_MESH_SAMPLE_DISTANCE;
            var layerMask = LayerMask.NameToLayer(GameConfig.WORLD_ONE_LAYER_NAME);
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, maxDistance, layerMask))
            {
                outPosition = hit.position;
                return true;
            }
            outPosition = position;
            return false;
        }

        /// <summary> Attempt to get the closest point on the World Two NavMesh. </summary>
        public static bool TryGetClosestPointOnWorldTwo(Vector3 position, out Vector3 outPosition)
        {
            var maxDistance = GameConfig.NAV_MESH_SAMPLE_DISTANCE;
            var layerMask = LayerMask.NameToLayer(GameConfig.WORLD_TWO_LAYER_NAME);
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, maxDistance, layerMask))
            {
                outPosition = hit.position;
                return true;
            }
            outPosition = position;
            return false;
        }
    }
}