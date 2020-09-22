using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Snare : CardFunction
{
	public int numCredits;
	public ServerColumn.ServerType serverType;
	bool cardAccessed;
	ServerColumn.ServerType? fromServer = null;

	protected override void OnEnable()
	{
		base.OnEnable();
		Card.OnCardAccessed += Card_OnCardAccessed;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		Card.OnCardAccessed += Card_OnCardAccessed;
	}

	private void Card_OnCardAccessed(Card card, ServerColumn.ServerType _serverType)
	{
		cardAccessed = card == this.card;
		fromServer = _serverType;
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(ForceReveal, CanForceReveal);
			if (i == 1) ability.SetExecutionAndCondition(ActivateAbility, CanActivate);
		}
	}

	bool CanForceReveal()
	{
		return !card.isInstalled && fromServer == serverType;
	}

	IEnumerator ForceReveal()
	{
		// Reveal Snare
		print("Revealed Snare!");
		yield break;
	}

	bool CanActivate()
	{
		return cardAccessed && fromServer != ServerColumn.ServerType.Archives;
	}

	IEnumerator ActivateAbility()
	{
		print("Snare!!");

		CardChooser.instance.ActivateFocus(null);
		bool? choiceMade = null;
		ActionOptions.instance.ActivateYesNo(ChoiceMade, string.Format("Activate ability {0}?", card.cardTitle), numCredits);
		while (!choiceMade.HasValue) yield return null;

		if (choiceMade.Value)
		{
			if (PlayCardManager.instance.TryAffordCost(card.myPlayer, numCredits))
			{
				bool damageDone = false;
				PlayCardManager.instance.DoNetDamage(DamageDone, 3);
				while (!damageDone) yield return null;

				PlayerNR.Runner.AddTags(1);

				void DamageDone()
				{
					damageDone = true;
				}

			}
		}

		print("Snare Done!!");

		void ChoiceMade(bool yes)
		{
			choiceMade = yes;
		}

		cardAccessed = false;
		fromServer = null;
		CardChooser.instance.DeactivateFocus();
	}
}
