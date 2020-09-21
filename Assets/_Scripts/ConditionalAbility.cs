using System.Collections;
using System.Linq;
using UnityEngine;

public class ConditionalAbility : MonoBehaviour
{
    public enum Condition { Turn_Begins, Card_Installed, Run_Ends, Agenda_Scored, Agenda_Stolen };
    public Condition[] conditions;

	public delegate IEnumerator Ability();
	public Ability IAbilityExecution;

	public delegate bool eConditionsMet();
	public eConditionsMet ConditionsMet;

	[HideInInspector]
	public Card card;

	private void Awake()
	{
		card = GetComponent<Card>();
	}

	private void OnEnable()
	{
		if (!card.isViewCard) AddToConditionsList();
	}

	private void OnDisable()
	{
		if (!card.isViewCard) RemoveFromConditionsList();
	}

	public void ExecuteAbility()
	{
		StartCoroutine(AbilityExecutionRoutine());
	}

	IEnumerator AbilityExecutionRoutine()
	{
		yield return IAbilityExecution();
		ConditionResolved();
	}

	void ConditionResolved()
	{
		ConditionalAbilitiesManager.instance.ConditionalAbilityResolved();
		Debug.Log("ConditionResolved - " + name);
	}

	void AddToConditionsList()
	{
		ConditionalAbilitiesManager.instance.AddConditional(this);
	}

	void RemoveFromConditionsList()
	{
		ConditionalAbilitiesManager.instance.RemoveConditional(this);
	}

	public void SetExecutionAndCondition(Ability _ability, eConditionsMet _conditionsMet)
	{
		IAbilityExecution = _ability; ConditionsMet = _conditionsMet;
	}


	public bool AreConditionsMet()
	{
		return ConditionsMet();
	}

	public bool HasCondition(Condition condition)
	{
		return conditions.Contains(condition);
	}



}
