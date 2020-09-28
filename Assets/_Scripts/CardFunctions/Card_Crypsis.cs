using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Crypsis : CardFunction
{
	public int numStrength, numVirusCounters;
	public bool brokeSubroutineOnEncounter;

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(BreakSubroutine, Clickable_Breaker);
			if (i == 1) ability.SetAbilityAndCondition(AddStrength, Clickable_Strength);
			if (i == 2) ability.SetAbilityAndCondition(PlaceVirusCounter, Clickable_Virus);
		}
	}

	bool Clickable_Breaker()
	{
		return RunOperator.instance.canBreakSubroutines;
	}
	IEnumerator BreakSubroutine()
	{
		RunOperator.instance.BreakingSubroutines(paidAbilities[0]);
		yield break;
	}


	bool Clickable_Strength()
	{
		return RunOperator.instance.isRunning;
	}
	IEnumerator AddStrength()
	{
		Card_Program programCard = card as Card_Program;
		programCard.ModifyStrength(numStrength);
		yield break;
	}

	bool Clickable_Virus()
	{
		return true;
	}
	IEnumerator PlaceVirusCounter()
	{
		card.NeutralCounters += numVirusCounters;
		yield break;
	}


	protected override void OnEnable()
	{
		base.OnEnable();
		RunOperator.OnSubroutineBroken += RunOperator_OnSubroutineBroken;
		RunOperator.OnIceEncountered += RunOperator_OnIceEncountered;
		RunOperator.OnIceEncountered_End += RunOperator_OnIceEncountered_End;
	}
	protected override void OnDisable()
	{
		base.OnEnable();
		RunOperator.OnSubroutineBroken -= RunOperator_OnSubroutineBroken;
		RunOperator.OnIceEncountered -= RunOperator_OnIceEncountered;
		RunOperator.OnIceEncountered_End -= RunOperator_OnIceEncountered_End;
	}

	private void RunOperator_OnSubroutineBroken(Subroutine subroutine, PaidAbility breakerAbility)
	{
		if (IsAnyPaidAbility(breakerAbility)) SetBrokeSubroutine(true);
	}
	private void RunOperator_OnIceEncountered(Card_Ice iceCard)
	{
		brokeSubroutineOnEncounter = false;
	}
	private void RunOperator_OnIceEncountered_End(Card_Ice iceCard)
	{

	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(RemovalProcess, CanTriggerCondition);
		}
	}

	void SetBrokeSubroutine(bool broke = true)
	{
		brokeSubroutineOnEncounter = broke;
		if (card.isViewCard) CardViewer.instance.GetCard(card.viewIndex, true).GetComponent<Card_Crypsis>().SetBrokeSubroutine(broke);
	}

	bool CanTriggerCondition()
	{
		return brokeSubroutineOnEncounter;
	}

	IEnumerator RemovalProcess()
	{
		Card viewCard = CardViewer.instance.GetCard(card.viewIndex, false);
		if (viewCard.NeutralCounters > 0)
		{
			viewCard.NeutralCounters--;
		}
		else
		{
			PlayCardManager.instance.TrashCard(PlayerNR.Runner, card);
		}

		SetBrokeSubroutine(false);
		yield break;
	}

	bool IsAnyPaidAbility(PaidAbility paidAbility)
	{
		foreach (var ability in paidAbilities)
		{
			if (ability == paidAbility) return true;
		}
		return false;
	}





}
