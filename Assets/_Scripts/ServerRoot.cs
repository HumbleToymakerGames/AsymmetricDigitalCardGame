using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ServerRoot : MonoBehaviour
{
	protected ServerColumn serverColumn;
    public Card_Upgrade installedUpgradeCard;
    public Transform serverRootT01;
	protected virtual void Awake()
	{
		serverColumn = GetComponentInParent<ServerColumn>();
	}

	protected virtual void OnEnable()
	{
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
	}
	protected virtual void OnDisable()
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


	public virtual void Access()
	{

	}


	public virtual SelectorNR[] AccessableSelectors()
	{
		return null;
	}

	public Card[] GetCardsInRoot()
	{
		List<Card> cards = new List<Card>();
		if (installedUpgradeCard) cards.Add(installedUpgradeCard);
		ServerRoot_Remote remoteServer = this as ServerRoot_Remote;
		if (remoteServer && remoteServer.installedRootCard) cards.Add(remoteServer.installedRootCard);
		return cards.ToArray();
	}

}
