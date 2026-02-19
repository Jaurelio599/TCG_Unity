using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    // --- SINGLETON: Para poder llamarlo desde el GameManager ---
    public static DeckSystem instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }
    // -----------------------------------------------------------

    [Header("Zona de Mano")]
    public Transform playerHandZone;

    [Header("Moldes (Prefabs Vacíos)")]
    public GameObject ritualPrefab;
    public GameObject soulPrefab;
    public GameObject willPrefab;

    [Header("Tu Mazo")]
    public List<CardData> deckList = new List<CardData>();

    void Start()
    {
        BarajarMazo();
        // NOTA: Ya no robamos aquí al inicio, lo hará el GameManager
    }

    // Función para robar 1 carta (La que ya tenías, intacta)
    public void DrawCard()
    {
        if (deckList.Count == 0)
        {
            Debug.Log("¡Mazo vacío!");
            return;
        }

        CardData currentData = deckList[0];
        GameObject selectedPrefab = null;

        switch (currentData.type.ToString()) 
        {
            case "Ritual": selectedPrefab = ritualPrefab; break;
            case "Soul": selectedPrefab = soulPrefab; break;
            case "Will": selectedPrefab = willPrefab; break;
            default:
                Debug.LogWarning("Tipo desconocido: " + currentData.type);
                selectedPrefab = ritualPrefab;
                break;
        }

        if (selectedPrefab != null)
        {
            GameObject newCardObj = Instantiate(selectedPrefab, playerHandZone);
            
            // Conectar visuales
            CardDisplay display = newCardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.SetUpCard(currentData);
            }

            // Añadir script de interacción (Para poder jugarla y pagar)
            // Esto lo veremos en el paso 3.
            // newCardObj.AddComponent<CardInteraction>(); 

            deckList.RemoveAt(0);
        }
    }

    // --- NUEVO: Función para robar VARIAS cartas ---
    public void DrawMultipleCards(int amount)
    {
        StartCoroutine(DrawRoutine(amount));
    }

    // Lo hacemos con una Corrutina para que se vea como salen una por una y no todas de golpe
    System.Collections.IEnumerator DrawRoutine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.2f); // Pequeña espera entre cartas
        }
    }

    public void BarajarMazo()
    {
        for (int i = 0; i < deckList.Count; i++)
        {
            CardData temp = deckList[i];
            int randomIndex = Random.Range(i, deckList.Count);
            deckList[i] = deckList[randomIndex];
            deckList[randomIndex] = temp;
        }
    }
}