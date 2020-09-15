using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subroutine : MonoBehaviour
{

    Button button;
    Color breakableColor = Color.red;
    Color ogColor;
    public bool isBroken;

	private void Awake()
	{
        button = GetComponent<Button>();
        ogColor = button.colors.normalColor;
        button.onClick.AddListener(Button_Clicked);
    }

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInteractable(bool interactable)
	{
        button.interactable = interactable;
	}


    public void ShowBreakable(bool breakable)
	{
        ColorBlock colors = button.colors;
        Color disCol = colors.normalColor = colors.selectedColor = breakable ? breakableColor : ogColor;
        disCol.a = colors.disabledColor.a;
        colors.disabledColor = disCol;
        button.colors = colors;
	}

    public void SetBroken(bool broken)
	{
        isBroken = broken;
        UpdateBrokenDisplay();
	}

    public void UpdateBrokenDisplay()
	{
        SetInteractable(!isBroken);
	}

    public void ResetSubroutine()
	{
        ShowBreakable(false);
        SetBroken(false);
    }


    public virtual bool CanBeBroken()
	{
        return !isBroken;
	}



    public virtual void Fire()
	{

	}


    public virtual void Bypass()
	{

	}



    public void Button_Clicked()
	{
        if (RunOperator.instance.isBreakingSubroutines)
		{
            RunOperator.instance.SubroutineBroken(this);
		}
	}



}
