public class CardFunction_PadCampaign : CardFunction
{
	public int numCredits;

	protected override bool CanExecuteConditionalAbility(int abilityIndex)
	{
		return card.isInstalled && card.isRezzed;
	}

	protected override void ExecuteConditionalAbility(int abilityIndex)
	{
		if (abilityIndex == 1) GainCredits();

		ConditionalAbilityResolved();
	}

	void GainCredits()
	{
		PlayerNR.Corporation.AddCredits(numCredits);
	}



}
