using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forfeiture : MonoBehaviour
{
	public static Forfeiture instance;

	private void Awake()
	{
		instance = this;
	}

	public void AddCardToForfeiture(Card card)
	{
		card.MoveCardTo(transform);
	}


}
