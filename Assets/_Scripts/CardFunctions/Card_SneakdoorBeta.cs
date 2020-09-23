using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SneakdoorBeta : CardFunction
{




	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(MakeRunOverrideServer, CanBeClickable);
		}
	}



	bool CanBeClickable()
	{
		return !RunOperator.instance.isRunning;
	}

	IEnumerator MakeRunOverrideServer()
	{
		RunOperator.instance.SetServerTypeOverride(ServerColumn.ServerType.HQ);
		ServerColumn archivesServer = ServerSpace.instance.GetServerOfType(ServerColumn.ServerType.Archives);
		RunOperator.instance.MakeRun(archivesServer);

		yield break;
	}

}
