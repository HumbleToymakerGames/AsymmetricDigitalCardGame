using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_MelangeMiningCorp : CardFunction
{
    public int numCredits;

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(GainCredits, CanBeClickable);
		}
	}

	bool CanBeClickable()
	{
		return card.isRezzed;
	}

	IEnumerator GainCredits()
    {
        PlayerNR.Corporation.AddCredits(numCredits);
		yield break;
    }



}
