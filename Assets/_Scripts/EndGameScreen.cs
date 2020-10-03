using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour
{
    public GameObject panelGO;
	public GameObject runnerWinTextGO, corpWinTextGO;

	private void Start()
	{
		panelGO.SetActive(false);
	}


	private void OnEnable()
	{
		GameManager.OnGameEnded += GameManager_OnGameEnded;
	}
	private void OnDisable()
	{
		GameManager.OnGameEnded -= GameManager_OnGameEnded;
	}

	private void GameManager_OnGameEnded(bool isRunner)
	{
		Activate();
		runnerWinTextGO.SetActive(isRunner);
		corpWinTextGO.SetActive(!isRunner);
	}

	public void Activate(bool activate = true)
	{
        CardChooser.instance.ActivateFocus();
        panelGO.SetActive(activate);
    }


	public void Button_Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}





}
