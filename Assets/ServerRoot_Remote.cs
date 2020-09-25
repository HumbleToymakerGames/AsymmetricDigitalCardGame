using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ServerRoot_Remote : ServerRoot
{
	public Card installedRootCard;
	public Transform serverRootT02;

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


    public override void Access(Card card)
	{
		if (card == installedRootCard)
		{
			card.Accessed();
			CardRevealer.instance.RevealCard(card, true, RunOperator.instance.ServerFinishedAccessing);
		}
		else
		{
			card.Accessed();
			CardRevealer.instance.RevealCard(card, true, RunOperator.instance.ServerFinishedAccessing);
		}
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
