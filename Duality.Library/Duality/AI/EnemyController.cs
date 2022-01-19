using UnityEngine;
using UnityEngine.AI;

namespace Duality
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour, IHasSpawnPoint, IWorldObject
    {
        [SerializeField] World activeWorld = World.One;
        [SerializeField] float range = 5f;
        [SerializeField] float targetForgetTime = 5f;

        [SerializeField] LayerMask playerMask = int.MaxValue;
        [SerializeField] LayerMask portalMask = int.MaxValue;

        private NavMeshAgent agent = null;
        private Coroutine teleportRoutine = null;


        private PlayerController targetPlayer = null;
        private Portal targetPortal = null;
        private float targetForgetCounter = 0f;

        public ObjectSpawn Spawn { get; set; }
        public bool IgnorePortals => true;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void CheckPath(Vector3 position, Vector3 target)
        {

        }

        private void Update()
        {
            CheckIfTargetIsGone();

            if (targetPlayer is null)
            {
                targetPlayer = CombatUtility.GetPlayerInAreaOrAcrossPortal(
                    transform.position, range, portalMask, playerMask
                );
            }

            UpdateTargetDestination(targetPlayer);

            if (agent.isOnOffMeshLink && teleportRoutine is null)
            {
                teleportRoutine = StartCoroutine(
                    NavUtility.HandleTeleport(agent, () =>
                    {
                        Debug.Log("Teleport complete");
                        teleportRoutine = null;
                        FlipWorld();
                    })
                );
            }
        }

        private void CheckIfTargetIsGone()
        {
            if (targetPlayer is null)
            {
                return;
            }

            var isPlayerInArea = CombatUtility.IsPlayerInAreaOrAcrossPortal(
                transform.position,
                range,
                targetPlayer,
                portalMask,
                playerMask
            );

            if (!isPlayerInArea)
            {
                targetForgetCounter -= Time.deltaTime;
                if (targetForgetCounter < 0f)
                {
                    Debug.Log("Lost player");
                    targetPlayer = null;
                }
            }
            else
            {
                targetForgetCounter = targetForgetTime;
            }
        }

        private void UpdateTargetDestination(PlayerController targetPlayer)
        {
            // No target just set destination to our current location.
            if (targetPlayer is null)
            {
                agent.destination = transform.position;
                agent.isStopped = true;
                return;
            }

            agent.destination = targetPlayer.transform.position;
            agent.isStopped = false;
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
                Gizmos.DrawWireSphere(targetPlayer.transform.position, 1.25f);
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

        }
    }
}