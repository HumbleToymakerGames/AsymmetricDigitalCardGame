using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<SelectorNR> currentChosenSelectors = new List<SelectorNR>();
    public int targetNumToSelect;

    public Color highlightColor, focusColor, selectedColor;

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
        if (isFocused)
		{
            if (currentChosenSelectors.Contains(selector))
            {
                currentChosenSelectors.Remove(selector);
                selector.FocusSelected(false);
            }
            else if (currentChosenSelectors.Count < targetNumToSelect)
            {
                currentChosenSelectors.Add(selector);
                selector.FocusSelected();
            }
            if (currentChosenSelectors.Count == targetNumToSelect)
			{
                if (targetNumToSelect > 1)
                ActionOptions.instance.Display_Continue(ContinuePressed, false);
                else DeactivateFocus();

            }
			else
			{
                ActionOptions.instance.ActivateActionMessage(string.Format("Selected: {0}/{1}", currentChosenSelectors.Count, targetNumToSelect));
            }

            void ContinuePressed()
			{
                DeactivateFocus();
			}

        }
	}

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


    UnityAction<SelectorNR[]> currentCallback;
    public void ActivateFocus(UnityAction<SelectorNR[]> callback = null, int numToSelect = 1, params SelectorNR[] selectors)
	{
        if (selectors.Length == 0) Debug.LogWarning("Warning: CardChooser activated w/o selectors, no possible return");
        if (selectors.Length < numToSelect)
        {
            Debug.LogWarning("Warning: CardChooser target number lower than possible choices, revising");
            numToSelect = selectors.Length;
        }

        currentFocusedSelectors = selectors;
        isFocused = true;
        targetNumToSelect = numToSelect;
        currentCallback = callback;

        foreach (var selector in selectors)
		{
            selector.ActivateFocus(true);
		}
	}

    public void DeactivateFocus()
	{
        if (currentFocusedSelectors != null)
        {
            foreach (var selector in currentFocusedSelectors)
            {
                selector.ActivateFocus(false);
                selector.FocusSelected(false);
            }
        }
        currentFocusedSelectors = null;
        isFocused = false;
        print(currentCallback != null);
        ActionOptions.instance.HideAllOptions();
        currentCallback?.Invoke(currentChosenSelectors.ToArray());
        currentCallback = null;
        currentChosenSelectors.Clear();
    }




}
