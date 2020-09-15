using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerColumn : PlayArea_Spot
{
    public PlayArea_Spot serverGO;
    protected IAccessable serverProtected;
    public IInstallable ssisd;
    public Transform iceColumnT;
    public List<Card_Ice> iceInColumn = new List<Card_Ice>();


	protected override void Awake()
	{
		base.Awake();
        serverProtected = serverGO as IAccessable;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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


}
