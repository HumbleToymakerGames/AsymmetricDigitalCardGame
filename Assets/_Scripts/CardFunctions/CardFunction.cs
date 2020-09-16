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
		SubscribeToConditions();
	}

	protected virtual void OnDisable()
	{
		UnSubscribeToConditions();
	}



	protected virtual void SubscribeToConditions()
	{

	}

	protected virtual void UnSubscribeToConditions()
	{

	}

	#region Conditional Abilities

	public void TryExecuteConditionalAbility(int abilityIndex)
	{
		if (CanExecuteConditionalAbility(abilityIndex))
			ExecuteConditionalAbility(abilityIndex);
		else ConditionalAbilityResolved();
	}

	protected virtual void ExecuteConditionalAbility(int abilityIndex)
	{
		// Add catch-all cavets here (check if installed)

		// Do whatever action to resolve ability, then callback ConditionalAbilityResolved()
	}

	protected virtual bool CanExecuteConditionalAbility(int abilityIndex)
	{
		return true;
	}


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
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			conditionalAbilities[i].myAbilityIndex = i + 1;
		}
	}

}
