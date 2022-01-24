using System.Collections.Generic;
using UnityEngine;

namespace Duality
{   
    [System.Serializable]
    public struct DamageResistance
    {
        public DamageTypes damageType;
        public float reduction;        
    }

    public class Health : MonoBehaviour, IDamagable, IKillable {
        [Tooltip("If invincible can not talk damage or die")]
        [SerializeField] bool isInvincible = false;
        [SerializeField] bool isAlive = true;
        [SerializeField] float maxHealth = 10f;

        [SerializeField] List<DamageResistance> resistances;
        

        public bool IsAlive => isAlive;
        public bool IsInvincible => isInvincible;
        public float MaxHealth => maxHealth;

        public float CurHealth { get; private set; }

        public event System.Action Damaged = null;
        public event System.Action Killed = null;

        private void Awake() {
            Reset();    
        }

        public void Reset()
        {
            isAlive = true;
            CurHealth = MaxHealth;
        }

        public bool Damage(float amount, DamageTypes damageType)
        {
            // Can't damage the dead!
            if (!IsAlive || IsInvincible)
            {
                return false;
            }

            float resistance = GetResistanceValue(damageType);

            // If greater then 1 then immune to the damage type
            if (resistance >= 1f)
            {
                return false;
            }

            ApplyDamage(amount * (1f - Mathf.Clamp01(resistance)));
            return true;
        }

        private void ApplyDamage(float amount)
        {
            if (!IsAlive)
            {
                return;
            }

            CurHealth -= amount;
            Damaged?.Invoke();

            if (CurHealth <= 0f)
            {
                Kill();
            }
        }

        public void Kill()
        {
            if (!IsAlive)
            {
                return;
            }

            isAlive = false;
            Killed?.Invoke();
        }

        public float GetResistanceValue(DamageTypes damageType)
        {
            foreach(var resistance in resistances)
            {
                if (resistance.damageType == damageType)
                {
                    return resistance.reduction;
                }
            }

            return 0f;
        }
    }
}