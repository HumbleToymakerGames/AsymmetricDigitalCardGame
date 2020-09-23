using System.Collections;
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
		StartPaidAbilitiesWindow();
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

	void StopPaidAbilitiesWindow()
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

			SetPriorityPlayer(GameManager.CurrentTurnPlayer);
		}
		print("PaidAbilitiesWindow Closed");

	}


	IEnumerator ExecuteWindow()
	{
		print("PaidAbilitiesWindow Executing...");
		numberAbilitiesUsed = 0;
		bool waitingForContinue = true;
		ActionOptions.instance.Display_Continue(Continue);
		while (waitingForContinue) yield return null;

		void Continue()
		{
			waitingForContinue = false;
		}

	}


	public void SetPriorityPlayer(PlayerNR newPriorityPlayer)
	{
		PriorityPlayer = newPriorityPlayer;
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
		print("Paused");
		if (IsResolving())
		{
			pausedPriorityPlayer = PriorityPlayer;
			PriorityPlayer = null;
			isPaused = true;
		}
	}

	public void ResumePaidWindow()
	{
		print("Resumed	");
		if (isPaused)
		{
			PriorityPlayer = pausedPriorityPlayer;
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
