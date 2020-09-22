using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Precognition : CardFunction
{
    public int numCards;

    public override void ActivateFunction()
    {
        base.ActivateFunction();
        LookAtDeck();
    }

    void LookAtDeck()
    {
        PlayArea.instance.DeckNR(PlayerNR.Corporation).RevealCards(numCards);
    }
}
