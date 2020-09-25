using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_InsideJob : CardFunction
{

	public override void ActivateInstantFunction()
	{
		base.ActivateInstantFunction();
		MakeRunBypassIce();
	}

	void MakeRunBypassIce()
	{

		List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var chooser in FindObjectsOfType<RunChooser>())
		{
			if (chooser.myGO.activeSelf) selectors.Add(chooser.GetComponentInChildren<SelectorNR>());
		}

		if (selectors.Count > 0)
		{
			CardChooser.instance.ActivateFocus(Chosen, 1, selectors.ToArray());

			void Chosen(SelectorNR[] selectorNRs)
			{
				RunOperator.instance.bypassNextIce = true;
				RunOperator.instance.MakeRun(selectorNRs[0].GetComponentInParent<RunChooser>().targetServer);
			}
		}



	}
}
