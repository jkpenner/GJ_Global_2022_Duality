using UnityEngine;
using UnityEngine.AI;

namespace Duality
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour, IWorldObject
    {
        [SerializeField] World activeWorld = World.One;
        [SerializeField] float range = 5f;

        [SerializeField] LayerMask playerMask = int.MaxValue;
        [SerializeField] LayerMask portalMask = int.MaxValue;

        private NavMeshAgent navAgent = null;


        private Transform targetPlayer = null;
        private Portal targetPortal = null;

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
        }

        private void CheckPath(Vector3 position, Vector3 target)
        {

        }

        private void Update()
        {
            CheckIfTargetIsGone();

            // if (targetPlayer is null)
            // {
                var player = CheckPlayerInArea(transform.position, range);
                if (player != null)
                {
                    targetPlayer = player.transform;
                    targetPortal = null;
                }
                else
                {
                    var found = CheckPlayerAcrossPortalInArea(transform.position, range);
                    if (found.Item1 != null && found.Item2 != null)
                    {
                        targetPlayer = found.Item1.transform;
                        targetPortal = found.Item2;
                    }
                    else
                    {
                        targetPlayer = null;
                        targetPortal = null;
                    }
                }
            // }

            UpdateTargetDestination(targetPlayer, targetPortal);
        }

        private void CheckIfTargetIsGone()
        {
            // if (targetPlayer is null)
            // {
            //     return;
            // }

            // // Moving to a portal go there first, as along as in same world
            // if (targetPortal != null && targetPortal.World == activeWorld)
            // {
            //     return;
            // }

            // targetPortal = null;

            // // Check if the target player is out of range
            // if (Vector3.Distance(targetPlayer.position, transform.position) > range)
            // {
            //     Debug.Log("Clearing target, player out of range");
            //     targetPlayer = null;
            //     targetPortal = null;
            // }
        }

        private void UpdateTargetDestination(Transform targetPlayer, Portal targetPortal)
        {
            // No target just set destination to our current location.
            if (targetPlayer is null)
            {
                navAgent.destination = transform.position;
                return;
            }

            // No portal needed to go through, go straight to the player.
            if (targetPortal is null)
            {
                navAgent.destination = targetPlayer.transform.position;
            }
            // Need to go through portal, go there first
            else
            {
                navAgent.destination = targetPortal.transform.position;
            }
        }

        private PlayerController CheckPlayerInArea(Vector3 position, float radius, bool sameWorld = true)
        {

            foreach (var collider in Physics.OverlapSphere(position, radius, int.MaxValue, QueryTriggerInteraction.Ignore))
            {
                // Look for a player in the same active world
                var playerController = collider.GetComponent<PlayerController>();
                if (playerController is null)
                {
                    continue;
                }

                if (sameWorld && playerController.ActiveWorld != activeWorld)
                {
                    continue;
                }

                return playerController;
            }

            return null;
        }

        private (PlayerController, Portal) CheckPlayerAcrossPortalInArea(Vector3 position, float radius)
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
                var distFromPortal = (portal.transform.position - transform.position).magnitude;
                var remainingRange = Mathf.Max(range - distFromPortal, 0f);

                var connectedPosition = portal.ConnectedPortal.transform.position;
                var player = CheckPlayerInArea(connectedPosition, remainingRange, false);

                if (player is null)
                {
                    continue;
                }

                return (player, portal);
            }

            Debug.Log("No Portals found in range");
            return (null, null);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, range);

            foreach (var collider in Physics.OverlapSphere(transform.position, range, portalMask, QueryTriggerInteraction.Collide))
            {
                // Check if we overlapped as portal
                var portal = collider.GetComponent<Portal>();
                if (portal is null)
                {
                    continue;
                }

                Debug.Log($"Found portal {collider.name}");

                // Check if there is an target on the otherside of the portal.
                var distFromPortal = (portal.transform.position - transform.position).magnitude;
                var remainingRange = Mathf.Max(range - distFromPortal, 0f);

                var connectedPosition = portal.ConnectedPortal.transform.position;

                Gizmos.DrawWireSphere(connectedPosition, remainingRange);
            }

            if (targetPortal != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(targetPortal.transform.position, 1.25f);
            }
            else if (targetPlayer != null) 
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(targetPlayer.position, 1.25f);
            }
        }

        public void SetWorld(World world)
        {
            activeWorld = world;
        }

        public void FlipWorld()
        {
            if (activeWorld == World.One)
            {
                SetWorld(World.Two);
            }
            else
            {
                SetWorld(World.One);
            }
        }

        public void WrapPosition(Vector3 position, Quaternion rotation)
        {
            navAgent.updatePosition = false;
            navAgent.updateRotation = false;

            navAgent.Warp(position);
            navAgent.transform.rotation = rotation;

            navAgent.updatePosition = true;
            navAgent.updateRotation = true;
        }
    }
}