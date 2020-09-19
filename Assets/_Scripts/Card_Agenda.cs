using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card_Agenda : Card, IInstallable, IAccessable
{
	public int scoringAmount;
	public int advancementTargetAmount;
	public TextMeshProUGUI scoringText;

	protected override void Awake()
	{
		base.Awake();
		UpdateScoringText();
	}

	public override bool CanSelect()
	{
		bool canSelect = false;
		if (IsCardInHand())
		{
			canSelect = PlayCardManager.instance.CanInstallCard(this);
		}
		else
		{
			canSelect = isInstalled;
		}
		return base.CanSelect() && canSelect;
	}


	public bool CanInstall()
	{
		return cardCost.CanAffordCard(PlayerNR.Corporation.Credits);
	}


	public void Access()
	{
		print("Agenda Accessed");
		CardRevealer.instance.RevealCard(this);
	}




	public void UpdateScoringText()
	{
		scoringText.text = scoringAmount.ToString();
	}








}
