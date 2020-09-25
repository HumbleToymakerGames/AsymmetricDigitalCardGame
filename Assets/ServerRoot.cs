using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ServerRoot : MonoBehaviour
{
    public Card_Upgrade installedUpgradeCard;
    public Transform serverRootT01;

	private void OnEnable()
	{
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
	}
	private void OnDisable()
	{
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
	}
	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (card == installedUpgradeCard && !installed)
		{
			UpgradeCardUnInstalled();
		}
	}

	public virtual void InstallUpgrade(Card_Upgrade upgradeCard)
	{
		if (installedUpgradeCard)
		{
			PlayCardManager.instance.TrashCard(PlayerNR.Corporation, installedUpgradeCard);
		}
		installedUpgradeCard = upgradeCard;
	}

	void UpgradeCardUnInstalled()
	{
		installedUpgradeCard = null;
	}

	public virtual bool HasCardsInstalled()
	{
		return installedUpgradeCard != null;
	}


	public virtual void Access(Card card)
	{

	}
}
