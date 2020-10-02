using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardAdvancer : MonoBehaviour
{
	Card card;
	CardReferences cardRefs;
	Card_Agenda agendaCard;
	public int advancementAmount;
	public bool isScored;

	private void Awake()
	{
		card = GetComponent<Card>();
		cardRefs = GetComponent<CardReferences>();
		agendaCard = GetComponent<Card_Agenda>();
		UpdateAdvancementTexts();
		cardRefs.advancementCost_BackGO.SetActive(false);
	}

	public void AdvanceCard()
	{
		advancementAmount++;
		UpdateAdvancementTexts();
		if (!card.isViewCard) CardViewer.instance.GetCard(card.viewIndex, false).cardAdvancer.AdvanceCard();
	}

	public bool CanBeAdvanced()
	{
		return !isScored;
	}

	public bool CanBeScored()
	{
		if (!isScored && agendaCard && advancementAmount >= agendaCard.advancementTargetAmount) return true;
		return false;
	}

	public void CardScored(bool scored = true)
	{
		isScored = scored;
		if (!card.isViewCard) CardViewer.instance.GetCard(card.viewIndex, false).cardAdvancer.CardScored(scored);
	}

	void UpdateAdvancementTexts()
	{
		cardRefs.advancementText_Back.text = advancementAmount.ToString();
		string frontString = advancementAmount.ToString();
		if (agendaCard) frontString += "/" + agendaCard.advancementTargetAmount;
		cardRefs.advancementText_Front.text = frontString;
		if (advancementAmount > 0) cardRefs.advancementCost_BackGO.SetActive(true);
	}


}
