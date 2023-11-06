using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CascadeCardGame.Cards {
    public enum Suit {
        Clubs,
        Spades,
        Diamonds,
        Hearts
    }

    public class Card : MonoBehaviour {
        public int value;
        public Suit suit;
    }
}