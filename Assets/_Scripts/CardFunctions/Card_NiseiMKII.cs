using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_NiseiMKII : CardFunction
{
	public int numAgendaCountersPlaced;
	public bool cardScored;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardScored += PlayCardManager_OnCardScored;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardScored -= PlayCardManager_OnCardScored;
	}

	private void PlayCardManager_OnCardScored(Card card)
	{
		if (card == this.card) SetCardScored(true);
	}

	void SetCardScored(bool scored = true)
	{
		cardScored = scored;
		if (!card.isViewCard)
		{
			Card viewCard = CardViewer.instance.GetCard(card.viewIndex, false);
			viewCard.GetComponent<Card_NiseiMKII>().SetCardScored(scored);
		}
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(PlaceAgenaCounters, CanPlaceAgendaCounters);
		}
	}
	bool CanPlaceAgendaCounters()
	{
		return cardScored;
	}

	public IEnumerator PlaceAgenaCounters()
	{
		card.NeutralCounters = numAgendaCountersPlaced;
		if (!card.isViewCard)
		{
			Card_NiseiMKII viewCard = CardViewer.instance.GetCard(card.viewIndex, false).GetComponent<Card_NiseiMKII>();
			viewCard.StartCoroutine(viewCard.PlaceAgenaCounters());
		}

		yield break;
	}



	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(EndRun, CanBeClickable);
		}
	}

	bool CanBeClickable()
	{
		return cardScored && RunOperator.instance.isRunning && card.NeutralCounters > 0;
	}

	IEnumerator EndRun()
	{
		RunOperator.instance.EndRun(false);
		card.NeutralCounters--;
		yield break;
	}


}
