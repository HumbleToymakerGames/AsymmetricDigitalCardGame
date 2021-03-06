﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ServerRoot_Remote : ServerRoot
{
	public Card installedRootCard;
	public Transform serverRootT02;

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
		if (card == installedRootCard && !installed)
		{
			UnInstallRootCard();
		}
	}


	public void UnInstallRootCard()
	{
		installedRootCard = null;
	}

	public override void InstallUpgrade(Card_Upgrade upgradeCard)
	{
		Transform parentT;
		if (installedUpgradeCard)
		{
			parentT = installedUpgradeCard.transform.parent;
		}
		else
		{
			parentT = GetRandomRootT();
		}
		base.InstallUpgrade(upgradeCard);

		upgradeCard.MoveCardTo(parentT);
	}

	public void InstallRootCard(Card card)
	{
		if (installedRootCard)
		{
			Transform parentT = installedRootCard.transform.parent;
			PlayCardManager.instance.TrashCard(PlayerNR.Corporation, installedRootCard);
			installedRootCard = card;
			card.MoveCardTo(parentT);
		}
		else
		{
			Transform randomT = GetRandomRootT();
			installedRootCard = card;
			card.MoveCardTo(randomT);
		}
	}


    public override void Access()
	{

	}

	public override SelectorNR[] AccessableSelectors()
	{
		Card[] cards = GetCardsInRoot();
		SelectorNR[] selectors = new SelectorNR[cards.Length];
		for (int i = 0; i < cards.Length; i++)
		{
			selectors[i] = cards[i].selector;
		}
		return selectors;
	}



	public Transform GetRandomRootT()
	{
		List<Transform> rootTs = new List<Transform>() { serverRootT01, serverRootT02 };
		if (installedRootCard && installedUpgradeCard) return null;
		else if (installedRootCard) rootTs.Remove(installedRootCard.transform.parent);
		else if (installedUpgradeCard) rootTs.Remove(installedUpgradeCard.transform.parent);
		return rootTs[Random.Range(0, rootTs.Count)];
	}





	public override bool HasCardsInstalled()
	{
		return base.HasCardsInstalled() || installedRootCard != null;
	}

}
