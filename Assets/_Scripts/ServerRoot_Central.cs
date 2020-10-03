using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRoot_Central : ServerRoot
{
    public GameObject accessableGO;
    public IAccessable targetAccessable;


	protected override void Awake()
	{
		base.Awake();
        targetAccessable = accessableGO.GetComponent<IAccessable>();
	}

	public override void InstallUpgrade(Card_Upgrade upgradeCard)
	{
		base.InstallUpgrade(upgradeCard);
		installedUpgradeCard.MoveCardTo(serverRootT01);
	}

	public override void Access()
	{
		base.Access();
        targetAccessable.Access();
	}

	public Card[] GetAccessedCards()
	{
		return targetAccessable.GetAccessedCards();
	}

	public override SelectorNR[] AccessableSelectors()
	{
		Card[] cards = GetCardsInRoot();
		List<SelectorNR> selectors = new List<SelectorNR>();
		foreach (var card in cards)
		{
			selectors.Add(card.selector);
		}
		selectors.Add(serverColumn.selectorRoot);

		return selectors.ToArray();
	}

}
