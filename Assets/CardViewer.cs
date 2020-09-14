using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
	public static CardViewer instance;
    public Card_Program program;
    public Card_Ice ice;

	public CardViewWindow viewWindow_Primary, viewWindow_Secondary;

	public struct CardPair
	{
		public Card realCard, viewCard;
	}
	public Dictionary<int, CardPair> cardPairDict = new Dictionary<int, CardPair>();

	private void Awake()
	{
		instance = this;
	}

	void Start()
    {
		StartCoroutine(CardPopulateDelay());
    }

	void ParentCard_Start(Card card)
	{
		card.FlipCardUp(true);
		SelectorNR selector = card.GetComponentInChildren<SelectorNR>();
		if (selector)
		{
			selector.highlighter.gameObject.SetActive(false);
			selector.gameObject.SetActive(false);
		}
		ParentCard(card, true);
	}

	void ParentCard(Card card, bool primary = true)
	{
		if (primary) viewWindow_Primary.ParentCard(card);
		else viewWindow_Secondary.ParentCard(card);
	}

	public void ViewCard(int viewIndex, bool primary = true)
	{
		bool viewer = false;
		if (IsCardBeingViewed(viewIndex, ref viewer)) return;

		if (primary) viewWindow_Primary.ViewCard(viewIndex);
		else viewWindow_Secondary.ViewCard(viewIndex);
	}

	public void HideAllCards(bool primary)
	{
		foreach (var kvp in cardPairDict)
		{
			Card viewCard = kvp.Value.viewCard;
			Transform cardsT = primary ? viewWindow_Primary.cardsT : viewWindow_Secondary.cardsT;
			if (viewCard.transform.parent == cardsT)
				kvp.Value.viewCard.gameObject.SetActive(false);
		}
	}

	IEnumerator CardPopulateDelay()
	{
		yield return new WaitForSeconds(1);
		int viewIndex = 0;

		// Deck cards
		List<Card> allDeckCards = new List<Card>();
		for (int i = 0; i < PlayerNR.Runner.deckCards.Length; i++)
		{
			allDeckCards.Add(PlayerNR.Runner.deckCards[i]);
		}
		for (int i = 0; i < PlayerNR.Corporation.deckCards.Length; i++)
		{
			allDeckCards.Add(PlayerNR.Corporation.deckCards[i]);
		}

		for (int i = 0; i < allDeckCards.Count; i++)
		{
			Card deckCard = allDeckCards[i];
			deckCard.viewIndex = viewIndex;

			Card viewCard = Instantiate(deckCard);
			cardPairDict[viewIndex] = new CardPair() { realCard = deckCard, viewCard = viewCard };
			ParentCard_Start(viewCard);
			viewIndex++;
		}

		// Identity cards
		for (int i = 0; i < 2; i++)
		{
			Card identityCard = i == 0 ? PlayerNR.Runner.identity : PlayerNR.Corporation.identity;
			identityCard.viewIndex = viewIndex;

			Card viewCard = Instantiate(identityCard);
			cardPairDict[viewIndex] = new CardPair() { realCard = identityCard, viewCard = viewCard };
			ParentCard_Start(viewCard);
			viewIndex++;
		}

		yield return new WaitForSeconds(0.25f);
		PaidAbility[] allPaidAbilities = GetComponentsInChildren<PaidAbility>(true);
		for (int i = 0; i < allPaidAbilities.Length; i++)
		{
			allPaidAbilities[i].SetClickable(true);
			//CardPair cardPair;
			//if (cardPairDict.TryGetValue(allPaidAbilities[i].cardFunction.card.viewIndex, out cardPair))
			//{
			//	allPaidAbilities[i].SetCardFunctions(cardPair.realCard.cardFunction, cardPair.viewCard.cardFunction);
			//}
		}

		HideAllCards(true);
		HideAllCards(false);

	}

	public int targetIndex;
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.G))
		{
			CardPair pair;
			if(cardPairDict.TryGetValue(targetIndex, out pair))
			{
				Debug.Log(pair.realCard.name, pair.realCard);
				//ViewCard(targetIndex, false);
				//ViewCard(targetIndex);
				PinCard(targetIndex, false);
			}
		}
    }

	public Card GetCard(int viewIndex, bool real)
	{
		CardPair cardPair;
		if (cardPairDict.TryGetValue(viewIndex, out cardPair))
		{
			return real ? cardPair.realCard : cardPair.viewCard;
		}
		return null;
	}


	public void PinCard(int viewIndex, bool primary = true)
	{
		Card card = GetCard(viewIndex, false);
		bool viewer = false;
		if (card && IsCardBeingViewed(card.viewIndex, ref viewer) && viewer != primary) return;

		if (primary) viewWindow_Primary.PinCard(card);
		else viewWindow_Secondary.PinCard(card);
	}



	public bool IsCardBeingViewed(int viewIndex, ref bool viewer)
	{
		bool primary = viewWindow_Primary.currentViewIndex == viewIndex;
		bool secondary = viewWindow_Secondary.currentViewIndex == viewIndex;
		if (primary) viewer = true;
		if (secondary) viewer = false;
		return primary || secondary;
	}


}
