using UnityEngine;

namespace Duality
{
    public class PortalPair : MonoBehaviour
    {
        public const float OFFSET = 1f;

        [SerializeField] Portal worldOnePortal = null;
        [SerializeField] Portal worldTwoPortal = null;

        public Portal WorldOne => worldOnePortal;
        public Portal WorldTwo => worldTwoPortal;

        private void OnDrawGizmos()
        {
            if (WorldOne is null || WorldTwo is null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                WorldOne.transform.position + WorldOne.transform.up * OFFSET,
                WorldTwo.transform.position + WorldTwo.transform.up * OFFSET
            );
        }
    }
}