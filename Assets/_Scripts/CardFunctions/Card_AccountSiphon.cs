using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_AccountSiphon : CardFunction
{
	public int numCreditsLost, numCreditsGained, numTagsTaken;
	public ServerColumn.ServerType serverToRun;
	public bool makingMyRun, serverRan;


	protected override void OnEnable()
	{
		base.OnEnable();
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
	}

	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		if (makingMyRun)
		{
			if (success)
			{
				if (serverType == serverToRun)
				{
					serverRan = true;
				}
			}
			makingMyRun = false;
		}
	}

	public override void ActivateInstantFunction()
	{
		base.ActivateInstantFunction();
		MakeRunForCredits();
	}

	void MakeRunForCredits()
	{
		ServerColumn targetServer = ServerSpace.instance.GetServerOfType(serverToRun);
		RunOperator.instance.MakeRun(targetServer);
		makingMyRun = true;
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(ActivateAbility, CanActivate);
		}
	}

	bool CanActivate()
	{
		return serverRan;
	}

	IEnumerator ActivateAbility()
	{
		print("AccountSiphon!!");

		CardChooser.instance.ActivateFocus(null);
		bool? choiceMade = null;
		ActionOptions.instance.ActivateYesNo(ChoiceMade, string.Format("Activate ability {0}?", card.cardTitle));
		while (!choiceMade.HasValue) yield return null;

		if (choiceMade.Value)
		{
			RunOperator.instance.StopRunCompletedRoutine();

			int numCreditsToLose = Mathf.Min(numCreditsLost, PlayerNR.Corporation.Credits);
			if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, numCreditsToLose))
			{
				int numCreditsToGain = numCreditsGained * numCreditsToLose;
				PlayerNR.Runner.AddCredits(numCreditsToGain);
				PlayerNR.Runner.Tags += numTagsTaken;
			}

		}

		CardChooser.instance.DeactivateFocus();
		serverRan = false;


		void ChoiceMade(bool yes)
		{
			choiceMade = yes;
		}
	}



}
