using UnityEngine;
using UnityEngine.EventSystems;

public class BottoneHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scalaNormale = 1f;
    public float scalaHover = 1.1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * scalaHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * scalaNormale;
    }
}