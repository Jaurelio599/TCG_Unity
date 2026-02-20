using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BringToFrontOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Actívalo para que la carta resalte por encima de todo. En la UI sobresaldrá de los bordes, ¡lo cual es ideal para leerla bien!")]
    public bool useCanvasSorting = true;

    private Canvas myCanvas;

    void Start()
    {
        if (useCanvasSorting)
        {
            myCanvas = GetComponent<Canvas>();
            if (myCanvas == null)
            {
                myCanvas = gameObject.AddComponent<Canvas>();
                gameObject.AddComponent<GraphicRaycaster>();
            }
            myCanvas.overrideSorting = false; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Solo usamos el Canvas. Ya no movemos la carta de lugar en la lista.
        if (useCanvasSorting && myCanvas != null)
        {
            myCanvas.overrideSorting = true;
            myCanvas.sortingOrder = 100; 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useCanvasSorting && myCanvas != null)
        {
            myCanvas.overrideSorting = false;
        }
    }
}