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
    Color ogColor;
    BoxCollider2D boxCollider;
    public bool isFocused;

    public delegate void SelectorClicked(SelectorNR selector);
    public static event SelectorClicked OnSelectorClicked;

	private void Awake()
	{
        selectable = GetComponentInParent<ISelectableNR>();
        ogColor = highlighter.color;
        boxCollider = GetComponent<BoxCollider2D>();
    }

	void Start()
    {
        highlighter.enabled = false;
        Highlight(false);
    }

    public void Hover(bool hover = true)
	{
        Highlight(hover);
	}

    public void Highlight(bool highlight = true)
	{
        if (selectable != null && selectable.CanHighlight(highlight))
		{

            highlighter.enabled = isFocused ? true : highlight;
            
            if (selectable.CanSelect())
			{
                if (isFocused)
                {
                    highlighter.color = highlight ? ogColor : CardChooser.instance.focusColor;
                }
                else highlighter.color = ogColor;
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
        if (selectable != null && selectable.CanSelect())
		{
            OnSelectorClicked?.Invoke(this);
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

}
