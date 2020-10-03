using UnityEngine;

public class TurnUI : MonoBehaviour, ISelectableNR
{
	public GameObject runnerGO, corpGO, nextTurnDisplayGO;
	public GameObject priorityGO_Corp, priorityGO_Runner;

	private void Awake()
	{
		nextTurnDisplayGO.SetActive(false);
	}

	private void OnEnable()
	{
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
		PlayerNR.Corporation.OnActionPointsChanged += Corporation_OnActionPointsChanged;
		PlayerNR.Runner.OnActionPointsChanged += Runner_OnActionPointsChanged;
		PaidAbilitiesManager.OnPaidWindowChanged += PaidAbilitiesManager_OnPaidWindowChanged;
	}
	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		PlayerNR.Corporation.OnActionPointsChanged -= Corporation_OnActionPointsChanged;
		PlayerNR.Runner.OnActionPointsChanged -= Runner_OnActionPointsChanged;
		PaidAbilitiesManager.OnPaidWindowChanged -= PaidAbilitiesManager_OnPaidWindowChanged;
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		runnerGO.SetActive(isRunner);
		corpGO.SetActive(!isRunner);
	}
	private void Runner_OnActionPointsChanged()
	{
		TryActivateNextTurnDisplay();
	}

	private void Corporation_OnActionPointsChanged()
	{
		TryActivateNextTurnDisplay();
	}
	private void PaidAbilitiesManager_OnPaidWindowChanged(PlayerNR newPriority)
	{
		if (newPriority == null)
		{
			priorityGO_Corp.SetActive(false);
			priorityGO_Runner.SetActive(false);
		}
		else
		{
			priorityGO_Corp.SetActive(!newPriority.IsRunner());
			priorityGO_Runner.SetActive(newPriority.IsRunner());
		}
	}

	void TryActivateNextTurnDisplay()
	{
		nextTurnDisplayGO.SetActive(GameManager.CurrentTurnPlayer.ActionPoints == 0);
	}


	public bool CanHighlight(bool highlight = true)
	{
		if (highlight) return GameManager.CurrentTurnPlayer.ActionPoints == 0;
		return true;
	}

	public bool CanSelect()
	{
		return CanHighlight();
	}

	public void Highlighted()
	{

	}

	public void Selected()
	{
		GameManager.instance.StartNextTurn();
	}
}
