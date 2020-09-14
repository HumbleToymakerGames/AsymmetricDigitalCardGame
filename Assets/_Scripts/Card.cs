using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, ISelectableNR
{
    CardReferences cardRefs;
    CanvasGroup canvasGroup;
    [HideInInspector]
    public CardFunction cardFunction;
    [HideInInspector]
    public CardCost cardCost;

    [Header("Card Data")]
    public PlayerSide playerSide;
    public PlayerSide controllingPlayerSide;
   // [HideInInspector]
    public PlayerNR myPlayer;
    public CardType cardType;
    public CardSubType cardSubType;
    public string cardTitle;

    [Header("Play Data")]
    [HideInInspector]
    public bool isFaceUp = true;
    float cardFlipTransitionTime = 1f;


    public int viewIndex;
    public bool isViewCard;



    protected virtual void Awake()
	{
        isFaceUp = true;
        cardRefs = GetComponent<CardReferences>();
        cardFunction = GetComponent<CardFunction>();
        cardCost = GetComponent<CardCost>();
        canvasGroup = GetComponent<CanvasGroup>();
        myPlayer = controllingPlayerSide == PlayerSide.Runner ? PlayerNR.Runner : PlayerNR.Corporation;
        ActivateRaycasts(false);
        Pinned(false, true);
        Pinned(false, false);
    }

    protected virtual void OnEnable()
	{
		PlayCardManager.OnCardInstalled += OnCardInstalled;
		myPlayer.OnCreditsChanged += MyPlayer_OnCreditsChanged;
	}

	protected virtual void OnDisable()
    {
        PlayCardManager.OnCardInstalled -= OnCardInstalled;
		myPlayer.OnCreditsChanged -= MyPlayer_OnCreditsChanged;
    }
    protected virtual void OnCardInstalled(Card card, bool installed)
	{

	}

    private void MyPlayer_OnCreditsChanged()
    {
        cardFunction?.UpdatePaidAbilitesActive_Credits(myPlayer.Credits);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateCardTitle();
        UpdateCardTypes();
        UpdateCardCost();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsCardType(CardType _cardType)
	{
        return cardType == _cardType;
	}
    public bool IsCardSubType(CardSubType _cardSubType)
    {
        return cardSubType == _cardSubType;
    }

    public bool IsOwnedByPlayerSide(PlayerSide _playerSide)
	{
        return playerSide == _playerSide;
	}

    void UpdateCardCost()
	{
        cardRefs.UpdateCardCost(cardCost.costOfCard);
	}

    void UpdateCardTitle()
    {
        cardRefs.UpdateCardTitle(cardTitle);
    }

    void UpdateCardTypes()
	{
        cardRefs.UpdateCardTypes(cardType.TypeToString(), cardSubType.TypeToString());
    }



    // Installation
    protected virtual void CardInstalled()
	{

	}

    protected virtual void CardUnInstalled()
    {

    }

    [ContextMenu("FlipCard")]
    public void FlipCard()
	{
        isFaceUp = !isFaceUp;
        UpdateCardFlipDisplay();
	}

    public void FlipCardUp(bool immediate = false)
	{
        isFaceUp = true;
        UpdateCardFlipDisplay(immediate);
	}
    public void FlipCardDown(bool immediate = false)
    {
        isFaceUp = false;
        UpdateCardFlipDisplay(immediate);
    }

    void UpdateCardFlipDisplay(bool immediate = false)
	{
        float time = immediate ? 0 : cardFlipTransitionTime;
        if (isFaceUp)
		{
            transform.DOLocalRotate(Vector3.zero, time);
		}
        else
		{
            transform.DOLocalRotate(Vector3.up * 180, time);
        }
    }

    public bool IsCardInHand()
	{
        return PlayArea.instance.HandNR(myPlayer).IsCardInHand(this);
	}

    public bool IsCardCurrentTurns()
	{
        return playerSide == GameManager.CurrentTurnPlayer.playerSide;
	}

    public bool IsCardControlledByCurrentTurn()
	{
        return controllingPlayerSide == GameManager.CurrentTurnPlayer.playerSide;
    }


	public bool CanHighlight()
	{
        return IsCardControlledByCurrentTurn();
        //return true;
	}

	public virtual bool CanSelect()
	{
        return CanHighlight();
	}

	public void Highlighted()
	{
	}

	public void Selected()
	{
        if (IsCardInHand())
		{
            ActivateCardFromHand();
		}
        //FlipCard();
        print(IsCardInHand());
	}

    public void Pinned(bool pinned = true, bool primary = true)
    {
        Image pin = primary ? cardRefs.cardPin_Primary : cardRefs.cardPin_Secondary;
        pin.enabled = pinned;
    }

    public void ActivateCardFromHand()
	{
        if (this is IInstallable)
		{
            IInstallable installable = this as IInstallable;
            PlayCardManager.instance.TryInstallCard(installable);
		}
		else if (this is IActivateable)
		{
            IActivateable activateable = this as IActivateable;
            PlayCardManager.instance.TryActivateEvent(activateable);
        }
    }

    public void ActivateVisuals(bool activate = true)
    {
        canvasGroup.alpha = activate ? 1 : 0;
    }

    public void ActivateRaycasts(bool activate = true)
	{
        canvasGroup.blocksRaycasts = activate;
	}



    //private void OnValidate()
    //{
    //       if (titleText) titleText.text = cardTitle;
    //}





}



static class CardExtensions
{
    public static void MoveCardTo(this Card card, Transform parent)
	{
        // Remove from list
        PlayArea_Spot currentSpot = card.GetComponentInParent<PlayArea_Spot>();
        PlayArea_Spot newSpot = parent.GetComponentInParent<PlayArea_Spot>();
        if (currentSpot && newSpot != currentSpot)
        {
            currentSpot.RemoveCard(card);
        }

        card.ParentCardTo(parent);
    }

    public static void ParentCardTo(this Card card, Transform parent)
	{
        

        // Parent card
        RectTransform rt = card.GetComponent<RectTransform>();
        card.transform.localScale = Vector3.one;
        card.transform.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = Vector2.one * 0.5f;
        rt.anchoredPosition = Vector3.zero;
        
    }
}
