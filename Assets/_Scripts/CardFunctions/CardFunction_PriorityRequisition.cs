using Boo.Lang;

public class CardFunction_PriorityRequisition : CardFunction
{
	protected override bool CanExecuteConditionalAbility(int abilityIndex)
	{
		return card.isInstalled && card.isRezzed && card.cardAdvancer.isScored;
	}

	protected override void ExecuteConditionalAbility(int abilityIndex)
	{
		if (abilityIndex == 1) RezIce();

		ConditionalAbilityResolved();
	}

	void RezIce()
	{
		print("ICE REZZED!!!");

		CardChooser.instance.ActivateFocus(null);

		Card_Ice[] iceCards = FindObjectsOfType<Card_Ice>();
		List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var ice in iceCards)
		{
			if (ice.isInstalled && !ice.isRezzed)
			{
				selectors.Add(ice.selector);
			}
		}

		if (selectors.Count > 0) ActionOptions.instance.ActivateYesNo(RezChoice, "Rez Ice?", 0);


		void RezChoice(bool yes)
		{
			if (yes)
			{
				print("YESES");

				CardChooser.instance.ActivateFocus(IceChosen, selectors.ToArray());

				void IceChosen(SelectorNR selector)
				{
					selector.GetComponentInParent<Card>().Rez();
				}
			}
			else
			{
				CardChooser.instance.DeactivateFocus();
			}
		}

		//PlayerNR.Corporation.AddCredits(numCredits);
	}




}
