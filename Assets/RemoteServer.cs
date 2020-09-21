using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteServer : PlayArea_Spot, IAccessable
{
    public IAccessable installedCard;
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
		if (card == installedCard as Card && !installed)
		{
			UnInstallCard();
		}
	}

    public void InstallCard(IAccessable accessableCard)
	{
		if (HasCardInstalled())
		{
			PlayCardManager.instance.TrashCard(PlayerNR.Corporation, installedCard as Card);
		}

        installedCard = accessableCard;
		Card card = accessableCard as Card;
		card.MoveCardTo(serverRootT);
	}

	public void UnInstallCard()
	{
		installedCard = null;
	}

    public void Access()
	{
        installedCard?.Access();
	}

	public override void RemoveCard(Card card)
	{
		if (card == (Card)installedCard) UnInstallCard();
	}

	public bool HasCardInstalled() { return installedCard != null; }

}
