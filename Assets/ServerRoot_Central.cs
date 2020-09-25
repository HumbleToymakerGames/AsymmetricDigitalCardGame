using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRoot_Central : ServerRoot
{
    public GameObject accessableGO;
    public IAccessable targetAccessable;

	private void Awake()
	{
        targetAccessable = accessableGO.GetComponent<IAccessable>();
    }

	public override void InstallUpgrade(Card_Upgrade upgradeCard)
	{
		base.InstallUpgrade(upgradeCard);
		installedUpgradeCard.MoveCardTo(serverRootT01);
	}

	public override void Access(Card card)
	{
		base.Access(card);
        targetAccessable.Access();
	}



}
