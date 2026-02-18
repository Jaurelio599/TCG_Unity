using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración Visual")]
    public float escalaHover = 1.5f; // Crece un 50% (más notorio)
    public float velocidad = 20f;    // Velocidad de la animación

    private Vector3 escalaOriginal;
    private Canvas miCanvas;
    private int ordenOriginal;

    void Start()
    {
        // Guardamos la escala base (normalmente 1,1,1)
        escalaOriginal = transform.localScale;
        
        // Configuramos el Canvas para el ordenamiento
        miCanvas = GetComponent<Canvas>();
        if (miCanvas != null)
        {
            ordenOriginal = miCanvas.sortingOrder;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1. ¡Pase VIP! Se pone encima de todas
        if (miCanvas != null) miCanvas.sortingOrder = 50;

        // 2. Animamos solo la escala (El Pivot Y=0 hace que crezca hacia arriba)
        StopAllCoroutines();
        StartCoroutine(AnimarEscala(escalaOriginal * escalaHover));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 1. Vuelta a la normalidad
        if (miCanvas != null) miCanvas.sortingOrder = ordenOriginal;

        // 2. Recuperar tamaño original
        StopAllCoroutines();
        StartCoroutine(AnimarEscala(escalaOriginal));
    }

    System.Collections.IEnumerator AnimarEscala(Vector3 objetivo)
    {
        // Mientras no tengamos el tamaño deseado...
        while (Vector3.Distance(transform.localScale, objetivo) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, objetivo, Time.deltaTime * velocidad);
            yield return null;
        }
        transform.localScale = objetivo;
    }
}