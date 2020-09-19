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

    List<Card_Program> strengthModifiedProgramCards = new List<Card_Program>();
    List<Card_Ice> encounteredIceCards = new List<Card_Ice>();

    public delegate void CardBeingApproached(Card card, int encounterIndex);
    public static event CardBeingApproached OnCardBeingApproached;

    public delegate void RunEvent();
    public static event RunEvent OnRunStarted;

    public delegate void RunEnded(bool success);
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
        OnCardBeingApproached?.Invoke(currentIceEncountered, currentServerColumn.currentIceIndex);
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
        OnRunStarted?.Invoke();
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

			//currentRunChooser.
			//int 

			// Put ice into secondary View Window (pinned)

			//bool strongEnough = IsCardStrongEnough(currentProgramEncountering, currentIceEncountered));
			//print("Strong enough - " + CanCardEncounter_Strength(currentProgramEncountering, currentIceEncountered));

			CardViewer.instance.SetCardsClickable(true);

			//if (currentProgramEncountering) SetAbleAbilitiesActive();



		}
		else
		{
			StartCoroutine(RunCompleted());
		}
	}

	IEnumerator RunCompleted()
    {
        yield return new WaitForSeconds(0.5f);
        isEncounteringIce = false;
        TryRequestRezServerRoot();

        while (waitForRezRequest) yield return null;

        currentServerColumn.AccessServer();
        EndRun(true);
    }


    void TryRequestRezServerRoot()
    {
        IAccessable accessableCard = currentServerColumn.GetRemoteServerRoot();
        if (accessableCard != null)
        {
            currentServerCardEncountered = (Card)accessableCard;
            if (accessableCard is Card_Asset && !currentServerCardEncountered.isRezzed)
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
            CardViewer.instance.SetCardsClickable(false);

            ResetAllStrengths();
            Card_Program.OnProgramStrengthModified -= Card_Program_OnProgramStrengthModified;

            currentServerColumn?.ResetIceIndex();
            ResetAllIces();

            isEncounteringIce = false;
            currentIceEncountered = null;
            currentProgramEncountering = null;
            currentPaidAbility = null;
            currentServerColumn = null;
            encounteredIceCards.Clear();
            ActionOptions.instance.HideAllOptions();
            isRunning = false;
            OnRunEnded?.Invoke(success);
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


}
