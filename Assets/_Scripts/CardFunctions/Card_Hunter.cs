using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Hunter : CardFunction
{

	public int traceStrength;
	public int numTags;

	public override void AssignSubroutines(Subroutine[] subroutines)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
			if (i == 0) subroutines[i].SetAbility(PerformTrace);
		}
	}

	IEnumerator PerformTrace()
	{
		bool? traceSuccess = null;
		yield return TraceRunner.instance.RunTrace(TraceFinished, traceStrength);
		while (!traceSuccess.HasValue) yield return null;

		if (traceSuccess.Value) yield return PlayCardManager.instance.TagRunner(numTags);

		void TraceFinished(bool success)
		{
			traceSuccess = success;
		}
	}

}
