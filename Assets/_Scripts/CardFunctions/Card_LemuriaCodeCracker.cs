using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_LemuriaCodeCracker : CardFunction
{

	public ServerColumn.ServerType targetServer;
	public int numCredits, numClicks;
	public bool runMade, myTurn;


	protected override void OnEnable()
	{
		base.OnEnable();
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
	}

	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		if (success)
		{
			if (serverType == targetServer)
			{
				runMade = true;
			}
		}
	}
	private void GameManager_OnTurnChanged(bool isRunner)
	{
		myTurn = isRunner;
		runMade = false;
	}

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(ExposeCard, CanBeClickable);
		}
	}

	bool CanBeClickable()
	{
		return runMade && myTurn;
	}

	IEnumerator ExposeCard()
	{
		List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var card in ServerSpace.instance.transform.GetComponentsInChildren<Card>())
		{
			if (card.isInstalled && !card.isRezzed)
			{
				selectors.Add(card.selector);
			}
		}

		bool cardChosen = false;
		if (selectors.Count > 0)
		{
			CardChooser.instance.ActivateFocus(CardChosen, 1, selectors.ToArray());

			while (!cardChosen) yield return null;

			void CardChosen(SelectorNR[] selectorsNR)
			{
				PlayCardManager.instance.ExposeCard(selectorsNR[0].GetComponentInParent<Card>());
				cardChosen = true;
			}
		}
	}

	
}
