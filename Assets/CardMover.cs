using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMover : MonoBehaviour
{
	[HideInInspector]
	public Card myCard;
	CardRevealer cardRevealer;
	public GameObject buttonsPanelGO;

	private void Awake()
	{
		cardRevealer = GetComponentInParent<CardRevealer>();
	}

	private void Update()
	{
		Vector3 pos = transform.localPosition;
		pos.z = -transform.GetSiblingIndex();
		transform.localPosition = pos;
	}

	public void SetCard(Card card)
	{
		card.MoveCardTo(transform);
		card.transform.SetSiblingIndex(0);
		myCard = card;
	}

	public void SetMoveable(bool moveable = true)
	{
		buttonsPanelGO.SetActive(moveable);
	}

	public void MoveCard(bool right)
	{
		int curIndex = transform.GetSiblingIndex();
		int cardCount = transform.parent.childCount;
		if (right) curIndex+=2; else curIndex--;
		curIndex = Mathf.Clamp(curIndex, 0, cardCount);
		transform.SetSiblingIndex(curIndex);

		if (right) cardRevealer.emtpyImageT.transform.SetSiblingIndex(curIndex - 1);
	}
	public void Button_Right()
	{
		MoveCard(true);
	}
	public void Button_Left()
	{
		MoveCard(false);
	}


}
