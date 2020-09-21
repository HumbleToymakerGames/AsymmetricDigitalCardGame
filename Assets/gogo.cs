using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class gogo : MonoBehaviour
{
    List<UnityAction> actions = new List<UnityAction>();
    public List<CardSubType> subs;

    void Start()
    {
        //UnityAction<bool> banger = (dang) => print(dang);
        //actions.Add(banger);

        bool t = false;
        ganger((g) => g.banger(t));

        print(transform.GetSiblingIndex());

    }

    void Update()
    {
    }

    void ganger(UnityAction<gogo> action)
	{
        action.Invoke(this);
	}

   


    void banger()
	{

	}

    void banger(bool i)
	{
        print("bool - " + i);
	}

    void banger(int i, string s)
	{

	}

}
