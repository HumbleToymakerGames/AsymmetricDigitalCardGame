using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.CodeDom;

public class CardRevealer : MonoBehaviour
{
    public static CardRevealer instance;
    public GameObject panelGO;
    public RectTransform cardT, cardsT;
    public Transform emtpyImageT;
    public GameObject cardButtonsGO, cardsButtonsGO;
    public Button cardButton_Return, cardButton_Trash, cardButton_Steal;
    public TextMeshProUGUI trashButtonText;
    const string trashTextFormat = "Trash ({0}cc)";
    public Card[] realCards, currentCards;
    public CardMover cardMoverPrefab;
    CardMover[] cardMovers = new CardMover[0];
    bool isMovingCards, isRevealing, isAccessing;
    PlayArea_Spot currentPlayAreaSpot;

    public Vector3 endRevealPos;
    public float transitionTime_Move;
    public float endRevealScale;
    public float revealPauseTime;

    Card targetRealCard, targetRevealCard;
    public Vector3 targetRealCard_OGPosition, targetRealCard_OGScale;

    DiscardPile corpDiscard;

    private void Awake()
	{
        instance = this;
        Activate(false);
        cardsButtonsGO.SetActive(false);
        corpDiscard = PlayArea.instance.DiscardNR(PlayerNR.Corporation);
    }

    private void OnEnable()
	{
		CardChooser.OnHoveredOverCard += CardChooser_OnHoveredOverCard;
	}
    private void OnDisable()
    {
        CardChooser.OnHoveredOverCard -= CardChooser_OnHoveredOverCard;
    }

    private void CardChooser_OnHoveredOverCard(Card card)
	{
        if (isRevealing && moveEmptyRoutine == null)
            moveEmptyRoutine = StartCoroutine(MoveEmptyDelay(card));
	}

    Coroutine moveEmptyRoutine;
    IEnumerator MoveEmptyDelay(Card card)
	{
        for (int i = 0; i < cardMovers.Length; i++)
        {
            if (cardMovers[i].myCard == card)
            {
                emtpyImageT.SetSiblingIndex(cardMovers.Length);
                int index = cardMovers[i].transform.GetSiblingIndex();
                emtpyImageT.SetSiblingIndex(index+1);
                break;
            }
        }
        yield return new WaitForSeconds(0.1f);
        moveEmptyRoutine = null;
    }

	public void Activate(bool activate = true)
	{
        panelGO.SetActive(activate);
        isRevealing = activate;
        if (!activate)
		{
            cardButtonsGO.SetActive(false);
            cardsButtonsGO.SetActive(false);
        }
    }

    public void RevealCard(Card card, bool accessed)
	{
        targetRealCard = card;
        isAccessing = accessed;

        StartCoroutine(PanelActivationWaiter(true));

        targetRealCard_OGPosition = card.transform.position;

        cardT.position = targetRealCard_OGPosition;
        cardT.localScale = Vector3.one;

        targetRevealCard = Instantiate(card, cardT);
        targetRealCard_OGScale = targetRevealCard.transform.localScale;
        targetRevealCard.MoveCardTo(cardT);
        targetRevealCard.transform.localScale = targetRealCard_OGScale;
        targetRevealCard.ActivateRaycasts(false);
        targetRevealCard.FlipCard(false, true);
        //revealedCard.transform.localScale = card.transform.lossyScale;


        StopRevealRoutine();
        revealRoutine = StartCoroutine(RevealRoutine(targetRevealCard));

        
    }

    IEnumerator PanelActivationWaiter(bool isSingleCard)
	{
        while (ConditionalAbilitiesManager.IsResolvingConditionals()) yield return null;
        Activate();
        cardButtonsGO.SetActive(isSingleCard);
        cardsButtonsGO.SetActive(!isSingleCard);
        UpdateCardButtons();
    }

    Coroutine revealRoutine;
    Sequence revealSequence;
    void StopRevealRoutine()
	{
        if (revealRoutine != null)
        {
            StopCoroutine(revealRoutine);
            revealSequence.Kill();
        }

    }

    IEnumerator RevealRoutine(Card revealedCard)
	{
        revealSequence = DOTween.Sequence();
        Tweener poser = cardT.DOAnchorPos(endRevealPos, transitionTime_Move);
        revealSequence.Append(poser);
        Tweener scaler = targetRevealCard.transform.DOScale(endRevealScale, transitionTime_Move);
        revealSequence.Join(scaler);
        revealSequence.SetEase(Ease.InOutQuad);

        revealedCard.FlipCard(true);

        yield return revealSequence.WaitForCompletion();
        yield break;
    }

    IEnumerator UnrevealRoutine(Card unrevealedCard, Vector3 targetPos, Vector3 targetScale)
	{
        revealSequence = DOTween.Sequence();
        Tweener poser = cardT.DOMove(targetPos, transitionTime_Move);
        revealSequence.Append(poser);
        Tweener scaler = targetRevealCard.transform.DOScale(targetScale, transitionTime_Move);
        revealSequence.Join(scaler);
        revealSequence.SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(0.5f);
        unrevealedCard.FlipCard(targetRealCard.isFaceUp);
        yield return revealSequence.WaitForCompletion();

        Destroy(unrevealedCard.gameObject);
        Activate(false);
    }


