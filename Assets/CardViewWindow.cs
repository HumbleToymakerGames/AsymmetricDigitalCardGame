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
	public static event CardViewEvent OnCardPinnedToView;
	public static event CardViewEvent OnCardViewed;

	private void Awake()
	{
        cardViewer = GetComponentInParent<CardViewer>();
		PinCard(null);
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
			cardViewer.ShowLinkIcon(false);
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
			}
			else
			{
				cardViewer.PinCard(card.viewIndex);
				Card viewCard = cardViewer.GetCard(card.viewIndex, false);
				OnCardPinnedToView?.Invoke(viewCard);
			}
		}
		else cardViewer.PinCard(-1);
	}


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ViewCard(int viewIndex, bool activateRaycasts = false)
	{
		cardViewer.HideAllCards(isPrimaryView);
		CardViewer.CardPair pair;
		if (cardViewer.cardPairDict.TryGetValue(viewIndex, out pair))
		{
			ParentCard(pair.viewCard);
			pair.viewCard.ActivateVisuals(true);
			pair.viewCard.ActivateRaycasts(activateRaycasts);
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
			cardViewer.GetCard(currentViewIndex, true)?.Pinned(false, isPrimaryView);
			ViewCard(card.viewIndex, isPrimaryView && card.isInstalled && cardViewer.cardsClickableOnView);
			cardViewer.GetCard(currentViewIndex, true).Pinned(true, isPrimaryView);
		}
		else
		{
			isPinning = false;
			pinGO.SetActive(false);
			cardViewer.HideAllCards(isPrimaryView);
			cardViewer.GetCard(currentViewIndex, true)?.Pinned(false, isPrimaryView);
			cardViewer.ShowLinkIcon(false);
			currentViewIndex = -1;
		}
	}
}
