using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class AttackStateScript : StateMachineBehaviour
{
    [SerializeField] private float AtkTriggerFrame = 0, AnimLength = -1;
    private float AtkTriggerTime;
    private float AtkTimer = 0;
    private bool AtkTriggered = false;
    private Unit UnitScript;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UnitScript = animator.GetComponent<Unit>();
        if(AnimLength == -1)
        {
            AnimLength = stateInfo.length * animator.speed;
            Debug.Log("Something wrong with the animator length");
        }
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AtkTriggerTime = AtkTriggerFrame / 60;
        if(stateInfo.IsName("Attack") || stateInfo.IsName("AttackOverride") || UnitScript.AbilityAffectedByAtkSpeed)
        {
            animator.speed = UnitScript.AtkSpeed * AnimLength;
        }
        else if(stateInfo.IsName("Ability") && !UnitScript.AbilityAffectedByAtkSpeed)
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
            Spawn(stateInfo);
        }
        else if(GameManager.Instance.State != GameManager.GameState.Battle)
        {
            animator.Play("Attack", 0, 1f);
        }
    }
    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(AtkTriggered == false && GameManager.Instance.State == GameManager.GameState.Battle)
        {
            Spawn(stateInfo);
        }
        AtkTriggered = false;
        AtkTimer = 0;
    }
    private void Spawn(AnimatorStateInfo stateInfo)
    {
        if(stateInfo.IsName("AttackOverride") && UnitScript.AttackOverride)
        {
            UnitScript.SpawnAttack(UnitScript.AttackOverride);
        }
        else if(stateInfo.IsName("Attack") && UnitScript.AttackPrefab)
        {
            if(UnitScript.AttackOverride)
                UnitScript.SpawnAttack(UnitScript.AttackOverride);
            else
                UnitScript.SpawnAttack(UnitScript.AttackPrefab);
        }
        else if(stateInfo.IsName("Ability") && UnitScript.AbilityPrefab)
            UnitScript.SpawnAttack(UnitScript.AbilityPrefab);
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
