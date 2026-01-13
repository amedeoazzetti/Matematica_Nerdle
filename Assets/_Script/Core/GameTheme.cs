using UnityEngine;

// Questo attributo aggiunge una voce al menu "Create" di Unity
[CreateAssetMenu(fileName = "NewTheme", menuName = "Nerdle/Theme")]
public class GameTheme : ScriptableObject
{
    [Header("Colori Feedback")]
    public Color CorrectColor;      // Verde (Giusto posto)
    public Color WrongPosColor;     // Viola (Posto sbagliato)
    public Color AbsentColor;       // Nero/Grigio (Non presente)
    
    [Header("Colori Base")]
    public Color DefaultCellColor;  // Colore casella vuota (non ancora controllata)
    public Color TextColor;         // Colore dei numeri (es. bianco)
}