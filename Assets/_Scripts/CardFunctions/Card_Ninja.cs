using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Ninja : CardFunction
{
    public int strengthToAdd;

	public override void ActivateInstantFunction()
	{
		base.ActivateInstantFunction();
	}


	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(BreakSubroutine, CanBeClickable);
			if (i == 1) ability.SetAbilityAndCondition(AddStrength, CanBeClickable);
		}
	}

	bool CanBeClickable()
	{
		return true;
	}

	IEnumerator BreakSubroutine()
	{
		RunOperator.instance.BreakingSubroutines(paidAbilities[0]);
		yield break;
	}

	IEnumerator AddStrength()
	{
		Card_Program programCard = card as Card_Program;
		programCard.ModifyStrength(strengthToAdd);
		yield break;
	}

}
