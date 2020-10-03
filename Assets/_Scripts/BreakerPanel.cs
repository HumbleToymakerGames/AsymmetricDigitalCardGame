using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BreakerPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static BreakerPanel instance;
    public GameObject panelGO, messageGO, cancelGO;
    static Button button;

	private void Awake()
	{
        instance = this;
        button = GetComponent<Button>();
        Activate(false);
    }


    public void Activate(bool activate = true)
	{
        panelGO.SetActive(activate);
        button.enabled = activate;
    }

    public void Button_Clicked()
	{
        RunOperator.instance.SubroutineBreakCancelled();
        Activate(false);
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
        messageGO.SetActive(false);
        cancelGO.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
	{
        messageGO.SetActive(true);
        cancelGO.SetActive(false);
	}
}
