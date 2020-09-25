using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_ForgedActivationOrders : CardFunction
{

	public override void ActivateInstantFunction()
	{
		base.ActivateInstantFunction();
		ForceIceRez();
	}

	void ForceIceRez()
	{
		List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var ice in ServerSpace.instance.GetComponentsInChildren<Card_Ice>())
		{
			if (!ice.isRezzed) selectors.Add(ice.selector);
		}
		if (selectors.Count <= 0) return;

		CardChooser.instance.ActivateFocus(IceChosen, 1, selectors.ToArray());

		void IceChosen(SelectorNR[] selectorNRs)
		{
			CardChooser.instance.ActivateFocus(null);
			Card chosenCard = selectorNRs[0].GetComponentInParent<Card>();
			ActionOptions.instance.ActivateYesNo(Choice, card.cardTitle, chosenCard.cardCost.costOfCard, "Rez Ice", "Trash Ice");

			void Choice(bool rez)
			{
				if (rez)
				{
					if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, chosenCard.cardCost.costOfCard))
					{
						chosenCard.Rez();
					}
				}
				else
				{
					PlayCardManager.instance.TrashCard(PlayerNR.Corporation, chosenCard);
				}
				CardChooser.instance.DeactivateFocus();
			}
		}

	}
}
