using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class gogo : MonoBehaviour
{
    public string text;

    public List<int> inters;

    // Start is called before the first frame update
    void Start()
    {
        //printing = printer();
        printing = inters.GetEnumerator();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
		{
            print("bool: " +printing.MoveNext());
            //print("num: " + printing.MoveNext());

        }

    }


    IEnumerator printing;
    IEnumerator printer()
	{
		for (int i = 0; i < 10; i++)
		{
            print(i);
            yield return null;
		}
	}



}
