using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEntity : Entity
{
    public new void Initialize()
    {
        base.Initialize();
        Health.OnDeath += Die;
    }

    public void ReceiveDamage(CombatEntity attacker, int damage)
    {
        Debug.Log("i'm been attacked by " + attacker.entityName);
        Health.ApplyDamage(damage);
    }

    public void DealDamageTo(CombatEntity victim, int damage)
    {
        Debug.Log("i'm attacking " + victim.entityName);
        victim.ReceiveDamage(this, damage);
    }

    public void Die()
    {
        Debug.Log("i'm dead");
    }
}
