using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunOperator : MonoBehaviour
{
    public static RunOperator instance;

    public Card_Ice currentIceEncountered;
    public Card_Program currentProgramEncountering;
    public PaidAbility currentPaidAbility;
    public ServerColumn currentServerColumn;

    public bool isRunning;
    public bool isBreakingSubroutines;
    public bool isEncounteringIce;

    public ServerColumn.ServerType? serverTypeOverride = null;

    List<Card_Program> strengthModifiedProgramCards = new List<Card_Program>();
    List<Card_Ice> encounteredIceCards = new List<Card_Ice>();

    public delegate void Approached(int encounterIndex);
    public static event Approached OnApproached;

    public delegate void IceEncountered(Card_Ice iceCard);
    public static event IceEncountered OnIceEncountered;

    public delegate void RunEvent();
    public static event RunEvent OnRunStarted;

    public delegate void RunEnded(bool success, ServerColumn.ServerType serverType);
    public static event RunEnded OnRunEnded;

    

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
        CardViewer.instance.ShowLinkIcon(isEncounteringIce && currentIceEncountered.isRezzed && CardViewer.currentPinnedCard && CardViewer.currentPinnedCard.cardFunction.HasBreakerOfType(currentIceEncountered.cardSubType));
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

    void BypassedIce()
	{
        bypassedIce = true;
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
        isEncounteringIce = true;
        Card viewCard = CardViewer.instance.GetCard(currentIceEncountered.viewIndex, false) as Card_Ice;
        CardViewer.instance.PinCard(viewCard.viewIndex, false);
        viewCard.SetClickable(false);
        CardChooser.instance.ActivateFocus(null);

        //yield return new WaitForSeconds(0.25f);

        OnApproached?.Invoke(currentServerColumn.currentIceIndex);

        ResetAllStrengths();

        yield return PaidAbilitiesManager.instance.StartPaidAbilitiesWindow();

        if (!currentIceEncountered.isRezzed)
		{
            PaidAbilitiesManager.instance.PausePaidWindow();
            bool? rezChoice = null;
            ActionOptions.instance.Display_RezCard(RezChoice, currentIceEncountered.cardCost.costOfCard, true);
            while (!rezChoice.HasValue) yield return null;

            void RezChoice(bool yes)
			{
                rezChoice = false;
                if (yes)
				{
                    if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, currentIceEncountered.cardCost.costOfCard))
					{
                        rezChoice = true;
                    }

                }
                PaidAbilitiesManager.instance.ResumePaidWindow();
            }

            if (rezChoice.Value)
			{
                currentIceEncountered.Rez();
            }

        }


        if (currentIceEncountered.isRezzed)
		{
            OnIceEncountered?.Invoke(currentIceEncountered);

            while (ConditionalAbilitiesManager.IsResolvingConditionals()) yield return null;

            currentIceEncountered = viewCard as Card_Ice;
            if (!encounteredIceCards.Contains(currentIceEncountered)) encounteredIceCards.Add(currentIceEncountered);

            canBreakSubroutines = true;
            ActionOptions.instance.Display_FireRemainingSubs(true);
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

    Coroutine runCompletedRoutine;
	IEnumerator RunCompleted()
    {
        OnApproached?.Invoke(currentServerColumn.currentIceIndex);

        CardViewer.instance.PinCard(-1, false);
        Card serverCard = currentServerColumn.GetRemoteServerRoot();
        if (serverCard)
        {
            CardViewer.instance.PinCard(serverCard.viewIndex, false);
        }

        yield return PaidAbilitiesManager.instance.StartPaidAbilitiesWindow();
        yield return JackOutRoutine();

        bool? rezChoice = null;
        if (serverCard)
        {
            if (TryRequestRezOnRemoteServer(serverCard, Choice))
            {
                while (!rezChoice.HasValue) yield return null;
            }
        }

        void Choice(bool yes)
		{
            rezChoice = false;
            if (yes)
			{
                if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, serverCard.cardCost.costOfCard))
				{
                    serverCard.Rez();
                    rezChoice = true;
				}
            }
		}

        yield return new WaitForSeconds(0.25f);

        ServerColumn serverColumnAccessed = serverTypeOverride.HasValue? ServerSpace.instance.GetServerOfType(serverTypeOverride.Value) : currentServerColumn;
        EndRun(true);

        CardChooser.instance.ActivateFocus();
        while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Run_Ends)) yield return null;

        Card installedCard = null;
        if (serverColumnAccessed.HasRootInstalled(ref installedCard))
		{
            serverColumnAccessed.AccessServer(serverColumnAccessed.serverType);

            while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Card_Accessed)) yield return null;

            print("RunCompleted Over!");

        }
        CardChooser.instance.DeactivateFocus();
    }


    bool TryRequestRezOnRemoteServer(Card serverCard, UnityAction<bool> callback)
	{
		if (serverCard.IsCardType(CardType.Asset) && !serverCard.isRezzed)
		{
			Card viewCard = CardViewer.instance.GetCard(serverCard.viewIndex, false);
			CardViewer.instance.PinCard(viewCard.viewIndex, false);
			viewCard.SetClickable(false);

			ActionOptions.instance.Display_RezCard(callback, serverCard.cardCost.costOfCard, true);
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
        currentIceEncountered.ShowSubroutinesBreakable(true);
        currentIceEncountered.SetClickable();
        BreakerPanel.instance.Activate();

        isBreakingSubroutines = true;
    }

    public void SubroutineBroken(Subroutine subroutine)
    {
        currentIceEncountered.ShowSubroutinesBreakable(false);
        subroutine.SetBroken(true);
        currentIceEncountered.SetSubroutineToBypass(subroutine);
        currentIceEncountered.SetClickable(false);
        BreakerPanel.instance.Activate(false);
        isBreakingSubroutines = false;
        if (currentIceEncountered.AllSubroutinesBypassed())
        {
            BypassedIce();
            print("BYPASSED ICE");
        }
    }

    public void SubroutineBreakCancelled()
    {
        currentIceEncountered.ShowSubroutinesBreakable(false);
        currentIceEncountered.SetClickable(false);
        isBreakingSubroutines = false;
        PlayCardManager.instance.ReversePaidAbility(currentPaidAbility);
        currentPaidAbility = null;
    }

    public void FireRemainingSubroutines()
	{
        currentIceEncountered.FireAllRemainingSubroutines();
        if (isRunning) BypassedIce();
    }


    public bool IsCardStrongEnough(Card_Program programCard)
    {
        return programCard.strength >= currentIceEncountered.strength;
    }

    public bool IceIsType(CardSubType subType)
    {
        return currentIceEncountered.IsCardSubType(subType);
    }

    //public void Reset

    [ContextMenu("EndRun")]
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
            currentIceEncountered = null;
            currentProgramEncountering = null;
            currentPaidAbility = null;
            ServerColumn.ServerType serverType = currentServerColumn.serverType;
            if (serverTypeOverride.HasValue && success) serverType = serverTypeOverride.Value;
            currentServerColumn = null;
            encounteredIceCards.Clear();
            ActionOptions.instance.HideAllOptions();
            isRunning = false;
            StopRun();
            OnRunEnded?.Invoke(success, serverType);
            SetServerTypeOverride(null);
            CardChooser.instance.DeactivateFocus();
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

    public void ResetAllIces()
	{
		for (int i = 0; i < encounteredIceCards.Count; i++)
		{
            encounteredIceCards[i].ResetSubroutines();
            encounteredIceCards[i].ResetSubroutinesToFire();
        }
    }


    public void SetServerTypeOverride(ServerColumn.ServerType? serverType)
	{
        serverTypeOverride = serverType;
	}



}
