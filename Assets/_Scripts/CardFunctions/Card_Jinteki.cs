using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Jinteki : CardFunction
{
	public int numNetDamage;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardScored += PlayCardManager_OnCardScored;
		PlayCardManager.OnCardStolen += PlayCardManager_OnCardStolen;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardScored -= PlayCardManager_OnCardScored;
		PlayCardManager.OnCardStolen -= PlayCardManager_OnCardStolen;
	}

	private void PlayCardManager_OnCardScored(Card card)
	{

	}
	private void PlayCardManager_OnCardStolen(Card card)
	{

	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(NetDamage, CanDoNetDamage);
		}
	}


	bool CanDoNetDamage()
	{
		return true;
	}

	IEnumerator NetDamage()
	{
		// Do net damage

		bool damageDone = false;
		PlayCardManager.instance.DoNetDamage(DamageDone, numNetDamage);
		while (!damageDone) yield return null;

		void DamageDone()
		{
			print("net damage done.");
			damageDone = true;
		}
	}







}
