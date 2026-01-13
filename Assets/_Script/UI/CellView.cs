using UnityEngine;
using UnityEngine.UI;
using TMPro; // Fondamentale per usare TextMeshPro

public class CellView : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [SerializeField] private Image _bgImage;
    [SerializeField] private TextMeshProUGUI _text; // Nota: non usare 'Text' standard

    // Imposta la lettera/numero nella casella
    public void SetCharacter(char c)
    {
        _text.text = c.ToString();
    }

    // Cambia il colore di sfondo
    public void SetColor(Color c)
    {
        _bgImage.color = c;
    }
}