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
        private int actions;
        private bool isBot;
        private int playerLayer;

        private void Start() {
            playerLayer = gameObject.layer;
            score = 0;
            actions = 0;
        }

        public void Setup(int startingHandSize, bool setAsBot = false) {
            isBot = setAsBot;
            score = 0;
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
    }
}
