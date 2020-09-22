using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_EasyMark : CardFunction
{

	public int numCredits;

	public override void ActivateFunction()
	{
		base.ActivateFunction();
		GainCredits();
	}

	void GainCredits()
	{
		card.myPlayer.AddCredits(numCredits);
	}


}
