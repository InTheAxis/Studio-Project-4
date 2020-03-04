using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreCounter
{
    public static int interactionScore { get; private set; } = 0;
    public static int itemScore { get; private set; } = 0;
    public static int reviveScore { get; private set; } = 0;
    public static int total { get => interactionScore + itemScore + reviveScore; }

    public static void Reset()
    {
        interactionScore = itemScore = reviveScore = 0;
    }

    public static void addInteractionScore(int amt)
    {
        interactionScore += amt;
    }
    public static void addItemScore(int amt)
    {
        itemScore += amt;
    }
    public static void addReviveScore(int amt)
    {
        reviveScore += amt;
    }
}
