using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SneakdoorBeta : CardFunction
{

	public override void ActivatePaidAbility(int index)
	{
		base.ActivatePaidAbility(index);
		if (index == 1) MakeRunOverrideServer();
	}


	void MakeRunOverrideServer()
	{
		RunOperator.instance.SetServerTypeOverride(ServerColumn.ServerType.HQ);
		ServerColumn archivesServer = ServerSpace.instance.GetServerOfType(ServerColumn.ServerType.Archives);
		RunOperator.instance.MakeRun(archivesServer);
	}

}
