using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noshadow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("noShadow")]
    public void noShadow()
	{
		foreach (var shadow in GetComponentsInChildren<Shadow>())
		{
            shadow.enabled = false;
		}
	}

    [ContextMenu("noMask")]
    public void noMask()
    {
        foreach (var shadow in GetComponentsInChildren<Mask>())
        {
            shadow.enabled = false;
        }
    }




}
