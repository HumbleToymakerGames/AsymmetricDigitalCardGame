using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_ArmitageCodeBusting : CardFunction
{
	public int numCreditsToStart;
	public int numCreditsToTake;
	[SerializeField]

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
		if (card == this.card && installed)
		{
			PlaceCredits();
		}
	}

	void PlaceCredits()
	{
		UpdateCounter(numCreditsToStart);
	}


	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(TakeCredits, CanBeClickable);
		}
	}

	bool CanBeClickable()
	{
		return card.isInstalled;
	}

	IEnumerator TakeCredits()
	{
		card.NeutralCounters = Mathf.Max(card.NeutralCounters - numCreditsToTake, 0);
		UpdateCounter(card.NeutralCounters);
		card.myPlayer.Credits += numCreditsToTake;

		if (card.NeutralCounters <= 0)
		{
			Card realCard = CardViewer.instance.GetCard(card.viewIndex, true);
			PlayCardManager.instance.TrashCard(PlayerNR.Runner, realCard);
		}

		yield break;
	}




	void UpdateCounter(int count)
	{
		card.NeutralCounters = count;
	}



}
