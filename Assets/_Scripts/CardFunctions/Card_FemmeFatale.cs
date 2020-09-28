using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_FemmeFatale : CardFunction
{
	public int numStrength;
	public Card_Ice chosenIceCard;
	bool installed;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
		RunOperator.OnIceEncountered += RunOperator_OnIceEncountered;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
		RunOperator.OnIceEncountered -= RunOperator_OnIceEncountered;
	}

	private void PlayCardManager_OnCardInstalled(Card card, bool _installed)
	{
		if (card == this.card && _installed)
		{
			installed = true;
		}
	}
	private void RunOperator_OnIceEncountered(Card_Ice iceCard)
	{
		paidAbilities[2].payments[0].amount = iceCard.subroutines.Length;
	}

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(BreakSubroutine, Clickable_Breaker);
			if (i == 1) ability.SetAbilityAndCondition(AddStrength, Clickable_Strength);
			if (i == 2) ability.SetAbilityAndCondition(BypassIce, Clickable_BypassIce);
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

	bool Clickable_BypassIce()
	{
		return RunOperator.instance.isEncounteringIce && RunOperator.instance.currentIceEncountered == chosenIceCard;
	}
	IEnumerator BypassIce()
	{
		RunOperator.instance.BreakAllSubroutines();
		yield break;
	}



	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(ChooseIceCard, CanChooseIceCard);
		}
	}

	bool CanChooseIceCard()
	{
		return installed;
	}

	IEnumerator ChooseIceCard()
	{

		List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var ice in FindObjectsOfType<Card_Ice>())
		{
			if (ice.isInstalled) selectors.Add(ice.selector);
		}
		if (selectors.Count <= 0)
		{
			installed = false;
			yield break;
		}

		bool chosen = false;
		CardChooser.instance.ActivateFocus(Choose, 1, selectors.ToArray());
		ActionOptions.instance.ActivateActionMessage("Choose Ice for bypassing");
		while (!chosen) yield return null;

		void Choose(SelectorNR[] selectorNRs)
		{
			chosen = true;
			SetChosenIce(selectorNRs[0].GetComponentInParent<Card_Ice>());
		}
		installed = false;
	}

	void SetChosenIce(Card_Ice iceCard)
	{
		chosenIceCard = iceCard;
		if (!card.isViewCard) CardViewer.instance.GetCard(card.viewIndex, false).GetComponent<Card_FemmeFatale>().SetChosenIce(iceCard);
	}

}
