using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunOperator : MonoBehaviour
{
    public static RunOperator instance;

    public Card_Ice currentIceEncountered;
    public Card_Program currentProgramEncountering;
    public PaidAbility currentPaidAbility;
    public ServerColumn currentServerColumn;
    public Card currentServerCardEncountered;

    public bool isRunning;
    public bool isBreakingSubroutines;
    public bool isEncounteringIce;

    public ServerColumn.ServerType? serverTypeOverride = null;

    List<Card_Program> strengthModifiedProgramCards = new List<Card_Program>();
    List<Card_Ice> encounteredIceCards = new List<Card_Ice>();

    public delegate void CardBeingApproached(Card card, int encounterIndex);
    public static event CardBeingApproached OnCardBeingApproached;

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
        CardViewWindow.OnCardPinnedToView += CardViewWindow_OnCardPinnedToView;
		CardViewWindow.OnCardViewed += CardViewWindow_OnCardViewed;
    }

	private void OnDisable()
    {
        CardViewWindow.OnCardPinnedToView -= CardViewWindow_OnCardPinnedToView;
		CardViewWindow.OnCardViewed -= CardViewWindow_OnCardViewed;
    }

    private void CardViewWindow_OnCardPinnedToView(Card card)
    {
        if (card && card.IsCardType(CardType.Program))
        {
            if (currentProgramEncountering)
            {
                currentProgramEncountering.cardFunction.OnPaidAbilityActivated -= CardFunction_OnPaidAbilityActivated;
            }
            currentProgramEncountering = card as Card_Program;
            currentProgramEncountering.cardFunction.OnPaidAbilityActivated += CardFunction_OnPaidAbilityActivated;
            //if (isRunning) SetAbleAbilitiesActive();
            
        }
    }

    private void CardViewWindow_OnCardViewed(Card card)
    {
        if (currentIceEncountered && card.cardFunction)
            CardViewer.instance.ShowLinkIcon(currentIceEncountered.isRezzed && card.cardFunction.HasBreakerOfType(currentIceEncountered.cardSubType));
    }

    private void CardFunction_OnPaidAbilityActivated()
    {
        if (isRunning) OnCardBeingApproached?.Invoke(currentIceEncountered, currentServerColumn.currentIceIndex);
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
        MoveToNextIce();
    }

    [ContextMenu("Move To Next Ice")]
    public void MoveToNextIce()
	{
        if (nextIceRoutine != null) StopCoroutine(nextIceRoutine);
        nextIceRoutine = StartCoroutine(MoveToNextIceRoutine());
	}

    Coroutine nextIceRoutine;
    public bool waitForRezRequest;
	IEnumerator MoveToNextIceRoutine()
	{
        yield return new WaitForSeconds(0.1f);
		isRunning = true;
        waitForRezRequest = false;
        CardViewer.instance.PinCard(-1, false);
        OnRunStarted?.Invoke();

        if (currentServerColumn.GetNextIce(ref currentIceEncountered))
		{
            isEncounteringIce = true;
            Card viewCard = CardViewer.instance.GetCard(currentIceEncountered.viewIndex, false) as Card_Ice;
			CardViewer.instance.PinCard(viewCard.viewIndex, false);
            viewCard.ActivateRaycasts(false);

            yield return new WaitForSeconds(0.25f);


            ResetAllStrengths();

            OnCardBeingApproached?.Invoke(currentIceEncountered, currentServerColumn.currentIceIndex);

            waitForRezRequest = !currentIceEncountered.isRezzed;

            if (waitForRezRequest) ActionOptions.instance.Display_RezCard(RezCardChoice, currentIceEncountered.cardCost.costOfCard, true);
            while (waitForRezRequest) yield return null;
            currentIceEncountered.Rez();

            yield return new WaitForSeconds(0.25f);

            // var becomes view card
            currentIceEncountered = viewCard as Card_Ice;
            if (!encounteredIceCards.Contains(currentIceEncountered)) encounteredIceCards.Add(currentIceEncountered);

            ActionOptions.instance.Display_FireRemainingSubs(true);


			CardViewer.instance.SetCardsClickable(true);

		}
		else
		{
            runRoutine = StartCoroutine(RunCompleted());
		}
	}

    public void StopRunRoutine()
	{
        if (runRoutine != null) StopCoroutine(runRoutine);
	}

    Coroutine runRoutine;
	IEnumerator RunCompleted()
    {
        yield return new WaitForSeconds(0.5f);
        isEncounteringIce = false;
        TryRequestRezOnRemoteServer();

        while (waitForRezRequest) yield return null;

        ServerColumn serverColumnAccessed = serverTypeOverride.HasValue? ServerSpace.instance.GetServerOfType(serverTypeOverride.Value) : currentServerColumn;
        EndRun(true);

        while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Run_Ends)) yield return null;

        Card installedCard = null;
        if (serverColumnAccessed.HasRootInstalled(ref installedCard))
		{
            serverColumnAccessed.AccessServer(serverColumnAccessed.serverType);

            while (ConditionalAbilitiesManager.IsResolvingConditionals(ConditionalAbility.Condition.Card_Accessed)) yield return null;

            print("RunCompleted Over!");

        }

    }


    void TryRequestRezOnRemoteServer()
    {
        currentServerCardEncountered = currentServerColumn.GetRemoteServerRoot();
        if (currentServerCardEncountered != null)
        {
            if (currentServerCardEncountered.IsCardType(CardType.Asset) && !currentServerCardEncountered.isRezzed)
            {
                OnCardBeingApproached?.Invoke(currentServerCardEncountered, currentServerColumn.iceInColumn.Count);
                Card viewCard = CardViewer.instance.GetCard(currentServerCardEncountered.viewIndex, false);
                CardViewer.instance.PinCard(viewCard.viewIndex, false);
                viewCard.ActivateRaycasts(false);

                ActionOptions.instance.Display_RezCard(RezCardChoice, currentServerCardEncountered.cardCost.costOfCard, true);
                waitForRezRequest = true;
            }
        }
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
        currentIceEncountered.ActivateRaycasts();
        BreakerPanel.instance.Activate();

        isBreakingSubroutines = true;
    }

    public void SubroutineBroken(Subroutine subroutine)
    {
        currentIceEncountered.ShowSubroutinesBreakable(false);
        subroutine.SetBroken(true);
        currentIceEncountered.SetSubroutineToBypass(subroutine);
        currentIceEncountered.ActivateRaycasts(false);
        BreakerPanel.instance.Activate(false);
        isBreakingSubroutines = false;
        if (currentIceEncountered.AllSubroutinesBypassed())
        {
            MoveToNextIce();
            print("BYPASSED ICE");
        }
    }

    public void SubroutineBreakCancelled()
    {
        currentIceEncountered.ShowSubroutinesBreakable(false);
        currentIceEncountered.ActivateRaycasts(false);
        isBreakingSubroutines = false;
        PlayCardManager.instance.ReversePaidAbility(currentPaidAbility);
        currentPaidAbility = null;
    }

    public void FireRemainingSubroutines()
	{
        currentIceEncountered.FireAllRemainingSubroutines();
        if (isRunning) MoveToNextIce();
    }

    public void RezCardChoice(bool rezCard)
	{
        if (isEncounteringIce)
        {
            if (rezCard)
            {
                if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, currentIceEncountered.cardCost.costOfCard))
                {
                    waitForRezRequest = false;
                    if (currentProgramEncountering)
                        CardViewer.instance.ShowLinkIcon(currentProgramEncountering.cardFunction.HasBreakerOfType(currentIceEncountered.cardSubType));
                }
            }
            else
            {
                MoveToNextIce();
            }
        }
        else
		{
            if (rezCard)
			{
                if (PlayCardManager.instance.TryAffordCost(PlayerNR.Corporation, currentServerCardEncountered.cardCost.costOfCard))
                {
                    currentServerCardEncountered.Rez();
                    waitForRezRequest = false;
                }
            }
            else
			{
                waitForRezRequest = false;
            }

        }
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
            currentIceEncountered = null;
            currentProgramEncountering = null;
            currentPaidAbility = null;
            ServerColumn.ServerType serverType = currentServerColumn.serverType;
            if (serverTypeOverride.HasValue && success) serverType = serverTypeOverride.Value;
            currentServerColumn = null;
            encounteredIceCards.Clear();
            ActionOptions.instance.HideAllOptions();
            isRunning = false;
            OnRunEnded?.Invoke(success, serverType);
            SetServerTypeOverride(null);
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
