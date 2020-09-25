using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_DataDealer : CardFunction
{
	public int numCredits;

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0)
			{
				ability.SetAbilityAndCondition(DealData, CanBeClickable);
				ability.SetCostsAndPayments(PayCosts, CanAfford);
			}
		}
	}

	bool CanAfford()
	{
		return PlayArea.instance.ScoringAreaNR(PlayerNR.Runner).cardsInScoringArea.Count > 0;
	}

	void PayCosts()
	{
		PlayCardManager.instance.ForfeitAgenda(PlayerNR.Runner, PlayArea.instance.ScoringAreaNR(PlayerNR.Runner).cardsInScoringArea[0]);
	}

	bool CanBeClickable()
	{
		return true;
	}

	IEnumerator DealData()
	{
		card.myPlayer.AddCredits(numCredits);
		yield break;
	}
}
