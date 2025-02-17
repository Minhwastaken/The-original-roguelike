using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class AttackStateScript : StateMachineBehaviour
{
    [SerializeField] private float AtkTriggerFrame = 0, AnimLength = -1;
    private float AtkTriggerTime;
    private float AtkTimer = 0;
    private bool AtkTriggered = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(AnimLength == -1)
        {
            AnimLength = stateInfo.length * animator.speed;
            Debug.Log("Something wrong with the animator length");
        }
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AtkTriggerTime = AtkTriggerFrame / 60;
        if(stateInfo.IsName("Attack") || animator.GetComponent<Unit>().Stats.AbilityAffectedByAtkSpeed)
        {
            animator.speed = animator.GetComponent<Unit>().AtkSpeed * AnimLength;
            if(animator.gameObject.name == "Bow *")
                Debug.Log(animator.speed);
        }
        else if(stateInfo.IsName("Ability") && !animator.GetComponent<Unit>().Stats.AbilityAffectedByAtkSpeed)
        {
            animator.speed = 1;
        }
        else
        {
            Debug.Log("What state we in???");
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AtkTimer += Time.deltaTime * animator.speed;
        if (AtkTimer >= AtkTriggerTime && !AtkTriggered && GameManager.Instance.State == GameManager.GameState.Battle)
        {
            AtkTriggered = true;
            if(stateInfo.IsName("Attack") && animator.GetComponent<Unit>().Stats.Attack)
                animator.GetComponent<Unit>().SpawnAttack(animator.GetComponent<Unit>().Stats.Attack);
            else if(stateInfo.IsName("Ability") && animator.GetComponent<Unit>().Stats.Ability)
                animator.GetComponent<Unit>().SpawnAttack(animator.GetComponent<Unit>().Stats.Ability);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AtkTriggered = false;
        AtkTimer = 0;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
