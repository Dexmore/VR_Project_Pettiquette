using UnityEngine;
using UnityEngine.AI;

public class AnimalLogic : MonoBehaviour
{
    [Header("Animal Settings")]
    public float affinty; // ģ�е�

    [Header("Reference")]
    public Transform player; // �÷��̾��� position
    public Transform neckPos;
    public Transform mouthPos;

    [Header("Follow Setting")]
    public float leashFollowDistance = 3f;

    private IAnimalState currentState;
    private NavMeshAgent nav;
    private Animator anim;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        //ChangeState(new IdleState()); // �ʱ� ���¸� IdleState�� ����
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(IAnimalState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }

    public void SetPosition(Vector3 pos)
    {
        nav.SetDestination(pos);
    }

    public void StopMove()
    {
        nav.isStopped = true;
        nav.ResetPath();
    }

    public bool IsFarFromPlayer()
    {
        if(neckPos == null || player == null)
        {
            return false;
        }

        return Vector3.Distance(neckPos.position, player.position) > leashFollowDistance;
    }

    public float Distance(Vector3 target)
    {
        return Vector3.Distance(transform.position, target);
    }

    public void SetAnimationState(int state)
    {
        anim.SetInteger("State", state);
    }
}
