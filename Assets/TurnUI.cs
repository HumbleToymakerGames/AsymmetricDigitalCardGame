using UnityEngine;

public class TurnUI : MonoBehaviour, ISelectableNR
{
	public GameObject runnerGO, corpGO, nextTurnDisplayGO;

	private void Awake()
	{
		nextTurnDisplayGO.SetActive(false);
	}

	private void OnEnable()
	{
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
		PlayerNR.Corporation.OnActionPointsChanged += Corporation_OnActionPointsChanged;
		PlayerNR.Runner.OnActionPointsChanged += Runner_OnActionPointsChanged;
	}

	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		PlayerNR.Corporation.OnActionPointsChanged -= Corporation_OnActionPointsChanged;
		PlayerNR.Runner.OnActionPointsChanged -= Runner_OnActionPointsChanged;
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
