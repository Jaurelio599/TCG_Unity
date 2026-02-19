using UnityEngine;
using TMPro; // Necesario para TextMeshPro
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("--- INFORMACIÓN GENERAL ---")]
    public TMP_Text turnText;       // Ej: "Turno 3"
    public TMP_Text phaseText;      // Ej: "Main Phase"

    [Header("--- ESTADÍSTICAS DEL JUGADOR ---")]
    public TMP_Text playerHPText;
    public TMP_Text ritualManaText; // El contador azul
    public TMP_Text soulManaText;   // El contador morado

    [Header("--- SELLOS (Voluntad) ---")]
    // Arrastra aquí los textos pequeños que están al lado de cada icono de sello
    public TMP_Text fireSealText;
    public TMP_Text waterSealText;
    public TMP_Text earthSealText;
    public TMP_Text windSealText;
    public TMP_Text voidSealText;
    public TMP_Text lightSealText;

    [Header("--- ESTADÍSTICAS DEL OPONENTE ---")]
    public TMP_Text opponentHPText;
    
    // Start se ejecuta al iniciar
    void Start()
    {
        // NOS SUSCRIBIMOS AL EVENTO:
        // "Oye GameManager, avísame cuando cambie algo para actualizar los textos"
        if (GameManager.instance != null)
        {
            GameManager.instance.OnGameStateChanged += UpdateUI;
            
            // Forzamos una actualización inicial para que no empiece todo en 0 o vacío
            UpdateUI();
        }
    }

    // Es buena práctica "desuscribirse" si el objeto se destruye para evitar errores
    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnGameStateChanged -= UpdateUI;
        }
    }

    // Esta función se llama AUTOMÁTICAMENTE cada vez que cambia la vida, maná o fase
    public void UpdateUI()
    {
        GameManager gm = GameManager.instance;

        // 1. TURNO Y FASE
        if (turnText != null) 
            turnText.text = "Turno: " + gm.turnCount;
        
        if (phaseText != null) 
            phaseText.text = FormatPhaseName(gm.currentPhase.ToString());

        // 2. JUGADOR
        if (playerHPText != null) 
            playerHPText.text = gm.playerHP.ToString();

        if (ritualManaText != null) 
            ritualManaText.text = gm.playerRitualMana.ToString(); // + "/" + gm.manaCap; (Opcional si quieres mostrar el tope)

        if (soulManaText != null) 
            soulManaText.text = gm.playerSoulMana.ToString();

        // 3. OPONENTE
        if (opponentHPText != null) 
            opponentHPText.text = gm.opponentHP.ToString();

        // 4. SELLOS (Usamos una función auxiliar para no repetir código)
        UpdateSealText(CostType.FireSeal, fireSealText, gm);
        UpdateSealText(CostType.WaterSeal, waterSealText, gm);
        UpdateSealText(CostType.EarthSeal, earthSealText, gm);
        UpdateSealText(CostType.WindSeal, windSealText, gm);
        UpdateSealText(CostType.VoidSeal, voidSealText, gm);
        UpdateSealText(CostType.LightSeal, lightSealText, gm);
    }

    // Función auxiliar para verificar si tienes sellos y actualizar el texto
    void UpdateSealText(CostType type, TMP_Text label, GameManager gm)
    {
        if (label == null) return;

        if (gm.playerSeals.ContainsKey(type))
        {
            label.text = "x" + gm.playerSeals[type].ToString();
        }
        else
        {
            label.text = "x0";
        }
    }

    // Embellece el nombre de la fase (opcional)
    string FormatPhaseName(string rawName)
    {
        switch (rawName)
        {
            case "StartPhase": return "Inicio";
            case "MainPhase": return "Principal";
            case "EndPhase": return "Final";
            case "EnemyTurn": return "Turno Enemigo";
            default: return rawName;
        }
    }
}