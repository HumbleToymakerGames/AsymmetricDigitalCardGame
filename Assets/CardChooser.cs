using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;

public class CardChooser : MonoBehaviour
{
    public static CardChooser instance;
    public SelectorNR hoveredSelector;
    public delegate void HoveredOverCard(Card card);
    public static event HoveredOverCard OnHoveredOverCard;
    public static event HoveredOverCard OnCardPinned;

    public bool isFocused;
    SelectorNR[] currentFocusedSelectors;

    public Color focusColor;

	private void Awake()
	{
        instance = this;
	}

	private void OnEnable()
	{
		SelectorNR.OnSelectorClicked += SelectorNR_OnSelectorClicked;
	}
    private void OnDisable()
    {
        SelectorNR.OnSelectorClicked -= SelectorNR_OnSelectorClicked;
    }
    private void SelectorNR_OnSelectorClicked(SelectorNR selector)
	{
        if (isFocused && currentFocusedSelectors.Contains(selector))
		{
            DeactivateFocus(selector);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		if (Input.GetMouseButtonDown(0))
		{
			if (hoveredSelector)
			{
                //hoveredSelector.FlipCard();
                if (isFocused && !currentFocusedSelectors.Contains(hoveredSelector)) return;

                hoveredSelector.Click();
			}
		}
        else if (Input.GetMouseButtonDown(1))
		{
            if (hoveredSelector)
			{
                Card card = hoveredSelector.GetComponentInParent<Card>();
                OnCardPinned?.Invoke(card);
            }
            else
			{
                OnCardPinned?.Invoke(null);
			}
        }
    }


	private void FixedUpdate()
	{
        RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);

        if (hit)
		{
            //Card card = hit.collider.GetComponent<Card>();
            SelectorNR selector = hit.collider.GetComponent<SelectorNR>();
            if (selector)
            {
                if (selector != hoveredSelector)
                {
                    //Debug.Log("selector - " + hit.collider.name, hit.collider);
                    if (hoveredSelector) hoveredSelector.Hover(false);
                    if (isFocused)
                    {
                        if (currentFocusedSelectors.Contains(selector))
                        {
                            selector.Hover();
                        }
                    }
                    else
                    {
                        selector.Hover();
                    }

                    Card card = selector.GetComponentInParent<Card>();
                    OnHoveredOverCard?.Invoke(card);

                    hoveredSelector = selector;
                }
            }
		}
        else
		{
            if (hoveredSelector)
            {
                hoveredSelector.Hover(false);
                OnHoveredOverCard?.Invoke(null);
                hoveredSelector = null;
            }
        }

	}


    UnityAction<SelectorNR> currentCallback;
    public void ActivateFocus(UnityAction<SelectorNR> callback, params SelectorNR[] selectors)
	{
        currentFocusedSelectors = selectors;
        isFocused = true;
        currentCallback = callback;
        if (selectors.Length == 0) Debug.LogWarning("Warning: CardChooser activated w/o selectors, no possible return");

        foreach (var selector in selectors)
		{
            selector.ActivateFocus(true);
		}
	}

    public void DeactivateFocus(SelectorNR clickedSelector = null)
	{
        if (currentFocusedSelectors != null)
        {
            foreach (var selector in currentFocusedSelectors)
            {
                selector.ActivateFocus(false);
            }
        }
        currentFocusedSelectors = null;
        isFocused = false;
        print(currentCallback != null);
        currentCallback?.Invoke(clickedSelector);
        currentCallback = null;
        ActionOptions.instance.HideAllOptions();
    }











}
