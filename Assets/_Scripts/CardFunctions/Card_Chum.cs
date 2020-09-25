using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Chum : CardFunction
{
	public int strengthModifier, numNetDamage;

	public override void AssignSubroutines(Subroutine[] subroutines)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
			if (i == 0) subroutines[i].SetAbility(StrengthenIce);
		}
	}

	IEnumerator StrengthenIce()
	{
		RunOperator.instance.strengthModifier = strengthModifier;
		RunOperator.instance.netDamageModifier = numNetDamage;
		yield break;
	}
}
