using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Enigma : CardFunction
{
	public int numClicksLost;

	public override void AssignSubroutines(Subroutine[] subroutines)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
			if (i == 0) subroutines[i].SetAbility(LoseClicks);
			if (i == 1) subroutines[i].SetAbility(EndRun);
		}
	}

	IEnumerator LoseClicks()
	{
		PlayerNR.Runner.ActionPoints -= numClicksLost;
		yield break;
	}

	IEnumerator EndRun()
	{
		RunOperator.instance.EndRun(false);
		yield break;
	}
}
