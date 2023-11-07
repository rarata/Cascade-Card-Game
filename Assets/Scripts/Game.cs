using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CascadeCardGame.Players;
using CascadeCardGame.CardSlots;
using CascadeCardGame.Cards;
using CascadeCardGame.PlayFields;

public class Game : MonoBehaviour
{
    public int numPlayers;
    public List<Player> players;
    public PlayField playField;
    public GameObject newGameButton;
    public List<Text> playerNameText;
    public List<Text> handSizeText;
    public List<Text> actionsText;
    public List<Text> scoreText;
    public Text winnerText;
    private int firstPlayerIndex = 0;
    private int currentPlayerIndex;
    private Player currentPlayer;

    // TODO: extract methods and attributes that make sense to a "PlayField" class

    // Start is called before the first frame update
    void Start() {
        playField.SetUpPlayField(numPlayers);
        SetupNewGame();
    }

    // Update is called once per frame
    void Update() {
        HandleClicks();
        // NextTurn: if current player is out of actions, move to next player
        if (currentPlayer.HasNoActions()) {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            currentPlayer = players[currentPlayerIndex];
            currentPlayer.SetActions(1);
            if (currentPlayer.hand.cards.Count == 0) {
                // If the player starts his or her turn with no cards, they get a free draw
                currentPlayer.DrawCard();
            }
        }
        // EndGame: check if game is over & display score
        if (playField.IsFilled()) {
            newGameButton.SetActive(true);
            winnerText.text = "Winner: " + GetWinnerName();
            winnerText.enabled = true;
        }
        // Update the score game status text (score, # of cards in hands)
        UpdateStatusDisplay();
    }

    private string GetWinnerName() {
        string winnerName = "";
        int maxScore = 0;
        for (int i = 0; i < players.Count; i++) {
            int thisScore = players[i].GetScore();
            if (thisScore > maxScore) {
                winnerName = players[i].playerName;
                maxScore = thisScore;
            } 
            else if (thisScore == maxScore) {
                winnerName = "Tie";
            }
        }
        return winnerName;
    }

    public void SetupNewGame() {
        // Clear the play field
        playField.Clear();
 
        // Set up each player with a starting hand
        int numCards = 4;
        for (int i = 0; i < players.Count; i++) {
            players[i].Setup(numCards);
            numCards++;
        }
        currentPlayer = players[firstPlayerIndex]; // TODO: update first player selection
        currentPlayer.SetActions(1);

        // Play the triangle root cards onto the field
        List<CardSlot> rootCardSlots = playField.GetRootCardSlots();
        for (int i = 0; i < rootCardSlots.Count; i++) {
            int playerIndex = i % players.Count;
            Card rootCard = players[playerIndex].deck.DrawCard();
            rootCardSlots[i].AddCard(rootCard);
        }

        // Hide the new game button and winner text
        newGameButton.SetActive(false);
        winnerText.enabled = false;
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
        (bool isValidPlay, int newDraws, int newActions) = targetSlot.AddCard(cardToPlace);
        if (isValidPlay) {
            playField.DeselectCardSlot();
            // TODO: consolidate these player calls
            currentPlayer.ExecutePlayAction(cardToPlace, newDraws, newActions);
        } else {
            Debug.Log($"Invalid Play: {cardToPlace} in slot {targetSlot}");
        }
    }

    public void DrawCard() {
        currentPlayer.ExecuteDrawAction();
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
            scoreText[i].text = players[i].GetScore().ToString();
        }
    }
}
