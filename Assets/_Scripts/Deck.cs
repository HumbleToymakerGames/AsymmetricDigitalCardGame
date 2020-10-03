using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : PlayArea_Spot, ISelectableNR, IAccessable
{

    public Transform cardsParentT;
    public List<Card> cardsInDeck;
    public GameObject deckCardBacking;

	protected override void Awake()
	{
		base.Awake();
	}

	public void SetCardsToDeck(Card[] cards)
	{
        cardsInDeck = new List<Card>();
		for (int i = 0; i < cards.Length; i++)
		{
            cardsInDeck.Add(cards[i]);
        }
        ShuffleDeck();
        UpdateCardsToParent();
        MakeAllDeckFaceDown();
    }

	void UpdateCardsToParent()
	{
        // Reverse because lowest in hierarchy is visually on top of deck (first card, last child)
        for (int i = cardsInDeck.Count-1; i >= 0; i--)
		{
            cardsInDeck[i].MoveCardTo(cardsParentT);
        }
    }

    public void ShuffleDeck()
	{
        cardsInDeck = Shuffle(cardsInDeck);
	}


    public static List<T> Shuffle<T>(List<T> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            T temp = _list[i];
            int randomIndex = Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }

        return _list;
    }

    void MakeAllDeckFaceDown()
    {
        for (int i = 0; i < cardsInDeck.Count; i++)
        {
            cardsInDeck[i].FlipCard(false, true);
        }
    }

    public bool IsDeckEmpty()
	{
        return cardsInDeck.Count == 0;
	}


    public bool DrawNextCard(ref Card nextCard)
	{
        if (!IsDeckEmpty())
		{
            nextCard = cardsInDeck[0];
            cardsInDeck.RemoveAt(0);

            deckCardBacking.SetActive(!IsDeckEmpty());
            return true;
		}
        return false;
	}

	public bool CanHighlight(bool highlight = true)
	{
        return IsCurrentTurns();
	}

	public bool CanSelect()
	{
        return CanHighlight() && !IsDeckEmpty()
            && PlayCardManager.instance.CanDrawAnotherCard();
            //&& GameManager.instance.IsCurrentTurn(;
	}

	public void Highlighted()
	{
	}

	public void Selected()
	{
        PlayCardManager.instance.TryDrawNextCard();
	}

	public void Access()
	{

    }

    public Card[] GetAccessedCards()
    {
        return !IsDeckEmpty() ? new Card[] { cardsInDeck[0] } : null;
    }

    public void RevealCards(int numCards)
	{
        CardRevealer.instance.RevealCards(cardsInDeck.GetRange(0,5).ToArray(), true, this);
    }

    public override void RemoveCard(Card card)
    {
        cardsInDeck.Remove(card);
    }

	public override List<Card> MyDeck()
	{
        return cardsInDeck;
	}

	protected override void SetMyDeck(List<Card> deck)
	{
        cardsInDeck = deck;
	}



}

