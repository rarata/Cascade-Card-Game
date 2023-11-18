using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CascadeCardGame.Players;
using CascadeCardGame.CardSlots;
using CascadeCardGame.Cards;
using CascadeCardGame.PlayFields;

public class Game : MonoBehaviour
{
    public enum GameState {
        PreGame,
        InGame,
        PostGame
    }

    public int numPlayers;
    public List<Player> players;
    public PlayField playField;
    public GameObject setupUI;
    public GameObject matchUI;
    public GameObject playerUI;
    public InputField playerNameInputField;
    public Dropdown botDifficultyDropdown;
    public GameObject PlayButton;
    public GameObject nextGameButton;
    public List<Text> playerNameText;
    public List<Text> handSizeText;
    public List<Text> actionsText;
    public List<Text> cardsPlayedText;
    public List<Text> scoreText;
    public Text winnerNameText;
    public Text winnerScoreText;
    private int firstPlayerIndex = 0;
    private int currentPlayerIndex;
    private Player currentPlayer;
    private GameState gameState;

    // TODO: extract methods and attributes that make sense to a "PlayField" class

    // Start is called before the first frame update
    void Start() {
        gameState = GameState.PreGame;
        playField.SetUpPlayField(numPlayers);
    }

    // Update is called once per frame
    void Update() {
        if (gameState == GameState.InGame) {
            HandleClicks();
            // EndGame: check if game is over & display score
            if (playField.IsFilled()) {
                currentPlayer.SetActions(0);
                ShowGameResultsAndUpdateScore();
                gameState = GameState.PostGame;
            }
            // NextTurn: if current player is out of actions, move to next player
            else if (currentPlayer.HasNoActions()) {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                currentPlayer = players[currentPlayerIndex];
                currentPlayer.SetActions(1);
                if (currentPlayer.hand.cards.Count == 0) {
                    // If the player starts his or her turn with no cards, they get a free draw
                    currentPlayer.DrawCard();
                }
            }
            // Update the score game status text (score, # of cards in hands)
            UpdateStatusDisplay();
        }
        
    }

    public void PlayButtonPress () {
        SetupNewGame(true);
        players[0].playerName = playerNameInputField.text;
        int selectedDifficulty = botDifficultyDropdown.value;
        players[1].botDifficulty = botDifficultyDropdown.options[selectedDifficulty].text;
        setupUI.SetActive(false);
        matchUI.SetActive(true);
        playerUI.SetActive(true);
        gameState = GameState.InGame;
    }

    private void ShowGameResultsAndUpdateScore() {
        // WARNING: only built for 2 players currently, doesn't account for incomplete game
        nextGameButton.SetActive(true);
        int[] cardsPlayed = new int[numPlayers];
        for (int i = 0; i < players.Count; i++) {
            cardsPlayed[i] = players[i].GetCardsPlayed();
        }
        int minCardsPlayed = cardsPlayed.Min();
        int maxCardsPlayed = cardsPlayed.Max();
        int winnerPoints = maxCardsPlayed - minCardsPlayed + 1; // in a 2-player game, there cannot be a tie
        int winnerIndex = cardsPlayed.ToList().IndexOf(maxCardsPlayed);
        string winnerName = players[winnerIndex].playerName;
        winnerNameText.text = "Game Winner: " + winnerName;
        winnerNameText.enabled = true;
        winnerScoreText.text = "Points: " + winnerPoints;
        winnerScoreText.enabled = true;
        players[winnerIndex].AddScore(winnerPoints);
    }

    public void SetupNewGame(bool firstGame = false) {
        gameState = GameState.InGame;
        // Clear the play field
        playField.Clear();

        // Select first player
        if (!firstGame) {
            firstPlayerIndex = (firstPlayerIndex + 1) % players.Count;
        }
        currentPlayerIndex = firstPlayerIndex;
        currentPlayer = players[currentPlayerIndex]; // TODO: update first player selection
 
        // Set up each player with a starting hand
        int numCards = 4;
        for (int i = 0; i < players.Count; i++) {
            int playerNum = (i + firstPlayerIndex) % players.Count;
            int numActions = (playerNum == firstPlayerIndex) ? 1 : 0;
            players[playerNum].SetupNewGame(numCards, numActions);
            numCards++;
        }

        // Play the triangle root cards onto the field
        List<CardSlot> rootCardSlots = playField.GetRootCardSlots();
        for (int i = 0; i < rootCardSlots.Count; i++) {
            int playerIndex = i % players.Count;
            Card rootCard = players[playerIndex].deck.DrawCard();
            rootCardSlots[i].AddCard(rootCard);
        }

        // Hide the new game button and winner text
        nextGameButton.SetActive(false);
        winnerNameText.enabled = false;
        winnerScoreText.enabled = false;
    }

    public void PlaceCard() {
        // Method to have the current player place a card from their hand onto a card slot
        Card cardToPlace = currentPlayer.GetSelectedCard();
        CardSlot targetSlot = playField.GetSelectedCardSlot();
        if (cardToPlace == null || targetSlot == null) {
            Debug.LogError("Card or Slot not selected"); // TODO: only show Place Card button when available
            return;
        }
        // check if cardToPlace in hand
        if (!currentPlayer.hand.cards.Contains(cardToPlace)) {
            Debug.LogError("Requested to place card that is not in the current player's hand: {cardToPlace}");
        }
        // check if cardSlot exists on playField
        if (!playField.Contains(targetSlot)) {
            Debug.LogError("Requested to add card to non-existent slot: {targetSlot}");
        }
        // attempt to add card to the cardSlot.  If successful, remove it from the hand and update player attributes.
        if (currentPlayer.ExecutePlayAction(cardToPlace, targetSlot)) {
            playField.DeselectCardSlot();
        } else {
            Debug.Log($"Invalid Play: {cardToPlace} in slot {targetSlot}");
        }
    }

    public void DrawCard() {
        if (currentPlayer.ExecuteDrawAction()) {
            return;
        } else {
            Debug.Log("No cards left in deck, cannot draw card");
        }
    }

    private void HandleClicks() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            playField.SelectCardSlotOnClick(ray);
            currentPlayer.SelectCardOnClick(ray);
        }
    }

    private void UpdateStatusDisplay() {
        for (int i = 0; i < playerNameText.Count; i++) {
            string prefix = "";
            string actionsString = "";
            if (i == currentPlayerIndex) {
                prefix = ">";
                actionsString = currentPlayer.GetActions().ToString();
            }
            playerNameText[i].text = prefix + players[i].GetName();
            handSizeText[i].text = players[i].GetHandSize().ToString();
            actionsText[i].text = actionsString;
            cardsPlayedText[i].text = players[i].GetCardsPlayed().ToString();
            scoreText[i].text = players[i].GetScore().ToString();
        }
    }
}
