using System.Collections.Generic;
using UnityEngine;
using TMPro; // Serve per gestire i testi

public class GameManager : MonoBehaviour
{
    [Header("Configurazione")]
    [SerializeField] private GameTheme _theme;       // Il file dei colori
    [SerializeField] private int _rows = 6;          // Numero di tentativi
    [SerializeField] private string _targetEquation = "10+5=15"; // La soluzione (per ora fissa)

    [Header("Riferimenti Prefab")]
    [SerializeField] private GameObject _rowPrefab;  // Il prefab della Riga
    [SerializeField] private Transform _gridContainer; // Dove mettere le righe

    // Variabili di stato (memoria del gioco)
    private CellView[,] _gridCells; // Matrice per ricordarci tutte le caselle
    private int _currentRowIndex = 0;
    private int _currentColIndex = 0;
    private string _currentGuess = "";
    private bool _isGameOver = false;

    private void Start()
    {
        InitializeGrid();
    }

    private void Update()
    {
        if (_isGameOver) return;
        HandleInput();
    }

    // 1. Crea la griglia visiva all'avvio
    private void InitializeGrid()
    {
        // Pulisce eventuali vecchie righe (utile per restart)
        foreach (Transform child in _gridContainer) 
            Destroy(child.gameObject);

        _gridCells = new CellView[_rows, 8]; // 8 colonne fisse per Nerdle

        for (int r = 0; r < _rows; r++)
        {
            // Crea una nuova riga dentro il contenitore
            GameObject rowObj = Instantiate(_rowPrefab, _gridContainer);
            
            // Prende tutte le celle dentro quella riga (sono figlie)
            CellView[] cellsInRow = rowObj.GetComponentsInChildren<CellView>();

            for (int c = 0; c < 8; c++)
            {
                _gridCells[r, c] = cellsInRow[c];
                _gridCells[r, c].SetCharacter(' '); // Pulisce il testo
                _gridCells[r, c].SetColor(_theme.DefaultCellColor);
            }
        }
    }

    // 2. Ascolta la tastiera
    private void HandleInput()
    {
        // Tasto INVIO
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitGuess();
            return;
        }

        // Tasto BACKSPACE (Cancella)
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (_currentColIndex > 0)
            {
                _currentColIndex--;
                _gridCells[_currentRowIndex, _currentColIndex].SetCharacter(' ');
                _currentGuess = _currentGuess.Substring(0, _currentGuess.Length - 1);
            }
            return;
        }

        // Scrittura numeri e simboli (solo se non siamo a fine riga)
        if (_currentColIndex < 8)
        {
            string input = Input.inputString;
            if (!string.IsNullOrEmpty(input))
            {
                char c = input[0];
                // Accetta solo numeri e operatori validi
                if (char.IsDigit(c) || "+-*/=".Contains(c.ToString()))
                {
                    _gridCells[_currentRowIndex, _currentColIndex].SetCharacter(c);
                    _currentGuess += c;
                    _currentColIndex++;
                }
            }
        }
    }

    // 3. Controlla il tentativo
    private void SubmitGuess()
    {
        // A. La riga deve essere piena (8 caratteri)
        if (_currentColIndex != 8) return; 

        // B. L'equazione deve essere matematicamente valida
        if (!EquationEvaluator.IsValidEquation(_currentGuess))
        {
            Debug.Log("Equazione non valida! I calcoli non tornano.");
            return; // Qui in futuro metteremo un effetto "shake" o errore visivo
        }

        // C. Calcola i colori (Logica Nerdle)
        CheckColors();

        // D. Controlla Vittoria/Sconfitta
        if (_currentGuess == _targetEquation)
        {
            Debug.Log("VITTORIA!");
            _isGameOver = true;
        }
        else
        {
            _currentRowIndex++;
            _currentColIndex = 0;
            _currentGuess = "";

            if (_currentRowIndex >= _rows)
            {
                Debug.Log("GAME OVER - Perso!");
                _isGameOver = true;
            }
        }
    }

    private void CheckColors()
    {
        char[] solutionChars = _targetEquation.ToCharArray();
        char[] guessChars = _currentGuess.ToCharArray();
        
        // Array temporaneo per segnare quali lettere della soluzione abbiamo già "usato" per i colori
        bool[] solutionUsed = new bool[8]; 
        Color[] finalColors = new Color[8];

        // Passo 1: Trova i VERDI (Corretti e al posto giusto)
        for (int i = 0; i < 8; i++)
        {
            if (guessChars[i] == solutionChars[i])
            {
                finalColors[i] = _theme.CorrectColor;
                solutionUsed[i] = true; // Questo carattere della soluzione è "preso"
                guessChars[i] = '0';    // Segniamo come fatto
            }
            else
            {
                finalColors[i] = _theme.AbsentColor; // Default provvisorio
            }
        }

        // Passo 2: Trova i VIOLA (Corretti ma posto sbagliato)
        for (int i = 0; i < 8; i++)
        {
            if (finalColors[i] == _theme.CorrectColor) continue; // Salta i già verdi

            // Cerchiamo se il carattere esiste altrove nella soluzione
            for (int j = 0; j < 8; j++)
            {
                // Se c'è quel carattere, e non è stato ancora "usato" da un altro verde/viola
                if (!solutionUsed[j] && solutionChars[j] == _currentGuess[i])
                {
                    finalColors[i] = _theme.WrongPosColor;
                    solutionUsed[j] = true;
                    break; // Trovato, smettiamo di cercare per questo carattere
                }
            }
        }

        // Applica i colori alla UI
        for (int i = 0; i < 8; i++)
        {
            _gridCells[_currentRowIndex, i].SetColor(finalColors[i]);
        }
    }
}