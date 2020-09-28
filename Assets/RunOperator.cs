using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunOperator : MonoBehaviour
{
    public static RunOperator instance;

    public Card_Ice currentIceEncountered, currentIceEncountered_View;
    public Card_Program currentProgramEncountering;
    public PaidAbility currentPaidAbility;
    public ServerColumn currentServerColumn;

    public bool isRunning;
    public bool isBreakingSubroutines;
    public bool isEncounteringIce;
    public int? strengthModifier, netDamageModifier;
    public bool bypassNextIce;

    public ServerColumn.ServerType? serverTypeOverride = null;

    List<Card_Program> strengthModifiedProgramCards = new List<Card_Program>();
    List<Card_Ice> encounteredIceCards = new List<Card_Ice>();

    public delegate void Approached(int encounterIndex);
    public static event Approached OnApproached;

    public delegate void IceEncountered(Card_Ice iceCard);
    public static event IceEncountered OnIceEncountered;
    public static event IceEncountered OnIceEncountered_End;

    public delegate void RunEvent();
    public static event RunEvent OnRunStarted;

    public delegate void RunEnded(bool success, ServerColumn.ServerType serverType);
    public static event RunEnded OnRunEnded;

    public delegate void RunBeingMade(ServerColumn server);
    public static event RunBeingMade OnRunBeingMade;

    public delegate void eSubroutineBroken(Subroutine subroutine, PaidAbility breakerAbility);
    public static event eSubroutineBroken OnSubroutineBroken;

    private void Awake()
    {
        instance = this;
    }


    private void OnEnable()
    {
		CardViewWindow.OnCardViewed += CardViewWindow_OnCardViewed;
    }

	private void OnDisable()
    {
		CardViewWindow.OnCardViewed -= CardViewWindow_OnCardViewed;
    }


    private void CardViewWindow_OnCardViewed(Card card)
    {
        if (card.IsCardType(CardType.Program))
		{
            //currentProgramEncountering = card as Card_Program;
		}
    }




	private void Update()
	{
        CardViewer.instance.ShowLinkIcon(isEncounteringIce && CardViewer.currentPinnedCard && CardViewer.currentPinnedCard.cardFunction.HasBreakerOfType(currentIceEncountered.cardSubType));

        if (isRunning)
		{
            if (!CardChooser.instance.isFocused)
			{
                CardChooser.instance.ActivateFocus();
			}
		}
	}

	void RunStructure()
	{
        // Initiate 'Run' and declare attacked server

        // Collect 'Bad Publicity' credits to be used during run

        // Approach the outermost piece of 'Ice'

            // ## Window for Paid Abilities

        // Continue 'Run' or 'Jack Out' (non-first piece of ice)

            // ## Window for Paid Abilities

        // Corp can rez approached ice and non-ice cards

        // Runner encounters ice if rezzed, passes if not

            // ## Window for Paid Abilities

        // Icebreakers can interact with the encountered ice

        // Resolve all unbroken subroutines on encountered ice

        // Access server or approach next piece of ice

        // Runner approached the attacked server

            // ## Window for Paid Abilities

        // Continue 'Run' or 'Jack Out' (non-first piece of ice)

            // ## Window for Paid Abilities

        // Corp can rez approached ice and non-ice cards

        // Run is considered to be 'successful'

        // Runner accesses cards in server, trashing them if desired

        // Run ends

    }

    public void MakeRun(ServerColumn serverColumn)
    {
        if (isRunning) return;

        Card_Program.OnProgramStrengthModified += Card_Program_OnProgramStrengthModified;
        currentServerColumn = serverColumn;
        OnRunBeingMade?.Invoke(serverColumn);
        StartRun();
        //MoveToNextIce();
    }

    void StartRun()
	{
        StopRun();
        runRoutine = StartCoroutine(RunRoutine());
	}

    void StopRun()
	{
        if (runRoutine != null) StopCoroutine(runRoutine);
	}

    Coroutine runRoutine;
    IEnumerator RunRoutine()
	{
        isRunning = true;
        OnRunStarted?.Invoke();

        int numIceEncountered = 0;
        while (currentServerColumn.GetNextIce(ref currentIceEncountered))
		{
            if (numIceEncountered > 0)
            {
                yield return JackOutRoutine();
            }

            yield return MoveToIce();
            numIceEncountered++;
        }

        yield return StartRunCompletedRoutine();

    }

    public bool bypassedIce, canBreakSubroutines;

    public void BypassedIce(bool subroutinesFired)
	{
        if (!strengthModifier.HasValue && netDamageModifier.HasValue)
        {
            if (subroutinesFired)
            {
                PlayCardManager.instance.DoRunnerDamage(DamageDone, netDamageModifier.Value, DamageType.Net);

                void DamageDone()
				{
                    bypassedIce = true;
                }
            }
            netDamageModifier = null;
        }
        else
		{
            bypassedIce = true;
		}
    }


    [ContextMenu("Move To Next Ice")]
    public Coroutine MoveToIce()
	{
        if (nextIceRoutine != null) StopCoroutine(nextIceRoutine);
        return nextIceRoutine = StartCoroutine(IceEncounterRoutine());
	}

    Coroutine nextIceRoutine;
	IEnumerator IceEncounterRoutine()
	{
        yield return new WaitForSeconds(0.1f);
        CardViewer.instance.PinCard(-1, false);

        bypassedIce = canBreakSubroutines = false;
        currentIceEncountered_View = CardViewer.instance.GetCard(currentIceEncountered.viewIndex, false) as Card_Ice;
        CardViewer.instance.PinCard(currentIceEncountered_View.viewIndex, false);
        currentIceEncountered_View.SetClickable(false);
        CardChooser.instance.ActivateFocus(null);
        ResetIce(currentIceEncountered_View);

        //yield return new WaitForSeconds(0.25f);

        OnApproached?.Invoke(currentServerColumn.currentIceIndex);

        ResetAllStrengths();

        yield return PaidAbilitiesManager.instance.StartPaidAbilitiesWindow();

        if (!currentIceEncountered.isRezzed)
		{
            bool? rezChoice = null;
            ActionOptions.instance.Display_RezCard(RezChoice, currentIceEncountered.CostOfCard(), true);
            while (!rezChoice.HasValue) yield return null;

            void RezChoice(bool yes)
			{
                rezChoice = false;
                if (yes)
				{
                    if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, currentIceEncountered.CostOfCard()))
					{
                        rezChoice = true;
                    }

                }
            }

            if (rezChoice.Value)
			{
                currentIceEncountered.Rez();
            }

        }

        if (currentIceEncountered.isRezzed)
		{
            if (strengthModifier.HasValue)
			{
                currentIceEncountered_View.SetStrength(currentIceEncountered_View.strength + strengthModifier.Value);
                strengthModifier = null;
            }

            isEncounteringIce = true;
            OnIceEncountered?.Invoke(currentIceEncountered);
            while (ConditionalAbilitiesManager.IsResolvingConditionals()) yield return null;

            if (!encounteredIceCards.Contains(currentIceEncountered_View)) encounteredIceCards.Add(currentIceEncountered_View);

            canBreakSubroutines = true;
            yield return PaidAbilitiesManager.instance.StartPaidAbilitiesWindow();

            if (!bypassedIce) ActionOptions.instance.Display_FireRemainingSubs(true);

            OnIceEncountered_End?.Invoke(currentIceEncountered);
            while (ConditionalAbilitiesManager.IsResolvingConditionals()) yield return null;

            if (bypassNextIce)
			{
                BypassedIce(false);
                bypassNextIce = false;
			}

            while (!bypassedIce) yield return null;
            canBreakSubroutines = false;
        }

        isEncounteringIce = false;
    }


    IEnumerator JackOutRoutine()
	{
        PaidAbilitiesManager.instance.PausePaidWindow();

        bool choiceMade = false;
        ActionOptions.instance.ActivateYesNo(Choice, "Jack Out?");
        while (!choiceMade) yield return null;

        void Choice(bool yes)
        {
            choiceMade = true;
            if (yes) EndRun(false);
            PaidAbilitiesManager.instance.ResumePaidWindow();
        }
    }

    Coroutine StartRunCompletedRoutine()
	{
        return runCompletedRoutine = StartCoroutine(RunCompleted());
    }

    public void StopRunCompletedRoutine()
	{
        if (runCompletedRoutine != null) StopCoroutine(runCompletedRoutine);
	}

    bool waitingForCardUnReveal;
    Coroutine runCompletedRoutine;
	IEnumerator RunCompleted()
    {
        OnApproached?.Invoke(currentServerColumn.currentIceIndex);

        CardViewer.instance.PinCard(-1, false);

        yield return PaidAbilitiesManager.instance.StartPaidAbilitiesWindow();
        yield return JackOutRoutine();

        ServerColumn serverColumnAccessed = serverTypeOverride.HasValue ? ServerSpace.instance.GetServerOfType(serverTypeOverride.Value) : currentServerColumn;
        OnRunEnded?.Invoke(true, serverColumnAccessed.serverType);
        yield return new WaitForSeconds(0.25f);
        while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Run_Ends)) yield return null;

        if (serverColumnAccessed.IsRemoteServer())
        {
            Card[] cardsInRoot = serverColumnAccessed.GetCardsInRoot();
            if (cardsInRoot.Length > 0)
            {
                List<SelectorNR> selectors = new List<SelectorNR>();
                for (int i = 0; i < cardsInRoot.Length; i++)
                {
                    selectors.Add(cardsInRoot[i].selector);
                }

                for (int i = 0; i < cardsInRoot.Length; i++)
                {
                    bool chosen = false;
                    CardChooser.instance.ActivateFocus(Chosen, 1, selectors.ToArray());
                    while (!chosen) yield return null;
                    while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Card_Accessed)) yield return null;

                    print("Waiting for Unreveal...");
                    waitingForCardUnReveal = true;
                    while (waitingForCardUnReveal) yield return null;
                    print("DONE Waiting for Unreveal...");

                    yield return new WaitForSeconds(0.25f);
                    while (ConditionalAbilitiesManager.IsResolvingConditionals()) yield return null;

                    void Chosen(SelectorNR[] selectorNRs)
                    {
                        Card chosenServerCard = selectorNRs[0].GetComponentInParent<Card>();
                        serverColumnAccessed.AccessServer(chosenServerCard);
                        chosen = true;
                        selectors.Remove(selectorNRs[0]);
                    }
                }
            }
        }
        else
		{
            CardChooser.instance.ActivateFocus();
            serverColumnAccessed.AccessServer();
            while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Card_Accessed)) yield return null;

        }

        CardChooser.instance.DeactivateFocus();
        print("RunCompleted Over!");
        EndRun(true);
    }

    public void ServerFinishedAccessing()
	{
        waitingForCardUnReveal = false;
	}


    bool TryRequestRezOnRemoteServer(Card serverCard, UnityAction<bool> callback)
	{
		if (serverCard.IsCardType(CardType.Asset) && !serverCard.isRezzed)
		{
			Card viewCard = CardViewer.instance.GetCard(serverCard.viewIndex, false);
			CardViewer.instance.PinCard(viewCard.viewIndex, false);
			viewCard.SetClickable(false);

			ActionOptions.instance.Display_RezCard(callback, serverCard.CostOfCard(), true);
			return true;
		}
		return false;
    }




    private void Card_Program_OnProgramStrengthModified(Card_Program programCard)
    {
        if (!strengthModifiedProgramCards.Contains(programCard))
            strengthModifiedProgramCards.Add(programCard);
    }

    public void BreakingSubroutines(PaidAbility paidAbility)
    {
        if (!isRunning) return;
        currentPaidAbility = paidAbility;
        currentIceEncountered_View.ShowSubroutinesBreakable(true);
        currentIceEncountered_View.SetClickable();
        BreakerPanel.instance.Activate();

        isBreakingSubroutines = true;
    }

    public void SubroutineBroken(Subroutine subroutine)
    {
        currentIceEncountered_View.ShowSubroutinesBreakable(false);
        subroutine.SetBroken(true);
        currentIceEncountered_View.SetSubroutineToBypass(subroutine);
        currentIceEncountered_View.SetClickable(false);
        BreakerPanel.instance.Activate(false);
        isBreakingSubroutines = false;
        OnSubroutineBroken?.Invoke(subroutine, currentPaidAbility);
        if (currentIceEncountered_View.AllSubroutinesBypassed())
        {
            BypassedIce(false);
            print("BYPASSED ICE");
        }
    }

    public void SubroutineBreakCancelled()
    {
        currentIceEncountered_View.ShowSubroutinesBreakable(false);
        currentIceEncountered_View.SetClickable(false);
        isBreakingSubroutines = false;
        PlayCardManager.instance.ReversePaidAbility(currentPaidAbility);
        currentPaidAbility = null;
    }

    public Coroutine FireRemainingSubroutines()
	{
        return StartCoroutine(FireSubroutinesRoutine());
	}
    public IEnumerator FireSubroutinesRoutine()
    {
        yield return currentIceEncountered_View.FireAllRemainingSubroutines(null);
        BypassedIce(true);
    }

    public void BreakAllSubroutines()
	{
		foreach (var subroutine in currentIceEncountered_View.subroutines)
		{
            subroutine.SetBroken(true);
        }
        BypassedIce(false);
    }


    public bool IsCardStrongEnough(Card_Program programCard)
    {
        return programCard.strength >= currentIceEncountered_View.strength;
    }

    public bool IceIsType(CardSubType subType)
    {
        return currentIceEncountered.IsCardSubType(subType);
    }

    //public void Reset

    public void EndRun(bool success)
    {
        if (isRunning)
        {
            CardViewer.instance.PinCard(-1, false);
            //CardViewer.instance.SetCardsClickable(false);

            ResetAllStrengths();
            Card_Program.OnProgramStrengthModified -= Card_Program_OnProgramStrengthModified;

            currentServerColumn?.ResetIceIndex();
            ResetAllIces();

            isEncounteringIce = false;
            canBreakSubroutines = false;
            currentIceEncountered = currentIceEncountered_View = null;
            currentProgramEncountering = null;
            currentPaidAbility = null;
            strengthModifier = netDamageModifier = null;
            bypassNextIce = false;
            ServerColumn.ServerType serverType = currentServerColumn.serverType;
            if (serverTypeOverride.HasValue && success) serverType = serverTypeOverride.Value;
            currentServerColumn = null;
            encounteredIceCards.Clear();
            ActionOptions.instance.HideAllOptions();
            isRunning = false;
            StopRun();
            if (!success) OnRunEnded?.Invoke(success, serverType);
            SetServerTypeOverride(null);
            CardChooser.instance.DeactivateFocus();
            PaidAbilitiesManager.instance.SetPriorityToTurn();
            print("Run Ended.");
        }
    }

    public void ResetAllStrengths()
    {
        for (int i = 0; i < strengthModifiedProgramCards.Count; i++)
        {
            strengthModifiedProgramCards[i].ResetStrength();
        }
        strengthModifiedProgramCards.Clear();
    }

    void ResetAllIces()
	{
		for (int i = 0; i < encounteredIceCards.Count; i++)
		{
            ResetIce(encounteredIceCards[i]);
        }
    }

    void ResetIce(Card_Ice viewCard)
	{
        viewCard.ResetSubroutines();
        viewCard.ResetSubroutinesToFire();
        viewCard.ResetStrength();
    }


    public void SetServerTypeOverride(ServerColumn.ServerType? serverType)
	{
        serverTypeOverride = serverType;
	}



}
