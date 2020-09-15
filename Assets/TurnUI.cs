using UnityEngine;

public class TurnUI : MonoBehaviour
{
    public GameObject runnerGO, corpGO;

	private void OnEnable()
	{
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
	}
	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		runnerGO.SetActive(isRunner);
		corpGO.SetActive(!isRunner);
	}

}
