using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RockPaperScissors : MonoBehaviour
{
    public enum Hand
    {
        Rock,
        Paper,
        Scissors
    };

    public Hand selectedHand;

    private void Start()
    {
        Debug.Log(sizeof(Hand));
        Debug.Log(DoGame(selectedHand) ? "You win" : "You lose");
    }

    private Dictionary<Hand, Hand> gameMap = new Dictionary<Hand, Hand>()
    {
        { Hand.Rock, Hand.Scissors },
        { Hand.Scissors, Hand.Paper },
        { Hand.Paper, Hand.Rock }
    };

    public bool DoGame(Hand hand)
    {
        var rnd = (Hand)Random.Range(0, sizeof(Hand));
        Debug.Log(hand + " " + rnd);
        return gameMap[hand] == rnd;
    }
    
}
