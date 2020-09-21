using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFunction_MelangeMiningCorp : CardFunction
{
    public int numCredits;
	public override void ActivateFunction()
	{
		base.ActivateFunction();
	}

	public override void ActivatePaidAbility(int index)
	{
		base.ActivatePaidAbility(index);
		if (index == 1) GainCredits();
	}


	void GainCredits()
    {
        PlayerNR.Corporation.AddCredits(numCredits);
    }



}
