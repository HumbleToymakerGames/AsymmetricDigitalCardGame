using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_WallOfStatic : CardFunction
{
	public override void AssignSubroutines(Subroutine[] subroutines)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
			if (i == 0) subroutines[i].SetAbility(EndRun);
		}
	}


	IEnumerator EndRun()
	{
		RunOperator.instance.EndRun(false);
		yield break;
	}
}
