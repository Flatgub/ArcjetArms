using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Mechanic : IAiTemplate
{
    public int MaxHealth = 20;
    public int moveSpeed = 2;
    public int healAmount = 5;

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Mechanic";
        entity.appearance.sprite = EntityFactory.GetEnemySprite("Mechanic");
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        entity.AIController.AddAction(new MoveTowardsEnemy(moveSpeed),1);
        entity.AIController.AddAction(new ApplyHealing(healAmount), 1);
    }
}
