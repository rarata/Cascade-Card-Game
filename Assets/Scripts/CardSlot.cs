using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleCardGame.Cards;

namespace TriangleCardGame.CardSlots {
    public class CardSlot : MonoBehaviour
    {
        public CardSlot leftParent;
        public CardSlot rightParent;
        public bool isAvailable;
        private bool isRoot = false;
        public bool isEmpty = true; // TODO: make private
        public Card card = null; // TODO: make private
        private Vector3 offset = new Vector3(0.0f, 0.001f, 0.0f);
        private MeshRenderer meshRenderer;

        void Start() {
            isAvailable = false;
            if (leftParent == null && rightParent == null) {
                isRoot = true;
                isAvailable = true;
            }
            meshRenderer = GetComponent<MeshRenderer>();
        }
        
        void Update()
        {   
            // Check for updates in availability
            if (!isRoot) {
                if (!leftParent.isEmpty && !rightParent.isEmpty && isEmpty) {
                    isAvailable = true;
                }
            }
            // Display the slot if available
            meshRenderer.enabled = isAvailable;
        }

        public bool IsValidPlay(Card cardToCheck) {
            if (isRoot & isEmpty) { return true; }
            int leftValue = leftParent.card.value;
            int rightValue = rightParent.card.value;
            int checkValue = cardToCheck.value;
            if (!isAvailable) {
                return false;
            }
            if (checkValue >= leftValue && checkValue >= rightValue) {
                return true;
            }
            if (checkValue <= leftValue && checkValue <= rightValue) {
                return true;
            }
            return false; // check value is between left and right; invalid play
        }

        public (bool, int, int) AddCard(Card cardToAdd) {
            // returns a boolean of success/failure, an int for number of draws to add, and an int for number of actions to add
            // also physically moves the card gameObject to the card slot
            int newDraws = 0;
            int newActions = 0;
            if (IsValidPlay(cardToAdd)) {
                card = cardToAdd;
                isEmpty = false;
                isAvailable = false;
                card.transform.position = transform.position + offset;
                card.transform.rotation = transform.rotation;
                newDraws += NumMatchingSuits(card);
                newActions += NumMatchingNumbers(card);
                if (IsThreeInARow(card)) {
                    newDraws++;
                    newActions++;
                }
                return (true, newDraws, newActions);
            } else {
                return (false, 0, 0);
            }
        }

        private int NumMatchingSuits(Card cardToCheck) {
            // since this is private, it cannot be called in an invalid situation; no need to check validity
            if (isRoot) { return 0; }
            int num = 0;
            num += (leftParent.card.suit == cardToCheck.suit) ? 1 : 0;
            num += (rightParent.card.suit == cardToCheck.suit) ? 1 : 0;
            return num;
        }

        private int NumMatchingNumbers(Card cardToCheck) {
            if (isRoot) { return 0; }
            int num = 0;
            num += (leftParent.card.value == cardToCheck.value) ? 1 : 0;
            num += (rightParent.card.value == cardToCheck.value) ? 1 : 0;
            return num;
        }

        private bool IsThreeInARow(Card cardToCheck) {
            if (isRoot) { return false; }
            int leftValue = leftParent.card.value;
            int rightValue = rightParent.card.value;
            int checkValue = cardToCheck.value;
            if (leftValue+1 == rightValue) {
                if (checkValue+1 == leftValue || checkValue-1 == rightValue) {
                    return true;
                }
            }
            if (rightValue+1 == leftValue) {
                if (checkValue+1 == rightValue || checkValue-1 == leftValue) {
                    return true;
                }
            }
            return false;
        }

        public void Clear() {
            // Resets the slot and removes attached cards
            isAvailable = false;
            isRoot = false;
            isEmpty = true;
            card = null;
            Start();
        }
    }
}