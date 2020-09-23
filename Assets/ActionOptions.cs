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

	public GameObject actionMessageGO;
	public TextMeshProUGUI actionMessageText;

	public GameObject continueGO;

	[Header("CounterVars")]
	public GameObject counterGO;
	public TextMeshProUGUI counterTitleText, counterText;
	int currentCounterCount, currentCounterMax;


	const string rezCostFormat = "Yes ({0}cc)";

	private void Awake()
	{
		instance = this;
		HideAllOptions();
	}

	private void OnEnable()
	{
		CardViewer.OnCardPinned += CardViewer_OnCardPinned;
		PlayerNR.Corporation.OnCreditsChanged += OnCreditsChanged;
		PlayerNR.Runner.OnCreditsChanged += OnCreditsChanged;
	}
	private void OnDisable()
	{
		CardViewer.OnCardPinned -= CardViewer_OnCardPinned;
		PlayerNR.Corporation.OnCreditsChanged -= OnCreditsChanged;
		PlayerNR.Runner.OnCreditsChanged -= OnCreditsChanged;
	}

	private void CardViewer_OnCardPinned(Card card, bool primary)
	{
		if (card == null)
		{
			//if (!RunOperator.instance.isRunning)
			//	HideAllOptions();
		}
	}
	private void OnCreditsChanged()
	{
		UpdateInteractables();
	}

	public void Display_FireRemainingSubs(bool showOnly = false)
	{
		if (isYesNo) return;
		TryHideAllOptions(showOnly);
		fireRemainingSubsButton.gameObject.SetActive(true);
		UpdateInteractables();
	}

	public void Display_AdvanceCard(bool showOnly = false)
	{
		if (isYesNo) return;
		TryHideAllOptions(showOnly);
		advanceCardButton.gameObject.SetActive(true);
		UpdateInteractables();
	}

	public void Display_ScoreAgenda(bool showOnly = false)
	{
		if (isYesNo) return;
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
		if (isYesNo) return;
		TryHideAllOptions(showOnly);
		cancelBack = callBack;
		cancelButton.gameObject.SetActive(true);
	}

	UnityAction continueCallback;
	public void Display_Continue(UnityAction callback, bool showonly = false)
	{
		if (isYesNo) return;
		TryHideAllOptions(showonly);
		continueGO.SetActive(true);
		continueCallback = callback;
	}

	public void Display_RezCard(UnityAction<bool> callback, int rezCost, bool showOnly = false)
	{
		TryHideAllOptions(showOnly, false);
		ActivateYesNo(callback, "Rez Card?");
		yesText.text = string.Format(rezCostFormat, rezCost);
	}

	UnityAction<bool> yesnoCallback;
	bool isYesNo;
	public void ActivateYesNo(UnityAction<bool> callback, string title, int yesCost = -123)
	{
		HideAllOptions();
		yesnoTitleText.text = title;
		yesnoCallback = callback;
		yesButton.interactable = PlayCardManager.instance.CanAffordCost(PlayerNR.Corporation, yesCost);
		yesText.text = string.Format(rezCostFormat, yesCost == -123 ? "" : yesCost.ToString());
		isYesNo = true;
		yesNoPanelGO.SetActive(true);
	}

	public void ActivateActionMessage(string message)
	{
		if (isYesNo) return;
		HideAllOptions();
		actionMessageGO.SetActive(true);
		actionMessageText.text = message;
	}

	UnityAction<int> currentCounterCallback;
	public void ActivateCounter(UnityAction<int> callback, string title, int maxCount)
	{
		HideAllOptions();
		SetCounterCount(1);
		currentCounterCallback = callback;
		counterGO.SetActive(true);
		currentCounterMax = maxCount;
	}



	void TryHideAllOptions(bool showOnly, bool isButton = true)
	{
		if (showOnly) HideAllOptions();
		if (isButton)
		{
			yesNoPanelGO.SetActive(false);
			actionMessageGO.SetActive(false);
		}
	}

	public void HideAllOptions()
	{
		if (isYesNo) return;
		//print("HideAllOptions");
		fireRemainingSubsButton.gameObject.SetActive(false);
		advanceCardButton.gameObject.SetActive(false);
		scoreAgendaButton.gameObject.SetActive(false);
		cancelButton.gameObject.SetActive(false);
		continueGO.SetActive(false);
		yesNoPanelGO.SetActive(false);
		actionMessageGO.SetActive(false);
		counterGO.SetActive(false);
	}

	public void UpdateInteractables()
	{
		advanceCardButton.interactable = CardViewer.currentPinnedCard && PlayCardManager.instance.CanAdvanceCard(CardViewer.currentPinnedCard);
		scoreAgendaButton.interactable = CardViewer.currentPinnedCard && CardViewer.currentPinnedCard.CanBeScored();
	}

	public void SetCounterCount(int count)
	{
		currentCounterCount = Mathf.Clamp(count, 0, currentCounterMax);
		UpdateCounterDisplay();
	}

	public void UpdateCounterDisplay()
	{
		counterText.text = currentCounterCount.ToString();
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

	public void Button_Continue()
	{
		continueGO.SetActive(false);
		continueCallback?.Invoke();
		continueCallback = null;
	}

	public void Button_YesNo(bool yes)
	{
		isYesNo = false;
		HideAllOptions();
		yesnoCallback?.Invoke(yes);
		yesnoCallback = null;
	}


	public void Button_Counter(bool positive)
	{
		if (positive)
		{
			SetCounterCount(currentCounterCount + 1);
		}
		else
		{
			SetCounterCount(currentCounterCount - 1);
		}
	}

	public void Button_CounterAccept()
	{
		currentCounterCallback?.Invoke(currentCounterCount);
		currentCounterCallback = null;
		currentCounterCount = 0;
	}



	#endregion


}
