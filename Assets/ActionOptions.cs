using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ActionOptions : MonoBehaviour
{
	public static ActionOptions instance;
    public Button fireRemainingSubsButton;
	public Button advanceCardButton;
	public Button scoreAgendaButton;
	public Button cancelButton;

	public GameObject yesNoPanelGO;
	public Button yesButton;
	public TextMeshProUGUI yesText, yesnoTitleText;

	const string rezCostFormat = "Yes ({0}cc)";

	private void Awake()
	{
		instance = this;
		HideAllOptions();
	}

	private void OnEnable()
	{
		CardViewWindow.OnCardPinnedToView += CardViewWindow_OnCardPinnedToView;
		PlayerNR.Corporation.OnCreditsChanged += OnCreditsChanged;
		PlayerNR.Runner.OnCreditsChanged += OnCreditsChanged;
	}

	private void OnDisable()
	{
		CardViewWindow.OnCardPinnedToView -= CardViewWindow_OnCardPinnedToView;
		PlayerNR.Corporation.OnCreditsChanged -= OnCreditsChanged;
		PlayerNR.Runner.OnCreditsChanged -= OnCreditsChanged;
	}
	private void CardViewWindow_OnCardPinnedToView(Card card)
	{
		if (card == null)
		{
			if (!RunOperator.instance.isRunning)
				HideAllOptions();
		}
	}
	private void OnCreditsChanged()
	{
		UpdateInteractables();
	}

	public void Display_FireRemainingSubs(bool showOnly = false)
	{
		TryHideAllOptions(showOnly);
		fireRemainingSubsButton.gameObject.SetActive(true);
		UpdateInteractables();
	}

	public void Display_AdvanceCard(bool showOnly = false)
	{
		TryHideAllOptions(showOnly);
		advanceCardButton.gameObject.SetActive(true);
		UpdateInteractables();
	}

	public void Display_ScoreAgenda(bool showOnly = false)
	{
		TryHideAllOptions(showOnly);
		if (CardViewer.currentPinnedCard)
		{
			scoreAgendaButton.gameObject.SetActive(CardViewer.currentPinnedCard.IsScoreable());
		}
		UpdateInteractables();
	}

	UnityAction cancelBack;
	public void Display_Cancel(UnityAction callBack, bool showOnly = false)
	{
		TryHideAllOptions(showOnly);
		cancelBack = callBack;
		cancelButton.gameObject.SetActive(true);
	}

	public void Display_RezCard(UnityAction<bool> callback, int rezCost, bool showOnly = false)
	{
		TryHideAllOptions(showOnly);
		ActivateYesNo(callback, "Rez Card?");
		yesButton.interactable = PlayCardManager.instance.CanAffordCost(PlayerNR.Corporation, rezCost);
		yesText.text = string.Format(rezCostFormat, rezCost);
	}

	UnityAction<bool> yesnoCallback;
	public void ActivateYesNo(UnityAction<bool> callback, string title, int yesCost = -123)
	{
		HideAllOptions();
		yesNoPanelGO.SetActive(true);
		yesnoTitleText.text = title;
		yesnoCallback = callback;
		yesText.text = string.Format(rezCostFormat, yesCost == -123 ? "" : yesCost.ToString());
	}


	void TryHideAllOptions(bool showOnly)
	{
		if (showOnly) HideAllOptions();
	}

	public void HideAllOptions()
	{
		fireRemainingSubsButton.gameObject.SetActive(false);
		advanceCardButton.gameObject.SetActive(false);
		scoreAgendaButton.gameObject.SetActive(false);
		cancelButton.gameObject.SetActive(false);
		yesNoPanelGO.SetActive(false);
	}

	public void UpdateInteractables()
	{
		advanceCardButton.interactable = CardViewer.currentPinnedCard && PlayCardManager.instance.CanAdvanceCard(CardViewer.currentPinnedCard);
		scoreAgendaButton.interactable = CardViewer.currentPinnedCard && CardViewer.currentPinnedCard.CanBeScored();

	}


	#region Buttons

	public void Button_FireRemainingSubs()
	{
		RunOperator.instance.FireRemainingSubroutines();
	}

	public void Button_AdvanceCard()
	{
		if (CardViewer.currentPinnedCard)
		{
			if(PlayCardManager.instance.TryAdvanceCard(CardViewer.currentPinnedCard))
			{
				UpdateInteractables();
			}
		}
	}

	public void Button_ScoreCard()
	{
		if (CardViewer.currentPinnedCard)
		{
			if (PlayCardManager.instance.TryScoreAgenda(PlayerNR.Corporation, CardViewer.currentPinnedCard as Card_Agenda))
			{
				UpdateInteractables();
			}
		}
	}

	public void Button_Cancel()
	{
		cancelButton.gameObject.SetActive(false);
		cancelBack?.Invoke();
		cancelBack = null;
	}

	public void Button_YesNo(bool yes)
	{
		HideAllOptions();
		yesnoCallback?.Invoke(yes);
		yesnoCallback = null;
	}

	#endregion


}
