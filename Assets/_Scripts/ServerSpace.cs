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

    public void InstallCard(IInstallable installableCard)
	{
        choosingInstallColumn = true;
        currentInstallableCard = installableCard;

		for (int i = 0; i < serverColumns.Length; i++)
		{
            serverColumns[i].SetInteractable(true);
        }
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

            for (int i = 0; i < serverColumns.Length; i++)
            {
                serverColumns[i].SetInteractable(false);
            }
        }
	}



}
