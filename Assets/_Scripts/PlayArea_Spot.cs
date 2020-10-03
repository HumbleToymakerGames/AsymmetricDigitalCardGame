using System.Collections.Generic;
using UnityEngine;

public abstract class PlayArea_Spot : MonoBehaviour
{
    public PlayerSide playerSide;
	[HideInInspector]
    public PlayerNR myPlayer;

	protected virtual void Awake()
	{
		SetMyPlayer(playerSide == PlayerSide.Runner ? PlayerNR.Runner : PlayerNR.Corporation);
	}

	public void SetMyPlayer(PlayerNR player)
	{
        myPlayer = player;
	}

	public virtual void RemoveCard(Card card)
	{

	}
	public void UpdateCardSiblings((int, int)[] view_sib_Indexes)
	{
		List<Card> myDeck = MyDeck();
		List<Card> newDeck = new List<Card>(myDeck);
		foreach (var view_sib in view_sib_Indexes)
		{
			int view = view_sib.Item1; int sib = view_sib.Item2;

			foreach (var card in myDeck)
			{
				if (card.viewIndex == view)
				{
					newDeck.Remove(card);
					newDeck.Insert(sib, card);
				}
			}
		}

		SetMyDeck(newDeck);
		RefreshDeckSiblingIndexes();
	}

	public virtual List<Card> MyDeck()
	{
		return null;
	}

	protected virtual void SetMyDeck(List<Card> deck)
	{

	}

	[ContextMenu("ADWD")]
	public void RefreshDeckSiblingIndexes()
	{
		List<Card> myDeck = MyDeck();
		for (int i = 0; i < myDeck.Count; i++)
		{
			myDeck[i].transform.SetSiblingIndex(0);
		}
	}


	public bool IsCurrentTurns()
	{
		return playerSide == GameManager.CurrentTurnPlayer.playerSide;
	}

	

}
