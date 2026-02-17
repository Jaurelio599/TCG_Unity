using UnityEngine;
using System.Collections.Generic;

// Este script es tu biblioteca gráfica central.
// Aquí defines qué sprite corresponde a qué nombre.
[CreateAssetMenu(menuName = "TCG/System/Game Assets Database")]
public class GameAssets : ScriptableObject
{
    // Singleton súper simple para acceder desde cualquier lado
    private static GameAssets _instance;
    public static GameAssets i 
    {
        get {
            if (_instance == null) _instance = Resources.Load<GameAssets>("GameAssetsDatabase");
            return _instance;
        }
    }

    [Header("Iconos de Elementos")]
    // Usamos una lista de pares para simular un diccionario visible en el inspector
    [SerializeField] private List<ElementMapping> elementIcons;

    [Header("Iconos de Triggers")]
    public Sprite iconOnSummon; // Flecha hacia abajo, entrada, etc.
    public Sprite iconOnDeath;  // Calavera, tumba, etc.

    // Función para buscar el sprite por nombre
    public Sprite GetElementSprite(string elementName)
    {
        foreach (var mapping in elementIcons)
        {
            if (mapping.elementName.ToLower() == elementName.ToLower())
                return mapping.icon;
        }
        Debug.LogWarning($"No se encontró icono para el elemento: {elementName}");
        return null; // O devuelve un sprite de "interrogación" por defecto
    }

    // Clase auxiliar para el mapeo en el inspector
    [System.Serializable]
    public class ElementMapping
    {
        public string elementName; // Ej: "Water"
        public Sprite icon;        // El sprite del sello azul
    }
}