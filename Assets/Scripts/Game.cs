using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TriangleCardGame.Players;
using TriangleCardGame.CardSlots;
using TriangleCardGame.Cards;

public class Game : MonoBehaviour
{
    public int numPlayers;
    public List<Player> players;
    public List<CardSlot> rootCardSlots;
    public List<CardSlot> mainCardSlots;
    public CardSlot finalCardSlot;
    public GameObject newGameButton;
    public List<Text> playerNameText;
    public List<Text> handSizeText;
    public List<Text> actionsText;
    public List<Text> scoreText;
    public Text winnerText;
    private List<CardSlot> cardSlots;
    private int firstPlayerIndex = 0;
    private int currentPlayerIndex;
    private Player currentPlayer;
    private int slotToPlay; //TEMP
    private CardSlot selectedCardSlot;
    private float selectedCardSlotScaleFactor = 1.2f;

    // TODO: extract methods and attributes that make sense to a "PlayField" class

    // Start is called before the first frame update
    void Start() {
        cardSlots = new List<CardSlot>(rootCardSlots);
        cardSlots.AddRange(mainCardSlots);
        cardSlots.Add(finalCardSlot);
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
        if (!finalCardSlot.isEmpty) {
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
        DeselectCardSlot();
        // Clear all player actions & select the first player
        for (int i = 0; i < players.Count; i++) {
            players[i].DeselectCard();
            players[i].SetActions(0);
        } // TODO: move this together with setup further down
        currentPlayer = players[firstPlayerIndex]; // TODO: update first player selection
        currentPlayer.SetActions(1);
        // Remove all cards from the cardSlots
        for (int i = 0; i < cardSlots.Count; i++) {
            cardSlots[i].Clear();
        }
        // Set up each player with a starting hand
        int numCards = 4;
        for (int i = 0; i < players.Count; i++) {
            players[i].Setup(numCards);
            numCards++;
        }
        // Play the triangle root cards onto the field
        for (int i = 0; i < rootCardSlots.Count; i++) {
            int playerIndex = i % players.Count;
            Card rootCard = players[playerIndex].deck.DrawCard();
            rootCardSlots[i].AddCard(rootCard);
        }
        // Hide the new game button
        newGameButton.SetActive(false);
        winnerText.enabled = false;
    }

    public void PlaceCard() {
        // Method to have the current player place a card from their hand onto a card slot
        Card cardToPlace = currentPlayer.GetSelectedCard();
        CardSlot targetSlot = selectedCardSlot;
        if (cardToPlace == null || targetSlot == null) {
            Debug.LogError("Card or Slot not selected"); // TODO: only show Place Card button when available
            return;
        }
        // check if cardToPlace in hand
        if (!currentPlayer.hand.cards.Contains(cardToPlace)) {
            Debug.LogError("Requested to place card that is not in the current player's hand: {cardToPlace}");
        }
        // check if cardSlot exists on playField
        if (!cardSlots.Contains(targetSlot)) {
            Debug.LogError("Requested to add card to non-existent slot: {targetSlot}");
        }
        // attempt to add card to the cardSlot.  If successful, remove it from the hand and update player attributes.
        (bool isValidPlay, int newDraws, int newActions) = targetSlot.AddCard(cardToPlace);
        if (isValidPlay) {
            DeselectCardSlot();
            currentPlayer.DeselectCard();
            currentPlayer.RemoveCardFromHand(cardToPlace);
            currentPlayer.AddScore(1);
            currentPlayer.DrawCards(newDraws);
            currentPlayer.AddAction(newActions);
            currentPlayer.RemoveAction(1); // use action for successfully placed card
            slotToPlay++;
        } else {
            Debug.Log($"Invalid Play: {cardToPlace} in slot {targetSlot}");
        }
    }

    public void DrawCard() {
        // Method to call when the Draw Card button is pressed
        currentPlayer.DrawCard();
        currentPlayer.RemoveAction(1);
    }

    private void HandleClicks() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            SelectCardSlotOnClick(ray); // TODO: extract to playField.SelectCardSlot()
            currentPlayer.SelectCardOnClick(ray);
        }
    }

    private void SelectCardSlotOnClick(Ray ray) { // TODO: extract to playField.SelectCardSlot()
        RaycastHit hit;
        int playSurfaceLayerNumber = gameObject.layer;
        LayerMask playSurfaceLayerMask = 1 << playSurfaceLayerNumber;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, playSurfaceLayerMask)) {
            GameObject clickedObject = hit.collider.gameObject;
            CardSlot clickedCardSlot = clickedObject.GetComponent<CardSlot>();
            if (clickedCardSlot != null) {
                DeselectCardSlot();
                SelectCardSlot(clickedCardSlot);
            }
        }
    }

    private void DeselectCardSlot() {
        if (selectedCardSlot != null) {
            selectedCardSlot.transform.localScale /= selectedCardSlotScaleFactor;
            selectedCardSlot = null;
        }
    }

    private void SelectCardSlot(CardSlot slotToSelect) {
        if (slotToSelect.isAvailable) {
            selectedCardSlot = slotToSelect;
            selectedCardSlot.transform.localScale *= selectedCardSlotScaleFactor;
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
