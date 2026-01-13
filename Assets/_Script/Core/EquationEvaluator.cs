using System;
using System.Collections.Generic;
using UnityEngine;

public static class EquationEvaluator
{
    // Funzione principale: restituisce TRUE se l'equazione è valida (es: "3+2=5")
    public static bool IsValidEquation(string input)
    {
        // 1. Controllo base: deve esserci un solo "="
        if (string.IsNullOrEmpty(input) || !input.Contains("=")) return false;
        
        string[] parts = input.Split('=');
        if (parts.Length != 2) return false;

        string leftSide = parts[0];
        string rightSide = parts[1];

        // 2. I lati non possono essere vuoti
        if (string.IsNullOrEmpty(leftSide) || string.IsNullOrEmpty(rightSide)) return false;

        // 3. Calcoliamo il valore a sinistra (l'espressione) e a destra (il risultato dichiarato)
        try
        {
            float calculatedLeft = EvaluateExpression(leftSide);
            float declaredRight = float.Parse(rightSide);

            // 4. Confronto con una piccola tolleranza (per sicurezza coi float)
            return Math.Abs(calculatedLeft - declaredRight) < 0.001f;
        }
        catch
        {
            // Se c'è un errore di parsing (es: "10++5"), l'equazione non è valida
            return false;
        }
    }

    // Un piccolo parser manuale per evitare librerie pesanti come System.Data
    private static float EvaluateExpression(string expression)
    {
        // Liste per numeri e operatori
        List<float> numbers = new List<float>();
        List<char> operators = new List<char>();

        string currentNum = "";

        // A. Parsing della stringa in numeri e simboli
        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if (char.IsDigit(c) || c == '.')
            {
                currentNum += c;
            }
            else if ("+-*/".IndexOf(c) >= 0)
            {
                if (currentNum == "") 
                {
                    // Gestione numeri negativi a inizio stringa o dopo un operatore
                     if (c == '-' && (numbers.Count == operators.Count)) 
                     {
                         currentNum += c;
                         continue;
                     }
                     throw new Exception("Sintassi Errata");
                }

                numbers.Add(float.Parse(currentNum));
                currentNum = "";
                operators.Add(c);
            }
        }
        // Aggiungi l'ultimo numero
        if (currentNum != "") numbers.Add(float.Parse(currentNum));

        if (numbers.Count == 0 || numbers.Count != operators.Count + 1)
            throw new Exception("Espressione incompleta");

        // B. Ordine delle operazioni: Prima Moltiplicazione e Divisione
        for (int i = 0; i < operators.Count; i++)
        {
            char op = operators[i];
            if (op == '*' || op == '/')
            {
                float n1 = numbers[i];
                float n2 = numbers[i + 1];
                float result = (op == '*') ? n1 * n2 : n1 / n2;

                // Sostituisci i due numeri con il risultato
                numbers[i] = result;
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                i--; // Torna indietro per controllare il prossimo operatore che ora ha cambiato indice
            }
        }

        // C. Ordine delle operazioni: Poi Addizione e Sottrazione
        float total = numbers[0];
        for (int i = 0; i < operators.Count; i++)
        {
            char op = operators[i];
            float nextNum = numbers[i + 1];

            if (op == '+') total += nextNum;
            else if (op == '-') total -= nextNum;
        }

        return total;
    }
}