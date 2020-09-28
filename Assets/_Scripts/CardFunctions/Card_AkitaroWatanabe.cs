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
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
	}

	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (card == this.card)
		{
			if (installed)
			{
				myServer = GetComponentInParent<ServerColumn>();
				modifiedIceCards = myServer.iceInColumn;
 				UpdateCardCosts();
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
					modifiedIceCards.Add(card as Card_Ice);
					UpdateCardCosts();
				}
				else
				{
					modifiedIceCards.Remove(card as Card_Ice);
					card.SetCardCostModifier(0);
				}
			}
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
