using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_Lancer : IAiTemplate
{
	public int MaxHealth = 30;
	public int moveSpeed = 3;
	public int attackRange = 3;
	public int RangeDamage = 3;
	public int attackDamage = 7;
	public int DifficultyScore { get { return 2;} }

	public void ApplyTo(Entity entity)
	{
		entity.entityName = "Lancer";
		entity.appearance.sprite = EntityFactory.GetEnemySprite("Lancer");
		entity.Health.SetMaxHealth(MaxHealth, updateHealth: true);
		entity.AIController.AddAction(new MoveWithinRangeOfPlayer(moveSpeed, attackRange), 7);
		entity.AIController.AddAction(new BasicMelee(attackDamage), 10);
		entity.AIController.AddAction(new BasicRanged(RangeDamage, attackRange), 5);
	}
}

