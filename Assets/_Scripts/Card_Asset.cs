using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Asset : Card, IInstallable
{


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

	//override void

	public bool CanInstall()
    {
        return cardCost.CanAffordCard(PlayerNR.Corporation.Credits);
    }

}
