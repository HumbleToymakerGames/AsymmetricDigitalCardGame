using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectorNR : MonoBehaviour
{
    public Image highlighter;
    public UnityEvent onHighlighted, onClicked;
    public ISelectableNR selectable;
    Color highlightOGColor;
    BoxCollider2D boxCollider;
    public bool isFocused, isFocusSelected;

    public delegate void SelectorClicked(SelectorNR selector);
    public static event SelectorClicked OnSelectorClicked;

	private void Awake()
	{
        selectable = GetComponentInParent<ISelectableNR>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

	void Start()
    {
        highlightOGColor = highlighter.color = CardChooser.instance.highlightColor;
        highlighter.enabled = false;
        Highlight(false);
    }

    public void Hover(bool hover = true)
	{
        Highlight(hover);
	}

    public void Highlight(bool highlight = true)
	{
        if (isFocused)
        {
            highlighter.enabled = true;
            if (highlight)
            {
                highlighter.color = highlightOGColor;
            }
            else
            {
                if (isFocusSelected) highlighter.color = CardChooser.instance.selectedColor;
                else highlighter.color = CardChooser.instance.focusColor;
            }
        }
        else if (selectable != null && selectable.CanHighlight(highlight))
		{
            highlighter.enabled = highlight;
            
            if (selectable.CanSelect())
			{
                highlighter.color = highlightOGColor;
            }
            else
			{
                highlighter.color = Color.black;
            }

            selectable.Highlighted();
            onHighlighted?.Invoke();
        }
        

    }


    public void Click()
	{
        if (isFocused)
        {
            OnSelectorClicked?.Invoke(this);
            highlighter.color = isFocusSelected ? CardChooser.instance.selectedColor : CardChooser.instance.focusColor;
            //Highlight(true);
        }
        else if (selectable != null && selectable.CanSelect())
		{
            selectable.Selected();
            onClicked?.Invoke();
            Highlight(true);
        }
        
    }


    public void Activate(bool activate = true)
	{
        Highlight(false);
        boxCollider.enabled = activate;
    }

    public void ActivateFocus(bool activate = true)
	{
        isFocused = activate;
        Highlight(false);
	}

    public void FocusSelected(bool selected = true)
	{
        isFocusSelected = selected;
        if (selected) Highlight(true);

    }

}
