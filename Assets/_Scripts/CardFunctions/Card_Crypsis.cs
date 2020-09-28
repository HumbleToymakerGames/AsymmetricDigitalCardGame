using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Crypsis : CardFunction
{
	public int numStrength, numVirusCounters;

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(BreakSubroutine, Clickable_Breaker);
			if (i == 1) ability.SetAbilityAndCondition(AddStrength, Clickable_Strength);
			if (i == 2) ability.SetAbilityAndCondition(PlaceVirusCounter, Clickable_Virus);
		}
	}

	bool Clickable_Breaker()
	{
		return RunOperator.instance.canBreakSubroutines;
	}
	IEnumerator BreakSubroutine()
	{
		RunOperator.instance.BreakingSubroutines(paidAbilities[0]);
		yield break;
	}


	bool Clickable_Strength()
	{
		return RunOperator.instance.isRunning;
	}
	IEnumerator AddStrength()
	{
		Card_Program programCard = card as Card_Program;
		programCard.ModifyStrength(numStrength);
		yield break;
	}

	bool Clickable_Virus()
	{
		return true;
	}
	IEnumerator PlaceVirusCounter()
	{
		card.NeutralCounters += numVirusCounters;
		yield break;
	}

}
