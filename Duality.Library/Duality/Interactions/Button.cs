using System;
using UnityEngine;

namespace Duality
{
    public class Button : MonoBehaviour, IDamagable
    {
        [SerializeField] DamageTypes enableType = DamageTypes.None;
        [SerializeField] bool debugEnable = false;

        public bool IsActive { get; private set; } = false;

        public event System.Action Activated = null;

        private void Update()
        {
            if (debugEnable)
            {
                Activate();
            }
        }

        public bool Damage(float amount, DamageTypes damageType)
        {
            if (IsActive)
            {
                return false;
            }

            if (enableType != DamageTypes.None && damageType != enableType)
            {
                return false;
            }

            Activate();
            return true;
        }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            Activated?.Invoke();
        }
    }
}