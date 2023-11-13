using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CascadeCardGame.Cards;

namespace CascadeCardGame.CardSlots {
    public class CardSlot : MonoBehaviour
    {
        private CardSlot leftParent;
        private CardSlot rightParent;
        public bool isAvailable = true;
        private bool isRoot = true;
        public bool isEmpty = true;
        private Card card = null;
        private Vector3 cardOffset = new Vector3(0.0f, 0.001f, 0.0f);
        private MeshRenderer meshRenderer;
        private Vector3 originalPosition;
        private Vector3 emphasisOffset = new Vector3(0.0f, 0.01f, 0.0f);

        void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
            originalPosition = transform.position;
        }
        
        void Update()
        {   
            // Check for updates in availability
            if (!isRoot) {
                if (!leftParent.isEmpty && !rightParent.isEmpty && isEmpty) {
                    isAvailable = true;
                }
            }
            // Display the slot and emphasize it if available
            UpdateDisplay();
        }

        public void AddParents(CardSlot leftParentToAdd, CardSlot rightParentToAdd) {
            leftParent = leftParentToAdd;
            rightParent = rightParentToAdd;
            isRoot = false;
            isAvailable = false;
        }

        public void Clear() {
            // Resets the slot and removes attached cards
            isAvailable = isRoot;
            isEmpty = true;
            card = null;
        }

        private void UpdateDisplay() {
            meshRenderer.enabled = isAvailable;
            if (isAvailable) {
                Emphasize();
            } else {
                DeEmphasize();
            }
        }

        public void Emphasize() {
            // Shifts the slot up a bit
            transform.position = originalPosition + emphasisOffset;
        }

        public void DeEmphasize() {
            // Shifts the slot back to its natural position
            transform.position = originalPosition;
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

        public (int, int) AddCard(Card cardToAdd) {
            // returns a boolean of success/failure, an int for number of draws to add, and an int for number of actions to add
            // also physically moves the card gameObject to the card slot
            if (IsValidPlay(cardToAdd)) {
                card = cardToAdd;
                isEmpty = false;
                isAvailable = false;
                card.transform.position = originalPosition + cardOffset;
                card.transform.rotation = transform.rotation;
                card.gameObject.layer = LayerMask.NameToLayer("PlaySurface");
                (int newDraws, int newActions) = GetPlayDrawsAndActions(cardToAdd);
                return (newDraws, newActions);
            } else {
                Debug.Log("Failed to add card to slot: " + cardToAdd.ToString() + " - " + gameObject.ToString());
                return (0, 0);
            }
        }

        public (int, int) GetPlayDrawsAndActions (Card cardToCheck) {
            int newDraws = 0;
            int newActions = 0;
            newDraws += NumMatchingSuits(cardToCheck);
            newActions += NumMatchingNumbers(cardToCheck);
            if (IsThreeInARow(cardToCheck)) {
                newDraws++;
                newActions++;
            }
            return (newDraws, newActions);
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
    }
}