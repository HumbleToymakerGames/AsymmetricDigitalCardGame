using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSpace : MonoBehaviour
{
    public static ServerSpace instance;
    ServerColumn[] serverColumns;
    public bool choosingInstallColumn;
    IInstallable currentInstallableCard;

	private void Awake()
	{
        instance = this;
        serverColumns = GetComponentsInChildren<ServerColumn>();
    }

	private void Start()
	{
		ActivateAllColumnSelectors(false, true);
		ActivateAllColumnSelectors(false, false);
    }

    public void TryInstallCard(IInstallable installableCard)
	{
        if (CanInstallCard(installableCard))
        {
            choosingInstallColumn = true;
            currentInstallableCard = installableCard;
            Card card = installableCard as Card;
            bool isIceCard = card.IsCardType(CardType.Ice);
            ActivateAllColumnSelectors(true, isIceCard);

            List<SelectorNR> selectors = new List<SelectorNR>();
            foreach (var server in FindObjectsOfType<ServerColumn>())
            {
                if (isIceCard)
                {
                    selectors.Add(server.selectorIce);
                }
                else if (server.IsRemoteServer())
                {
                    selectors.Add(server.selectorRoot);
                }
            }
            CardChooser.instance.ActivateFocus(null, 1, selectors.ToArray());
            ActionOptions.instance.Display_Cancel(
                () =>
                {
                    CardChooser.instance.DeactivateFocus();
                    choosingInstallColumn = false;
                    ActivateAllColumnSelectors(false, true);
                    ActivateAllColumnSelectors(false, false);
                }, true);
        }
    }

    public bool CanInstallCard(IInstallable installableCard)
	{
  //      Card card = installableCard as Card;
  //      bool isIceCard = card.IsCardType(CardType.Ice);
  //      if (!isIceCard)
		//{
		//	foreach (var server in serverColumns)
		//	{
  //              if (!server.HasRootInstalled()) return true;
		//	}
  //          return false;
		//}
        return true;
    }


    public void InstallCardToServer(IInstallable installableCard, ServerColumn targetServer)
	{
        Card card = installableCard as Card;
        card.FlipCard(false, true);
        card.isRezzed = false;

        if (card.IsCardType(CardType.Ice))
		{
            Card_Ice iceCard = installableCard as Card_Ice;
            targetServer.InstallIce(iceCard);
        }
        else if (card is IAccessable)
		{
            IAccessable accessableCard = card as IAccessable;
            targetServer.InstallCardToServer(accessableCard);
		}

        if (card is IRezzable)
		{
            IRezzable rezCard = card as IRezzable;
            //if (rezCard.CanRez())
        }



    }

    public void ServerChosen(ServerColumn serverColumn)
	{
        if (choosingInstallColumn)
		{
            InstallCardToServer(currentInstallableCard, serverColumn);
            choosingInstallColumn = false;
            ActivateAllColumnSelectors(false, true);
            ActivateAllColumnSelectors(false, false);
            PlayCardManager.CardInstalled((Card)currentInstallableCard, true);
            currentInstallableCard = null;
        }
    }



    void ActivateAllColumnSelectors(bool activate, bool ice)
	{
        for (int i = 0; i < serverColumns.Length; i++)
		{
            if (ice) serverColumns[i].ActivateSelector_Ice(activate);
            else serverColumns[i].ActivateSelector_Root(activate);
        }

    }




}
