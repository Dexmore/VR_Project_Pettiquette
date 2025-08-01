using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoopTrigger : MonoBehaviour
{
    public string poopStateName = "Poop";
    public float minInterval = 30f;
    public float maxInterval = 60f;
    public GameObject PoopObject;
    public Transform spawnAnchor;
    public float backOffset = 0.25f;
    public float yOffset = 0.02f;
    public LayerMask groundMask = ~0;

    private Animator animator;
    private AnimalLogic animalLogic;
    private Coroutine poopLoop;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animalLogic = GetComponent<AnimalLogic>();
    }

    private void OnEnable()
    {
        poopLoop = StartCoroutine(PoopLoop());
    }

    private void OnDisable()
    {
        if (poopLoop != null) StopCoroutine(poopLoop);
    }

    private IEnumerator PoopLoop()
    {
        while (true)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            if (animalLogic != null)
            {
                animalLogic.SetState(AnimalState.Poop);
                yield return StartCoroutine(WaitForPoopAnimation());
                SpawnPoop();
                animalLogic.SetState(AnimalState.Idle);
            }
        }
    }

    private IEnumerator WaitForPoopAnimation()
    {
        var poopHash = Animator.StringToHash(poopStateName);

        // 상태 진입 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(poopStateName))
            yield return null;

        // 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(poopStateName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }

    private void SpawnPoop()
    {
        if (PoopObject == null) return;

        Transform anchor = spawnAnchor != null ? spawnAnchor : transform;
        Vector3 pos = anchor.position - anchor.forward * backOffset;
        pos += Vector3.up * yOffset;

        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out RaycastHit hit, 2f, groundMask))
            pos = hit.point + Vector3.up * yOffset;

        Instantiate(PoopObject, pos, Quaternion.identity).name = "Poop";
    }
}
