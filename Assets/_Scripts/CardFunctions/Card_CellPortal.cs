using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_CellPortal : CardFunction
{

	public override void AssignSubroutines(Subroutine[] subroutines)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
			if (i == 0) subroutines[i].SetAbility(PortRunner);
		}
	}

	IEnumerator PortRunner()
	{
		RunOperator.instance.currentServerColumn.ResetIceIndex();
		if (card.isViewCard) CardViewer.instance.GetCard(card.viewIndex, true).DeRez();
		yield break;
	}
}
