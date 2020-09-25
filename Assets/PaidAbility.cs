using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaidAbility : MonoBehaviour
{
    [Header("Ability")]
    public Payment[] payments;
    public CardSubType breakerType;
    bool isBreakerRoutine;
    Button button;
    CanvasGroup canvasGroup;

    [System.Serializable]
    public struct Payment
	{
        public int amount;
        public Currency currency;
	}

    public delegate IEnumerator Ability();
    public Ability IAbilityExecution;

    public delegate bool ClickableConditions();
    public ClickableConditions ClickableConditionsMet;
    public ClickableConditions CanAffordConditons;

    public delegate void PaidAbilityActivated();
    public static event PaidAbilityActivated OnPaidAbilityActivated;
    public PaidAbilityActivated IPayCostsExecution;

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
        if (CanBeActive_Currency())
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
                            if (CanBeActive_Breaker())
                            {

                            }
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

    bool CanBeActive_Currency()
	{
        return CanAfford(cardFunction.card.myPlayer);
    }

    bool CanBeActive_Breaker()
	{
        if (isBreakerRoutine && !RunOperator.instance.canBreakSubroutines)
		{
            return false;
		}
        return true;
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
    public void SetCostsAndPayments(PaidAbilityActivated _payments, ClickableConditions _costs)
    {
        IPayCostsExecution = _payments;
        CanAffordConditons = _costs;
    }

    //   public void SetCardFunctions(CardFunction _cardFunction, CardFunction _viewCardFunction)
    //{
    //       cardFunction = _cardFunction;
    //       viewCardFunction = _viewCardFunction;
    //}

    public bool CanAfford(PlayerNR player)
	{
		foreach (var payment in payments)
		{
            if (payment.currency == Currency.Credits)
			{
                if (!player.CanAffordCost(payment.amount)) return false;
			}
            if (payment.currency == Currency.Clicks)
            {
                if (!player.CanAffordAction(payment.amount)) return false;
            }
            if (CanAffordConditons != null && !CanAffordConditons()) return false;
        }
        return true;
	}

    public void PayCosts(PlayerNR player)
	{
		foreach (var payment in payments)
		{
            if (payment.currency == Currency.Credits)
            {
                player.Credits -= payment.amount;
            }
            if (payment.currency == Currency.Clicks)
            {
                player.ActionPoints -= payment.amount;
            }

            IPayCostsExecution?.Invoke();
        }
	}

    public void ReverseCosts(PlayerNR player)
	{
        foreach (var payment in payments)
        {
            if (payment.currency == Currency.Credits)
            {
                player.Credits += payment.amount;
            }
            if (payment.currency == Currency.Clicks)
            {
                player.ActionPoints += payment.amount;
            }
        }
    }


}
