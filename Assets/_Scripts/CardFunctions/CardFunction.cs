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

	


	protected virtual void Awake()
	{
		card = GetComponent<Card>();
		GetPaidAbilities();
		GetConditionalAbilities();
	}


	protected virtual void OnEnable()
	{
		card.myPlayer.OnCreditsChanged += MyPlayer_OnCreditsChanged;
		card.myPlayer.OnActionPointsChanged += MyPlayer_OnActionPointsChanged;
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
		Card.OnCardRezzed += Card_OnCardRezzed;
		PaidAbility.OnPaidAbilityActivated += PaidAbility_OnPaidAbilityActivated;
		RunOperator.OnApproached += RunOperator_OnCardBeingApproached;
		RunOperator.OnIceEncountered += RunOperator_OnIceEncountered;
	}

	private void RunOperator_OnIceEncountered(Card_Ice iceCard)
	{
		//UpdatePaidAbilitesInteractable();
	}

	private void RunOperator_OnCardBeingApproached(int encounterIndex)
	{
		//UpdatePaidAbilitesInteractable();
	}

	private void PaidAbility_OnPaidAbilityActivated()
	{
		UpdatePaidAbilitesInteractable();
	}

	private void MyPlayer_OnCreditsChanged()
	{
		UpdatePaidAbilitesInteractable();
	}
	private void MyPlayer_OnActionPointsChanged()
	{
		UpdatePaidAbilitesInteractable();
	}

	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		UpdatePaidAbilitesInteractable();
	}
	private void Card_OnCardRezzed(Card card)
	{
		UpdatePaidAbilitesInteractable();
	}

	protected virtual void OnDisable()
	{
		card.myPlayer.OnCreditsChanged -= MyPlayer_OnCreditsChanged;
		card.myPlayer.OnActionPointsChanged -= MyPlayer_OnActionPointsChanged;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		Card.OnCardRezzed -= Card_OnCardRezzed;
		PaidAbility.OnPaidAbilityActivated -= PaidAbility_OnPaidAbilityActivated;
		RunOperator.OnApproached -= RunOperator_OnCardBeingApproached;
		RunOperator.OnIceEncountered -= RunOperator_OnIceEncountered;
	}


	#region Conditional Abilities


	public virtual bool CanExecuteConditionalAbility(ConditionalAbility ability)
	{
		return true;
	}

	#endregion

	public virtual void ActivateInstantFunction()
	{

	}


	public void UpdatePaidAbilitesInteractable()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			paidAbilities[i].UpdateAbilityInteractable();
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
		AssignPaidAbilities();
	}

	void GetConditionalAbilities()
	{
		conditionalAbilities = GetComponentsInChildren<ConditionalAbility>();
		AssignConditionalAbilities();
	}

	protected virtual void AssignConditionalAbilities()
	{

	}

	protected virtual void AssignPaidAbilities()
	{

	}

}
