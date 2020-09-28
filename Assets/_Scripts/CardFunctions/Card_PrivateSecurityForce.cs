using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_PrivateSecurityForce : CardFunction
{
	public int numMeatDamage;

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(DoMeatDamage, CanBeClickable);
		}
	}

	bool CanBeClickable()
	{
		return PlayerNR.Runner.Tags > 0;
	}

	IEnumerator DoMeatDamage()
	{
		yield return PlayCardManager.instance.DoRunnerDamage(null, numMeatDamage, DamageType.Meat);

	}

}
