using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : PlayArea_Spot, IAccessable
{
    public Transform cardsParentT;
    public List<Card> cardsInDiscard = new List<Card>();

	protected override void Awake()
	{
		base.Awake();
    }

    public void AddCardToDiscard(Card card)
	{
        card.MoveCardTo(cardsParentT);
        card.FlipCard(false);
        cardsInDiscard.Insert(0, card);
    }

	public override void RemoveCard(Card card)
	{
		base.RemoveCard(card);
        cardsInDiscard.Remove(card);
	}

	public void Access()
	{
		if (cardsInDiscard.Count > 0)
			CardRevealer.instance.RevealCards(cardsInDiscard.ToArray(), true, this);
	}


	public override List<Card> MyDeck()
	{
		return cardsInDiscard;
	}

	protected override void SetMyDeck(List<Card> deck)
	{
		cardsInDiscard = deck;
	}


}
