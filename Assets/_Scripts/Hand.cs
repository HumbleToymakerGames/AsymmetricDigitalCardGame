using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand : PlayArea_Spot, IAccessable
{
    public Transform cardsParentT;
    int maximumHandSize = 5;
    public float cardScaleInHand;

	public List<Card> cardsInHand = new List<Card>();

	[Header("Show Toggler")]
    bool isShowing = false;
    RectTransform rt;
    public int showYValue, hideYValue;
    public float transitionTime;

	protected override void Awake()
	{
		base.Awake();
        rt = GetComponent<RectTransform>();
    }

	private void OnEnable()
	{
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
	}
	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		ShowHand(myPlayer.IsRunner() == isRunner);
	}

	private void Start()
	{
        ToggleShowing();
	}

	public void AddCardsToHand(params Card[] cards)
	{
		for (int i = 0; i < cards.Length; i++)
		{
			cardsInHand.Add(cards[i]);
            cards[i].MoveCardTo(cardsParentT);
            ScaleCard(cards[i]);
			Vector3 localPos = cards[i].transform.localPosition;
			localPos.z = -1 * cardsInHand.Count;
			cards[i].transform.localPosition = localPos;
		}
	}

    void ScaleCard(Card card, bool scale = true)
	{
        float scaleFactor = scale ? cardScaleInHand : 1;
        card.transform.localScale = Vector3.one * scaleFactor;
    }

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.Space)) ToggleShowing();
	}


	void ToggleShowing()
	{
        isShowing = !isShowing;
        rt.DOAnchorPosY(isShowing ? showYValue : hideYValue, transitionTime);

    }

	public int NumberOfCardsInHand()
	{
		return cardsInHand.Count;
	}

	public bool HandSizeFull()
	{
		return NumberOfCardsInHand() >= maximumHandSize;
	}

	public bool IsCardInHand(Card card)
	{
		return cardsInHand.Contains(card);
	}

	public override void RemoveCard(Card card)
	{
		cardsInHand.Remove(card);
	}

	Card RandomCard()
	{
		if (cardsInHand.Count > 0)
			return cardsInHand[Random.Range(0, cardsInHand.Count)];
		return null;
	}

	public void Access()
	{
		Card randomCard = RandomCard();
		if (randomCard)
		{
			randomCard.Accessed();
			CardRevealer.instance.RevealCard(randomCard, true);
		}

		print("Hand accesssed");
	}



	void ShowHand(bool show)
	{
		foreach (var card in cardsInHand)
		{
			card.FlipCard(show);
		}
	}



}
