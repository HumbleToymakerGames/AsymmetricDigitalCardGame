using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_ProjectJunebug : CardFunction
{
	public int numCredits, numNetDamage;
	bool cardAccessed;

	protected override void OnEnable()
	{
		base.OnEnable();
		Card.OnCardAccessed += Card_OnCardAccessed;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		Card.OnCardAccessed -= Card_OnCardAccessed;
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(TryDoNetDamage, CanDoNetDamage);
		}
	}

	private void Card_OnCardAccessed(Card card, ServerColumn.ServerType serverType)
	{
		cardAccessed = card == this.card;
	}


	bool CanDoNetDamage()
	{
		return cardAccessed && card.isRezzed;
	}

	IEnumerator TryDoNetDamage()
	{
		bool? payOption = null;
		ActionOptions.instance.ActivateYesNo(PayOption, "Pay to activate?", numCredits);
		while (!payOption.HasValue) yield return null;
		if (!payOption.Value) yield break;

		void PayOption(bool yes)
		{
			payOption = false;
			if (yes)
			{
				if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, numCredits))
				{
					payOption = true;
				}
			}
		}


		print("junebug accessed");
		int numAdvancements = card.cardAdvancer.advancementAmount;

		bool damageDone = false;
		PlayCardManager.instance.DoNetDamage(DamageDone, numNetDamage * numAdvancements);
		while (!damageDone) yield return null;

		void DamageDone()
		{
			damageDone = true;
			print("JunDamage!");
		}




		cardAccessed = false;
		yield break;
	}





}
