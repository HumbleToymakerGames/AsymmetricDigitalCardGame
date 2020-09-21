using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerColumn : PlayArea_Spot, ISelectableNR
{
    public enum ServerType { Archives, RnD, HQ, Remote };
    [HideInInspector]
    public ServerType serverType;
    ServerSpace serverSpace;
    Button button;
    public GameObject serverGO;
    protected IAccessable serverProtected;
    public Transform iceColumnT;
    public SelectorNR selectorIce, selectorRoot;
    public List<Card_Ice> iceInColumn = new List<Card_Ice>();


	protected override void Awake()
	{
		base.Awake();
        serverSpace = GetComponentInParent<ServerSpace>();
        serverProtected = serverGO.GetComponent<IAccessable>();
        button = GetComponentInChildren<Button>();
        SetInteractable(false);
        GetServerType();
    }

    public void InstallCardToServer(IAccessable accessableCard)
	{
        if (serverProtected is RemoteServer)
        {
            RemoteServer remoteServer = serverProtected as RemoteServer;
            remoteServer.InstallCard(accessableCard);
        }
	}



    public int currentIceIndex = -1;
    public bool GetNextIce(ref Card_Ice card)
	{
        currentIceIndex++;
        if (currentIceIndex >= iceInColumn.Count) return false;
        else
		{
            card = iceInColumn[currentIceIndex];
            return true;
		}

    }

    public void ResetIceIndex()
	{
        currentIceIndex = -1;
	}


    public void InstallIce(Card_Ice card)
	{
        card.MoveCardTo(iceColumnT);
        card.transform.localEulerAngles = Vector3.forward * 180;
        iceInColumn.Insert(0, card);
    }


	public override void RemoveCard(Card card)
	{
		base.RemoveCard(card);
        iceInColumn.Remove(card as Card_Ice);
	}


    public void AccessServer()
	{
        serverProtected.Access();
    }

    public void SetInteractable(bool interactable = true)
	{
        button.interactable = interactable;
    }

    public bool CanHighlight(bool highlight = true)
	{
        if (highlight) return serverSpace.choosingInstallColumn;
        return true;
	}

	public bool CanSelect()
	{
        return CanHighlight();
	}

	public void Highlighted()
	{

    }

	public void Selected()
	{
        serverSpace.ServerChosen(this);
	}


    public void ActivateSelector_Ice(bool activate = true)
	{
        selectorIce.Activate(activate);
        SetInteractable(activate);
    }
    public void ActivateSelector_Root(bool activate = true)
    {
        selectorRoot.Activate(activate);
    }


    public bool IsRemoteServer()
	{
        return serverProtected is RemoteServer;
	}

    public IAccessable GetRemoteServerRoot()
	{
        if (serverProtected is RemoteServer)
            return (serverProtected as RemoteServer).installedCard;
        else return null;
	}

    public bool HasCardInServer(Card card)
	{
        if (IsRemoteServer())
		{
            RemoteServer remoteServer = serverProtected as RemoteServer;
            if (remoteServer.installedCard != null) return (remoteServer.installedCard as Card) == card;
		}
        return new List<Card>(iceInColumn).Contains(card);
    }

    public bool HasAnyCardsInServer()
	{
        if (IsRemoteServer())
        {
            RemoteServer remoteServer = serverProtected as RemoteServer;
            if (remoteServer.installedCard != null) return true;
        }
        return iceInColumn.Count > 0;
    }

    public bool HasRootInstalled()
	{
        if (IsRemoteServer()) return (serverProtected as RemoteServer).HasCardInstalled();
        return true;
	}



    void GetServerType()
	{
        if (serverProtected is DiscardPile) serverType = ServerType.Archives;
        else if (serverProtected is Hand) serverType = ServerType.HQ;
        else if (serverProtected is Deck) serverType = ServerType.RnD;
        else serverType = ServerType.Remote;
    }



}
