using UnityEngine;

namespace Duality
{
    public class DeathZone : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            var killable = other.GetComponent<IKillable>();
            if (killable != null)
            {
                killable.Kill();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}