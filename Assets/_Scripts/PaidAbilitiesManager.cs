﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaidAbilitiesManager : MonoBehaviour
{
    public static PaidAbilitiesManager instance;

	public delegate void PaidWindowChanged(PlayerNR newPriority);
	public static event PaidWindowChanged OnPaidWindowChanged;

	public static PlayerNR PriorityPlayer;

	public int numberAbilitiesUsed;
	public bool isPaused;

	private void Awake()
	{
        instance = this;
	}

	private void OnEnable()
	{
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
		PaidAbility.OnPaidAbilityActivated += PaidAbility_OnPaidAbilityActivated;
	}
	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		SetPriorityToTurn();
	}
	private void PaidAbility_OnPaidAbilityActivated()
	{
		numberAbilitiesUsed++;
	}

	public bool isResolving;
	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.B)) SwapPriorityPlayer();
		//if (Input.GetKeyDown(KeyCode.G)) SetPriorityPlayer(null);
		isResolving = IsResolving();
	}

	public Coroutine StartPaidAbilitiesWindow()
	{
		StopPaidAbilitiesWindow();
		paidAbitiliesRoutine = StartCoroutine(PaidAbilitiesWindow());
		return paidAbitiliesRoutine;
	}

	public void StopPaidAbilitiesWindow()
	{
		if (paidAbitiliesRoutine != null) StopCoroutine(paidAbitiliesRoutine);
	}

	Coroutine paidAbitiliesRoutine;
	public IEnumerator PaidAbilitiesWindow()
	{
		print("PaidAbilitiesWindow Started");
		SetPriorityPlayer(GameManager.CurrentTurnPlayer);

		if (RunOperator.instance.isRunning)
		{
			numberAbilitiesUsed = 0;
			int windowNum = 0;
			while (windowNum < 2 || numberAbilitiesUsed != 0)
			{
				yield return ExecuteWindow();
				windowNum++;
				SwapPriorityPlayer();
			}

		}

		SetPriorityPlayer(null);
		print("PaidAbilitiesWindow Closed");
	}


	IEnumerator ExecuteWindow()
	{
		print("PaidAbilitiesWindow Executing...");
		numberAbilitiesUsed = 0;
		bool waitingForContinue = true;
		ActionOptions.instance.Display_Continue(Continue, true);
		while (waitingForContinue)
		{
			yield return new WaitForSeconds(0.1f);
			if (waitingForContinue && ActionOptions.instance.AllOptionsHidden())
				ActionOptions.instance.Display_Continue(Continue, true);
		}

		void Continue()
		{
			waitingForContinue = false;
		}

	}

	public void SetPriorityToTurn()
	{
		SetPriorityPlayer(GameManager.CurrentTurnPlayer);
		StopPaidAbilitiesWindow();
	}


	public void SetPriorityPlayer(PlayerNR newPriorityPlayer)
	{
		PriorityPlayer = newPriorityPlayer;
		if (isPaused) pausedPriorityPlayer = newPriorityPlayer;
		OnPaidWindowChanged?.Invoke(newPriorityPlayer);
	}

	public void SwapPriorityPlayer()
	{
		if (IsResolving())
			SetPriorityPlayer(PriorityPlayer.IsRunner() ? PlayerNR.Corporation : PlayerNR.Runner);
	}


	PlayerNR pausedPriorityPlayer;
	public void PausePaidWindow()
	{
		if (IsResolving() && !isPaused)
		{
			print("PausedPaidWindow");
			pausedPriorityPlayer = PriorityPlayer;
			SetPriorityPlayer(null);
			isPaused = true;
		}
	}

	public void ResumePaidWindow()
	{
		if (isPaused)
		{
			print("ResumedPaidWindow");
			SetPriorityPlayer(pausedPriorityPlayer);
			pausedPriorityPlayer = null;
			isPaused = false;
		}
	}

	public bool IsMyPriority(PlayerNR player)
	{
		return PriorityPlayer == player;
	}

	public bool IsResolving()
	{
		return PriorityPlayer != null;
	}

}
