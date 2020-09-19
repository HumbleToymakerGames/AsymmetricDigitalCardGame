using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardTrasher : MonoBehaviour
{
    public TextMeshProUGUI trashCostText;
    [SerializeField]
    int trashCost;


	private void Awake()
	{
        UpdateTrashCostText();
    }

	public bool CanBeTrashed()
    {
        return true;
    }

    public int CostOfTrash()
	{
        return trashCost;
	}




    public void UpdateTrashCostText()
	{
        trashCostText.text = CostOfTrash().ToString();
    }


}
