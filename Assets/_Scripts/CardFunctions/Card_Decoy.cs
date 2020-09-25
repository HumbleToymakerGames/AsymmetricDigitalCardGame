using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Decoy : CardFunction
{
	public int numTagsToAvoid;
	TagWrapper tagWrapperCached;
	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnTagGiven_Pre += PlayCardManager_OnTagGiven_Pre;
	}
	protected override void OnDisable()
	{
		base.OnEnable();
		PlayCardManager.OnTagGiven_Pre += PlayCardManager_OnTagGiven_Pre;
	}
	private void PlayCardManager_OnTagGiven_Pre(TagWrapper tagWrapper)
	{
		tagWrapperCached = tagWrapper;
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(AvoidTag, CanAvoidTag);
		}
	}

	bool CanAvoidTag()
	{
		return card.isInstalled;
	}

	IEnumerator AvoidTag()
	{
		bool? choice = null;
		ActionOptions.instance.ActivateYesNo(Choice, string.Format("Activate {0}?", card.cardTitle));
		while (!choice.HasValue) yield return null;

		if (choice.Value)
		{
			tagWrapperCached.SubtractTags(numTagsToAvoid);
			PlayCardManager.instance.TrashCard(PlayerNR.Runner, card);
		}

		void Choice(bool yes)
		{
			choice = yes;
		}
	}


}
