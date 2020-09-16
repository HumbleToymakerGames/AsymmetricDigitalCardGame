using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Hardware : Card, IInstallable
{

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	public override bool CanSelect()
	{
		if (IsCardInHand() && IsCardSubType(CardSubType.Console) && RunnerRIG.instance.hasConsoleHardwareInstalled)
		{
			return false;
		}
		return base.CanSelect() &&
			PlayCardManager.instance.CanInstallCard(this);
	}

	public bool CanInstall()
	{
		return cardCost.CanAffordCard(PlayerNR.Runner.Credits);
	}

}
