using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newBossPattern", menuName = "Boss/Pattern", order = 1)]
public class BossPattern : ScriptableObject {

    [Header("Pattern actions:")]

    [Tooltip("The actions to be executed in sequence.")]
    public List<BossAction> actions;  

    [Header("Pattern choice criteria:")]

    [Header("NOTE: Check tooltips for usage.")]

    [Tooltip("Chance for this pattern to be randomly selected by a previous pattern when on it's nextPattern list.")]
    public int chance;

    public enum ChoiceType
    {
        Random,
        OnlyTrigger,
        TriggerOrRandom
    }
    [Tooltip("How this pattern may be chosen by a previous one. Random: This pattern can only be randomly chosen, OnlyTrigger: This pattern is only chosen when it's trigger is satisfied, TriggerOrRandom: this pattern will be chosen when it's trigger is satisfied or, if it's not, it can also be randomly chosen. NOTE: if multiple triggers are satisfied, the first pattern on the list will be chosen.")]
    public ChoiceType choiceType;

    public enum Trigger
    {
        PlayerOnRight,
        PlayerOnLeft,
        PlayerOnScreenRight,
        PlayerOnScreenLeft,
        PlayerOnScreenTop,
        PlayerOnScreenBottom,
        PlayerOnScreenDiagonalTop,
        PlayerOnScreenDiagonalBottom,
        PlayerOnScreenAntiDiagonalTop,
        PlayerOnScreenAntiDiagonalBottom
    }
    [Tooltip("The trigger to be satisfied in order for this pattern to be chosen by a previous one. NOTE: The criteria for the trigger to be satisfied is defined on a swich in BossScript.NextPattern, also: in case choiceType is set to Random this won't be used.")]
    public Trigger trigger;

    [Header("Next pattern options:")]
    [Tooltip("List of patterns that can be chosen to happen after this one.")]
    public List<BossPattern> nextPattern;  

}