using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_RocketMech : IAiTemplate
{
    public int MaxHealth = 20;
    public int moveSpeed = 2;
    public int attackDamage = 10;
    public int attackRange = 6;

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Rocket Deuce";
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        entity.AIController.AddAction(new MoveWithinRangeOfPlayer(moveSpeed, attackRange), 1);
        entity.AIController.AddAction(new BasicRanged(attackDamage, attackRange), 10);
    }
}
