using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // NUEVO: Necesario para detectar el click
using TMPro;

// Agregamos la interfaz IPointerClickHandler para que detecte el click del mouse
public class SpiritDeck : MonoBehaviour, IPointerClickHandler
{
    [Header("Tus Espíritus (Reserva)")]
    public List<CardData> spiritList = new List<CardData>();

    [Header("Referencias Visuales (Deck en Mesa)")]
    public GameObject visualPile; 
    public TMP_Text countText;

    [Header("UI - Visualizador de Cementerio")] // NUEVO
    public GameObject spiritWindowPanel;   // El Panel completo (con el botón X para cerrar)
    public Transform contentContainer;     // El objeto "Content" dentro del ScrollView
    public GameObject uiCardSpiritPrefab;  // El prefab "UI_Card_Spirit_Prefab"

    void Start()
    {
        UpdateVisuals();
        // Asegurarnos de que la ventana empiece cerrada
        if(spiritWindowPanel != null) spiritWindowPanel.SetActive(false);
    }

    // --- LÓGICA DE INTERACCIÓN (CLICK) ---
    // Esta función se dispara sola al dar click al objeto (si tiene Raycast Target)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AbrirVentanaEspiritus();
        }
    }

    void AbrirVentanaEspiritus()
    {
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

            // AQUÍ CONECTAMOS LOS DATOS:
            // Asumo que tu prefab tiene un script (ej. "CardDisplay" o "SpiritCardUI")
            // Cambia "TuScriptDeCartaUI" por el nombre real del script que tiene el prefab.
            
            var scriptDeCarta = nuevaCartaUI.GetComponent<CardDisplay>(); 
            if (scriptDeCarta != null)
            {
                scriptDeCarta.Setup(datosSpirit); // O como se llame tu función de iniciar
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
}