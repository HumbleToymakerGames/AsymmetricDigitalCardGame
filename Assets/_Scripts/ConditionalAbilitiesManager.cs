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
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
		PlayCardManager.OnCardScored += PlayCardManager_OnCardScored;
	}

	

	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		PlayCardManager.OnCardScored -= PlayCardManager_OnCardScored;
	}

	private void RunOperator_OnRunEnded(bool success)
	{
		if (success) StartResolvingConditionals(RunEnded_Successful);
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		StartResolvingConditionals(TurnBegins);
	}

	Card lastCardScored;
	private void PlayCardManager_OnCardScored(Card card)
	{
		lastCardScored = card;
		StartResolvingConditionals(CardScoredThis);
	}

	
	void StartResolvingConditionals(ConditionalCriteriaMet conditionalCriteriaMet)
	{
		List<ConditionalAbility> conditionalAbilities = new List<ConditionalAbility>();
		for (int i = 0; i < allConditionalAbilities.Count; i++)
		{
			ConditionalAbility currentAbility = allConditionalAbilities[i];
			if (conditionalCriteriaMet(currentAbility.condition, currentAbility.card)) conditionalAbilities.Add(currentAbility);
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

		currentConditionalsRoutine = conditionalAbilities_CurrentTurn.GetEnumerator();

		TryResolveNextAbility();
	}

	public void ConditionalAbilityResolved()
	{
		TryResolveNextAbility();
	}

	void TryResolveNextAbility()
	{
		if (currentConditionalsRoutine.MoveNext())
		{
			ConditionalAbility conditionalAbility = currentConditionalsRoutine.Current as ConditionalAbility;
			//print("player: " + conditionalAbility.card.myPlayer);
			conditionalAbility.ConditionsMet();
		}
		else
		{
			lastCardScored = null;
		}
	}


	IEnumerator currentConditionalsRoutine;
    IEnumerator ConditionalsRoutine(List<ConditionalAbility> conditionals)
	{
        List<ConditionalAbility> _conditionals = new List<ConditionalAbility>(conditionals);
		for (int i = 0; i < _conditionals.Count; i++)
		{
			print("Resolving conditional - " + i);
            _conditionals[i].ConditionsMet();
            yield return null;
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

	bool RunEnded_Successful(ConditionalAbility.Condition conditionProcessed, Card card)
	{
		return (conditionProcessed == ConditionalAbility.Condition.Run_Successful);
	}

	bool TurnBegins(ConditionalAbility.Condition conditionProcessed, Card card)
	{
		return (conditionProcessed == ConditionalAbility.Condition.Turn_Begins)
			&& card.myPlayer == GameManager.CurrentTurnPlayer;
	}
	bool CardScoredThis(ConditionalAbility.Condition conditionProcessed, Card card)
	{
		return (conditionProcessed == ConditionalAbility.Condition.Card_Scored_This)
			&& card == lastCardScored;
	}


	#endregion













}





