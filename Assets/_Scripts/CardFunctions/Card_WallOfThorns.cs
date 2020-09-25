using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_WallOfThorns : CardFunction
{

	public int numNetDamage;

	public override void AssignSubroutines(Subroutine[] subroutines)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
			if (i == 0) subroutines[i].SetAbility(DoNetDamage);
			if (i == 1) subroutines[i].SetAbility(EndRun);
		}
	}

	IEnumerator DoNetDamage()
	{
		bool damageDone = false;
		PlayCardManager.instance.DoNetDamage(DamageDone, numNetDamage);
		while (!damageDone) yield return null;

		void DamageDone()
		{
			damageDone = true;
		}
	}

	IEnumerator EndRun()
	{
		RunOperator.instance.EndRun(false);
		yield break;
	}

}
