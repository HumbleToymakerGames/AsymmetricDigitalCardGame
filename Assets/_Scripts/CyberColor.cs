using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CyberColor : MonoBehaviour
{
    public Color[] cyberColors;
	Card card;

	private void Awake()
	{
		card = GetComponentInParent<Card>();
	}

	private void Start()
	{
		Invoke("gogo", 3);
	}

	void gogo()
	{
		if (!card.isViewCard)
			ChangeColor();
	}



	void ChangeColor()
	{
		SetColor(cyberColors[Random.Range(0, cyberColors.Length)]);
	}

	public void SetColor(Color color)
	{
		GetComponent<Image>().color = color;
		if (!card.isViewCard)
		{
			Card viewCard = CardViewer.instance.GetCard(card.viewIndex, false);
			if (viewCard)
				viewCard.GetComponentInChildren<CyberColor>(true).SetColor(color);

		}
	}

    
}
