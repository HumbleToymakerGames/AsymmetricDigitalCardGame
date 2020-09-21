using System.Collections;
using System.Collections.Generic;

public class CardFunction_PriorityRequisition : CardFunction
{
	public bool cardScored;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayCardManager.OnCardScored += PlayCardManager_OnCardScored;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		PlayCardManager.OnCardScored -= PlayCardManager_OnCardScored;
	}

	private void PlayCardManager_OnCardScored(Card card)
	{
		cardScored = card == this.card;
	}

	protected override void AssignConditionalAbilities()
	{
		for (int i = 0; i < conditionalAbilities.Length; i++)
		{
			ConditionalAbility ability = conditionalAbilities[i];
			if (i == 0) ability.SetExecutionAndCondition(RezIce, CanRezIce);
		}
	}

	bool CanRezIce()
	{
		return cardScored;
	}

	IEnumerator RezIce()
	{
		Card_Ice[] iceCards = FindObjectsOfType<Card_Ice>();
		List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var ice in iceCards)
		{
			if (ice.isInstalled && !ice.isRezzed)
			{
				selectors.Add(ice.selector);
			}
		}

		ActionOptions.instance.HideAllOptions();
		if (selectors.Count > 0)
		{
			CardChooser.instance.ActivateFocus(null);
			bool madeChoice = false;
			ActionOptions.instance.ActivateYesNo(RezChoice, "Rez Ice?", 0);
			while (!madeChoice) yield return null;

			void RezChoice(bool yes)
			{
				madeChoice = true;
				if (yes)
				{
					CardChooser.instance.ActivateFocus(IceChosen, 1, selectors.ToArray());
					ActionOptions.instance.ActivateActionMessage("Select Ice to Rez");

					void IceChosen(SelectorNR[] selectorsChosen)
					{
						selectorsChosen[0].GetComponentInParent<Card>().Rez();
					}
				}
				else
				{
					CardChooser.instance.DeactivateFocus();
				}
			}
		}
	}




}
