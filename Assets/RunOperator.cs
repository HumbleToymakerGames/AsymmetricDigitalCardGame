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

    public bool isRunning;
    public bool isBreakingSubroutines;

    List<Card_Program> strengthModifiedProgramCards = new List<Card_Program>();
    List<Card_Ice> encounteredIceCards = new List<Card_Ice>();

    public delegate void IceBeingEncountered(Card_Ice iceCard);
    public static event IceBeingEncountered OnIceBeingEncountered;


    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        CardViewWindow.OnCardPinnedToView += CardViewWindow_OnCardPinnedToView;
    }
    private void OnDisable()
    {
        CardViewWindow.OnCardPinnedToView -= CardViewWindow_OnCardPinnedToView;
    }
    private void CardViewWindow_OnCardPinnedToView(Card card)
    {
        if (card.IsCardType(CardType.Program))
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

    private void CardFunction_OnPaidAbilityActivated()
    {
        OnIceBeingEncountered?.Invoke(currentIceEncountered);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

        isRunning = true;
        CardViewer.instance.PinCard(-1, false);

        if (currentServerColumn.GetNextIce(ref currentIceEncountered))
        {
            currentIceEncountered = CardViewer.instance.GetCard(currentIceEncountered.viewIndex, false) as Card_Ice;
            if (!encounteredIceCards.Contains(currentIceEncountered)) encounteredIceCards.Add(currentIceEncountered);

            CardViewer.instance.PinCard(currentIceEncountered.viewIndex, false);

            ResetAllStrengths();

            int iceStrength = currentIceEncountered.strength;
            OnIceBeingEncountered?.Invoke(currentIceEncountered);

            //int 

            // Put ice into secondary View Window (pinned)

            //bool strongEnough = IsCardStrongEnough(currentProgramEncountering, currentIceEncountered));
            //print("Strong enough - " + CanCardEncounter_Strength(currentProgramEncountering, currentIceEncountered));

            CardViewer.instance.SetCardsClickable(true);

            //if (currentProgramEncountering) SetAbleAbilitiesActive();



        }
        else
        {
            RunCompleted();
        }
    }

    void RunCompleted()
    {
        currentServerColumn.AccessServer();
        EndRun();
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
        isBreakingSubroutines = false;
        PlayCardManager.instance.ReversePaidAbility(currentPaidAbility);
        currentPaidAbility = null;
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
    public void EndRun()
    {
        if (isRunning)
        {
            CardViewer.instance.PinCard(-1, false);
            CardViewer.instance.SetCardsClickable(false);

            ResetAllStrengths();
            Card_Program.OnProgramStrengthModified -= Card_Program_OnProgramStrengthModified;

            currentIceEncountered.ResetSubroutinesToFire();
            currentServerColumn.ResetIceIndex();
            ResetAllIces();

            currentIceEncountered = null;
            currentProgramEncountering = null;
            currentPaidAbility = null;
            currentServerColumn = null;
            encounteredIceCards.Clear();
            isRunning = false;
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
        }
    }


}
