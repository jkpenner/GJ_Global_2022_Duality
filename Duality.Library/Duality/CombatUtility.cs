using UnityEngine;

namespace Duality
{
    public static class CombatUtility
    {
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

                Debug.Log($"Found portal {collider.name}");

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