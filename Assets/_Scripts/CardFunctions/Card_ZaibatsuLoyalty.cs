using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_ZaibatsuLoyalty : CardFunction
{
	public int numCreditsToPayForAbility;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardExposed_Pre += PlayCardManager_OnCardExposed_Pre;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardExposed_Pre -= PlayCardManager_OnCardExposed_Pre;
	}

	private void PlayCardManager_OnCardExposed_Pre(Card card)
	{

	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(ActivateAbility, CanBeActivated);
		}
	}

	bool CanBeActivated()
	{
		return card.isInstalled;
	}

	IEnumerator ActivateAbility()
	{
		print("Card_ZaibatsuLoyalty");

		CardChooser.instance.ActivateFocus(null);
		bool? choiceMade = null;

		if (!card.isRezzed)
		{
			ActionOptions.instance.ActivateYesNo(Choice, string.Format("Rez {0}?", card.cardTitle));
			while (!choiceMade.HasValue) yield return null;
			if (choiceMade.Value)
			{
				card.Rez();
			}
		}

		choiceMade = null;
		ActionOptions.instance.ActivateYesNo(Choice, string.Format("Activate {0}?", card.cardTitle));
		while (!choiceMade.HasValue) yield return null;

		if (choiceMade.Value)
		{
			choiceMade = null;
			ActionOptions.instance.ActivateYesNo(Choice, card.cardTitle, numCreditsToPayForAbility, "Pay credits", "Trash card");
			while (!choiceMade.HasValue) yield return null;


			if (choiceMade.Value)
			{
				if (PlayCardManager.instance.TryAffordCost(card.myPlayer, numCreditsToPayForAbility))
				{
					PlayCardManager.instance.StopExposeCard();
				}
			}
			else
			{
				PlayCardManager.instance.StopExposeCard();
				PlayCardManager.instance.TryTrashCard(card.myPlayer, card);
			}
		}


		CardChooser.instance.DeactivateFocus();



		void Choice(bool yes)
		{
			choiceMade = yes;
		}

		

		

	}





}
