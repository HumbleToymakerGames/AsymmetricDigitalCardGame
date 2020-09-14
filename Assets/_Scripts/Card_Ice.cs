using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Card_Ice : Card, IInstallable
{
    public TextMeshProUGUI strengthText;
    public int strength;

    Subroutine[] subroutines;
    public bool[] subroutinesToFire;

	protected override void Awake()
	{
		base.Awake();
        subroutines = GetComponentsInChildren<Subroutine>();
        subroutinesToFire = new bool[subroutines.Length];
        ResetSubroutinesToFire();
    }

	protected override void Start()
    {
        base.Start();
        strengthText.text = strength.ToString();
    }

    public override bool CanSelect()
    {
        return base.CanSelect() && PlayCardManager.instance.CanInstallCard(this);
    }


    public bool CanInstall()
    {
        return true;
        //cardCost.CanAffordCard(PlayerNR.Runner.Credits)
            //&& cardCost.CanUseMemorySpace(PlayerNR.Runner.MemoryUnitsAvailable);
    }

    public void ShowSubroutinesBreakable(bool breakable)
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
            subroutines[i].ShowBreakable(breakable);
		}
	}

    public void ResetSubroutines()
	{
        for (int i = 0; i < subroutines.Length; i++)
		{
            subroutines[i].ResetSubroutine();
		}

    }


    int currentSubroutineIndex = -1;
    public bool IsNextSubroutine()
	{
        currentSubroutineIndex++;
        return currentSubroutineIndex < subroutines.Length;
	}

    public bool AllSubroutinesBypassed()
	{
		for (int i = 0; i < subroutinesToFire.Length; i++)
		{
            if (subroutinesToFire[i]) return false;
		}
        return true;
	}

    public void FireAllRemainingSubroutines()
	{
		for (int i = 0; i < subroutines.Length; i++)
		{
            if (subroutinesToFire[i])
			{
                subroutines[i].Fire();
			}
		}
	}

    public void ResetSubroutinesToFire()
	{
		for (int i = 0; i < subroutinesToFire.Length; i++)
		{
            subroutinesToFire[i] = true;
		}
	}

    public void SetSubroutineToBypass(Subroutine subroutine)
	{
        int index = -1;
		for (int i = 0; i < subroutines.Length; i++)
		{
            if (subroutines[i] == subroutine)
			{
                index = i; break;
			}
		}

        subroutinesToFire[index] = false;
	}



}
