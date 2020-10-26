using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Scavva : IAiTemplate
{
    public int MaxHealth = 30;
    public int moveSpeed = 4;
    public int attackDamage = 5;
    public int attackRange = 1;
    // Start is called before the first frame update

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Scavva";
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        entity.AIController.AddAction(new MoveWithinRangeOfPlayer(moveSpeed, attackRange), 1);
        entity.AIController.AddAction(new BasicRanged(attackDamage, attackRange), 10);
    }
}
