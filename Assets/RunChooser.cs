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
	public GameObject myGO;

	private void Awake()
	{
		myRT = GetComponent<RectTransform>();
		myGO = transform.GetChild(0).gameObject;
	}

	private void Start()
	{
		if (targetServer.IsRemoteServer())
			myGO.SetActive(false);
	}

	private void OnEnable()
	{
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
		RunOperator.OnRunBeingMade += RunOperator_OnRunBeingMade;
	}
	private void OnDisable()
	{
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
		RunOperator.OnRunBeingMade -= RunOperator_OnRunBeingMade;
	}

	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (targetServer.IsRemoteServer())
			myGO.SetActive(targetServer.HasAnyCardsInServer());
	}
	private void RunOperator_OnRunBeingMade(ServerColumn server)
	{
		if (server == targetServer) RunningThisServer();
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

	[ContextMenu("Make Run")]
	public void Selected()
	{
		print("Runner selected");
		PlayCardManager.instance.TryMakeRun(targetServer);
	}

	void RunningThisServer()
	{
		RunOperator.OnApproached += RunOperator_OnCardBeingApproached;
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
		numIceInColumn = targetServer.iceInColumn.Count;
	}


	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		RunOperator.OnApproached -= RunOperator_OnCardBeingApproached;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		if (success) StartCoroutine(SetArrowToCardSpot(numIceInColumn));
		StartCoroutine(ResetArrow(success));
	}

	private void RunOperator_OnCardBeingApproached(int encounterIndex)
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
