using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class randcol : MonoBehaviour
{
    public Image im;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("go", 3);
    }

    void go()
	{
        im.color = Random.ColorHSV();
        name += GetComponent<Card>().viewIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
