using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaidAbility : MonoBehaviour
{
    [Header("Ability")]
    public int payAmount;
    public Currency currency;
    public CardSubType breakerType;
    bool isBreakerRoutine;
    Button button;

	CardFunction cardFunction;
    [HideInInspector]
    public int myAbilityIndex;

    //public CardFunction viewCardFunction;

	//public string AbilityText;
	private void Awake()
	{
        cardFunction = GetComponentInParent<CardFunction>();
        button = GetComponent<Button>();
        isBreakerRoutine = breakerType != CardSubType.NULL;
    }

	private void OnEnable()
	{
        if (cardFunction.card.isViewCard)
        {
			RunOperator.OnCardBeingApproached += RunOperator_OnCardBeingApproached;
			RunOperator.OnRunEnded += RunOperator_OnRunEnded;
        }
	}

	private void OnDisable()
    {
        if (cardFunction.card.isViewCard)
        {
            RunOperator.OnCardBeingApproached -= RunOperator_OnCardBeingApproached;
            RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
        }
    }

    private void RunOperator_OnCardBeingApproached(Card card, int encounterIndex)
	{
        if (card.IsCardType(CardType.Ice) && cardFunction.card.IsCardType(CardType.Program))
        {
            TrySetAbleAbilitiesActive();
        }
	}

    private void RunOperator_OnRunEnded(bool success)
    {
        if (cardFunction.card.IsCardType(CardType.Program))
        {
            TrySetAbleAbilitiesActive();
        }
    }

    void Start()
    {
        
    }

    public void ActivateOnCredits(int playerCredits)
	{
        TrySetAbleAbilitiesActive();
    }


    public void Button_Clicked()
	{
        PlayCardManager.instance.TryActivatePaidAbility(this);
    }

    public void ActivateAbility()
	{
        cardFunction.ActivatePaidAbility(myAbilityIndex);
        //if (viewCardFunction) viewCardFunction.ActivatePaidAbility(myAbilityIndex);
    }

    public void TrySetAbleAbilitiesActive()
	{
        bool interactable = false;
        if (CanBeActive_Credits(cardFunction.card.myPlayer.Credits))
		{
            if (RunOperator.instance.isEncounteringIce)
            {
                Card_Program programCard = cardFunction.card as Card_Program;
                bool isStrongEnough = RunOperator.instance.IsCardStrongEnough(programCard);
                if (CanBeActive_Strength(isStrongEnough))
                {
                    if (CanBeActive_Type())
                    {
                        interactable = true;
                    }
                    else interactable = false;
                }
            }
            else interactable = true;
		}

        button.interactable = interactable;
	}

    bool CanBeActive_Credits(int playerCredits)
	{
        return currency == Currency.Credits && playerCredits >= payAmount;

    }

    bool CanBeActive_Strength(bool isStrongEnough)
	{
        if (cardFunction.card.isViewCard && isBreakerRoutine)
        {
            if (!isStrongEnough)
            {
                return false;
            }
        }
        return true;
    }

    bool CanBeActive_Type()
	{
        if (cardFunction.card.isViewCard && isBreakerRoutine)
		{
            if (!RunOperator.instance.IceIsType(breakerType))
			{
                return false;
			}
		}
        return true;
    }


    //   public void SetCardFunctions(CardFunction _cardFunction, CardFunction _viewCardFunction)
    //{
    //       cardFunction = _cardFunction;
    //       viewCardFunction = _viewCardFunction;
    //}



}
