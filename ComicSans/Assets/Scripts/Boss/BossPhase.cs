using UnityEngine;

namespace ComicSans.Boss.ActionSystem
{

    // Contains the data for a phase in the Boss ActionSystem.    
    [CreateAssetMenu(fileName = "newBossPhase", menuName = "Boss/Phase", order = 1)]
    public class BossPhase : ScriptableObject {

        [Header("Phase start settings:")]

        [Tooltip("The Boss name to be used for this phase.")]
        public string bossPhaseName = "Boss";

        [Tooltip("The initial position the Boss will appear.")]
        public Vector2 initialPosition = new Vector2(0, 3);

        [Tooltip("Multiplier to the time the Boss will not take damage from hits.")]
        public float invincibilityMultiplier = 1.0f;

        [Tooltip("The AnimationController to be used by the Boss on this phase.")]
        public RuntimeAnimatorController animationController;

        [Tooltip("The initial pattern that will be executed after $invincibilityDuration seconds. Remember that this pattern's nextAction settings will determine the actions that will follow on this phase.")]
        public BossPattern firstPattern;

        [Header("Phase end settings:")]

        [Tooltip("When below this health the Boss will go to the next phase on the BossScript phases list. NOTE: If this is the last phase set this number to a negative value.")]
        public int healthToNextPhase = -1;

    }

}