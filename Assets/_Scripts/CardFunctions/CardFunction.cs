﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardFunction : MonoBehaviour
{
	[HideInInspector]
	public Card card;
	public PaidAbility[] paidAbilities;

	public delegate void PaidAbilityActivated();
	public event PaidAbilityActivated OnPaidAbilityActivated;

	protected virtual void Awake()
	{
		card = GetComponent<Card>();
		GetPaidAbilities();
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
}
