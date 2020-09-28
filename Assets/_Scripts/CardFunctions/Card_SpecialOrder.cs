using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SpecialOrder : CardFunction
{
	public override void ActivateInstantFunction()
	{
		base.ActivateInstantFunction();
		SpecialOrder();
	}

	void SpecialOrder()
	{
		Deck myDeck = PlayArea.instance.DeckNR(PlayerNR.Runner);
		foreach (var card in myDeck.cardsInDeck)
		{
			if (card is Card_Program)
			{
				if (card.cardFunction.HasBreaker())
				{
					PlayArea.instance.HandNR(PlayerNR.Runner).AddCardsToHand(card);
					card.FlipCard(true);
					myDeck.ShuffleDeck();
					break;
				}
			}
		}

	}
}
