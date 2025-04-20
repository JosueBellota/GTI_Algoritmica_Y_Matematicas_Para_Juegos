using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    [Header("Configuración del Juego")]
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public GameObject cardPrefab;
    public int initialCredits = 1000;
    public int minBet = 10;
    

    [Header("Controles UI")]
    public Button hitButton;
    public Button standButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text creditsText;
    public Text betText;
    public Text playerPointsText;
    public Button increaseBetButton;
    public Button decreaseBetButton;

    private int[] values = new int[52];
    private int cardIndex = 0;
    private int currentCredits;
    private int currentBet;
    private bool initialHand = true;
    private bool gameInProgress = false;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        currentCredits = initialCredits;
        currentBet = minBet;
        
        InitCardValues();
        ShuffleCards();
        SetupUI();
        UpdateUI();
    }

    private void InitCardValues()
    {
        for (int i = 0; i < 52; i++)
        {
            int value = (i % 13) + 1;
            values[i] = value > 10 ? 10 : (value == 1 ? 11 : value);
        }
    }
    private void UpdatePlayerPoints()
{
    int playerPoints = player.GetComponent<CardHand>().points;
    playerPointsText.text = $"Puntos: {playerPoints}";
}


    private void ShuffleCards()
    {
        for (int i = 0; i < 52; i++)
        {
            int r = Random.Range(i, 52);
            (faces[i], faces[r]) = (faces[r], faces[i]);
            (values[i], values[r]) = (values[r], values[i]);
        }
        cardIndex = 0;
        Debug.Log("Cartas barajadas. Mazo reiniciado.");
    }

private void SetupUI()
{
    hitButton.onClick.RemoveAllListeners(); // 🔥 Elimina cualquier duplicado
    hitButton.onClick.AddListener(Hit);
    
    standButton.onClick.RemoveAllListeners();
    standButton.onClick.AddListener(Stand);

    playAgainButton.onClick.RemoveAllListeners();
    playAgainButton.onClick.AddListener(PlayAgain);

    increaseBetButton.onClick.RemoveAllListeners();
    increaseBetButton.onClick.AddListener(IncreaseBet);

    decreaseBetButton.onClick.RemoveAllListeners();
    decreaseBetButton.onClick.AddListener(DecreaseBet);
}



private void StartNewGame()
{
    if (currentCredits < currentBet)
    {
        finalMessage.text = "CRÉDITOS INSUFICIENTES";
        return;
    }

    if (gameInProgress) return; // 🔥 Evita que el juego se inicie más de una vez

    gameInProgress = true;
    initialHand = true;

    currentCredits -= currentBet;  // ✅ Solo se resta la apuesta una vez
    UpdateUI();

    player.GetComponent<CardHand>().Clear();
    dealer.GetComponent<CardHand>().Clear();

    if (cardIndex > 30) ShuffleCards();

    for (int i = 0; i < 2; i++)
    {
        PushPlayer();
        PushDealer();
    }

    dealer.GetComponent<CardHand>().InitialToggle();
    CheckBlackjack();

        // Asegurar que la primera carta del dealer sea visible
    if (dealer.GetComponent<CardHand>().cards.Count > 0)
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
    }
}






private void PushPlayer()
{
    if (cardIndex >= values.Length)
    {
        Debug.LogWarning("¡Mazo vacío! Barajando de nuevo...");
        ShuffleCards();
    }

    player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
    cardIndex++;
    UpdatePlayerPoints();  // 🔹 Actualiza el texto con los puntos
    CalculateProbabilities();
}





    private void PushDealer()
    {
        if (cardIndex >= values.Length)
        {
            Debug.LogWarning("¡Mazo vacío! Barajando de nuevo...");
            ShuffleCards();
        }

        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

public void Hit()
{
    if (!gameInProgress || !hitButton.interactable) return;

    Debug.Log("Hit presionado");

    if (initialHand)
    {
        dealer.GetComponent<CardHand>().InitialToggle();
        initialHand = false;
    }

    hitButton.interactable = false;  // 🔥 Evitar múltiples clics seguidos

    PushPlayer(); // 🔥 Solo se ejecuta una vez

    Invoke(nameof(EnableHitButton), 0.5f); // 🔥 Reactivar después de 0.5 segundos

    if (player.GetComponent<CardHand>().points > 21)
    {
        finalMessage.text = "PLAYER LOSE";
        EndGame();
    }
}

private void EnableHitButton()
{
    hitButton.interactable = true;
}




    public void Stand()
    {
        if (!gameInProgress) return;

        if (initialHand)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
            initialHand = false;
        }

    // Mostrar TODAS las cartas del dealer al hacer stand
    foreach (var card in dealer.GetComponent<CardHand>().cards)
    {
        card.GetComponent<CardModel>().ToggleFace(true);
    }


        // Turno del dealer
        while (dealer.GetComponent<CardHand>().points < 17 && cardIndex < 52)
        {
            PushDealer();
        }

        DetermineWinner();
        EndGame();
    }

