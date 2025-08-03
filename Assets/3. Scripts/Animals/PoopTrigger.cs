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

    private AnimalState prePoopState;  // ★ Poop 이전 상태 저장

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

            // ★ 특정 상태 중에는 똥 안 싸게 처리 (예시: SitSatisfied, Eat)
            if (animalLogic.CurrentState == AnimalState.SitSatisfied ||
            animalLogic.CurrentState == AnimalState.Eat ||
            animalLogic.CurrentState == AnimalState.Bark ||
            animalLogic.CurrentState == AnimalState.Poop)
            {
                Debug.Log("[PoopTrigger] 현재 상태에서 똥 불가. 대기.");
                continue;
            }

            if (animalLogic != null)
            {
                prePoopState = animalLogic.CurrentState;  // ★ Poop 이전 상태 저장
                animalLogic.SetState(AnimalState.Poop);
                yield return StartCoroutine(WaitForPoopAnimation());
                SpawnPoop();

                Debug.Log($"[PoopTrigger] Poop 이후 원래 상태 복귀: {prePoopState}");
                animalLogic.SetState(prePoopState);  // ★ Poop 이후 복귀
            }
        }
    }

    private IEnumerator WaitForPoopAnimation()
    {
        float poopAnimLength = 5.4f;  // ★ 애니메이션 길이 (Arm_Corgi_Pissing의 실제 길이로 설정)
        Debug.Log($"[PoopCheck] Poop 애니메이션 길이: {poopAnimLength}초");

        yield return new WaitForSeconds(poopAnimLength);

        Debug.Log("[PoopCheck] Poop 애니메이션 종료됨 (시간 기반)");
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
