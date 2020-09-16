using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class CardRevealer : MonoBehaviour, ISelectableNR
{
    public static CardRevealer instance;
    public GameObject panelGO;
    public RectTransform cardT, cardsT;
    public Transform emtpyImageT;
    public Card[] currentCards;

    public Vector3 endRevealPos;
    public float transitionTime_Move;
    public float endRevealScale;
    public float revealPauseTime;

	private void Awake()
	{
        instance = this;
        Activate(false);
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
        StartCoroutine(MoveEmptyDelay(card));
	}

    IEnumerator MoveEmptyDelay(Card card)
	{
        for (int i = 0; i < currentCards.Length; i++)
        {
            if (currentCards[i] == card)
            {
                yield return new WaitForSeconds(0.1f);
                emtpyImageT.SetSiblingIndex(i + 1);
            }
        }
    }

	public void Activate(bool activate = true)
	{
        panelGO.SetActive(activate);
	}

    public void RevealCard(Card card, bool pingPong)
	{
        Activate();
		cardT.position = card.transform.position;
        cardT.localScale = Vector3.one;
        Card revealedCard = Instantiate(card, cardT);
        revealedCard.MoveCardTo(cardT);
        revealedCard.ActivateRaycasts(false);
        revealedCard.FlipCard(false, true);
        //revealedCard.transform.localScale = card.transform.lossyScale;

        StartCoroutine(RevealRoutine(revealedCard, pingPong));
        
	}

    IEnumerator RevealRoutine(Card revealedCard, bool pingPong)
	{
        var sequence = DOTween.Sequence();
        Tweener poser = cardT.DOAnchorPos(endRevealPos, transitionTime_Move);
        sequence.Append(poser);
        Tweener scaler = cardT.DOScale(endRevealScale, transitionTime_Move);
        sequence.Join(scaler);
        sequence.SetLoops(2, LoopType.Yoyo);
        sequence.SetEase(Ease.InOutQuad);

        revealedCard.FlipCard(true);

        yield return sequence.WaitForElapsedLoops(1);
        sequence.Pause();
        yield return new WaitForSeconds(revealPauseTime);


        if (pingPong) sequence.Play();
        else
        {
            sequence.Kill();
            sequence = DOTween.Sequence();
            sequence.Append(cardT.DOScale(0, transitionTime_Move));
        }

        yield return new WaitForSeconds(0.5f);
        revealedCard.FlipCard(false);
        yield return sequence.WaitForCompletion();

        Destroy(revealedCard.gameObject);
        Activate(false);
    }


    public void RevealCards(Card[] cards)
	{
        Activate();

        currentCards = new Card[cards.Length];
        for (int i = 0; i < cards.Length; i++)
		{
            Card revealedCard = Instantiate(cards[i], cardsT);
            revealedCard.MoveCardTo(cardsT);
            revealedCard.transform.localScale = Vector3.one * 2.5f;
            revealedCard.ActivateRaycasts(false);
            revealedCard.FlipCard(true, true);
            currentCards[i] = revealedCard;
        }
    }

    void DestroyAllCards()
	{
        for (int i = 0; i < currentCards.Length; i++)
		{
            Destroy(currentCards[i].gameObject);
		}
    }






    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public bool CanHighlight(bool highlight = true)
	{
        return true;
	}

	public bool CanSelect()
	{
        return true;
	}

	public void Highlighted()
	{

	}

	public void Selected()
	{
        DestroyAllCards();
        Activate(false);
    }
}
