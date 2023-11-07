using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CascadeCardGame.CardSlots;
using CascadeCardGame.Cards;

namespace CascadeCardGame.PlayFields {
    public class PlayField : MonoBehaviour
    {
        public GameObject cardSlotPrefab;
        public Transform baseSpawnPoint;
        public float verticalSpacing;
        public float horizontalSpacing;
        public float depthSpacing;
        
        public List<List<CardSlot>> cardSlots = new List<List<CardSlot>>();
        private int fieldSize;
        private CardSlot selectedCardSlot;
        private float selectedCardSlotScaleFactor = 1.2f;
        
        public void SetUpPlayField(int numPlayers) {
            if (numPlayers == 2) {
                fieldSize = 10;
            }
            else if (numPlayers == 3 || numPlayers == 4) {
                fieldSize = 12;
            }
            // Generate and position card slots
            Vector3 slotPosition = new Vector3();
            for (int i = fieldSize; i > 0; i--) {
                int rowNum = fieldSize - i;
                List<CardSlot> thisRow = new List<CardSlot>();
                slotPosition = baseSpawnPoint.position + baseSpawnPoint.forward*verticalSpacing*(i-1) +
                 -baseSpawnPoint.right*horizontalSpacing*(i-1)/2 +
                 -baseSpawnPoint.up*depthSpacing*(i-1);
                for (int j = 0; j < i; j++) {
                    GameObject newCardSlotObject = Instantiate(cardSlotPrefab, slotPosition, baseSpawnPoint.rotation);
                    newCardSlotObject.name = "CardSlot_"+rowNum.ToString()+"_"+j.ToString();
                    newCardSlotObject.transform.SetParent(gameObject.transform);
                    CardSlot thisCardSlot = newCardSlotObject.GetComponent<CardSlot>();
                    thisRow.Add(thisCardSlot);
                    if (i != fieldSize) {
                        thisCardSlot.AddParents(cardSlots[fieldSize-i-1][j],cardSlots[fieldSize-i-1][j+1]);
                    }
                    slotPosition = slotPosition + baseSpawnPoint.right*horizontalSpacing;
                }
                cardSlots.Add(thisRow);
            }
            // set selectedCardSlot to null
            selectedCardSlot = null;
        }

        public void Clear() {
            DeselectCardSlot();
            for (int i = 0; i < cardSlots.Count; i++) {
                for (int j = 0; j < cardSlots[i].Count; j++) {
                    cardSlots[i][j].Clear();
                }
            }
        }

        public void SelectCardSlotOnClick(Ray ray) { // TODO: extract to playField.SelectCardSlot()
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

        public List<CardSlot> GetAvailableCardSlots() {
            List<CardSlot> availableCardSlots = new List<CardSlot>();
            for (int i = 0; i < cardSlots.Count; i++) {
                for (int j = 0; j < cardSlots[i].Count; j++) {
                    if (cardSlots[i][j].isAvailable) {
                        availableCardSlots.Add(cardSlots[i][j]);
                    }
                }
            }
            return availableCardSlots;
        }

        public CardSlot GetSelectedCardSlot() {
            return selectedCardSlot;
        }

        public List<CardSlot> GetRootCardSlots() {
            return cardSlots[0];
        }

        public bool Contains(CardSlot cardSlot) {
            for (int i = 0; i < cardSlots.Count; i++) {
                if (cardSlots[i].Contains(cardSlot)) {
                    return true;
                }
            }
            return false;
        }

        public bool IsFilled() {
            return !cardSlots[fieldSize-1][0].isEmpty;
        }

        public void DeselectCardSlot() {
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
    }
}