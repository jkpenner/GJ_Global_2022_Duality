using System.Net;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

namespace Duality
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class TestFollower : MonoBehaviour
    {
        [SerializeField] Transform target = null;
        [SerializeField] bool randomizeTarget = true;
        [SerializeField] Vector3 randomAreaSize = new Vector3(10f, 10f, 10f);




        private NavMeshAgent agent = null;
        private Coroutine teleportRoutine = null;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.autoTraverseOffMeshLink = false;

            UpdateTargetRandom();
        }

        private void Update()
        {
            if (target is null)
            {
                agent.destination = transform.position;
                agent.isStopped = true;
                return;
            }

            agent.destination = target.position;
            agent.isStopped = false;

            agent.updateRotation = false;
            transform.Rotate(new Vector3(0f, 2f, 0f), Space.Self);
            // transform.rotation = Quaternion.LookRotation(agent.nextPosition - transform.position, transform.up);

            if (agent.isOnOffMeshLink && teleportRoutine is null)
            {
                teleportRoutine = StartCoroutine(
                    NavUtility.HandleTeleport(agent, () =>
                    {
                        Debug.Log("Teleport complete");
                        teleportRoutine = null;
                    })
                );
            }

            if (Vector3.Distance(target.position, transform.position) < 1f)
            {
                UpdateTargetRandom();
            }
        }

        private void UpdateTargetRandom()
        {
            var randomPosition = new Vector3(
                Random.Range(-randomAreaSize.x, randomAreaSize.x),
                Random.Range(-randomAreaSize.y, randomAreaSize.y),
                Random.Range(-randomAreaSize.z, randomAreaSize.z)
            );

            if (NavUtility.TryGetClosetPoint(randomPosition, out Vector3 position))
            {
                target.position = position;
            }
        }

        private void OnDrawGizmos()
        {
            if (randomizeTarget)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(Vector3.zero, randomAreaSize * 2f);
            }
        }
    }
}