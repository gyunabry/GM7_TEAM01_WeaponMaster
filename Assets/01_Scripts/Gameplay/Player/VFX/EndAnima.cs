using UnityEngine;

public class EndAnima : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        VFXAdd vfxm = animator.GetComponent<VFXAdd>();
        vfxm.ReturnToPool();
    }


}
