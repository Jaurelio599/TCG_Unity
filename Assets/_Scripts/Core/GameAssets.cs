using UnityEngine;
using System.Collections.Generic;

// --- 1. DEFINICIÓN DE TIPOS DE COSTO (Global) ---
// Definimos esto fuera de la clase para que cualquier script (como CardData) pueda usar "CostType".
public enum CostType
{
    RitualMana, // Maná para Rituales
    SoulMana,   // Maná para Almas
    // Sellos (Voluntad)
    FireSeal,
    WaterSeal,
    EarthSeal,
    WindSeal,
    VoidSeal,
    LightSeal,
    DarkSeal,
    GenericSeal,
    Health      // Por si alguna carta cuesta vida
}

[CreateAssetMenu(menuName = "TCG/System/Game Assets Database")]
public class GameAssets : ScriptableObject
{
    // --- 2. SINGLETON (Acceso desde cualquier lado) ---
    private static GameAssets _instance;
    public static GameAssets i
    {
        get
        {
            if (_instance == null) _instance = Resources.Load<GameAssets>("GameAssetsDatabase");
            return _instance;
        }
    }

    // --- 3. ICONOS DE ELEMENTOS (Arquetipos/Visuales) ---
    [Header("Iconos de Elementos")]
    [Tooltip("Mapeo de nombres (string) a iconos. Ej: 'Water' -> Sello Azul")]
    [SerializeField] private List<ElementMapping> elementIcons;

    // --- 4. ICONOS DE TRIGGERS (Efectos) ---
    [Header("Iconos de Triggers")]
    public Sprite iconOnSummon; // Flecha entrando, etc.
    public Sprite iconOnDeath;  // Calavera, tumba, etc.

    // --- 5. ICONOS DE COSTOS (Economía) ---
    [Header("Iconos de Costos")]
    [Tooltip("Mapeo del tipo de costo (Enum) a su icono correspondiente.")]
    [SerializeField] private List<CostIconMapping> costIcons;


    // ================= FUNCIONES DE BÚSQUEDA =================

    // Función A: Buscar Sprite de Elemento (por nombre texto)
    public Sprite GetElementSprite(string elementName)
    {
        // Si el string viene vacío o nulo, regresamos null sin error
        if (string.IsNullOrEmpty(elementName)) return null;

        foreach (var mapping in elementIcons)
        {
            if (mapping.elementName.ToLower() == elementName.ToLower())
                return mapping.icon;
        }
        
        Debug.LogWarning($"[GameAssets] No se encontró icono para el elemento llamado: {elementName}");
        return null;
    }

    // Función B: Buscar Sprite de Costo (por tipo Enum)
    public Sprite GetCostIcon(CostType type)
    {
        foreach (var mapping in costIcons)
        {
            if (mapping.type == type)
                return mapping.icon;
        }

        Debug.LogWarning($"[GameAssets] FALTANTE: No has asignado icono para el costo tipo: {type}");
        return null;
    }


    // ================= CLASES AUXILIARES (STRUCTS) =================

    [System.Serializable]
    public class ElementMapping
    {
        public string elementName; // Ej: "Water"
        public Sprite icon;        // El sprite del sello
    }

    [System.Serializable]
    public struct CostIconMapping
    {
        public CostType type;      // Ej: CostType.RitualMana
        public Sprite icon;        // El sprite del orbe morado
    }
}