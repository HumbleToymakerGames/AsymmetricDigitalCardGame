using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFunction_GabrielSantiago : CardFunction
{
	public bool canBeActivated, runSuccessful;
	public int numCredits;

	protected override void OnEnable()
	{
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
	}
	protected override void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
	}

	public override bool CanExecuteConditionalAbility(ConditionalAbility ability)
	{
		return ability.AreConditionsMet();
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(GainCredits, CanGainCredits);
		}
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		canBeActivated = isRunner;
		runSuccessful = false;
	}
	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		if (serverType == ServerColumn.ServerType.HQ)
		{
			if (canBeActivated)
			{
				runSuccessful = success;
			}
		}
	}

	bool CanGainCredits()
	{
		return canBeActivated && runSuccessful;
	}

	IEnumerator GainCredits()
	{
		PlayerNR.Runner.AddCredits(numCredits);
		canBeActivated = false;
		yield break;
	}


}
