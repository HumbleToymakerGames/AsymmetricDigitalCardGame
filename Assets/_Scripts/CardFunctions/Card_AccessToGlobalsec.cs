using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_AccessToGlobalsec : CardFunction
{
	public int amountLinks;
	bool cardInstalled;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
	}


	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(AddLink, CanAddLink);
		}
	}

	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (card == this.card && installed)
		{
			cardInstalled = true;
		}
	}

	bool CanAddLink()
	{
		return card.isInstalled && cardInstalled;
	}

	IEnumerator AddLink()
	{
		PlayerNR.Runner.LinkStrength += amountLinks;
		cardInstalled = false;
		yield break;
	}

}
