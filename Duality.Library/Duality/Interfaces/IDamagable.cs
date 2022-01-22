using UnityEngine;

namespace Duality
{
    public interface IDamagable
    {
        bool Damage(float amount, DamageTypes damageType);
    }
}