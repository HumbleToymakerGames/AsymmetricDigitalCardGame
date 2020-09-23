﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Infiltration : CardFunction
{
    public int numCredits;

    public override void ActivateInstantFunction()
    {
        base.ActivateInstantFunction();
        GainCreditsOrExpose();
    }

    void GainCreditsOrExpose()
	{
        CardChooser.instance.ActivateFocus(null);
        ActionOptions.instance.ActivateYesNo(YesNoChoice, "Gain 2 credits?");
	}

    void YesNoChoice(bool yes)
	{
        CardChooser.instance.DeactivateFocus();
        if (yes) GainCredits();
        else ExposeCard();
	}

    void GainCredits()
    {
        card.myPlayer.AddCredits(numCredits);
    }

    void ExposeCard()
	{
        List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var card in ServerSpace.instance.transform.GetComponentsInChildren<Card>())
		{
            if (card.isInstalled && !card.isRezzed)
			{
                selectors.Add(card.selector);
			}
		}

        if (selectors.Count > 0)
            CardChooser.instance.ActivateFocus(CardChosen, 1, selectors.ToArray());
    }


    void CardChosen(SelectorNR[] selectors)
	{
        PlayCardManager.instance.ExposeCard(selectors[0].GetComponentInParent<Card>());
    }


}
