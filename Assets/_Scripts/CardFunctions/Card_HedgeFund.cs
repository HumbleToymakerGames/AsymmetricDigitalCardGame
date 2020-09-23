using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_HedgeFund : CardFunction
{
    public int numCredits;

    public override void ActivateInstantFunction()
    {
        base.ActivateInstantFunction();
        GainCredits();
    }

    void GainCredits()
    {
        card.myPlayer.AddCredits(numCredits);
    }
}
