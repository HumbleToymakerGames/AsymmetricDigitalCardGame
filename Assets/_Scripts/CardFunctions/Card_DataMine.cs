using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_DataMine : CardFunction
{
	public int numRunnerDamage;
	public override void AssignSubroutines(Subroutine[] subroutines)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
			if (i == 0) subroutines[i].SetAbility(DoNetDamage);
		}
	}

	IEnumerator DoNetDamage()
	{
		yield return PlayCardManager.instance.DoRunnerDamage(null, numRunnerDamage, DamageType.Net);
		PlayCardManager.instance.TrashCard(PlayerNR.Corporation, CardViewer.instance.GetCard(card.viewIndex, true));
	}
}
