using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class CardRevealer : MonoBehaviour
{
    public static CardRevealer instance;
    public GameObject panelGO;
    public RectTransform cardT;
    public Vector3 endRevealPos;
    public float transitionTime_Move;
    public float endRevealScale;
    public float revealPauseTime;

	private void Awake()
	{
        instance = this;
        Activate(false);
    }


    public void Activate(bool activate = true)
	{
        panelGO.SetActive(activate);
	}

    public void RevealCard(Card card, bool pingPong)
	{
        Activate();
		cardT.position = card.transform.position;
        Card revealedCard = Instantiate(card, cardT);
        revealedCard.MoveCardTo(cardT);
        revealedCard.ActivateRaycasts(false);
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

        revealedCard.FlipCardUp();

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
        revealedCard.FlipCardDown();
        yield return sequence.WaitForCompletion();

        Destroy(revealedCard.gameObject);
        Activate(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
