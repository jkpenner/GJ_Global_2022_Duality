using UnityEngine;

namespace Duality
{
    public static class CombatUtility
    {
        /// <summary>
        /// Get a vector with the shortest distance, aimed from the start position to the target position.
        /// This will check all portals within the given range.
        /// </summary>
        public static Vector3 GetShortestVectorTowardsTarget(Vector3 start, Vector3 target, float portalRange, int portalMask)
        {
            // Calculate the base direction and distance to target
            Vector3 resultVector = target - start;
            float resultDistance = resultVector.magnitude;

            // Check for all near by portals, and check for player 
            foreach (var collider in Physics.OverlapSphere(start, portalRange, portalMask, QueryTriggerInteraction.Collide))
            {
                // Check if we overlapped as portal
                var portal = collider.GetComponent<Portal>();
                if (portal is null)
                {
                    continue;
                }

                if (TryGetVectorThroughPortal(start, target, portal, out Vector3 outVector))
                {
                    var distance = outVector.magnitude;
                    if (distance < resultDistance)
                    {
                        resultDistance = distance;
                        resultVector = outVector;
                    }
                }
            }

            return resultVector;
        }

        public static void DebugDrawVectorThroughPortal(Vector3 start, Vector3 direction, Color color, int portalMask = int.MaxValue)
        {
            var vectorDistance = direction.magnitude;
            var ray = new Ray(start, direction.normalized);

            // Check for all near by portals, and check for player 
            foreach (var collider in Physics.OverlapSphere(start, direction.magnitude, portalMask, QueryTriggerInteraction.Collide))
            {
                // Check if we overlapped as portal
                var portal = collider.GetComponent<Portal>();
                if (portal is null)
                {
                    continue;
                }

                // Ensure the calculated vector goes through the portal's trigger collider
                if (portal.PortalTrigger.Raycast(ray, out var hit, vectorDistance))
                {
                    // Debug.Log("Found portal for vector");
                    Debug.DrawLine(start, hit.point, Color.magenta);

                    var target = GetEndPointThroughPortal(start, direction, portal);
                    var targetDirection = GetDirectionThroughPortal(target, start, portal.ConnectedPortal);

                    var targetRay = new Ray(target, direction.normalized);

                    if (portal.ConnectedPortal.PortalTrigger.Raycast(ray, out var targetHit, vectorDistance))
                    {
                        Debug.DrawLine(target, targetHit.point, Color.magenta);
                    }

                    return;
                }
            }

            Debug.DrawLine(start, start + direction, Color.cyan);
        }

        public static Vector3 GetDirectionThroughPortal(Vector3 start, Vector3 target, Portal portal)
        {
            if (portal is null)
            {
                return Vector3.zero;
            }

            var inTransform = portal.transform;
            var outTransform = portal.ConnectedPortal.transform;

            // Transform start and target into local coords
            var startLocal = inTransform.InverseTransformPoint(start);
            var targetLocal = outTransform.InverseTransformPoint(target);

            // Rotate the targetLocal, since in / out should face opposite directions
            targetLocal = Quaternion.Euler(0f, 180f, 0f) * targetLocal;

            // Calculate a ray between the start and target locations
            return inTransform.TransformDirection(targetLocal - startLocal);
        }

        public static Vector3 GetEndPointThroughPortal(Vector3 start, Vector3 direction, Portal portal)
        {
            if (portal is null)
            {
                return Vector3.zero;
            }

            var inTransform = portal.transform;
            var outTransform = portal.ConnectedPortal.transform;

            var startLocal = inTransform.InverseTransformPoint(start);
            var directionLocal = inTransform.InverseTransformDirection(direction);

            var targetLocal = startLocal + directionLocal;
            targetLocal = Quaternion.Euler(0f, 180f, 0f) * targetLocal;

            return outTransform.TransformPoint(targetLocal);
        }

