using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardReferences : MonoBehaviour
{
    [Header("Card References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI cardTypeText, cardSubTypeText;
    public TextMeshProUGUI costText;
    public Image[] cardPin_Primary, cardPin_Secondary;
    public TextMeshProUGUI advancementText_Front, advancementText_Back;
    public GameObject advancementCost_BackGO;
    public TextMeshProUGUI neutralCounterText;

    public void UpdateCardTitle(string cardTitle)
	{
        titleText.text = cardTitle;
	}

    public void UpdateCardTypes(string cardType, string cardSubType)
	{
        cardTypeText.text = cardType;
        cardSubTypeText.text = cardSubType;
    }

    public void UpdateCardCost(int cardCost)
	{
        costText.text = cardCost.ToString();
    }

    public void UpdateNeutralCounter(int neutralCount)
	{
        neutralCounterText.text = neutralCount.ToString();
    }


}
