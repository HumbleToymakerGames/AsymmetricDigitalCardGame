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
    CanvasGroup canvasGroup;

    public delegate IEnumerator Ability();
    public Ability IAbilityExecution;

    public delegate bool ClickableConditions();
    public ClickableConditions ClickableConditionsMet;

    public delegate void PaidAbilityActivated();
    public static event PaidAbilityActivated OnPaidAbilityActivated;

    CardFunction cardFunction;

	private void Awake()
	{
        cardFunction = GetComponentInParent<CardFunction>();
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        isBreakerRoutine = breakerType != CardSubType.NULL;
    }

    void Start()
    {
        
    }

	private void Update()
	{
        SetClickable(AreConditionsMet());
	}

	[ContextMenu("AbleAbilities")]
    public void UpdateAbilityInteractable()
	{
        bool interactable = false;
        if (CanBeActive_Currency(cardFunction.card.myPlayer.Credits))
		{
            if (cardFunction.card.IsCardType(CardType.Program))
            {
                if (RunOperator.instance.isEncounteringIce && RunOperator.instance.currentIceEncountered.isRezzed)
                {
                    Card_Program programCard = cardFunction.card as Card_Program;
                    bool isStrongEnough = RunOperator.instance.IsCardStrongEnough(programCard);
                    if (CanBeActive_Strength(isStrongEnough))
                    {
                        if (CanBeActive_Type())
                        {
                            interactable = true;
                        }
                    }
                }
                else interactable = true;
            }
            else
			{
                interactable = true;
            }
        }

        button.interactable = interactable;
	}

    bool CanBeActive_Currency(int playerCredits)
	{
        if (currency == Currency.Credits)
		{
            return cardFunction.card.myPlayer.Credits >= payAmount;
        }
        else if (currency == Currency.Clicks)
		{
            return cardFunction.card.myPlayer.CanAffordAction(payAmount);
		}
        return false;
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


    public void Button_Clicked()
    {
        PlayCardManager.instance.TryActivatePaidAbility(this);
    }

    public void SetInteractable(bool interactable = true)
	{
        button.interactable = interactable;
	}

    public void SetClickable(bool clickable = true)
	{
        canvasGroup.blocksRaycasts = clickable;
	}








    public void ExecuteAbility()
    {
        StartCoroutine(AbilityExecutionRoutine());
        OnPaidAbilityActivated?.Invoke();
    }

    IEnumerator AbilityExecutionRoutine()
    {
        yield return IAbilityExecution();
        ConditionResolved();
    }

    void ConditionResolved()
    {
        //ConditionalAbilitiesManager.instance.ConditionalAbilityResolved(this);
        Debug.Log("ConditionResolved - " + name);
    }


    public bool AreConditionsMet()
    {
        return ClickableConditionsMet();
    }

    public void SetAbilityAndCondition(Ability _ability, ClickableConditions _conditionsMet)
	{
        IAbilityExecution = _ability;
        ClickableConditionsMet = _conditionsMet;
    }


    //   public void SetCardFunctions(CardFunction _cardFunction, CardFunction _viewCardFunction)
    //{
    //       cardFunction = _cardFunction;
    //       viewCardFunction = _viewCardFunction;
    //}



}
