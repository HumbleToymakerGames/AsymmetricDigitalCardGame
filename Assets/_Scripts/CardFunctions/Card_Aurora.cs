using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Aurora : CardFunction
{
    public int strengthToAdd;

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(BreakSubroutine, Clickable_Breaker);
			if (i == 1) ability.SetAbilityAndCondition(AddStrength, CanBeClickable);
		}
	}

	bool Clickable_Breaker()
	{
		return RunOperator.instance.canBreakSubroutines;
	}

	bool CanBeClickable()
	{
		return RunOperator.instance.isRunning;
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
