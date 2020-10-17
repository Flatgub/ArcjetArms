﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_LightAttacker : IAiTemplate
{
    public int MaxHealth = 30;
    public int moveSpeed = 3;
    public int attackDamage = 5;

    public void ApplyTo(Entity entity)
    {
        entity.entityName = "Light Mobile Attacker";
        entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
        entity.AIController.AddAction(new MoveTowardsPlayer(moveSpeed),1);
        entity.AIController.AddAction(new ApproachMelee(moveSpeed-1, attackDamage), 10);
        entity.AIController.AddAction(new BasicMelee(attackDamage), 20);
    }
}
