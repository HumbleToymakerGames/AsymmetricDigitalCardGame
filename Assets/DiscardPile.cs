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
        cardsInDiscard.Add(card);
    }

	public override void RemoveCard(Card card)
	{
		base.RemoveCard(card);
        cardsInDiscard.Remove(card);
	}

	public void Access()
	{
        CardRevealer.instance.RevealCards(cardsInDiscard.ToArray());
	}

}
