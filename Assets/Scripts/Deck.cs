using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CascadeCardGame.Cards;

namespace CascadeCardGame.Decks { 
    public class Deck : MonoBehaviour
    {
        public List<Card> fullDeck = new List<Card>(52);
        private Stack<Card> drawDeck = new Stack<Card>();
        private float cardSpacing = 0.002f;
        // Start is called before the first frame update
        public void Awake() {
            ShuffleDeck();
        }

        public void ShuffleDeck() {
            // Create a random number generator
            System.Random rng = new System.Random();

            // Make a copy of the fullDeck to shuffle
            List<Card> shuffledDeck = new List<Card>(fullDeck);

            // Use the Fisher-Yates shuffle algorithm to shuffle the copy
            int n = shuffledDeck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = shuffledDeck[k];
                shuffledDeck[k] = shuffledDeck[n];
                shuffledDeck[n] = value;
            }

            // Clear the drawDeck stack before re-filling it
            drawDeck.Clear();

            // Push the shuffled cards onto the drawDeck stack & move them to the deck location
            foreach (Card card in shuffledDeck)
            {
                drawDeck.Push(card);
                card.transform.position = transform.position;
                card.transform.rotation = transform.rotation;
                card.transform.Translate(Vector3.down*cardSpacing);
                card.transform.Rotate(Vector3.forward*180f);
            }
        }

        public void MoveAllCardsToLayer(int layer) {
            for (int i = 0; i < fullDeck.Count; i++) {
                if (fullDeck[i].gameObject.layer != layer) {
                    fullDeck[i].gameObject.layer = layer;
                }
            }
        }

        public Card DrawCard() {
            return drawDeck.Pop();
        }

        public bool IsEmpty() {
            return (drawDeck.Count == 0);
        }
    }
}
