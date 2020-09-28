using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_NeuralEMP : CardFunction
{
	public bool madeRun;

	protected override void OnEnable()
	{
		base.OnEnable();
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
		RunOperator.OnRunBeingMade += RunOperator_OnRunBeingMade;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		RunOperator.OnRunBeingMade -= RunOperator_OnRunBeingMade;
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		if (isRunner) madeRun = false;
	}
	private void RunOperator_OnRunBeingMade(ServerColumn server)
	{
		if (GameManager.CurrentTurnPlayer.IsRunner()) madeRun = true;
	}

	public override void ActivateInstantFunction()
    {
        base.ActivateInstantFunction();
        DoNetDamage();
    }


	public override bool CanPlay()
	{
		return base.CanPlay() && madeRun;
	}

	void DoNetDamage()
    {
		PlayCardManager.instance.DoRunnerDamage(null, 1, DamageType.Net);
    }
}
