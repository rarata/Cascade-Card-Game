## Cascade-Card-Game
Unity implementation of the card game Cascade.
This is a card game invented by Ryan Arata that can be played by 2-4 players.  Each player has a full deck of cards and fills out a triangle of cards starting with a pre-played top row.  The player that plays the most cards by the time the triangle is complete wins!

## Features
v0.0 (MVP)
Player versus 1 bot
Only one bot difficulty level
Single game, no matches
Ability to start a new game when done

## Rules
Objective:
- End the match with the most points; points are based on who plays the most cards in each game

Matches:
- Matches are a set of multiple games in which each player starts first an equal number of times

Game Setup:
- 2 Player
  - Each player alternates placing a card from the top of their deck face-up into the top row of the board until 10 cards are placed
  - Player 1 draws 4 cards, Player 2 draws 5 cards
- 3 Player
  - Each player alternates placing a card from the top of their deck face-up into the top row of the board until 12 cards are placed
  - Player 1 draws 4 cards, Player 2 draws 5 cards, and Player 3 draws 6 cards (TBR)
- 4 Player
  - Each player alternates placing a card from the top of their deck face-up into the top row of the board until 12 cards are placed
  - Player 1 draws 3 cards, Player 2 draws 4 cards, Player 3 draws 5 cards, and Player 4 draws 6 cards (TBR)

Play:
- Play progresses in turns starting with Player 1
- At the beginning of a turn, if the player has no cards in their hand, they may draw a card without taking an action
- The player starts the turn with 1 action
- Actions can be used to play a card or draw a card
- A card can be played in the slot below two other cards if its value NOT between the two cards (Aces count as 1)
- When a card is played, the player gets bonuses based on its relationship to the cards its played on top of:
  - For each matching value, the player adds an additional action to their turn
  - For each matching suit, the player draws a card (does not count as an action)
  - If the three cards make three-in-a-row, the player gets 1 draw and 1 action (in addition to any suit-matching draws)
- The turn ends when the player is out of actions
- The game ends when the triangle is complete

Scoring:
- A match is made of N games, where N is the number of players
- At the end of each game, players get a score based on how many more cards they played than the player who played the fewest card, with the player that played the most getting and extra 1 point
  - If two players tie for the most cards played, neither gets the extra point
- At the end of the match, the player with the most points wins