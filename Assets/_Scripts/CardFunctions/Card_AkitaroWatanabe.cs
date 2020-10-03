using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_AkitaroWatanabe : CardFunction
{
	public int costModifier;
	public ServerColumn myServer;
	List<Card_Ice> modifiedIceCards = new List<Card_Ice>();

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
		Card.OnCardRezzed += Card_OnCardRezzed;
	}


	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
		Card.OnCardRezzed -= Card_OnCardRezzed;
	}

	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (card == this.card)
		{
			if (installed)
			{
				TryUpdateCardCosts();
			}
			else
			{
				ReverseAllModifications();
			}
		}
		else if (this.card.isInstalled && card is Card_Ice)
		{
			if (card.GetComponentInParent<ServerColumn>() == myServer)
			{
				if (installed)
				{
					TryUpdateCardCosts();
				}
				else
				{
					modifiedIceCards.Remove(card as Card_Ice);
					card.SetCardCostModifier(0);
				}
			}
		}
	}
	private void Card_OnCardRezzed(Card card)
	{
		if (card == this.card)
		{
			TryUpdateCardCosts();
		}
	}

	void TryUpdateCardCosts()
	{
		if (card.isInstalled && card.isRezzed)
		{
			myServer = GetComponentInParent<ServerColumn>();
			modifiedIceCards = myServer.iceInColumn;
			UpdateCardCosts();
		}
	}

	void UpdateCardCosts()
	{
		foreach (var card in modifiedIceCards)
		{
			card.SetCardCostModifier(-costModifier);
		}
	}


	void ReverseAllModifications()
	{
		foreach (var ice in modifiedIceCards)
		{
			ice.SetCardCostModifier(0);
		}
	}


}
