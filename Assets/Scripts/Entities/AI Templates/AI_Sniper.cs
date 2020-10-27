using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Sniper : IAiTemplate
{
    public int MaxHealth = 20;
    public int moveSpeed = 2;
    public int attackDamage = 5;
    public int attackRange = 3;
    public int DifficultyScore { get { return 1;} }

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Sniper";
        entity.appearance.sprite = EntityFactory.GetEnemySprite("Sniper");
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        entity.AIController.AddAction(new MoveWithinRangeOfPlayer(moveSpeed, attackRange), 1);
        entity.AIController.AddAction(new BasicRanged(attackDamage, attackRange), 10);
    }
}
