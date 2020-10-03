using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : PlayArea_Spot, IAccessable
{
    public Transform cardsParentT;
    public List<Card> cardsInDiscard = new List<Card>();
	public GameObject discardCardBacking;

	protected override void Awake()
	{
		base.Awake();
		UpdateCardBacking();
	}

    public void AddCardToDiscard(Card card)
	{
        card.MoveCardTo(cardsParentT);
        card.FlipCard(false);
        cardsInDiscard.Insert(0, card);
		UpdateCardBacking();
	}

	public override void RemoveCard(Card card)
	{
		base.RemoveCard(card);
        cardsInDiscard.Remove(card);
		UpdateCardBacking();
	}

	public void Access()
	{

	}

	public Card[] GetAccessedCards()
	{
		return cardsInDiscard.ToArray();
	}

	public override List<Card> MyDeck()
	{
		return cardsInDiscard;
	}

	protected override void SetMyDeck(List<Card> deck)
	{
		cardsInDiscard = deck;
	}

	void UpdateCardBacking()
	{
		discardCardBacking.SetActive(cardsInDiscard.Count > 0);
	}


}
