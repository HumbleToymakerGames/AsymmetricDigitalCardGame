﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
	public static CardViewer instance;
	public CardViewWindow viewWindow_Primary, viewWindow_Secondary;
	public GameObject linkIcon;
	public static Card currentPinnedCard;

	public delegate void eCardPinned(Card card, bool primary);
	public static event eCardPinned OnCardPinned;

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
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
	}
	private void OnDisable()
	{
		RunOperator.OnRunStarted -= RunOperator_OnRunStarted;
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
	}

	private void RunOperator_OnRunStarted()
	{
		UpdateCardsClickable();
	}
	private void GameManager_OnTurnChanged(bool isRunner)
	{
		UpdateCardsClickable();
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
		Card realCard = GetCard(viewIndex, true);
		bool clickable = realCard ? realCard.CanBeClicked() : false;

		ReplicateCardState(viewIndex);
		if (primary) viewWindow_Primary.ViewCard(viewIndex, clickable);
		else viewWindow_Secondary.ViewCard(viewIndex, clickable);
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
				kvp.Value.viewCard.SetClickable(false);
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
			viewCard.name = viewCard.name.Replace("(Clone)", "");
			viewCard.name += "_VIEW";

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

			identityCard.isViewCard = true;
			Card viewCard = Instantiate(identityCard);
			identityCard.isViewCard = false;

			cardPairDict[viewIndex] = new CardPair() { realCard = identityCard, viewCard = viewCard };
			ParentCard_Start(viewCard);
			viewIndex++;
		}

		yield return new WaitForSeconds(0.25f);

		HideAllCards(true);
		HideAllCards(false);

	}

	public int targetIndex;

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

	public void UpdateCardsClickable()
	{
		Card primaryViewCard = GetCard(viewWindow_Primary.currentViewIndex, false);
		if (primaryViewCard) primaryViewCard.SetClickable(primaryViewCard.CanBeClicked());
	}


	void ReplicateCardState(int viewIndex)
	{
		CardPair cardPair;
		if (cardPairDict.TryGetValue(viewIndex, out cardPair))
		{
			Card realCard = cardPair.realCard;
			Card viewCard = cardPair.viewCard;

			viewCard.FlipCard(realCard.isFaceUp, true);
			if (realCard.IsOwnedByPlayerSide(PlayerSide.Corporation) && realCard.isInstalled)
				viewCard.FlipCard(true, true);
			viewCard.isRezzed = realCard.isRezzed;
			viewCard.isInstalled = realCard.isInstalled;

			viewCard.NeutralCounters = realCard.NeutralCounters;

		}
	}


	public void ShowLinkIcon(bool show)
	{
		linkIcon.SetActive(show);
	}


	public void CardPinned(Card card, bool primary)
	{
		OnCardPinned?.Invoke(card, primary);
	}



}
