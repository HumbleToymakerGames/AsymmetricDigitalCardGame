using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ConditionalAbility : MonoBehaviour
{
    public enum Condition {
		Turn_Begins = 0,
		Card_Installed = 1,
		Run_Ends = 2,
		Agenda_Scored = 3,
		Agenda_Stolen = 4,
		Card_Accessed = 5,
		Card_Exposed = 6,
		Card_Exposed_Pre = 7,
		Ice_Encountered = 8,
		Ice_Encountered_End = 11,
		Tag_Received_Pre = 9,
		Runner_Damage_Pre = 10,

	
	};
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

	UnityAction currentCallback;
	public void ExecuteAbility(UnityAction callback)
	{
		currentCallback = callback;
		StartCoroutine(AbilityExecutionRoutine());
	}

	IEnumerator AbilityExecutionRoutine()
	{
		yield return IAbilityExecution();
		ConditionResolved();
	}

	void ConditionResolved()
	{
		currentCallback?.Invoke();
		currentCallback = null;
		//ConditionalAbilitiesManager.instance.ConditionalAbilityResolved(this);
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
		if (ConditionsMet == null) Debug.Log("NOPE", this);
		return ConditionsMet();
	}

	public bool HasCondition(Condition condition)
	{
		return conditions.Contains(condition);
	}



}