private void DetermineWinner()
{
    if (!gameInProgress) return;  

    gameInProgress = false;  

    Debug.Log("🚨 DetermineWinner ejecutado 🚨");

    int playerPoints = player.GetComponent<CardHand>().points;
    int dealerPoints = dealer.GetComponent<CardHand>().points;

    if (playerPoints > 21)
    {
        finalMessage.text = "PLAYER LOSE";  
        // No se suma nada porque ya perdió la apuesta.
    }
    else if (dealerPoints > 21)
    {
        finalMessage.text = "PLAYER WIN";  
        currentCredits += currentBet * 2;  // ✅ Recupera la apuesta + gana el doble
    }
    else if (playerPoints > dealerPoints)
    {
        finalMessage.text = "PLAYER WIN";
        currentCredits += currentBet * 2;  // ✅ Recupera la apuesta + gana el doble
    }
    else if (dealerPoints > playerPoints)
    {
        finalMessage.text = "DEALER WIN";  
        // No se suma nada porque ya perdió la apuesta.
    }
    else
    {
        finalMessage.text = "EMPATE";  
        currentCredits += currentBet;  // ✅ Se devuelve solo la apuesta
    }

    UpdateUI();
}




    private void CheckBlackjack()
    {
        int playerPoints = player.GetComponent<CardHand>().points;
        int dealerPoints = dealer.GetComponent<CardHand>().points;

        if (playerPoints == 21 || dealerPoints == 21)
        {
            if (playerPoints == 21 && dealerPoints == 21)
            {
                finalMessage.text = "EMPATE (Blackjack)";
                currentCredits += currentBet;
            }
            else if (playerPoints == 21)
            {
                finalMessage.text = "BLACKJACK! GANAS";
                currentCredits += currentBet * 2 + currentBet / 2; // Pago 3:2
            }
            else
            {
                finalMessage.text = "DEALER BLACKJACK! PIERDES";
            }
            EndGame();
        }
    }

    private void EndGame()
    {
        gameInProgress = false;
        DisableButtons();
        UpdateUI();
    }

    public void PlayAgain()
    {
        if (currentCredits < minBet)
        {
            finalMessage.text = "SIN CRÉDITOS";
            return;
        }

        finalMessage.text = "";
        EnableButtons();
        StartNewGame();
    }

    public void IncreaseBet()
    {
        if (currentCredits >= currentBet + minBet)
        {
            currentBet += minBet;
            UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        if (currentBet > minBet)
        {
            currentBet -= minBet;
            UpdateUI();
        }
    }

    private void CalculateProbabilities()
    {
        if (player == null || dealer == null || probMessage == null) return;

        CardHand playerHand = player.GetComponent<CardHand>();
        if (playerHand == null || playerHand.cards.Count == 0) return;

        int playerPoints = playerHand.points;
        int cardsLeft = 52 - cardIndex;
        int cards17to21 = 0;
        int cardsBust = 0;

        for (int i = cardIndex; i < 52; i++)
        {
            int possibleValue = values[i] == 11 ? (playerPoints + 11 > 21 ? 1 : 11) : values[i];
            int newPoints = playerPoints + possibleValue;

            if (newPoints > 21 && playerHand.HasAces())
                newPoints -= 10;

            if (newPoints >= 17 && newPoints <= 21) cards17to21++;
            else if (newPoints > 21) cardsBust++;
        }

        float prob17to21 = cardsLeft > 0 ? (float)cards17to21 / cardsLeft : 0;
        float probBust = cardsLeft > 0 ? (float)cardsBust / cardsLeft : 0;
        float probDealerWins = CalculateDealerWinProbability();

        probMessage.text = $"Deal > Play: {probDealerWins:F4}\n17<=X<=21: {prob17to21:F4}\nX > 21: {probBust:F4}";
    }

    private float CalculateDealerWinProbability()
    {
        CardHand dealerHand = dealer.GetComponent<CardHand>();
        if (dealerHand == null || dealerHand.cards.Count == 0) return 0;

        CardModel firstCard = dealerHand.cards[0].GetComponent<CardModel>();
        if (firstCard == null) return 0;

        int visibleValue = firstCard.value;
        
        if (visibleValue >= 10) return 0.65f;
        if (visibleValue >= 7) return 0.55f;
        if (visibleValue >= 4) return 0.45f;
        return 0.35f;
    }

    private void UpdateUI()
    {
        creditsText.text = $"Credito:\n{currentCredits}";
        betText.text = $"Puntos:\n{currentBet}";
        
        increaseBetButton.interactable = currentCredits >= currentBet + minBet && !gameInProgress;
        decreaseBetButton.interactable = currentBet > minBet && !gameInProgress;
        playAgainButton.interactable = !gameInProgress && currentCredits >= minBet;
    }

    private void EnableButtons()
    {
        hitButton.interactable = true;
        standButton.interactable = true;
    }

    private void DisableButtons()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
    }
}