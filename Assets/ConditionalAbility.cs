﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CardFunction))]
public class ConditionalAbility : MonoBehaviour
{
    public enum Condition { Card_Installed, Run_Ends, Run_Successful, Run_Failure };
    public Condition condition;
	CardFunction cardFunction;
	[HideInInspector]
	public Card card;
	public int myAbilityIndex;

	private void Awake()
	{
		cardFunction = GetComponent<CardFunction>();
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



	public void ConditionsMet()
	{
		cardFunction.TryExecuteConditionalAbility(myAbilityIndex);
	}

	void ConditionResolved()
	{

	}






	void AddToConditionsList()
	{
		ConditionalAbilitiesManager.instance.AddConditional(this);
	}

	void RemoveFromConditionsList()
	{
		ConditionalAbilitiesManager.instance.RemoveConditional(this);
	}





}