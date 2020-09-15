using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionOptions : MonoBehaviour
{
	public static ActionOptions instance;
    public GameObject fireRemainingSubsGO;
    public GameObject rezCardPanelGO;
	public Button rezButtonYesGO;
	public TextMeshProUGUI rezCardYesText;

	const string rezCostFormat = "Yes ({0}cc)";

	private void Awake()
	{
		instance = this;
		HideAllOptions();
	}

	public void Display_FireRemainingSubs(bool showOnly = false)
	{
		TryHideAllOptions(showOnly);
		fireRemainingSubsGO.SetActive(true);
	}

	public void Display_RezCard(int rezCost, bool showOnly = false)
	{
		TryHideAllOptions(showOnly);
		rezButtonYesGO.interactable = PlayCardManager.instance.CanAffordCost(PlayerNR.Corporation, rezCost);
		rezCardPanelGO.SetActive(true);
		rezCardYesText.text = string.Format(rezCostFormat, rezCost);
	}


	void TryHideAllOptions(bool showOnly)
	{
		if (showOnly) HideAllOptions();
	}

	public void HideAllOptions()
	{
		fireRemainingSubsGO.SetActive(false);
		rezCardPanelGO.SetActive(false);
	}


	#region Buttons

	public void Button_FireRemainingSubs()
	{
		RunOperator.instance.FireRemainingSubroutines();
	}

	public void Button_RezCard(bool rezCard)
	{
		RunOperator.instance.RezCardChoice(rezCard);
		HideAllOptions();
	}

	#endregion


}
