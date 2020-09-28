using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public static PlayArea instance;

    [Header("Corporation Spots")]
    public IdentitySpot corpIdentity;
    public Deck corpDeck;
    public Hand corpHand;

    public ActionTracker corpActionTracker;
    public ActionsReferenceCard corpActionsReferenceCard;
    public DiscardPile corpDiscardPile;
    public ScoringArea corpScoringArea;

    [Header("Runner Spots")]
    public IdentitySpot runnerIdentity;
    public Deck runnerDeck;
    public Hand runnerHand;
    public ActionTracker runnerActionTracker;
    public ActionsReferenceCard runnerActionsReferenceCard;
    public DiscardPile runnerDiscardPile;
    public ScoringArea runnerScoringArea;


    private void Awake()
    {
        instance = this;
        GetAllReferences();
    }
    
    public void SetCardsToSpots(PlayerNR player)
    {
        IdentitySpot identity = IdentityNR(player);
        identity.SetIdentityCard(player.identity);
        DeckNR(player).SetCardsToDeck(player.deckCards);
    }

    public Card DrawCardFromDeck(PlayerNR player)
    {
        Deck deck = DeckNR(player);
        Card drawnCard = null;
        if (deck.DrawNextCard(ref drawnCard))
        {
            return drawnCard;
        }
        return null;
    }

    public int CostOfAction(PlayerNR player, int actionIndex)
    {
        return ActionRefCardNR(player).CostOfAction(actionIndex);
        //return -123;
    }


	#region

    public IdentitySpot IdentityNR(PlayerNR player)
	{
        return player.IsRunner() ? runnerIdentity : corpIdentity;
    }

    public Deck DeckNR(PlayerNR player)
	{
        return player.IsRunner() ? runnerDeck : corpDeck;
    }

    public Hand HandNR(PlayerNR player)
	{
        return player.IsRunner() ? runnerHand : corpHand;
    }


    public DiscardPile DiscardNR(PlayerNR player)
    {
        return player.IsRunner() ? runnerDiscardPile : corpDiscardPile;
    }

    public ScoringArea ScoringAreaNR(PlayerNR player)
	{
        return player.IsRunner() ? runnerScoringArea : corpScoringArea;
    }

    public ActionsReferenceCard ActionRefCardNR(PlayerNR player)
	{
        return player.IsRunner() ? runnerActionsReferenceCard : corpActionsReferenceCard;
	}


    #endregion


    void GetAllReferences()
    {
		foreach (var spot in GetComponentsInChildren<PlayArea_Spot>())
		{
            if (spot is IdentitySpot)
            {
                IdentitySpot identity = spot as IdentitySpot;
                if (spot.playerSide == PlayerSide.Runner) runnerIdentity = identity;
                else corpIdentity = identity;
            }
            else if (spot is Deck)
			{
                Deck deck = spot as Deck;
                if (spot.playerSide == PlayerSide.Runner) runnerDeck = deck;
                else corpDeck = deck;
			}
            else if (spot is Hand)
			{
                Hand hand = spot as Hand;
                if (spot.playerSide == PlayerSide.Runner) runnerHand = hand;
                else corpHand = hand;
            }
            else if (spot is ActionTracker)
            {
                ActionTracker tracker = spot as ActionTracker;
                if (spot.playerSide == PlayerSide.Runner) runnerActionTracker = tracker;
                else corpActionTracker = tracker;
            }
            else if (spot is ActionsReferenceCard)
            {
                ActionsReferenceCard referenceCard = spot as ActionsReferenceCard;
                if (spot.playerSide == PlayerSide.Runner) runnerActionsReferenceCard = referenceCard;
                else corpActionsReferenceCard = referenceCard;
            }
            else if (spot is DiscardPile)
            {
                DiscardPile discardPile = spot as DiscardPile;
                if (spot.playerSide == PlayerSide.Runner) runnerDiscardPile = discardPile;
                else corpDiscardPile = discardPile;
            }
            else if (spot is ScoringArea)
            {
                ScoringArea scoringArea = spot as ScoringArea;
                if (spot.playerSide == PlayerSide.Runner) runnerScoringArea = scoringArea;
                else corpScoringArea = scoringArea;
            }
        }
    }



}
