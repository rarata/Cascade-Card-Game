using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CascadeCardGame.Decks;
using CascadeCardGame.Hands;
using CascadeCardGame.Cards;

// TODO: prune unused methods
namespace CascadeCardGame.Players { 
    public class Player : MonoBehaviour
    {
        public string playerName;
        public bool isBot;
        public Hand hand;
        public Deck deck;
        private int score;
        private int actions;
        private int playerLayer;

        private void Start() {
            playerLayer = gameObject.layer;
            score = 0;
            actions = 0;
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
    }
}
