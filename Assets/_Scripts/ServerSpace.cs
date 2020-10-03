using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSpace : MonoBehaviour
{
    public static ServerSpace instance;
    public ServerColumn[] serverColumns;
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
            foreach (var server in serverColumns)
            {
                if (card is Card_Ice)
                {
                    selectors.Add(server.selectorIce);
                }
                else if (card is Card_Upgrade)
                {
                    selectors.Add(server.selectorRoot);
                }
                else
				{
                    if (server.IsRemoteServer())
                        selectors.Add(server.selectorRoot);
                }
            }
            CardChooser.instance.ActivateFocus(Chosen, 1, selectors.ToArray());
            ActionOptions.instance.Display_Cancel(
                () =>
                {
                    CardChooser.instance.DeactivateFocus();
                    choosingInstallColumn = false;
                    ActivateAllColumnSelectors(false, true);
                    ActivateAllColumnSelectors(false, false);
                    PlayCardManager.instance.CancelAction(PlayerNR.Corporation, PlayCardManager.CORP_INSTALL);
                }, true);

            void Chosen(SelectorNR[] selectorsChosen) { if (selectorsChosen.Length > 0) ServerChosen(selectorsChosen[0].GetComponentInParent<ServerColumn>()); }
        }
    }

    public bool CanInstallCard(IInstallable installableCard)
	{
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
        else
		{
            targetServer.InstallCardToServerRoot(card);
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


    public ServerColumn GetServerOfType(ServerColumn.ServerType serverType)
	{
		foreach (var server in serverColumns)
		{
            if (server.serverType == serverType) return server;
		}
        return null;
	}



}
