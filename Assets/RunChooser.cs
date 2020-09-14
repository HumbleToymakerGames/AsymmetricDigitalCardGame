using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunChooser : MonoBehaviour, ISelectableNR
{
	public ServerColumn targetServer;
	public bool CanHighlight()
	{
		return true;
	}

	public bool CanSelect()
	{
		return true;
	}

	public void Highlighted()
	{
	}

	public void Selected()
	{
		RunOperator.instance.MakeRun(targetServer);
	}
}
