using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Mortar : IAiTemplate
{
    public int MaxHealth = 15;
    public int moveSpeed = 2;
    public int attackDamage = 8;
    public int attackRange = 17;

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Mortar";
        entity.appearance.sprite = EntityFactory.GetEnemySprite("Mortar");
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        entity.AIController.AddAction(new MoveWithinRangeOfPlayer(moveSpeed, attackRange), 1);
        entity.AIController.AddAction(new BasicRanged(attackDamage, attackRange), 10);
    }
}
