using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteServer : PlayArea_Spot, IAccessable
{
    public Card installedCard;
	public Transform serverRootT;

	// Start is called before the first frame update
	void Start()
    {
        
    }

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
		if (card == installedCard && !installed)
		{
			UnInstallCard();
		}
	}

    public void InstallCard(Card card)
	{
		if (HasCardInstalled())
		{
			PlayCardManager.instance.TrashCard(PlayerNR.Corporation, installedCard);
		}

        installedCard = card;
		card.MoveCardTo(serverRootT);
	}

	public void UnInstallCard()
	{
		installedCard = null;
	}

    public void Access(ServerColumn.ServerType serverAccessed)
	{
		if (installedCard != null)
		{
			installedCard.Accessed(serverAccessed);
			CardRevealer.instance.RevealCard(installedCard, true);
		}
	}

	public override void RemoveCard(Card card)
	{
		if (card == (Card)installedCard) UnInstallCard();
	}

	public bool HasCardInstalled() { return installedCard != null; }

}
