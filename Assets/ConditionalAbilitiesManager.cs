using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ConditionalAbilitiesManager : MonoBehaviour
{
	public static ConditionalAbilitiesManager instance;
    public List<ConditionalAbility> allConditionalAbilities = new List<ConditionalAbility>();


	private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		RunOperator.OnRunEnded += RunOperator_OnRunEnded;
	}

	private void OnDisable()
	{
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
	}

	private void RunOperator_OnRunEnded(bool success)
	{
		if (success) StartResolvingConditionals(ConditionalAbility.Condition.Run_Successful);
	}

	void StartResolvingConditionals(ConditionalAbility.Condition condition)
	{
		List<ConditionalAbility> conditionalAbilities = new List<ConditionalAbility>();
		for (int i = 0; i < allConditionalAbilities.Count; i++)
		{
			if (allConditionalAbilities[i].condition == condition) conditionalAbilities.Add(allConditionalAbilities[i]);
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
			print("player: " + conditionalAbility.card.myPlayer);
			conditionalAbility.ConditionsMet();
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


}





