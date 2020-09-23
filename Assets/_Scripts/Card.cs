using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, ISelectableNR
{
    [HideInInspector]
    public CardReferences cardRefs;
    CanvasGroup canvasGroup;
    [HideInInspector]
    public CardFunction cardFunction;
    [HideInInspector]
    public CardCost cardCost;
    [HideInInspector]
    public CardTrasher cardTrasher;
    [HideInInspector]
    public CardAdvancer cardAdvancer;


    [Header("Card Data")]
    public PlayerSide playerSide;
    public PlayerSide controllingPlayerSide;
    [HideInInspector]
    public PlayerNR myPlayer;
    public CardType cardType;
    public CardSubType cardSubType;
    public string cardTitle;

    [Header("Play Data")]
    //[HideInInspector]
    public bool isFaceUp = true;
    public bool isRezzed = true;
    float cardFlipTransitionTime = 1f;


    public int viewIndex;
    public bool isViewCard;
    public bool isInstalled;
    public SelectorNR selector;

    public delegate void CardAccessed(Card card, ServerColumn.ServerType serverType);
    public static event CardAccessed OnCardAccessed;

    public delegate void CardRezzed(Card card);
    public static event CardRezzed OnCardRezzed;

    protected virtual void Awake()
	{
        isFaceUp = true;
        cardRefs = GetComponent<CardReferences>();
        cardFunction = GetComponent<CardFunction>();
        cardCost = GetComponent<CardCost>();
        cardTrasher = GetComponent<CardTrasher>();
        cardAdvancer = GetComponent<CardAdvancer>();
        canvasGroup = GetComponent<CanvasGroup>();
        selector = GetComponentInChildren<SelectorNR>();
        myPlayer = controllingPlayerSide == PlayerSide.Runner ? PlayerNR.Runner : PlayerNR.Corporation;
        SetClickable(false);
        Pinned(false, true);
        Pinned(false, false);
    }

    protected virtual void OnEnable()
	{
		PlayCardManager.OnCardInstalled += OnCardInstalled;
		CardViewer.OnCardPinned += CardViewer_OnCardPinned;
		PaidAbilitiesManager.OnPaidWindowChanged += PaidAbilitiesManager_OnPaidWindowChanged;
		RunOperator.OnRunStarted += RunOperator_OnRunStarted;
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
	}

	protected virtual void OnDisable()
    {
        PlayCardManager.OnCardInstalled -= OnCardInstalled;
        CardViewer.OnCardPinned -= CardViewer_OnCardPinned;
		PaidAbilitiesManager.OnPaidWindowChanged -= PaidAbilitiesManager_OnPaidWindowChanged;
        RunOperator.OnRunStarted -= RunOperator_OnRunStarted;
        RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
    }

    protected virtual void OnCardInstalled(Card card, bool installed)
	{
        if (card == this)
        {
            SetInstall();
        }
    }
    private void CardViewer_OnCardPinned(Card card, bool primary)
    {
        if (card == this) UpdateClickable();
    }
    private void PaidAbilitiesManager_OnPaidWindowChanged(PlayerNR newPriority)
    {
        UpdateClickable();
    }
    private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
    {
        UpdateClickable();
    }
    private void RunOperator_OnRunStarted()
    {
        UpdateClickable();
    }

    protected virtual void Start()
    {
        UpdateCardTitle();
        UpdateCardTypes();
        UpdateCardCost();
        cardRefs.UpdateNeutralCounter(0);
    }

	private void Update()
	{
        //UpdateClickable();
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

    public void SetInstall(bool installed = true)
	{
        isInstalled = installed;
        UpdateClickable();
        if (!isViewCard) CardViewer.instance.GetCard(viewIndex, false).SetInstall(installed);
    }

    [ContextMenu("FlipCardOver")]
    public void FlipCardOver()
	{
        UpdateCardFlipDisplay(!isFaceUp);
        CardViewer.instance.GetCard(viewIndex, false)?.UpdateCardFlipDisplay(!isFaceUp);
	}

    public void FlipCard(bool faceUp, bool immediate = false)
	{
        UpdateCardFlipDisplay(faceUp, immediate);
        CardViewer.instance.GetCard(viewIndex, false)?.UpdateCardFlipDisplay(faceUp);
    }

    public void Rez()
	{
        isRezzed = true;
        FlipCard(true);
        if (!isViewCard)
        {
            CardViewer.instance.GetCard(viewIndex, false)?.Rez();
            OnCardRezzed?.Invoke(this);
        }
    }

    public void DeRez()
	{
        isRezzed = false;
	}


    void UpdateCardFlipDisplay(bool faceUp, bool immediate = false)
	{
        isFaceUp = faceUp;
        float time = immediate ? 0 : cardFlipTransitionTime;
        if (faceUp)
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


	public virtual bool CanHighlight(bool highlight = true)
	{
        if (highlight) return IsCardControlledByCurrentTurn();
        return true;
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
        else if (isInstalled)
		{
            if (!CardChooser.instance.isFocused)
                CardViewer.instance.viewWindow_Primary.CardChooser_OnCardPinned(this);
        }
        //FlipCard();
        //print(IsCardInHand());
	}

    public virtual void ActivateCardOptions()
	{
        ActionOptions.instance.HideAllOptions();
        if (!IsCardInHand())
		{
            if (isInstalled)
			{
                if (CanBeAdvanced())
                {
                    ActionOptions.instance.Display_AdvanceCard(false);
                    if (IsScoreable())
					{
                        ActionOptions.instance.Display_ScoreAgenda(false);
					}
                }
                else
				{
                    if (!isRezzed)
					{
                        if (!IsCardType(CardType.Ice))
                        {
                            int costOfCard = cardCost.costOfCard;
                            ActionOptions.instance.Display_RezCard(RezChoice, costOfCard);

                            void RezChoice(bool yes)
                            {
                                if (yes)
                                {
                                    if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, costOfCard))
                                    {
                                        Rez();
                                    }
                                }
                            }
                        }
					}
				}
            }
		}

    }

    public void Pinned(bool pinned = true, bool primary = true)
    {
		foreach (var pin in primary ? cardRefs.cardPin_Primary : cardRefs.cardPin_Secondary)
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

    public void UpdateClickable()
	{
        SetClickable(CanBeClicked());
	}

    public void SetClickable(bool activate = true)
	{
        canvasGroup.blocksRaycasts = activate;
	}

    public virtual bool CanBeClicked()
    {
        bool hasPriority = false;
        if (PaidAbilitiesManager.instance.IsResolving())
        {
            hasPriority = PaidAbilitiesManager.instance.IsMyPriority(myPlayer);
        }
        return isInstalled && hasPriority && isViewCard && viewIndex == CardViewer.currentPinnedCard?.viewIndex;  
    }

    public bool IsTrashable() { return cardTrasher != null; }

    public bool IsAdvanceable() { return cardAdvancer != null; }

    public bool IsScoreable() { return this is Card_Agenda; }

    public bool CanBeTrashed()
	{
        if (IsTrashable()) return cardTrasher.CanBeTrashed();
        return false;
    }

    public bool CanBeAdvanced()
	{
        if (IsAdvanceable()) return cardAdvancer.CanBeAdvanced();
        return false;
    }

    public bool CanBeScored()
	{
        if (IsScoreable()) return cardAdvancer.CanBeScored();
        return false;
	}

    //private void OnValidate()
    //{
    //       if (titleText) titleText.text = cardTitle;
    //}


    public void ExposeCard()
	{
        CardRevealer.instance.RevealCard(this, false);
	}

	public void Accessed(ServerColumn.ServerType serverAccessed)
	{
        OnCardAccessed?.Invoke(this, serverAccessed);
    }
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
