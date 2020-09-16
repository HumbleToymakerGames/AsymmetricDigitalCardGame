using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardFunction_Desperado : CardFunction
{
	public int amountMemory;
	public int numCredits;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


	protected override void SubscribeToConditions()
	{
		base.SubscribeToConditions();
		// Make Successful Run
		//GainCredits();
	}

	protected override void UnSubscribeToConditions()
	{
		base.UnSubscribeToConditions();
	}

	protected override void ExecuteConditionalAbility(int abilityIndex)
	{
		if (abilityIndex == 1) AddMemory();
		else if (abilityIndex == 2) GainCredits();

		ConditionalAbilityResolved();
	}

	protected override bool CanExecuteConditionalAbility(int abilityIndex)
	{
		return card.isInstalled;
	}

	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		if (card == this.card)
		{
			AddMemory();
		}
	}



	void AddMemory()
	{
		PlayerNR.Runner.AddTotalMemory(amountMemory);
	}

	void GainCredits()
	{
		PlayerNR.Runner.AddCredits(numCredits);
	}

}
