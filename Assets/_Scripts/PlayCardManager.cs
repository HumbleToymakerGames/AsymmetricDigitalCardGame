using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public class PlayCardManager : MonoBehaviour
{
    public static PlayCardManager instance;

    // Action Point Indexes
    public const int
        RUNNER_DRAW_CARD = 1,
        RUNNER_GAIN_CREDIT = 2,
        RUNNER_INSTALL = 3,
        RUNNER_EVENT = 4,
        RUNNER_REMOVE_TAG = 5,
        RUNNER_MAKE_RUN = 6;
    public const int
        CORP_DRAW_CARD = 1,
        CORP_GAIN_CREDIT = 2,
        CORP_INSTALL = 3,
        CORP_OPERATION = 4,
        CORP_ADVANCE = 5,
        CORP_PURGE_VIRUSES = 6,
        CORP_TRASH_RESOURCE_TAGGED = 7;

    public delegate void eCardInstalled(Card card, bool installed);
    public static event eCardInstalled OnCardInstalled;

    public delegate void eCardEvent(Card card);
    public static event eCardEvent OnCardActivated;
    public static event eCardEvent OnCardTrashed;
    public static event eCardEvent OnCardScored;
    public static event eCardEvent OnCardStolen;
    public static event eCardEvent OnCardExposed_Pre;
    public static event eCardEvent OnCardExposed;

    [Header("Starting Points")]
    public int numActionPointsStart_Runner;
    public int numActionPointsStart_Corp;
    public int numMemoryUnitsStart;
    public int numTagsStart;


    private void Awake()
	{
        instance = this;
    }

    public void DrawCards(PlayerNR player, int numberOfCards)
	{
        Card[] drawnCards = new Card[numberOfCards];
		for (int i = 0; i < numberOfCards; i++)
		{
            Card drawnCard = PlayArea.instance.DrawCardFromDeck(player);
            drawnCards[i] = drawnCard;
            drawnCard.FlipCardOver();
		}

        Hand hand = PlayArea.instance.HandNR(player);
        hand.AddCardsToHand(drawnCards);
	}

    public bool TryDrawNextCard()
	{
        if (CanDrawAnotherCard())
		{
            Action_DrawNextCard(GameManager.CurrentTurnPlayer);
            return true;
		}
        return false;
	}

    public bool CanAffordAction(PlayerNR player, int actionIndex)
	{
        int costOfAction = PlayArea.instance.CostOfAction(player, actionIndex);
        return player.CanAffordAction(costOfAction);
	}

    public bool CanAffordCost(PlayerNR player, int cost)
	{
        return player.CanAffordCost(cost);
	}

    public bool TryAffordCost(PlayerNR player, int cost)
	{
        if (CanAffordCost(player, cost))
		{
            PayCost(player, cost);
            return true;
		}
        return false;
	}

    public bool CanDrawAnotherCard()
	{
        //!PlayArea.instance.IsHandSizeMaxed(GameManager.instance.runner);
        return CanAffordAction(GameManager.CurrentTurnPlayer, RUNNER_DRAW_CARD);
    }


    public bool CanGainCredit()
	{
        return CanAffordAction(GameManager.CurrentTurnPlayer, RUNNER_GAIN_CREDIT);
	}

    public void TryGainCredit()
	{
        if (CanGainCredit())
		{
            Action_GainCredit(GameManager.CurrentTurnPlayer);
        }
	}

    public bool CanInstallCard(IInstallable installableCard)
	{
        int actionIndex = GameManager.CurrentTurnPlayer.IsRunner() ? RUNNER_INSTALL : CORP_INSTALL;
        bool installSpace = GameManager.CurrentTurnPlayer.IsRunner() ? true : ServerSpace.instance.CanInstallCard(installableCard);
        return installSpace && CanAffordAction(GameManager.CurrentTurnPlayer, actionIndex) && installableCard.CanInstall();
	}

    public bool TryInstallCard(IInstallable installableCard)
	{
        if (CanInstallCard(installableCard))
		{
            Card card = (Card)installableCard;
            if (GameManager.CurrentTurnPlayer.IsRunner())
            {
                Action_InstallCard_Runner(installableCard);
                PayCostOfCard(GameManager.CurrentTurnPlayer, card);
                UseMemorySpaceOfCard(card);
            }
            else
			{
                Action_InstallCard_Corp(installableCard);
            }
                return true;
		}
        return false;
	}

    public bool CanMakeRun()
	{
        return CanAffordAction(PlayerNR.Runner, RUNNER_MAKE_RUN);
	}

    public bool TryMakeRun(ServerColumn targetServer)
    {
        if (CanMakeRun())
		{
            Action_MakeRun(targetServer);
            return true;
		}
        return false;
    }

    public bool CanActivateEvent(IActivateable activateableCard)
	{
        return CanAffordAction(GameManager.CurrentTurnPlayer, RUNNER_EVENT) && activateableCard.CanActivate();
	}

    public bool TryActivateEvent(IActivateable activateableCard)
	{
        if (CanActivateEvent(activateableCard))
		{
            Action_ActivateEvent(GameManager.CurrentTurnPlayer, activateableCard);
            Card card = (Card)activateableCard;
            PayCostOfCard(GameManager.CurrentTurnPlayer, card);
            SendCardToDiscard(GameManager.CurrentTurnPlayer, card);
        }
        return true;
	}

    public bool CanRemoveTag()
	{
        return CanAffordAction(PlayerNR.Runner, RUNNER_REMOVE_TAG)
            && CanAffordCost(PlayerNR.Runner, 2)
            && PlayerNR.Runner.Tags > 0;
    }

    public bool TryRemoveTag()
	{
        if (CanRemoveTag())
		{
            Action_RemoveTag();
            PayCost(PlayerNR.Runner, 2);

            return true;
		}
        return false;
	}

    public void TryActivatePaidAbility(PaidAbility paidAbility)
	{
        if (paidAbility.currency == Currency.Credits)
		{
            if (CanAffordCost(PaidAbilitiesManager.PriorityPlayer, paidAbility.payAmount))
			{
                paidAbility.ExecuteAbility();
                PayCost(PaidAbilitiesManager.PriorityPlayer, paidAbility.payAmount);
			}
		}
        else if (paidAbility.currency == Currency.Clicks)
		{
            if (PaidAbilitiesManager.PriorityPlayer.CanAffordAction(paidAbility.payAmount))
			{
                paidAbility.ExecuteAbility();
                PaidAbilitiesManager.PriorityPlayer.ActionPointsUsed(paidAbility.payAmount);
            }
        }
	}

    public void ReversePaidAbility(PaidAbility paidAbility)
	{
        PaidAbilitiesManager.PriorityPlayer.AddCredits(paidAbility.payAmount);
	}

    public bool TryTrashCard(PlayerNR trashingPlayer, Card card)
	{
        if (CanTrashCard(trashingPlayer, card))
		{
            PayCost(trashingPlayer, card.cardTrasher.CostOfTrash());
            PlayerNR trashedPlayer = trashingPlayer.IsRunner() ? PlayerNR.Corporation : PlayerNR.Runner;
            TrashCard(trashedPlayer, card);
            return true;
		}
        return false;
	}

    public bool CanTrashCard(PlayerNR trashingPlayer, Card card)
	{
        return card.CanBeTrashed() && CanAffordCost(trashingPlayer, card.cardTrasher.CostOfTrash());
	}

    public bool TryAdvanceCard(Card card)
	{
        if (CanAdvanceCard(card))
		{
            Action_AdvanceCard(card);
            PayCost(PlayerNR.Corporation, 1);
            return true;
        }
        return false;
	}

    public bool CanAdvanceCard(Card card)
	{
        return card.CanBeAdvanced()
            && CanAffordAction(PlayerNR.Corporation, CORP_ADVANCE)
            && CanAffordCost(PlayerNR.Corporation, 1);
	}

    public bool TryScoreAgenda(PlayerNR scoringPlayer, Card_Agenda card)
	{
        if ((scoringPlayer.IsRunner() && card.IsScoreable()) || (!scoringPlayer.IsRunner() && card.CanBeScored()))
        {
            ScoreAgenda(scoringPlayer, card);
            return true;
        }
        return false;
    }

    #region Actions
    void Action_DrawNextCard(PlayerNR player)
	{
        int costOfAction = PlayArea.instance.CostOfAction(player, player.IsRunner() ? RUNNER_DRAW_CARD : CORP_DRAW_CARD);
        player.ActionPointsUsed(costOfAction);
        DrawNextCard(player);
    }

    void Action_GainCredit(PlayerNR player)
	{
        int costOfAction = PlayArea.instance.CostOfAction(player, player.IsRunner() ? RUNNER_GAIN_CREDIT : CORP_GAIN_CREDIT);
        player.ActionPointsUsed(costOfAction);
        GainCredit(GameManager.CurrentTurnPlayer);
    }

    void Action_InstallCard_Runner(IInstallable installableCard)
	{
        int costOfAction = PlayArea.instance.CostOfAction(PlayerNR.Runner, RUNNER_INSTALL);
        PlayerNR.Runner.ActionPointsUsed(costOfAction);
        InstallCard_Runner(installableCard);
    }

    void Action_InstallCard_Corp(IInstallable installableCard)
    {
        int costOfAction = PlayArea.instance.CostOfAction(PlayerNR.Corporation, CORP_INSTALL);
        PlayerNR.Corporation.ActionPointsUsed(costOfAction);
        InstallCard_Corporation(installableCard);
    }

    void Action_MakeRun(ServerColumn targetServer)
    {
        int costOfAction = PlayArea.instance.CostOfAction(PlayerNR.Runner, RUNNER_MAKE_RUN);
        PlayerNR.Runner.ActionPointsUsed(costOfAction);
        MakeRun(targetServer);
    }


    void Action_ActivateEvent(PlayerNR player, IActivateable activateableCard)
	{
        int costOfAction = PlayArea.instance.CostOfAction(player, player.IsRunner() ? RUNNER_EVENT : CORP_OPERATION);
        player.ActionPointsUsed(costOfAction);
        ActivateCard(activateableCard);
    }

    void Action_RemoveTag()
	{
        int costOfAction = PlayArea.instance.CostOfAction(PlayerNR.Runner, RUNNER_REMOVE_TAG);
        PlayerNR.Runner.ActionPointsUsed(costOfAction);
        RemoveTag();
    }

    void Action_AdvanceCard(Card card)
	{
        int costOfAction = PlayArea.instance.CostOfAction(PlayerNR.Corporation, CORP_ADVANCE);
        PlayerNR.Corporation.ActionPointsUsed(costOfAction);
        AdvanceCard(card);
    }

    #endregion


    void PayCost(PlayerNR player, int cost)
	{
        player.Credits -= cost;
	}

    void PayCostOfCard(PlayerNR player, Card card)
	{
        int balance = player.Credits;
        if (card.cardCost.TryBuyCard(ref balance))
        {
            player.Credits = balance;
        }
        else print("Did not pay for card!!!");
	}

    public void CancelAction(PlayerNR player, int actionIndex)
	{
        int costOfAction = PlayArea.instance.CostOfAction(PlayerNR.Runner, actionIndex);
        player.ActionPoints += costOfAction;
    }

	void UseMemorySpaceOfCard(Card card)
	{
		int spaceAvailable = PlayerNR.Runner.MemoryUnitsAvailable;
		if (card.cardCost.TryUseMemorySpace(ref spaceAvailable))
		{
            PlayerNR.Runner.MemoryUnitsAvailable = spaceAvailable;
        }
        else print("Did not use Memory Space for card!!!");
    }


    void DrawNextCard(PlayerNR player)
	{
        DrawCards(player, 1);
    }

    void GainCredit(PlayerNR player)
	{
        player.AddCredits(1);
	}

    void InstallCard_Runner(IInstallable installableCard)
	{
        RunnerRIG.instance.InstallCard(installableCard);
        OnCardInstalled?.Invoke((Card)installableCard, true);
    }

    void InstallCard_Corporation(IInstallable installableCard)
	{
        ServerSpace.instance.TryInstallCard(installableCard);
        // Waiting on ice install process here
        //OnCardInstalled?.Invoke((Card)installableCard, true);
    }

    public static void CardInstalled(Card card, bool installed)
	{
        OnCardInstalled?.Invoke(card, installed);
    }

    void MakeRun(ServerColumn targetServer)
	{
        RunOperator.instance.MakeRun(targetServer);
    }

    void ActivateCard(IActivateable activateableCard)
	{
        activateableCard.ActivateCard();
        OnCardActivated?.Invoke((Card)activateableCard);
    }

    void RemoveTag()
	{
        PlayerNR.Runner.Tags--;
	}

    void AdvanceCard(Card card)
	{
        card.cardAdvancer.AdvanceCard();
	}

    void SendCardToDiscard(PlayerNR player, Card card)
	{
        PlayArea.instance.DiscardNR(player).AddCardToDiscard(card);
	}

    public void TrashCard(PlayerNR trashedPlayer, Card card)
	{
        SendCardToDiscard(trashedPlayer, card);
        if (card.isInstalled)
		{
            OnCardInstalled?.Invoke(card, false);
		}
        OnCardTrashed?.Invoke(card);
    }

    void ScoreAgenda(PlayerNR scoringPlayer, Card_Agenda agenda)
	{
        PlayArea.instance.ScoringAreaNR(scoringPlayer).AddCardToScoringArea(agenda);
        scoringPlayer.AddScore(agenda.scoringAmount);
        agenda.cardAdvancer.CardScored();

        if (!scoringPlayer.IsRunner()) OnCardScored?.Invoke(agenda);
        else OnCardStolen?.Invoke(agenda);
    }


    public void DoNetDamage(UnityAction callBack, int numDamage)
	{
        if (numDamage <= 0)
        {
            callBack?.Invoke();
            return;
        }
        List<SelectorNR> selectorNRs = new List<SelectorNR>();
		foreach (var card in PlayArea.instance.HandNR(PlayerNR.Runner).cardsInHand)
		{
            selectorNRs.Add(card.selector);
		}
        CardChooser.instance.ActivateFocus(NetDamageDone, numDamage, selectorNRs.ToArray());
        ActionOptions.instance.ActivateActionMessage(string.Format("Net Damage:\nRemove ({0}) cards", numDamage));

        void NetDamageDone(SelectorNR[] selectors)
		{
			foreach (var selector in selectors)
			{
                PlayArea.instance.DiscardNR(PlayerNR.Runner).AddCardToDiscard(selector.GetComponentInParent<Card>());
			}

            callBack?.Invoke();
		}
	}

    public void ExposeCard(Card card)
	{
        exposeCardRoutine = StartCoroutine(ExposeCardRoutine(card));
    }

    public void StopExposeCard()
	{
        if (exposeCardRoutine != null) StopCoroutine(exposeCardRoutine);
	}

    Coroutine exposeCardRoutine;
    IEnumerator ExposeCardRoutine(Card card)
	{
        OnCardExposed_Pre?.Invoke(card);
        while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Card_Exposed_Pre))
            yield return null;
        card.ExposeCard();
        OnCardExposed?.Invoke(card);
        while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Card_Exposed))
            yield return null;
    }




    public void StartTurn(PlayerNR playerTurn, bool isFirstTurn)
	{
        if (isFirstTurn)
        {
            PlayerNR.Runner.MemoryUnitsAvailable = numMemoryUnitsStart;
            PlayerNR.Runner.MemoryUnitsTotal = numMemoryUnitsStart;
        }

        if (playerTurn.IsRunner())
		{
            playerTurn.ActionPoints = numActionPointsStart_Runner;
            playerTurn.Tags = numTagsStart;
        }
        else
		{
            playerTurn.ActionPoints = numActionPointsStart_Corp;
		}
	}





}
