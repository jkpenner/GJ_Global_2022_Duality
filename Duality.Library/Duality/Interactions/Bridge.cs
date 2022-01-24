using System;
using UnityEngine;
using UnityEngine.AI;

namespace Duality
{
    public class Bridge : MonoBehaviour
    {
        [SerializeField] Button activator = null;
        [SerializeField] NavMeshObstacle navObstacle = null;
        [SerializeField] new Collider collider = null;

        [Header("Animations")]
        [SerializeField] Animator animator = null;
        [SerializeField] string enableKey = "Enabled";

        private void OnEnable()
        {
            if (activator is null)
            {
                return;
            }

            activator.Activated += OnActivated;
            SetActiveState(activator.IsActive);
        }

        private void OnDisable()
        {
            if (activator is null)
            {
                return;
            }

            activator.Activated -= OnActivated;
        }

        private void OnActivated()
        {
            Debug.Log("Bridge Activated");
            SetActiveState(true);
        }

        private void SetActiveState(bool isActive)
        {
            if (animator != null)
            {
                animator.SetBool(enableKey, isActive);
            }
            else
            {
                Debug.LogWarning("No animator assigned to bridge");
            }

            // navObstacle should be disabled when bridge is active
            navObstacle.enabled = !isActive;
            collider.enabled = isActive;
        }
    }
}