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
		}
		else
		{
			cardViewer.ViewCard(card.viewIndex, isPrimaryView);
		}
	}

	private void CardChooser_OnCardPinned(Card card)
	{
		if (card && isPinning && card.viewIndex == currentViewIndex)
		{
			PinCard(null);
		}
		else cardViewer.PinCard(card.viewIndex);
	}


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ViewCard(int viewIndex)
	{
		cardViewer.HideAllCards(isPrimaryView);
		CardViewer.CardPair pair;
		if (cardViewer.cardPairDict.TryGetValue(viewIndex, out pair))
		{
			pair.viewCard.gameObject.SetActive(true);
			ParentCard(pair.viewCard);
			currentViewIndex = viewIndex;
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
			ViewCard(card.viewIndex);
			cardViewer.GetCard(currentViewIndex, true).Pinned(true, isPrimaryView);
		}
		else
		{
			isPinning = false;
			pinGO.SetActive(false);
			cardViewer.HideAllCards(isPrimaryView);
			cardViewer.GetCard(currentViewIndex, true)?.Pinned(false, isPrimaryView);
			currentViewIndex = -1;
		}
	}
}
