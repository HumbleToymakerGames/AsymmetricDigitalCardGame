using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardFunction : MonoBehaviour
{
	[HideInInspector]
	public Card card;
	public PaidAbility[] paidAbilities;
	public ConditionalAbility[] conditionalAbilities;

	public delegate void PaidAbilityActivated();
	public event PaidAbilityActivated OnPaidAbilityActivated;


	protected virtual void Awake()
	{
		card = GetComponent<Card>();
		GetPaidAbilities();
		GetConditionalAbilities();
	}


	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}


	#region Conditional Abilities



	public virtual bool CanExecuteConditionalAbility(ConditionalAbility ability)
	{
		return true;
	}

	//public virtual bool IsConditionMet()
	//{

	//}


	public void ConditionalAbilityResolved()
	{
		ConditionalAbilitiesManager.instance.ConditionalAbilityResolved();
	}


	#endregion

	public virtual void ActivateFunction()
	{

	}

	public virtual void ActivatePaidAbility(int index)
	{
		OnPaidAbilityActivated?.Invoke();
	}

	public void UpdatePaidAbilitesActive_ActionPoints(int actionPoints)
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			paidAbilities[i].ActivateOnActionPoints(actionPoints);
		}
	}

	public void UpdatePaidAbilitesActive_Credits(int playerCredits)
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			paidAbilities[i].ActivateOnCredits(playerCredits);
		}
	}

	public bool HasBreakerOfType(CardSubType cardSubType)
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			if (paidAbilities[i].breakerType == cardSubType) return true;
		}
		return false;
	}


	void GetPaidAbilities()
	{
		paidAbilities = GetComponentsInChildren<PaidAbility>();
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			paidAbilities[i].myAbilityIndex = i + 1;
		}
	}

	void GetConditionalAbilities()
	{
		conditionalAbilities = GetComponentsInChildren<ConditionalAbility>();
		AssignConditionalAbilities();
	}

	protected virtual void AssignConditionalAbilities()
	{

	}

}
