using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
	public static CardViewer instance;
	public CardViewWindow viewWindow_Primary, viewWindow_Secondary;
	public bool cardsClickableOnView;
	public GameObject linkIcon;

	public class CardPair
	{
		public Card realCard, viewCard;
	}
	public Dictionary<int, CardPair> cardPairDict = new Dictionary<int, CardPair>();
	Card[] viewCards;

	private void Awake()
	{
		instance = this;
		ShowLinkIcon(false);
	}

	private void OnEnable()
	{
		RunOperator.OnRunStarted += RunOperator_OnRunStarted;
	}
	private void OnDisable()
	{
		RunOperator.OnRunStarted -= RunOperator_OnRunStarted;
	}

	private void RunOperator_OnRunStarted()
	{

	}

	void Start()
    {
		StartCoroutine(CardPopulateDelay());
    }

	void ParentCard_Start(Card card)
	{
		card.FlipCard(true, true);
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

		ReplicateCardState(viewIndex);
		if (primary) viewWindow_Primary.ViewCard(viewIndex, cardsClickableOnView);
		else viewWindow_Secondary.ViewCard(viewIndex, cardsClickableOnView);
	}

	public void HideAllCards(bool primary)
	{
		foreach (var kvp in cardPairDict)
		{
			Card viewCard = kvp.Value.viewCard;
			Transform cardsT = primary ? viewWindow_Primary.cardsT : viewWindow_Secondary.cardsT;
			if (viewCard.transform.parent == cardsT)
			{
				kvp.Value.viewCard.ActivateVisuals(false);
				kvp.Value.viewCard.ActivateRaycasts(false);
			}
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

		viewCards = new Card[allDeckCards.Count];
		for (int i = 0; i < allDeckCards.Count; i++)
		{
			Card deckCard = allDeckCards[i];
			deckCard.viewIndex = viewIndex;

			deckCard.isViewCard = true;
			Card viewCard = Instantiate(deckCard);
			deckCard.isViewCard = false;

			viewCards[i] = viewCard;
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
		SetCardsClickable(false);

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
		if (viewIndex != -1)
		{
			bool viewer = false;
			if (card && IsCardBeingViewed(card.viewIndex, ref viewer) && viewer != primary) return;
		}

		ReplicateCardState(viewIndex);
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


	public void SetCardsClickable(bool clickable = true)
	{
		cardsClickableOnView = clickable;
		Card primaryViewCard = GetCard(viewWindow_Primary.currentViewIndex, false);
		if (primaryViewCard && primaryViewCard.isInstalled) primaryViewCard.ActivateRaycasts(clickable);
	}


	void ReplicateCardState(int viewIndex)
	{
		CardPair cardPair;
		if (cardPairDict.TryGetValue(viewIndex, out cardPair))
		{
			Card realCard = cardPair.realCard;
			Card viewCard = cardPair.viewCard;

			viewCard.FlipCard(realCard.isFaceUp, true);
			viewCard.isRezzed = realCard.isRezzed;
			viewCard.isInstalled = realCard.isInstalled;

		}
	}


	public void ShowLinkIcon(bool show)
	{
		linkIcon.SetActive(show);
	}




}
