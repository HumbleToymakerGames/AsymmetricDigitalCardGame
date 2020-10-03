using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class gogo : MonoBehaviour
{
    List<UnityAction> actions = new List<UnityAction>();
    public List<CardSubType> subs;


    void Start()
    {
        //UnityAction<bool> banger = (dang) => print(dang);
        //actions.Add(banger);


    }
	private void OnEnable()
	{
		PlayCardManager.OnCardInstalled += PlayCardManager_OnCardInstalled;
	}
	private void OnDisable()
	{
		PlayCardManager.OnCardInstalled -= PlayCardManager_OnCardInstalled;
	}
	private void PlayCardManager_OnCardInstalled(Card card, bool installed)
	{
		//print("card: " + card.name +" installed -- " + installed);
	}




	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P)) print( DOTween.TotalPlayingTweens());
	}








}
