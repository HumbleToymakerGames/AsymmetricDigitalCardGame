using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Card_Desperado : CardFunction
{
	public int amountMemory;
	public int numCredits;
	public bool runSuccessful, cardInstalled;

	protected override void OnEnable()
	{
		base.OnEnable();
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
	}

	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		runSuccessful = success;
	}

	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (card == this.card && installed)
		{
			cardInstalled = true;
		}
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(AddMemory, CanAddMemory);
			if (i == 1) ability.SetExecutionAndCondition(GainCredits, CanGainCredits);
		}
	}

	bool CanGainCredits()
	{
		return card.isInstalled && runSuccessful;
	}

	bool CanAddMemory()
	{
		return card.isInstalled && cardInstalled;
	}

	IEnumerator AddMemory()
	{
		PlayerNR.Runner.AddTotalMemory(amountMemory);
		cardInstalled = false;
		print("Added Mem");
		yield break;
	}

	void RemoveMemory()
	{
		PlayerNR.Runner.AddTotalMemory(-1);
	}

	IEnumerator GainCredits()
	{
		PlayerNR.Runner.AddCredits(numCredits);
		runSuccessful = false;
		yield break;
	}

}