        /// <summary>
        /// Checks to see if there is a valid vector between start and target that goes through the given portal.
        /// </summary>
        public static bool TryGetVectorThroughPortal(Vector3 start, Vector3 target, Portal portal, out Vector3 outVector)
        {
            var direction = GetDirectionThroughPortal(start, target, portal);
            var ray = new Ray(start, direction.normalized);

            // Ensure the calculated vector goes through the portal's trigger collider
            if (portal.PortalTrigger.Raycast(ray, out var hit, direction.magnitude))
            {
                // Ensure we hit the front of the portal
                if (Vector3.Dot(hit.normal, portal.transform.forward) < 0.9f)
                {
                    outVector = Vector3.zero;
                    return false;
                }

                // Update outVector from local back to world coords
                outVector = direction;
                return true;
            }
            else
            {
                outVector = Vector3.zero;
                return false;
            }
        }


        public static bool IsPlayerInAreaOrAcrossPortal(Vector3 position, float radius, PlayerController target, int portalMask, int playerMask)
        {
            var result = IsPlayerInArea(position, radius, target, playerMask);
            if (!result)
            {
                result = IsPlayerAcrossPortalsInArea(position, radius, target, portalMask, playerMask);
            }
            return result;
        }

        public static bool IsPlayerInArea(Vector3 position, float radius, PlayerController target, int layerMask)
        {
            foreach (var collider in Physics.OverlapSphere(position, radius, layerMask, QueryTriggerInteraction.Ignore))
            {
                // Look for a player in the same active world
                var playerController = collider.GetComponent<PlayerController>();
                if (playerController == target)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPlayerAcrossPortalsInArea(Vector3 position, float radius, PlayerController target, int portalMask, int playerMask)
        {
            // Check for all near by portals, and check for player 
            foreach (var collider in Physics.OverlapSphere(position, radius, portalMask, QueryTriggerInteraction.Collide))
            {
                // Check if we overlapped as portal
                var portal = collider.GetComponent<Portal>();
                if (portal is null)
                {
                    continue;
                }

                // Check if there is an target on the otherside of the portal.
                var distFromPortal = (portal.transform.position - position).magnitude;
                var remainingRange = Mathf.Max(radius - distFromPortal, 0f);

                var connectedPosition = portal.ConnectedPortal.transform.position;
                var player = IsPlayerInArea(connectedPosition, remainingRange, target, playerMask);

                if (player == target)
                {
                    return true;
                }
            }

            return false;
        }

        public static PlayerController GetPlayerInAreaOrAcrossPortal(Vector3 position, float radius, int portalMask, int playerMask)
        {
            var result = GetPlayerInArea(position, radius, playerMask);
            if (result is null)
            {
                result = GetPlayerAcrossPortalsInArea(position, radius, portalMask, playerMask);
            }
            return result;
        }

        /// <summary> Finds the first player within range. </summary>
        /// <returns> First Player found if any </returns>
        public static PlayerController GetPlayerInArea(Vector3 position, float radius, int playerMask)
        {
            foreach (var collider in Physics.OverlapSphere(position, radius, playerMask, QueryTriggerInteraction.Ignore))
            {
                // Look for a player in the same active world
                var playerController = collider.GetComponent<PlayerController>();
                if (playerController is null)
                {
                    continue;
                }

                return playerController;
            }

            return null;
        }

        /// <summary> Check all nearby portals, for if a player is within range of their connected portal. </summary>
        /// <returns> First Player found if any </returns>
        public static PlayerController GetPlayerAcrossPortalsInArea(Vector3 position, float radius, int portalMask, int playerMask)
        {
            // Check for all near by portals, and check for player 
            foreach (var collider in Physics.OverlapSphere(position, radius, portalMask, QueryTriggerInteraction.Collide))
            {
                // Check if we overlapped as portal
                var portal = collider.GetComponent<Portal>();
                if (portal is null)
                {
                    continue;
                }

                // Debug.Log($"Found portal {collider.name}");

                // Check if there is an target on the otherside of the portal.
                var distFromPortal = (portal.transform.position - position).magnitude;
                var remainingRange = Mathf.Max(radius - distFromPortal, 0f);

                var connectedPosition = portal.ConnectedPortal.transform.position;
                var player = GetPlayerInArea(connectedPosition, remainingRange, playerMask);

                if (player is null)
                {
                    continue;
                }

                return player;
            }

            return null;
        }
    }
}