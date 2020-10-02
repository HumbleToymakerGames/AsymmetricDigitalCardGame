using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_CrashSpace : CardFunction
{
	public int numRecurringCredits;
	public int numDamageToPrevent;

	DamageWrapper damageWrapperCached;

	protected override void OnEnable()
	{
		base.OnEnable();
		GameManager.OnTurnChanged += GameManager_OnTurnChanged;
		PlayCardManager.OnDamageDone_Pre += PlayCardManager_OnDamageDone_Pre;
	}
	protected override void OnDisable()
	{
		base.OnEnable();
		GameManager.OnTurnChanged -= GameManager_OnTurnChanged;
		PlayCardManager.OnDamageDone_Pre -= PlayCardManager_OnDamageDone_Pre;
	}

	private void GameManager_OnTurnChanged(bool isRunner)
	{
		if (isRunner)
		{
			card.NeutralCounters = numRecurringCredits;
		}
	}
	private void PlayCardManager_OnDamageDone_Pre(DamageWrapper damageWrapper)
	{
		damageWrapperCached = damageWrapper;
	}

	protected override void AssignPaidAbilities()
	{
		for (int i = 0; i < paidAbilities.Length; i++)
		{
			PaidAbility ability = paidAbilities[i];
			if (i == 0) ability.SetAbilityAndCondition(RemoveTag, CanBeClickable);
		}
	}

	bool CanBeClickable()
	{
		return card.isInstalled &&
			PlayCardManager.instance.CanAffordAction(PlayerNR.Runner, PlayCardManager.RUNNER_REMOVE_TAG) &&
			card.NeutralCounters >= PlayCardManager.COST_REMOVE_TAG &&
			PlayerNR.Runner.Tags > 0;
	}

	IEnumerator RemoveTag()
	{
		PlayCardManager.instance.Action_RemoveTag();
		card.NeutralCounters -= PlayCardManager.COST_REMOVE_TAG;
		yield break;
	}



	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(PreventMeatDamage, CanPreventDamage);
		}
	}

	bool CanPreventDamage()
	{
		return card.isInstalled;
	}

	IEnumerator PreventMeatDamage()
	{
		if (damageWrapperCached.damageType == DamageType.Net)
		{
			bool? chosen = null;
			ActionOptions.instance.ActivateYesNo(Choice, string.Format("Prevent Damage: {0}?", card.cardTitle));
			while (!chosen.HasValue) yield return null;

			if (chosen.Value)
			{
				damageWrapperCached.SubtractDamage(numDamageToPrevent);
				PlayCardManager.instance.TrashCard(PlayerNR.Runner, card);
			}

			void Choice(bool yes)
			{
				chosen = yes;
			}

		}

		damageWrapperCached = null;
		yield break;
	}


}
