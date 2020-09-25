using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TraceRunner : MonoBehaviour
{
	public static TraceRunner instance;

	GameObject tracePanel;
	public bool isTracing;

	public TextMeshProUGUI equationText_Corp, equationText_Runner;
	string eqFormat = "{0} + {1} = {2}";

	public TextMeshProUGUI resultsText;

	private void Awake()
	{
		instance = this;
		tracePanel = transform.GetChild(0).gameObject;
		ActivatePanel(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K)) RunTrace(null, 3);
	}

	public Coroutine RunTrace(UnityAction<bool> callback, int baseTraceStrength)
	{
		if (traceRoutine != null) StopCoroutine(traceRoutine);
		traceRoutine = StartCoroutine(TraceRoutine(callback, baseTraceStrength));
		return traceRoutine;
	}

	Coroutine traceRoutine;
	public IEnumerator TraceRoutine(UnityAction<bool> callback, int baseTraceStrength)
	{
		isTracing = true;
		ActivatePanel();

		CardChooser.instance.ActivateFocus(null);
		equationText_Corp.text = equationText_Runner.text = resultsText.text = "";

		int corpBet = 0;
		bool counted = false;
		equationText_Corp.text = string.Format(eqFormat, baseTraceStrength, corpBet, baseTraceStrength + corpBet);
		ActionOptions.instance.ActivateCounter(CorpCounter, "Corp Bet", PlayerNR.Corporation.Credits);
		while (!counted) yield return null;
		equationText_Corp.text = string.Format(eqFormat, baseTraceStrength, corpBet, baseTraceStrength + corpBet);
		PlayerNR.Corporation.Credits -= corpBet;

		void CorpCounter(int bet)
		{
			corpBet = bet;
			counted = true;
		}

		int runnerBaseStrength = PlayerNR.Runner.LinkStrength;
		int runnerBet = 0;
		counted = false;
		equationText_Runner.text = string.Format(eqFormat, runnerBaseStrength, runnerBet, runnerBaseStrength + runnerBet);
		ActionOptions.instance.ActivateCounter(RunnerCounter, "Runner Bet", PlayerNR.Runner.Credits);
		while (!counted) yield return null;
		equationText_Runner.text = string.Format(eqFormat, runnerBaseStrength, runnerBet, runnerBaseStrength + runnerBet);
		PlayerNR.Runner.Credits -= runnerBet;

		ActionOptions.instance.HideAllOptions();

		void RunnerCounter(int bet)
		{
			runnerBet = bet;
			counted = true;
		}

		int corpTrace = baseTraceStrength + corpBet;
		int runnerTrace = runnerBaseStrength + runnerBet;

		bool successfulTrace = corpTrace > runnerTrace;

		string prefix = successfulTrace ? "" : " Not";
		resultsText.text = string.Format("\"Trace:{0} Successful\"", prefix);

		yield return new WaitForSeconds(2);

		callback?.Invoke(successfulTrace);

		CardChooser.instance.DeactivateFocus();
		isTracing = false;
		ActivatePanel(false);
	}

	public void ActivatePanel(bool activate = true)
	{
		tracePanel.SetActive(activate);
	}








}
