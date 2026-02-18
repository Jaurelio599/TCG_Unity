using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    [Header("Zona de Mano")]
    public Transform playerHandZone;

    [Header("Moldes (Prefabs Vacíos)")]
    public GameObject ritualPrefab;
    public GameObject soulPrefab;
    public GameObject willPrefab;

    [Header("Tu Mazo (Arrastra tus ScriptableObjects aquí)")]
    public List<CardData> deckList = new List<CardData>();

    public void DrawCard()
    {
        // 1. Validar si quedan cartas
        if (deckList.Count == 0)
        {
            Debug.Log("¡Mazo vacío!");
            return;
        }

        // 2. Sacar la data de la primera carta (Top Deck)
        CardData currentData = deckList[0];

        // 3. Elegir el prefab correcto según el string 'type' de tu CardData
        GameObject selectedPrefab = null;

        // IMPORTANTE: Asegúrate de que en tus cartas el campo 'type' 
        // diga exactamente "Ritual", "Soul" o "Will" (respetando mayúsculas)
        switch (currentData.type) 
        {
            case "Ritual":
                selectedPrefab = ritualPrefab;
                break;
            case "Soul":
                selectedPrefab = soulPrefab;
                break;
            case "Will":
                selectedPrefab = willPrefab;
                break;
            default:
                Debug.LogWarning("Tipo de carta desconocido: " + currentData.type);
                selectedPrefab = ritualPrefab; // Prefab por defecto por si acaso
                break;
        }

        // 4. Instanciar la carta física en la mano
        if (selectedPrefab != null)
        {
            GameObject newCardObj = Instantiate(selectedPrefab, playerHandZone);

            // 5. ¡AQUÍ ESTÁ LA MAGIA! Conectamos el dato con el visual
            CardDisplay display = newCardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.SetUpCard(currentData); // Llamamos a la función nueva
            }

            // 6. Quitar la carta del mazo lógico
            deckList.RemoveAt(0);
        }
    }

    void Start()
    {
        BarajarMazo();
    }

    public void BarajarMazo()
    {
        // Algoritmo Fisher-Yates (El estándar para barajar)
        for (int i = 0; i < deckList.Count; i++)
        {
            CardData temp = deckList[i];
            int randomIndex = Random.Range(i, deckList.Count);
            deckList[i] = deckList[randomIndex];
            deckList[randomIndex] = temp;
        }
    }
}