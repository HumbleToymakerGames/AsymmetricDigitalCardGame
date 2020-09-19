using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditBank : MonoBehaviour, ISelectableNR
{

	public bool CanHighlight(bool highlight = true)
	{
		return true;
	}

	public bool CanSelect()
	{
		return PlayCardManager.instance.CanGainCredit();
	}

	public void Highlighted()
	{
	}

	public void Selected()
	{
		PlayCardManager.instance.TryGainCredit();
	}

}
