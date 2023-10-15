using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleCardGame.Decks;
using TriangleCardGame.Hands;
using TriangleCardGame.Cards;

namespace TriangleCardGame.Players { 
    public class Player : MonoBehaviour
    {
        public string playerName;
        public Hand hand;
        public Deck deck;
        private int score;
        public int cardsPlayedThisGame;
        private int actions;

        private void Start() {
            score = 0;
            actions = 0;
        }

        public void Setup(int startingHandSize) {
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

        public void UpdateScore(int points) {
            score = score + points;
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

        public bool HasNoActions() {
            return (actions == 0);
        }

        public Card GetSelectedCard() {
            return hand.GetSelectedCard();
        }

        public void SelectCardOnClick(GameObject clickedObject) {
            hand.SelectCardOnClick(clickedObject);
        }

        public void DeselectCard() {
            hand.DeselectCard();
        }

        public void RemoveCardFromHand(Card cardToRemove) {
            hand.RemoveCard(cardToRemove);
        }
    }
}
