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
		RunOperator.OnCardAccessed += Card_OnCardAccessed;
		PlayCardManager.OnCardExposed += PlayCardManager_OnCardExposed;
		PlayCardManager.OnCardExposed_Pre += PlayCardManager_OnCardExposed_Pre;
		RunOperator.OnIceEncountered += RunOperator_OnIceEncountered;
		PlayCardManager.OnTagGiven_Pre += PlayCardManager_OnTagGiven_Pre;
		PlayCardManager.OnDamageDone_Pre += PlayCardManager_OnDamageDone_Pre;
		RunOperator.OnIceEncountered_End += RunOperator_OnIceEncountered_End;
	}
	private void OnDisable()
	{
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
		RunOperator.OnRunEnded -= RunOperator_OnRunEnded;
		PlayCardManager.OnCardScored -= PlayCardManager_OnCardScored;
		PlayCardManager.OnCardStolen -= PlayCardManager_OnCardStolen;
		RunOperator.OnCardAccessed -= Card_OnCardAccessed;
		PlayCardManager.OnCardExposed -= PlayCardManager_OnCardExposed;
		PlayCardManager.OnCardExposed_Pre -= PlayCardManager_OnCardExposed_Pre;
		RunOperator.OnIceEncountered -= RunOperator_OnIceEncountered;
		PlayCardManager.OnTagGiven_Pre -= PlayCardManager_OnTagGiven_Pre;
		PlayCardManager.OnDamageDone_Pre -= PlayCardManager_OnDamageDone_Pre;
		RunOperator.OnIceEncountered_End -= RunOperator_OnIceEncountered_End;
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
	private void Card_OnCardAccessed(Card card, ServerColumn.ServerType serverType)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Card_Accessed);
	}
	private void PlayCardManager_OnCardExposed(Card card)
	{

	}
	private void PlayCardManager_OnCardExposed_Pre(Card card)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Card_Exposed_Pre);
	}
	private void RunOperator_OnIceEncountered(Card_Ice iceCard)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Ice_Encountered);
	}
	private void PlayCardManager_OnTagGiven_Pre(TagWrapper tagWrapper)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Tag_Received_Pre);
	}
	private void PlayCardManager_OnDamageDone_Pre(DamageWrapper damageWrapper)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Runner_Damage_Pre);
	}
	private void RunOperator_OnIceEncountered_End(Card_Ice iceCard)
	{
		StartResolvingConditionals(ConditionalAbility.Condition.Ice_Encountered_End);
	}


	class ConditionalsPackage : MonoBehaviour
	{
		public Coroutine conditionalsManagerRoutine;
		public ConditionalAbility.Condition conditionResolving;

		public ConditionalsPackage(ConditionalAbility.Condition _conditionResolving)
		{
			conditionResolving = _conditionResolving;
			conditionalsManagerRoutine = instance.StartCoroutine(ResolvingConditionals());
		}

		IEnumerator ResolvingConditionals()
		{
			yield return instance.StartConditionalManagerRoutine(conditionResolving);
			conditionalsManagerRoutine = null;
		}
	}

	static List<ConditionalsPackage> conditionalsPackages = new List<ConditionalsPackage>();

	void Push(ConditionalsPackage conditionalsPackage) { conditionalsPackages.Add(conditionalsPackage); }
	ConditionalsPackage Pop()
	{
		if (conditionalsPackages.Count > 0)
		{
			ConditionalsPackage package = conditionalsPackages[conditionalsPackages.Count - 1];
			conditionalsPackages.RemoveAt(conditionalsPackages.Count - 1);
			return package;
		}
		return null; }

	static bool HasPackageWithCondition(ConditionalAbility.Condition condition)
	{
		foreach (var package in conditionalsPackages)
		{
			if (package.conditionResolving == condition) return true;
		}
		return false;
	}

	static bool IsResolvingCondition(ConditionalAbility.Condition condition)
	{
		int? conditionIndex = null;
		for (int i = 0; i < conditionalsPackages.Count; i++)
		{
			if (conditionalsPackages[i].conditionResolving == condition) conditionIndex = i;
		}
		return conditionIndex.HasValue && conditionalsPackages.Count >= conditionIndex;
	}

	void StartResolvingConditionals(ConditionalAbility.Condition condition)
	{
		if (conditionalsPackages.Count == 0) PaidAbilitiesManager.instance.PausePaidWindow();
		ConditionalsPackage conditionalsPackage = new ConditionalsPackage(condition);
		Push(conditionalsPackage);

	}

	Coroutine StartConditionalManagerRoutine(ConditionalAbility.Condition condition)
	{
		return StartCoroutine(ConditionalManagerRoutine(condition));
	}

	IEnumerator ConditionalManagerRoutine(ConditionalAbility.Condition condition)
	{
		yield return new WaitForSeconds(0.25f);

		List<ConditionalAbility> conditionalAbilities = new List<ConditionalAbility>();
		for (int i = 0; i < instance.allConditionalAbilities.Count; i++)
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

		yield return ConditionalsRoutine(conditionalAbilities_CurrentTurn);

		Pop();
		if (conditionalsPackages.Count == 0)
		{
			PaidAbilitiesManager.instance.ResumePaidWindow();
		}

	}

    IEnumerator ConditionalsRoutine(List<ConditionalAbility> conditionals)
	{
		foreach (var ability in conditionals)
		{
			print("ConditionalAbilityResolving..." + ability.conditions[0].ToString());

			bool waitForConditionResolved = true;
			ability.ExecuteAbility(ConditionResolved);
			while (waitForConditionResolved) yield return null;

			void ConditionResolved()
			{
				waitForConditionResolved = false;
				Debug.Log("ConditionalAbilityResolved - " + ability.conditions[0].ToString(), ability);
			}

			yield return new WaitForSeconds(0.1f);
		}
		print("ConditionalsRoutine Over!");
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

	public static bool IsResolvingConditionals(ConditionalAbility.Condition? _condition = null)
	{
		return _condition.HasValue ? IsResolvingCondition(_condition.Value) : conditionalsPackages.Count > 0;
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





