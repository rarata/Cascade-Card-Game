using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CascadeCardGame.Decks;
using CascadeCardGame.Hands;
using CascadeCardGame.Cards;
using CascadeCardGame.CardSlots;
using CascadeCardGame.PlayFields;

// TODO: prune unused methods
namespace CascadeCardGame.Players { 
    public class Player : MonoBehaviour
    {
        public string playerName;
        public bool isBot;
        public Hand hand;
        public Deck deck;
        public PlayField playField;
        private int score;
        private int actions;
        private int playerLayer;
        private int playRating = 7;
        private int botDrawRating = 5;
        private int botActionRating = 9;

        private void Start() {
            playerLayer = gameObject.layer;
            score = 0;
            actions = 0;
            if (isBot) {
                if (playerName == "") {
                    SelectRandomBotName();
                }
                StartCoroutine(RunBot());
            }
        }

        public void Setup(int startingHandSize) {
            score = 0;
            actions = 0;
            deck.MoveAllCardsToLayer(playerLayer);
            deck.ShuffleDeck();
            hand.ClearHand();
            DrawCards(startingHandSize);
        }

        public void DrawHand() {
            hand.ClearHand();
        }

        public void DrawCard() {
            Card drawnCard = deck.DrawCard();
            hand.AddCard(drawnCard);
        }

        public void DrawCards(int numCards) {
            for (int i = 0; i < numCards; i++) {
                DrawCard();
            }
        }

        public void ExecuteDrawAction() {
            DrawCard();
            actions--;
        }

        public void ExecutePlayAction(Card cardToPlace, int drawsToAdd, int actionsToAdd) {
            RemoveCardFromHand(cardToPlace);
            DrawCards(drawsToAdd);
            AddAction(actionsToAdd);
            actions--;
            score++;
        }

        public void AddScore(int points = 1) {
            score = score + points;
        }

        public int GetScore() {
            return score;
        }

        public void AddAction(int actionsToAdd = 1) {
            actions += actionsToAdd;
        }

        public void RemoveAction(int actionsToRemove = 1) {
            actions -= actionsToRemove;
        }

        public void SetActions(int numActions) {
            actions = numActions;
        }

        public int GetActions() {
            return actions;
        }

        public bool HasNoActions() {
            return (actions == 0);
        }

        public Card GetSelectedCard() {
            return hand.GetSelectedCard();
        }

        public void SelectCardOnClick(Ray ray) {
            RaycastHit hit;
            LayerMask playerLayerMask = 1 << playerLayer;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayerMask)) {
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject != null) {
                    hand.SelectCardOnClick(clickedObject);
                }
            }
        }

        public void DeselectCard() {
            hand.DeselectCard();
        }

        public void RemoveCardFromHand(Card cardToRemove) {
            hand.RemoveCard(cardToRemove);
        }

        public int GetHandSize() {
            return hand.GetSize();
        }

        public string GetName() {
            return playerName;
        }


        // Bot functions
        private void SelectRandomBotName() {
            List<string> firstNames = new List<string>
            {"Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Hank", "Ivy", "Jack"};
            List<string> lastNames = new List<string>
            {"Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor"};
            string firstName = firstNames[Random.Range(0, firstNames.Count)];
            string lastName = lastNames[Random.Range(0, lastNames.Count)];
            playerName = firstName + " " + lastName;
        }

        private IEnumerator RunBot() {
            while (true) {
                if (actions > 0) {
                    yield return StartCoroutine(ExecuteBotTurn());
                }
                yield return null;
            }
        }

        private IEnumerator ExecuteBotTurn() {
            float pauseDuration = Random.Range(0.5f,2.0f);
            yield return new WaitForSeconds(pauseDuration);
            HardBotPerformAction();
        }

        private void EasyBotPerformAction() {
            // The easy bot randomly selects a play from all viable plays, favoring better plays 
            // based on their weight from playRatings
            List<(Card,CardSlot)> viablePlays = FindViablePlays();
            List<int> playRatings = RateViablePlays(viablePlays);
            int totalRating = playRatings.Sum() + botDrawRating;
            int randomValue = Random.Range(0,totalRating);
            int cumulativeValue = 0;
            for (int i = 0; i < playRatings.Count; i++) {
                cumulativeValue += playRatings[i];
                if (randomValue < cumulativeValue) {
                    Card cardToPlay = viablePlays[i].Item1;
                    CardSlot cardSlot = viablePlays[i].Item2;
                    (bool isValidPlay, int drawsToAdd, int actionsToAdd) = cardSlot.AddCard(cardToPlay);
                    Debug.Log("A.PlayField Add: " + cardToPlay.ToString() + " - " + cardSlot.ToString());
                    if (isValidPlay) {
                        Debug.Log("B.Player play: " + cardToPlay.ToString() + " - " + cardSlot.ToString());
                        ExecutePlayAction(cardToPlay, drawsToAdd, actionsToAdd);
                        return;
                    } else {
                        Debug.Log("C.Play failed for some reason: "+ cardToPlay.ToString() + " - " + cardSlot.ToString());
                    }
                }
            }
            // if the for loop didn't get to the totalRating, then draw a card as the action
            ExecuteDrawAction();
        }

        private void HardBotPerformAction() {
            // The hard bot selects the highest-rated play from all viable plays
            List<(Card,CardSlot)> viablePlays = FindViablePlays();
            List<int> playRatings = RateViablePlays(viablePlays);
            if (playRatings.Count > 0) {
                int maxIndex = playRatings.IndexOf(playRatings.Max());
                Card cardToPlay = viablePlays[maxIndex].Item1;
                CardSlot cardSlot = viablePlays[maxIndex].Item2;
                (bool isValidPlay, int drawsToAdd, int actionsToAdd) = cardSlot.AddCard(cardToPlay);
                Debug.Log("A.PlayField Add: " + cardToPlay.ToString() + " - " + cardSlot.ToString());
                if (isValidPlay) {
                    Debug.Log("B.Player play: " + cardToPlay.ToString() + " - " + cardSlot.ToString());
                    ExecutePlayAction(cardToPlay, drawsToAdd, actionsToAdd);
                    return;
                } else {
                    Debug.Log("C.Play failed for some reason: "+ cardToPlay.ToString() + " - " + cardSlot.ToString());
                }
            } else {
                ExecuteDrawAction();
            }
        }

        private List<(Card, CardSlot)> FindViablePlays() { // TODO: implement
            List<(Card, CardSlot)> viablePlays = new List<(Card, CardSlot)>();
            List<CardSlot> availableCardSlots = playField.GetAvailableCardSlots();
            for (int i = 0; i < availableCardSlots.Count; i++) {
                for (int j = 0; j < hand.cards.Count; j++) {
                    if (availableCardSlots[i].IsValidPlay(hand.cards[j])) {
                        Debug.Log("valid play found: " + hand.cards[j].ToString() + " - " + availableCardSlots[i].ToString());
                        viablePlays.Add((hand.cards[j],availableCardSlots[i]));
                    }
                }
            }
            return viablePlays;
        }

        private List<int> RateViablePlays(List<(Card, CardSlot)> viablePlays) {
            // TODO: improvement - look forward to next play
            // TODO: use machine learning to get best weights
            List<int> playRatings = new List<int>();
            for (int i = 0; i < viablePlays.Count; i++) {
                Card card = viablePlays[i].Item1;
                CardSlot cardSlot = viablePlays[i].Item2;
                (int draws, int actions) = cardSlot.GetPlayDrawsAndActions(card);
                playRatings.Add(draws*botDrawRating + actions*botActionRating + playRating);
            }
            return playRatings;
        }
    }
}
