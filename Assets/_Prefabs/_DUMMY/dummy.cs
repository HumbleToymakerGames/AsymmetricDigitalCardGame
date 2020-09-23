using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy : CardFunction
{
	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(ActivateAbility, CanBeActivated);
		}
	}

	bool CanBeActivated()
	{
		return card.isInstalled;
	}

	IEnumerator ActivateAbility()
	{

		print("dummy1");
		yield return new WaitForSeconds(5);
		print("dummy2");



	}
}
