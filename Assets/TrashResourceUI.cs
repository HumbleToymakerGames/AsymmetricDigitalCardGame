using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashResourceUI : MonoBehaviour, ISelectableNR
{
	public bool CanHighlight(bool highlight = true)
	{
		return !GameManager.CurrentTurnPlayer.IsRunner();
	}

	public bool CanSelect()
	{
		return CanHighlight() && PlayCardManager.instance.CanTrashResource();
	}

	public void Highlighted()
	{
	}

	public void Selected()
	{
		PlayCardManager.instance.TryTrashResource();
	}
}