    public void RevealCards(Card[] cards, bool moveable, PlayArea_Spot playArea_Spot)
	{
        Activate();
        realCards = cards;
        isMovingCards = moveable;
        currentPlayAreaSpot = playArea_Spot;

        currentCards = new Card[cards.Length];
        cardMovers = new CardMover[cards.Length];
        for (int i = 0; i < cards.Length; i++)
		{
            CardMover mover = Instantiate(cardMoverPrefab, cardsT);

            Card revealedCard = Instantiate(cards[i], cardsT);
            revealedCard.ActivateRaycasts(false);
            revealedCard.FlipCard(true, true);

            mover.SetCard(revealedCard);
            mover.SetMoveable(moveable);

            currentCards[i] = revealedCard;
            cardMovers[i] = mover;
        }
        emtpyImageT.SetSiblingIndex(1);

    }

    void DestroyAllCards()
	{
        for (int i = 0; i < cardMovers.Length; i++)
		{
            Destroy(cardMovers[i].gameObject);
		}
        currentCards = new Card[0];
        cardMovers = new CardMover[0];
        currentPlayAreaSpot = null;
        targetRealCard = null;
    }

    void TryMoveRealCards()
	{
        if (isMovingCards)
		{
            int baseDeckIndex = 1000;
            List<Card> deckFrom = currentPlayAreaSpot.MyDeck();
			foreach (var card in realCards)
			{
                int cardIndex = deckFrom.IndexOf(card);
                if (cardIndex < baseDeckIndex)
				{
                    baseDeckIndex = cardIndex;
				}
			}

            emtpyImageT.SetSiblingIndex(cardsT.childCount);
            (int, int)[] view_sibs = new (int, int)[realCards.Length];
            int realCardCount = realCards.Length;
			for (int i = 0; i < realCards.Length; i++)
			{
                Card card = realCards[i];
				foreach (var moveCard in cardMovers)
				{
                    if (card.viewIndex == moveCard.myCard.viewIndex)
					{
                        int moveIndex = moveCard.transform.GetSiblingIndex();
                        view_sibs[i] = (card.viewIndex, baseDeckIndex + moveIndex);
					}
				}
			}

            currentPlayAreaSpot.UpdateCardSiblings(view_sibs);
            isMovingCards = false;
		}
	}


    void UpdateCardButtons()
	{
        SetAllCardButtonsInteractable(false);
        cardButton_Return.interactable = true;

        if (isAccessing)
        {
            if (targetRealCard is Card_Asset)
            {
                cardButton_Return.interactable = true;

                bool cardIsTrashable = PlayCardManager.instance.CanTrashCard(GameManager.CurrentTurnPlayer, targetRealCard);
                if (targetRealCard.CanBeTrashed()) trashButtonText.text = string.Format(trashTextFormat, targetRealCard.cardTrasher.CostOfTrash());
                else trashButtonText.text = "Trash";
                cardButton_Trash.interactable = cardIsTrashable;

                cardButton_Steal.interactable = false;
            }
            else if (targetRealCard is Card_Agenda)
            {
                SetAllCardButtonsInteractable(false);
                cardButton_Steal.interactable = true;
            }
        }
        else
		{
            cardButton_Return.interactable = true;
        }
    }


    public void Button_ReturnCard()
	{
        StopRevealRoutine();
        revealRoutine = StartCoroutine(UnrevealRoutine(targetRevealCard, targetRealCard_OGPosition, targetRealCard_OGScale));
        SetAllCardButtonsInteractable(false);
    }

    public void Button_TrashCard()
	{
        if (PlayCardManager.instance.TryTrashCard(GameManager.CurrentTurnPlayer, targetRealCard))
		{
            StopRevealRoutine();
            revealRoutine = StartCoroutine(UnrevealRoutine(targetRevealCard, corpDiscard.cardsParentT.position, corpDiscard.cardsParentT.localScale));
        }
        SetAllCardButtonsInteractable(false);

    }

    public void Button_StealCard()
	{
        SetAllCardButtonsInteractable(false);
        if (PlayCardManager.instance.TryScoreAgenda(PlayerNR.Runner, targetRealCard as Card_Agenda))
		{
            Transform scoringAreaT = PlayArea.instance.ScoringAreaNR(PlayerNR.Runner).cardsParentT;
            StopRevealRoutine();
            revealRoutine = StartCoroutine(UnrevealRoutine(targetRevealCard, scoringAreaT.position, scoringAreaT.localScale));
        }
    }

    void SetAllCardButtonsInteractable(bool interactable = true)
	{
        cardButton_Return.interactable = interactable;
        cardButton_Trash.interactable = interactable;
        cardButton_Steal.interactable = interactable;
    }


    public void Button_ReturnCards()
	{
        TryMoveRealCards();
        DestroyAllCards();
        Activate(false);
        cardsButtonsGO.SetActive(false);
    }








}
