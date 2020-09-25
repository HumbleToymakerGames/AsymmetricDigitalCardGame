using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Upgrade : Card, IInstallable
{









	public bool CanInstall()
	{
		return cardCost.CanAffordCard(PlayerNR.Corporation.Credits);
	}
}
