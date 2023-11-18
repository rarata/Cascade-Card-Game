using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CascadeCardGame.Cards;

namespace CascadeCardGame.Hands {
    public class Hand : MonoBehaviour
    {
        public List<Card> cards;
        private string sortType;
        private Transform handCenter;
        private Card selectedCard;
        private float cardSpacing = 0.02f;
        private Vector3 cardRotation = -5f*Vector3.forward;
        private Vector3 selectedCardTranslation = 0.04f*Vector3.forward;

        void Awake() {
            handCenter = transform;
            sortType = "value";
            selectedCard = null;
        }

        public void ClearHand() {
            DeselectCard();
            cards.Clear();
            DisplayHand();
        }

        public void AddCard(Card cardToAdd) {
            // Add card to hand and sort it into correct location
            cards.Add(cardToAdd);
            SortHand();
            DisplayHand();
        }

        public void RemoveCard(Card cardToRemove) {
            if (cards.Contains(cardToRemove)) {
                if (selectedCard == cardToRemove) {
                    DeselectCard();
                }
                cards.Remove(cardToRemove);
                DisplayHand();
            } else {
                Debug.LogError("Requested to remove card from hand that is not in hand: {cardToRemove}");
            }
        }

        public Card GetSelectedCard() {
            return selectedCard;
        }

        public void Reorganize(string sortBy) {
            if (sortBy.Equals("suit", StringComparison.OrdinalIgnoreCase) || sortBy.Equals("value", StringComparison.OrdinalIgnoreCase)) {
                sortType = sortBy;
                DisplayHand();
            } else {
                Debug.LogError("Invalid sorting option: {sortBy}. Please specify 'suit' or 'value'.");
            }
        }

        public void SelectCardOnClick(GameObject clickedObject) {
            Card clickedCard = clickedObject.GetComponent<Card>();
            if (clickedCard != null) {
                if (cards.Contains(clickedCard)) {
                    selectedCard = clickedCard;
                    DisplayHand();
                }
            }
        }

        public void DeselectCard() {
            selectedCard = null;
        }

        public int GetSize() {
            return cards.Count;
        }

        private void SortHand() {
            if (sortType.Equals("suit", StringComparison.OrdinalIgnoreCase)) {
                cards.Sort((card1, card2) => card1.suit.CompareTo(card2.suit));
            } else if (sortType.Equals("value", StringComparison.OrdinalIgnoreCase)) {
                cards.Sort((card1, card2) => card1.value.CompareTo(card2.value));
            } else {
                Debug.LogError("Invalid sorting option: {sortType}. Please specify 'suit' or 'value'.");
            }
        }

        private void DisplayHand() {
            // Updates the location of the cards to display the hand
            float handWidth = (cards.Count - 1) * cardSpacing;

            // Calculate the start position for the first card
            Vector3 startPosition = handCenter.position - handCenter.right * (handWidth / 2);

            // Iterate through the cards in the hand and position them in a row
            for (int i = 0; i < cards.Count; i++)
            {
                // Calculate the position for the current card
                Vector3 cardPosition = startPosition + handCenter.right * (i * cardSpacing);

                // Set the card's position and rotation
                cards[i].transform.position = cardPosition;
                cards[i].transform.rotation = handCenter.rotation;
                cards[i].transform.Rotate(cardRotation); // Tilt cards 5deg
            }

            // Shift up currently selected card to highlight it
            if (selectedCard != null) {
                selectedCard.transform.Translate(selectedCardTranslation);
            }
        }
    }
}