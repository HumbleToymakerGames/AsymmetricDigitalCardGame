using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringArea : PlayArea_Spot
{
    public Transform cardsParentT;
    public List<Card_Agenda> cardsInScoringArea = new List<Card_Agenda>();

    public void AddCardToScoringArea(Card_Agenda card)
    {
        card.MoveCardTo(cardsParentT);
        card.Rez();
        cardsInScoringArea.Add(card);
    }

    public override void RemoveCard(Card card)
    {
        base.RemoveCard(card);
        cardsInScoringArea.Remove(card as Card_Agenda);
    }


}
