﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerSide currentTurnSide;
    public static PlayerNR CurrentTurnPlayer;

    public delegate void TurnChanged(bool isRunner);
    public static event TurnChanged OnTurnChanged;
    public static event TurnChanged OnGameEnded;


    [Header("Game Data")]
    public int numCardsToDrawFirstHand;
    public int numCreditsToStartWith;

    public bool hasGameStarted = false;

    private void Awake()
	{
        instance = this;
        UpdateCurrentTurn(currentTurnSide);
    }

	private void OnEnable()
	{
		PlayCardManager.OnCardScored += PlayCardManager_OnCardScored;
		PlayCardManager.OnCardStolen += PlayCardManager_OnCardStolen;
	}
	private void OnDisable()
    {
        PlayCardManager.OnCardScored -= PlayCardManager_OnCardScored;
		PlayCardManager.OnCardStolen -= PlayCardManager_OnCardStolen;
    }

    private void PlayCardManager_OnCardScored(Card card)
	{
        CheckForWinState();
    }
    private void PlayCardManager_OnCardStolen(Card card)
    {
        CheckForWinState();
    }

    void Start()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
	{
        yield return new WaitForSeconds(0.25f);

        SpawnAndSetCards(PlayerNR.Corporation);
        SpawnAndSetCards(PlayerNR.Runner);

        yield return new WaitForSeconds(0.25f);

        SetPlayerCredits(PlayerNR.Runner, numCreditsToStartWith);
        SetPlayerCredits(PlayerNR.Corporation, numCreditsToStartWith);
        DrawFirstHands();
        StartNextTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) StartNextTurn();
    }




    void SpawnAndSetCards(PlayerNR player)
	{
        player.e_PlayerDeck.identityCard.controllingPlayerSide = player.playerSide;
        Card_Identity identityCard = Instantiate(player.e_PlayerDeck.identityCard);

        Card[] e_deckCards = player.e_PlayerDeck.deckCards;
        int numCards = e_deckCards.Length;
        Card[] deckCards = new Card[numCards];
        for (int i = 0; i < numCards; i++)
		{
            e_deckCards[i].controllingPlayerSide = player.playerSide;
            Card card = Instantiate(e_deckCards[i]);
            card.name = card.name.Replace("(Clone)", "");
            deckCards[i] = card;
        }

        player.SetPlayerCards(identityCard, deckCards);
        PlayArea.instance.SetCardsToSpots(player);

	}

    void SetPlayerCredits(PlayerNR player, int numCredits)
	{
        player.Credits = numCredits;
	}


    void DrawFirstHands()
	{
        PlayCardManager.instance.DrawCards(PlayerNR.Corporation, numCardsToDrawFirstHand);
        PlayCardManager.instance.DrawCards(PlayerNR.Runner, numCardsToDrawFirstHand);
    }


    public void StartNextTurn()
	{
        currentTurnSide = currentTurnSide == PlayerSide.Runner ? PlayerSide.Corporation : PlayerSide.Runner;
        UpdateCurrentTurn(currentTurnSide);
        PlayCardManager.instance.StartTurn(CurrentTurnPlayer, !hasGameStarted);
        hasGameStarted = true;
        OnTurnChanged?.Invoke(CurrentTurnPlayer.IsRunner());

    }

    void UpdateCurrentTurn(PlayerSide side)
	{
        currentTurnSide = side;
        CurrentTurnPlayer = side == PlayerSide.Runner ? PlayerNR.Runner : PlayerNR.Corporation;
    }


    public bool IsCurrentTurn(PlayerSide playerSide)
	{
        return currentTurnSide == playerSide;
	}

    public void SetPriority(PlayerNR player)
	{
        PlayerNR.Runner.hasPriority = player.IsRunner();
        PlayerNR.Corporation.hasPriority = !player.IsRunner();
    }

    public void EndGame_Damage()
	{
        OnGameEnded?.Invoke(false);
    }

    void CheckForWinState()
	{
        if (PlayerNR.Corporation.Score >= 7)
		{
            OnGameEnded?.Invoke(false);
		}
        else if (PlayerNR.Runner.Score >= 7)
		{
            OnGameEnded?.Invoke(true);
        }
	}



}
