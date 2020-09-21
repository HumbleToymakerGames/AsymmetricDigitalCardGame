using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ConditionalAbilitiesManager : MonoBehaviour
{
	public static ConditionalAbilitiesManager instance;
    public List<ConditionalAbility> allConditionalAbilities = new List<ConditionalAbility>();

	public delegate bool ConditionalCriteriaMet(ConditionalAbility.Condition conditionProcessed, Card card);

	private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
		PlayCardManager.OnCardScored += PlayCardManager_OnCardScored;
		PlayCardManager.OnCardStolen += PlayCardManager_OnCardStolen;
	}
	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		PlayCardManager.OnCardScored -= PlayCardManager_OnCardScored;
		PlayCardManager.OnCardStolen -= PlayCardManager_OnCardStolen;
	}

	private void RunOperator_OnRunEnded(bool success, ServerColumn.ServerType serverType)
	{
		if (success) StartResolvingConditionals(ConditionalAbility.Condition.Run_Ends);
	}
	private void GameManager_OnTurnChanged(bool isRunner)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Turn_Begins);
	}
	private void PlayCardManager_OnCardScored(Card card)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Agenda_Scored);
	}
	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Card_Installed);
	}
	private void PlayCardManager_OnCardStolen(Card card)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Agenda_Stolen);
	}





	void StartResolvingConditionals(ConditionalAbility.Condition condition)
	{
		if (resolvingRoutine != null)
		{
			StopCoroutine(resolvingRoutine);
			print("resolvingRoutine stopped");
		}
		resolvingRoutine = StartCoroutine(ResolvingConditionalsRoutine(condition));
	}


	Coroutine resolvingRoutine;
	IEnumerator ResolvingConditionalsRoutine(ConditionalAbility.Condition condition)
	{
		yield return new WaitForSeconds(0.25f);

		List<ConditionalAbility> conditionalAbilities = new List<ConditionalAbility>();
		for (int i = 0; i < allConditionalAbilities.Count; i++)
		{
			ConditionalAbility currentAbility = allConditionalAbilities[i];
			if (currentAbility.HasCondition(condition) && currentAbility.AreConditionsMet())
			{
				conditionalAbilities.Add(currentAbility);

			}
		}

		// Resolve in order of priority
		List<ConditionalAbility> conditionalAbilities_CurrentTurn = new List<ConditionalAbility>(conditionalAbilities);
		foreach (var item in conditionalAbilities)
		{
			if (item.card.myPlayer == GameManager.CurrentTurnPlayer)
			{
				conditionalAbilities_CurrentTurn.Remove(item);
				conditionalAbilities_CurrentTurn.Insert(0, item);
			}
		}


		StartCoroutine(ConditionalsRoutine(conditionalAbilities_CurrentTurn));

	}

	public void ConditionalAbilityResolved()
	{
		waitForConditionResolved = false;
	}

	bool waitForConditionResolved;
	IEnumerator currentConditionalsRoutine;
    IEnumerator ConditionalsRoutine(List<ConditionalAbility> conditionals)
	{
		foreach (var ability in conditionals)
		{
			waitForConditionResolved = true;
			ability.ExecuteAbility();
			while (waitForConditionResolved) yield return null;
		}
	}


    public void AddConditional(ConditionalAbility conditionalAbility)
	{
		TryAddToList(conditionalAbility);
	}

	public void RemoveConditional(ConditionalAbility conditionalAbility)
	{
		RemoveFromList(conditionalAbility);
	}







	void TryAddToList(ConditionalAbility conditionalAbility)
	{
        if (!allConditionalAbilities.Contains(conditionalAbility)) allConditionalAbilities.Add(conditionalAbility);
	}

	void RemoveFromList(ConditionalAbility conditionalAbility)
	{
		allConditionalAbilities.Remove(conditionalAbility);
	}




	#region Conditional Criterias

	//bool RunEnded_Successful(ConditionalAbility.Condition conditionProcessed, Card card)
	//{
	//	return (conditionProcessed == ConditionalAbility.Condition.Run_Successful);
	//}

	//bool TurnBegins(ConditionalAbility.Condition conditionProcessed, Card card)
	//{
	//	return (conditionProcessed == ConditionalAbility.Condition.Turn_Begins)
	//		&& card.myPlayer == GameManager.CurrentTurnPlayer;
	//}
	//bool CardScoredThis(ConditionalAbility.Condition conditionProcessed, Card card)
	//{
	//	return (conditionProcessed == ConditionalAbility.Condition.Card_Scored_This)
	//		&& card == lastCardScored;
	//}


	#endregion













}





