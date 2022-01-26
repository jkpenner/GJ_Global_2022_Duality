using UnityEngine;
using UnityEngine.AI;

namespace Duality
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour, IHasSpawnPoint, IWorldObject
    {
        [SerializeField] float range = 5f;
        [SerializeField] float targetForgetTime = 5f;

        [SerializeField] LayerMask playerMask = int.MaxValue;
        [SerializeField] LayerMask portalMask = int.MaxValue;

        private NavMeshAgent agent = null;
        private Coroutine teleportRoutine = null;


        [Header("World Settings")]
        [SerializeField] World activeWorld = World.White;
        [SerializeField] EnemyVisual blackWorldVisual = null;
        [SerializeField] EnemyVisual whiteWorldVisual = null;
        [SerializeField] bool flipWorldVisuals = false;

        private PlayerController targetPlayer = null;
        private Portal targetPortal = null;
        private float targetForgetCounter = 0f;

        public ObjectSpawn Spawn { get; set; }
        public bool IgnorePortals => true;

        private EnemyVisual activeVisual = null;

        private Vector3 targetPlayerDirection = Vector3.zero;

        public Health Health { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            Health.Killed += OnKilled;

            agent = GetComponent<NavMeshAgent>();
            ChangeVisual(activeWorld);
        }


        private void CheckPath(Vector3 position, Vector3 target)
        {

        }

        private void ChangeVisual(World world)
        {
            if (world == World.White)
            {
                if (flipWorldVisuals)
                {
                    activeVisual = blackWorldVisual;
                }
                else
                {
                    activeVisual = whiteWorldVisual;
                }
            }
            else
            {
                if (flipWorldVisuals)
                {
                    activeVisual = whiteWorldVisual;
                }
                else
                {
                    activeVisual = blackWorldVisual;
                }
            }

            whiteWorldVisual.gameObject.SetActive(whiteWorldVisual == activeVisual);
            blackWorldVisual.gameObject.SetActive(blackWorldVisual == activeVisual);
        }

        private void Update()
        {
            if (!Health.IsAlive)
            {
                targetPlayer = null;
                UpdateTargetDestination(null);
                return;
            }

            CheckIfTargetIsGone();

            if (targetPlayer is null)
            {
                targetPlayer = CombatUtility.GetPlayerInAreaOrAcrossPortal(
                    transform.position, range, portalMask, playerMask
                );
            }

            UpdateTargetDestination(targetPlayer);

            // update the aim direction towards the player
            if (targetPlayer != null)
            {
                var currentSpawnPoint = activeVisual.Shoot.CurrentSpawnPoint;

                targetPlayerDirection = CombatUtility.GetShortestVectorTowardsTarget(
                    currentSpawnPoint.position, targetPlayer.AITargetingTransform.position, range, portalMask
                );

                CombatUtility.DebugDrawVectorThroughPortal(currentSpawnPoint.position, targetPlayerDirection, Color.green, portalMask);

                activeVisual.Shoot.Fire(
                    currentSpawnPoint.position + targetPlayerDirection
                );
            }
            else
            {
                targetPlayerDirection = Vector3.zero;
            }

            if (agent.isOnOffMeshLink && teleportRoutine is null)
            {
                teleportRoutine = StartCoroutine(
                    NavUtility.HandleTeleport(agent, () =>
                    {
                        // Debug.Log("Teleport complete");
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
                    // Debug.Log("Lost player");
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

                // Debug.Log($"Found portal {collider.name}");

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
            ChangeVisual(world);
        }

        public void FlipWorld()
        {
            if (activeWorld == World.White)
            {
                SetWorld(World.Black);
            }
            else
            {
                SetWorld(World.White);
            }
        }

        public void WrapPosition(Vector3 position, Quaternion rotation)
        {
            agent.Warp(position);
            transform.position = position;
            transform.rotation = rotation;
        }
        
        private void OnKilled()
        {
            if (Spawn is null)
            {
                Destroy(this.gameObject);
                return;
            }

            Spawn.Respawn(this.gameObject);
            Health.Reset();
        }
    }
}