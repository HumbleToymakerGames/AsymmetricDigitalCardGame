using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_BankJob : CardFunction
{
	public int numCreditsToStart;
	[SerializeField]
	bool cardInstalled, runSuccessful;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
	}

	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		if (success)
		{
			runSuccessful = true;
		}
	}
	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (card == this.card && installed)
		{
			cardInstalled = true;
			PlaceCredits();
		}
	}
	void PlaceCredits()
	{
		UpdateCounter(numCreditsToStart);
	}


	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(TakeCredits, CanTakeCredits);
		}
	}

	bool CanTakeCredits()
	{
		return card.isInstalled && cardInstalled && runSuccessful;
	}

	IEnumerator TakeCredits()
	{
		print("BankJob!!");


		CardChooser.instance.ActivateFocus(null);
		bool? choiceMade = null;
		ActionOptions.instance.ActivateYesNo(ChoiceMade, string.Format("Activate {0}?", card.cardTitle));
		while (!choiceMade.HasValue) yield return null;

		if (choiceMade.Value)
		{

			RunOperator.instance.StopRunCompletedRoutine();

			choiceMade = null;
			int counterCount = 0;
			ActionOptions.instance.ActivateCounter(Counter, "Number credits", card.NeutralCounters);

			void Counter(int count)
			{
				counterCount = count;
				choiceMade = true;
			}

			while (!choiceMade.HasValue) yield return null;

			PlayerNR.Runner.AddCredits(counterCount);

			card.NeutralCounters -= counterCount;
			card.cardRefs.UpdateNeutralCounter(card.NeutralCounters);
			if (card.NeutralCounters <= 0)
			{
				PlayCardManager.instance.TrashCard(PlayerNR.Runner, card);
			}

		}

		CardChooser.instance.DeactivateFocus();


		void ChoiceMade(bool yes)
		{
			choiceMade = yes;
		}

		runSuccessful = false;
	}


	void UpdateCounter(int count)
	{
		card.NeutralCounters = count;
		card.cardRefs.UpdateNeutralCounter(card.NeutralCounters);
	}

}
