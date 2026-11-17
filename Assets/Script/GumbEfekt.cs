using UnityEngine;
using UnityEngine.EventSystems;

public class GumbEfekt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float povecanje = 1.08f;
    public float brzina = 10f;
    private Vector3 cilj = Vector3.one;

    public void OnPointerEnter(PointerEventData e)
    {
        cilj = Vector3.one * povecanje;
    }

    public void OnPointerExit(PointerEventData e)
    {
        cilj = Vector3.one;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, cilj, Time.unscaledDeltaTime * brzina);
    }
}