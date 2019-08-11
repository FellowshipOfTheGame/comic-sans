using UnityEngine;

namespace ComicSans.Boss
{

	// Handles some special transitions in the Boss Animator system.
	public class BossStateMachineHandler : StateMachineBehaviour {

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {			
		//
		//}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

			// Starts the proper ending to the scene when the Boss/Player dies.
			// Should be setup to be called by the Boss animator after the Win and Die animation states.
			if(stateInfo.IsName("Win") ) 
				GameController.instance.StartEndScene(false);
			else if(stateInfo.IsName("Die"))
				GameController.instance.StartEndScene(true);

		}

		// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}
	}

}