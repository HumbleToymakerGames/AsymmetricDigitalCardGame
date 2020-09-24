using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewWindow : MonoBehaviour
{
    CardViewer cardViewer;
	public bool isPrimaryView;
    public Transform cardsT;
    public bool isPinning;
    public GameObject pinGO;
    public int currentViewIndex = -1;
    public float scale;
	public Vector2 anchor;

	public delegate void CardViewEvent(Card card);
	public static event CardViewEvent OnCardViewed;

	private void Awake()
	{
        cardViewer = GetComponentInParent<CardViewer>();
	}

	private void OnEnable()
	{
		if (isPrimaryView)
		{
			CardChooser.OnHoveredOverCard += CardChooser_OnHoveredOverCard;
			CardChooser.OnCardPinned += CardChooser_OnCardPinned;
		}
	}

	private void OnDisable()
	{
		if (isPrimaryView)
		{
			CardChooser.OnHoveredOverCard -= CardChooser_OnHoveredOverCard;
			CardChooser.OnCardPinned -= CardChooser_OnCardPinned;
		}
	}

	private void CardChooser_OnHoveredOverCard(Card card)
	{
		if (isPinning) return;

		if (card == null)
		{
			cardViewer.HideAllCards(isPrimaryView);
			currentViewIndex = -1;
		}
		else
		{
			cardViewer.ViewCard(card.viewIndex, isPrimaryView);
		}
	}

	public void CardChooser_OnCardPinned(Card card)
	{
		if (card)
		{
			if (isPinning && card.viewIndex == currentViewIndex)
			{
				PinCard(null);
				cardViewer.CardPinned(null, isPrimaryView);
			}
			else
			{
				cardViewer.PinCard(card.viewIndex);
				Card viewCard = cardViewer.GetCard(card.viewIndex, false);
				cardViewer.CardPinned(viewCard, isPrimaryView);
			}
		}
		else
		{
			cardViewer.PinCard(-1);
			cardViewer.CardPinned(null, isPrimaryView);
		}
	}

	void Start()
    {
		PinCard(null);
	}

	public void ViewCard(int viewIndex, bool activateRaycasts = false)
	{
		cardViewer.HideAllCards(isPrimaryView);
		CardViewer.CardPair pair;
		if (cardViewer.cardPairDict.TryGetValue(viewIndex, out pair))
		{
			ParentCard(pair.viewCard);
			pair.viewCard.ActivateVisuals(true);
			pair.viewCard.SetClickable(activateRaycasts && pair.realCard.myPlayer == GameManager.CurrentTurnPlayer);
			currentViewIndex = viewIndex;
			OnCardViewed?.Invoke(pair.viewCard);
		}
		else
		{
			currentViewIndex = -1;
		}
	}

	public void ParentCard(Card card)
	{
		card.ParentCardTo(cardsT);
		RectTransform rt = card.GetComponent<RectTransform>();
		rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
		card.transform.localScale = Vector3.one * scale;
	}

	public void PinCard(Card card)
	{
		if (card)
		{
			isPinning = true;
			pinGO.SetActive(true);
			Card realCard_Old = cardViewer.GetCard(currentViewIndex, true);
			realCard_Old?.Pinned(false, isPrimaryView);
			ViewCard(card.viewIndex, isPrimaryView && card.CanBeClicked());
			Card realCard = cardViewer.GetCard(currentViewIndex, true);
			if (realCard)
			{
				realCard.Pinned(true, isPrimaryView);
				if (isPrimaryView) CardViewer.currentPinnedCard = realCard;

				if (realCard.isInstalled && !RunOperator.instance.isRunning)
					realCard.ActivateCardOptions();
			}
		}
		else
		{
			isPinning = false;
			pinGO.SetActive(false);
			cardViewer.HideAllCards(isPrimaryView);
			cardViewer.GetCard(currentViewIndex, true)?.Pinned(false, isPrimaryView);
			currentViewIndex = -1;
			if (!RunOperator.instance.isRunning) ActionOptions.instance.HideAllOptions();
			if (isPrimaryView) CardViewer.currentPinnedCard = null;

		}
	}
}
