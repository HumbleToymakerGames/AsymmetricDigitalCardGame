﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card_Identity : Card
{
    [Header("Identity")]
    public string cardSubTitle;
    public TextMeshProUGUI subTitleText;
    public TextMeshProUGUI linkStrengthText;
    public int baseLinkStrength;

	protected override void Start()
	{
		base.Start();
        UpdateSubTitle();
        UpdateLinkStrengthText();
    }

	public override bool CanHighlight(bool highlight = true)
	{
        return false;
	}

    void UpdateSubTitle()
	{
        subTitleText.text = cardSubTitle;
    }

	void UpdateLinkStrengthText()
    {
        linkStrengthText.text = baseLinkStrength.ToString();
    }

}
