using System.Collections;

public class CardFunction_PadCampaign : CardFunction
{
	public int numCredits;
	bool myTurn;

	protected override void OnEnable()
	{
		base.OnEnable();
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		myTurn = card.myPlayer.IsRunner() == isRunner;
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(GainCredits, CanGainCredits);
		}
	}

	bool CanGainCredits()
	{
		return card.isInstalled && card.isRezzed && myTurn;
	}

	IEnumerator GainCredits()
	{
		PlayerNR.Corporation.AddCredits(numCredits);
		myTurn = false;
		yield break;
	}



}
