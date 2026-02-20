using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using TMPro;
using System.Linq; 

public class SpiritDeck : MonoBehaviour, IPointerClickHandler
{
    // --- SINGLETON PARA ACCESO GLOBAL ---
    public static SpiritDeck instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    // -------------------------------------------

    [Header("Tus Espíritus (Reserva)")]
    public List<CardData> spiritList = new List<CardData>();

    [Header("Referencias Visuales (Deck en Mesa)")]
    public GameObject visualPile; 
    public TMP_Text countText;

    [Header("UI - Visualizador de Cementerio")]
    public GameObject spiritWindowPanel;   
    public Transform contentContainer;     
    public GameObject uiCardSpiritPrefab;  

    public GameObject closeButton; // <--- ¡NUEVO! Arrastraremos aquí tu botón de la 'X'

    void Start()
    {
        UpdateVisuals();
        // Asegurarnos de que la ventana empiece cerrada
        if(spiritWindowPanel != null) spiritWindowPanel.SetActive(false);
    }

    // --- LÓGICA DE INTERACCIÓN (CLICK) ---
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AbrirVentanaEspiritus();
        }
    }

    void AbrirVentanaEspiritus()
    {

if (spiritWindowPanel == null || contentContainer == null || uiCardSpiritPrefab == null) return;

        spiritWindowPanel.SetActive(true);
        
        // ¡NUEVO! Si abrimos para mirar, SÍ mostramos el botón de cerrar
        if (closeButton != null) closeButton.SetActive(true);

        if (spiritWindowPanel == null || contentContainer == null || uiCardSpiritPrefab == null)
        {
            Debug.LogError("Faltan asignar referencias de UI en el Inspector de SpiritDeck");
            return;
        }

        // 1. Mostrar la ventana
        spiritWindowPanel.SetActive(true);

        // 2. Limpiar las cartas viejas que se hayan quedado ahí
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // 3. Crear las cartas nuevas basadas en la lista actual
        foreach (CardData datosSpirit in spiritList)
        {
            GameObject nuevaCartaUI = Instantiate(uiCardSpiritPrefab, contentContainer);
            
            var scriptDeCarta = nuevaCartaUI.GetComponent<CardDisplay>(); 
            if (scriptDeCarta != null)
            {
                scriptDeCarta.Setup(datosSpirit); 
            }
        }
    }

    // --- TUS FUNCIONES ORIGINALES ---

    public CardData GetSpiritDataByName(string spiritName)
    {
        foreach (CardData spirit in spiritList)
        {
            if (spirit.cardName == spiritName) return spirit;
        }
        Debug.LogWarning("No se encontró el Espíritu: " + spiritName);
        return null;
    }

    public void ReturnSpirit(CardData returnedSpirit)
    {
        spiritList.Add(returnedSpirit);
        UpdateVisuals();
    }

    public void RemoveSpirit(CardData spiritToRemove)
    {
        if (spiritList.Contains(spiritToRemove))
        {
            spiritList.Remove(spiritToRemove);
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (visualPile != null)
            visualPile.SetActive(spiritList.Count > 0);

        if (countText != null)
            countText.text = spiritList.Count.ToString();
    }

    [ContextMenu("Imprimir Lista de Espíritus")]
    public void DebugPrintList()
    {
        Debug.Log("--- CONTENIDO DE LA GRAVE ZONE ---");
        foreach (var s in spiritList)
        {
            Debug.Log($"- Espíritu: {s.cardName}");
        }
    }

    // --- LA MAGIA DE BÚSQUEDA PARA RITUALES ---
    // Esta función recibe las reglas del Ritual y devuelve los Espíritus válidos
    public List<CardData> GetValidSpiritsForSummon(CardData.SummonRequirement req)
    {
        List<CardData> validSpirits = new List<CardData>();

        foreach (CardData card in spiritList)
        {
            // Omitimos cartas que no sean espíritus
            if (card.type.ToString() != "Spirit") continue;

            // 1. Chequeo de Nombre Específico
            if (!string.IsNullOrEmpty(req.specificCardName))
            {
                if (card.cardName != req.specificCardName) continue; 
            }

            // 2. CHEQUEO DE NIVEL (Leyéndolo de la lista de costos)
            int cardLevel = 0;
            if (card.costs != null)
            {
                foreach (CardCost cost in card.costs)
                {
                    // Comparamos el enum pasándolo a string para asegurarnos de que lo lea bien
                    if (cost.type.ToString() == "Level") 
                    {
                        cardLevel = cost.amount;
                        break; 
                    }
                }
            }

            // Si el ritual pide un nivel máximo (ej. maxLevel = 3) y la carta lo supera, la ignoramos
            if (req.maxLevel > 0 && cardLevel > req.maxLevel) continue; 

            // 3. Chequeo de Arquetipo
            if (!string.IsNullOrEmpty(req.requiredArchetype))
            {
                if (card.archetype != req.requiredArchetype) continue;
            }

            // 4. Chequeo de Elemento
           if (req.allowedElements != null && req.allowedElements.Count > 0)
            {
                bool hasValidElement = false;
                // Ahora comparamos ElementType directo con ElementType
                foreach (ElementType allowed in req.allowedElements)
                {
                    if (card.element == allowed) 
                    {
                        hasValidElement = true;
                        break;
                    }
                }
                if (!hasValidElement) continue; 
            }

            // Si pasa todos los filtros, lo agregamos a las opciones válidas
            validSpirits.Add(card);
        }

        return validSpirits;
    }


    // --- NUEVO: FUNCIÓN PARA ELEGIR UNA CARTA ---
    public void AbrirVentanaDeSeleccion(List<CardData> opciones, System.Action<CardData> onSelectedCallback)
    {
        if (spiritWindowPanel == null || contentContainer == null || uiCardSpiritPrefab == null) return;

        spiritWindowPanel.SetActive(true);
        
        // ¡NUEVO! Si es un ritual, APAGAMOS el botón para obligarlo a elegir
        if (closeButton != null) closeButton.SetActive(false);

        if (spiritWindowPanel == null || contentContainer == null || uiCardSpiritPrefab == null) return;

        // 1. Mostrar la ventana
        spiritWindowPanel.SetActive(true);

        // 2. Limpiar las cartas viejas
        foreach (Transform child in contentContainer) Destroy(child.gameObject);

        // 3. Crear solo las opciones válidas
        foreach (CardData carta in opciones)
        {
            GameObject nuevaCartaUI = Instantiate(uiCardSpiritPrefab, contentContainer);
            
            var scriptDeCarta = nuevaCartaUI.GetComponent<CardDisplay>(); 
            if (scriptDeCarta != null) scriptDeCarta.Setup(carta);

            // 4. ¡LA MAGIA! Le agregamos un botón a la UI para poder darle click
            UnityEngine.UI.Button btn = nuevaCartaUI.GetComponent<UnityEngine.UI.Button>();
            if (btn == null) btn = nuevaCartaUI.AddComponent<UnityEngine.UI.Button>();

            // 5. ¿Qué pasa al darle click?
           btn.onClick.AddListener(() =>
            {
                // AHORA LLAMAMOS AL PANEL DE DETALLES
                CardDetailsManager.instance.ShowCardDetails(carta, "Invocar Espíritu", (cartaElegida) => 
                {
                    // ESTO ES LO QUE PASARÁ CUANDO LE DEN CLICK AL BOTÓN "INVOCAR ESPÍRITU"
                    spiritWindowPanel.SetActive(false); // Cerramos la ventana de espíritus
                    onSelectedCallback(cartaElegida);   // Avisamos que ya la eligieron para invocar
                });
            });
        }
    }
}