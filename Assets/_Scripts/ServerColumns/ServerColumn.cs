using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerColumn : PlayArea_Spot, ISelectableNR
{
    public enum ServerType { Archives, RND, HQ, Remote };
    public ServerType serverType;
    ServerSpace serverSpace;
    Button button;
    public ServerRoot serverRoot;
    public Transform iceColumnT;
    public SelectorNR selectorIce, selectorRoot;
    public List<Card_Ice> iceInColumn = new List<Card_Ice>();


	protected override void Awake()
	{
		base.Awake();
        serverSpace = GetComponentInParent<ServerSpace>();
        button = GetComponentInChildren<Button>();
        SetInteractable(false);

    }

    public void InstallCardToServerRoot(Card card)
	{
        if (card is Card_Upgrade)
		{
            serverRoot.InstallUpgrade(card as Card_Upgrade);
		}
        else
		{
            (serverRoot as ServerRoot_Remote).InstallRootCard(card);
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
        serverRoot.Access();
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
        return serverRoot is ServerRoot_Remote;
	}

    public bool HasAnyCardsInServer()
	{
        return HasRootCardsInstalled() || iceInColumn.Count > 0;
    }

    public bool HasRootCardsInstalled()
	{
        return serverRoot.HasCardsInstalled();
	}


    public SelectorNR[] AccessableSelectors()
    {
        return serverRoot.AccessableSelectors();
    }


}
