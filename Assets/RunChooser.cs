using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RunChooser : MonoBehaviour, ISelectableNR
{
	RectTransform myRT;
	public ServerColumn targetServer;
	public RectTransform arrowRT;
	int numIceInColumn;
	float transitionTime = 1f;

	private void Awake()
	{
		myRT = GetComponent<RectTransform>();
	}

	public bool CanHighlight(bool highlight = true)
	{
		if (highlight) return !RunOperator.instance.isRunning && GameManager.CurrentTurnPlayer.IsRunner();
		return true;
	}

	public bool CanSelect()
	{
		return CanHighlight() && PlayCardManager.instance.CanMakeRun();
	}

	public void Highlighted()
	{
	}

	public void Selected()
	{
		RunOperator.OnIceBeingEncountered += RunOperator_OnIceBeingEncountered;
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
		numIceInColumn = targetServer.iceInColumn.Count;
		PlayCardManager.instance.TryMakeRun(targetServer);
	}

	private void RunOperator_OnRunEnded(bool success)
	{
		RunOperator.OnIceBeingEncountered -= RunOperator_OnIceBeingEncountered;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		if (success) StartCoroutine(SetArrowToCardSpot(numIceInColumn));
		StartCoroutine(ResetArrow(success));
	}

	private void RunOperator_OnIceBeingEncountered(Card_Ice iceCard, int encounterIndex)
	{
		StartCoroutine(SetArrowToCardSpot(encounterIndex));
	}

	public IEnumerator SetArrowToCardSpot(int encounterIndex)
	{
		float firstSpot = 350f;
		float spotBuffer = 75f;

		float finalWidth = firstSpot - spotBuffer * (numIceInColumn - encounterIndex);

		Vector2 size = arrowRT.sizeDelta;
		size.x = finalWidth;

		yield return arrowRT.DOSizeDelta(size, transitionTime).WaitForCompletion();

	}

	public IEnumerator ResetArrow(bool success)
	{
		if (success) yield return new WaitForSeconds(2);
		if (success) yield return new WaitForSeconds(2);

		Vector2 size = arrowRT.sizeDelta;
		size.x = 80;
		yield return arrowRT.DOSizeDelta(size, transitionTime).WaitForCompletion();
	}




}
