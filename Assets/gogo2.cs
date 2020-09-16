using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gogo2 : gogo1
{


	public override void ganger()
	{
		print("waiuter");
		StartCoroutine(waiter());
	}


	IEnumerator waiter()
	{
		yield return new WaitForSeconds(3);
		base.ganger();
	}


}
