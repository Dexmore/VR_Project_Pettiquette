using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AnimalType { Small, Medium, Large }
public enum AnimalState { Idle, FreeWalk, FollowPlayer, LeashFollow, GoToFeed, Eat, Fetch, SitSatisfied, Bark }
public enum PetAnimation { Idle, Walk, EatStart, EatEnd, SitStart, SitEnd, Fetch, Bark }

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AnimalLogic : MonoBehaviour
{
    [Header("고유 식별자")]
    public string petId;

    [Header("Animal Type & CurrentState")]
    public AnimalType type = AnimalType.Small;
    public AnimalState currentState = AnimalState.Idle;

    [Header("Check Player")]
    public Transform player;

    [Header("Free Walk Setting")]
    public float walkRadius = 5f;
    public float checkInterval = 5f;

    [Header("FreeWalk Delay")]
    public float idleToWalkDelay = 3f;
    private float idleTimer = 0f;

    [Header("Follow 설정")]
    public float callDistance = 1.5f;

    [Header("Leash 설정")]
    [SerializeField] private bool isLeashed;
    public Transform NeckPos;
    public float leashFollowDistance = 2f;

    [Header("Leash 연결 대상")]
    public Transform leashTargetTransform;

    [Header("Fetch System")]
    public Transform mouthPos;

    [Header("Feed Settings")]
    public float eatDuration = 2f;

    [Header("Bark Settings")]
    public float barkDistance = 3f;              // 짖기 감지 거리
    public LayerMask barkTargetLayer;            // 짖기 대상 레이어 마스크

    private NavMeshAgent nav;
    private Animator anim;
    private AudioClipInventory audios;
    private AudioSource audioSource;

    private float behaviourTimer;
    private float leashWalkTimer;
    private float sitWaitTimer;

    private AnimalFetchHandler fetchHandler;
    private AnimalFeedHandler feedHandler;
    private AnimalAnimation animationHandler;

    // ---------- Bark / 이동 관련 필드 ----------
    private Transform currentBarkTarget = null;
    private float barkSoundCooldown = 2f;        // 짖음 재생 간격(초)
    private float nextBarkAt = 0f;               // 전역 쿨다운 타임스탬프

    private Vector3 prevLeashTargetPos;              // 줄 기준 대상(플레이어/지정 타겟)의 이전 위치
    private float leashTargetMoveThreshold = 0.2f;   // 움직임 판단 임계값 (m/s)
    private float leashBarkDistanceMul = 1.1f;       // 줄 억제 민감도(거리 배수)

    // ---------- LeashFollow에서 사용 ----------
    private Vector3 previousPlayerPos;
    private float playerMoveThreshold = 0.01f;   // 매우 미세한 흔들림도 잡는 기존 임계값(LeashFollow 내부용)

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audios = GetComponent<AudioClipInventory>();
        audioSource = GetComponent<AudioSource>();

        fetchHandler = new AnimalFetchHandler(this);
        feedHandler = new AnimalFeedHandler(this);
        animationHandler = new AnimalAnimation(anim);

        if (player == null)
        {
            var xrCam = Camera.main;
            if (xrCam != null)
                player = xrCam.transform;
            else
                Debug.LogWarning("[AnimalLogic] XR Main Camera를 찾을 수 없습니다.");
        }
    }

    private void Start()
    {
        ChangeState(AnimalState.Idle);
        nav.updateRotation = false;

        // 첫 프레임 초기화
        var leashTarget = leashTargetTransform != null ? leashTargetTransform : player;
        if (leashTarget != null)
        {
            prevLeashTargetPos = leashTarget.position;
            previousPlayerPos = leashTarget.position;
        }
    }

    private void Update()
    {
        if (currentState == AnimalState.GoToFeed || currentState == AnimalState.Eat)
        {
            feedHandler.UpdateFeed();
            return;
        }

        if (currentState == AnimalState.Bark)
        {
            CheckBarkTarget(); // 타겟이 사라지면 여기서 해제
            return;
        }

        // Bark가 아닐 때만 나머지 동작 실행
        CheckBarkTarget();
        UpdateStateSwitch();
        UpdateRotation();
    }

    // ---------- 가장 가까운 타겟 선택 ----------
    private Transform GetClosestTarget(Vector3 from, Collider[] hits)
    {
        Transform best = null;
        float bestSqr = float.MaxValue;
        for (int i = 0; i < hits.Length; i++)
        {
            float d = (hits[i].transform.position - from).sqrMagnitude;
            if (d < bestSqr)
            {
                bestSqr = d;
                best = hits[i].transform;
            }
        }
        return best;
    }

    // ---------- Bark 타겟/상태 관리 (개선) ----------
    private void CheckBarkTarget()
    {
        Transform leashTarget = leashTargetTransform != null ? leashTargetTransform : player;

        float distToLeashTarget = 0f;
        float leashTargetSpeed = 0f;
        if (leashTarget != null)
        {
            distToLeashTarget = Vector3.Distance(transform.position, leashTarget.position);
            leashTargetSpeed = (leashTarget.position - prevLeashTargetPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            prevLeashTargetPos = leashTarget.position;
        }

        bool leashTargetMoving = leashTargetSpeed > leashTargetMoveThreshold;
        bool leashTooTight = distToLeashTarget > leashFollowDistance * leashBarkDistanceMul;

        // ★ Leash 억제: 플레이어가 '꽤' 움직이고(AND) 줄이 '확실히' 당겨질 때만 억제
        if (IsLeashed && (leashTargetMoving && leashTooTight))
        {
            if (currentState == AnimalState.Bark)
                ChangeState(AnimalState.LeashFollow);   // 따라오도록 전환
            return; // 이 프레임에는 Bark 검사/재생 안 함
        }

        // 타겟 감지
        Collider[] hits = Physics.OverlapSphere(transform.position, barkDistance, barkTargetLayer);
        // Debug.Log($"[Bark] hits: {hits.Length}");

        if (hits.Length > 0)
        {
            Transform newTarget = GetClosestTarget(transform.position, hits); // 가장 가까운 타겟 선택

            // ★ 타겟 전환 처리: 이전 타겟 정리 + 쿨다운 초기화
            if (currentBarkTarget != null && currentBarkTarget != newTarget)
            {
                var prevSurprise = currentBarkTarget.GetComponent<AISurpriseHandler>();
                if (prevSurprise != null && prevSurprise.IsSurprised)
                    prevSurprise.EndSurpriseImmediately();

                nextBarkAt = 0f; // 새 타겟에 즉시 짖을 수 있도록
            }

            currentBarkTarget = newTarget;

            if (currentState != AnimalState.Bark)
                ChangeState(AnimalState.Bark);

            // 전역 쿨다운으로 반복 보장
            if (Time.time >= nextBarkAt)
            {
                PlayBarkSound();
                nextBarkAt = Time.time + barkSoundCooldown;

                var surprise = currentBarkTarget.GetComponent<AISurpriseHandler>();
                if (surprise != null) surprise.TriggerSurprise();
            }
        }
        else
        {
            if (currentBarkTarget != null)
            {
                var surprise = currentBarkTarget.GetComponent<AISurpriseHandler>();
                if (surprise != null && surprise.IsSurprised)
                    surprise.EndSurpriseImmediately();
                currentBarkTarget = null;
            }

            if (currentState == AnimalState.Bark)
                ChangeState(IsLeashed ? AnimalState.LeashFollow : AnimalState.FreeWalk);
        }
    }

    private void PlayBarkSound()
    {
        if (audios != null && audios.BarkSound.Length > 0 && audioSource != null)
        {
            int i = Random.Range(0, audios.BarkSound.Length);
            audioSource.PlayOneShot(audios.BarkSound[i]);
            Debug.Log("[BarkSound] 재생됨");
        }
    }

    private void UpdateStateSwitch()
    {
        switch (currentState)
        {
            case AnimalState.Idle:
                UpdateIdle();
                break;
            case AnimalState.FreeWalk:
                UpdateWalk();
                break;
            case AnimalState.FollowPlayer:
                UpdateFollow();
                break;
            case AnimalState.LeashFollow:
                UpdateLeashFollow();
                break;
            case AnimalState.Fetch:
                fetchHandler.UpdateFetch();
                break;
            case AnimalState.SitSatisfied:
                WaitForPatting();
                break;
        }
    }

    public void ChangeState(AnimalState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        EnterStateSwitch(newState);
    }

    private void EnterStateSwitch(AnimalState state)
    {
        switch (state)
        {
            case AnimalState.Idle:
                nav.isStopped = true;
                nav.ResetPath();
                idleTimer = 0f;
                animationHandler.SetAnimation(PetAnimation.Idle);
                break;

            case AnimalState.FreeWalk:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
                MoveRandomPoint();
                break;

            case AnimalState.FollowPlayer:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
                break;

            case AnimalState.LeashFollow:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
                leashWalkTimer = 0f;
                MoveRandomPointInLeashArea();
                break;

            case AnimalState.GoToFeed:
                feedHandler.EnterFeed();
                break;

            case AnimalState.Eat:
                feedHandler.EnterEat();
                if (audios != null && audios.EatSound != null && audioSource != null)
                    audioSource.PlayOneShot(audios.EatSound);
                break;

            case AnimalState.Fetch:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Fetch);
                break;

            case AnimalState.SitSatisfied:
                nav.isStopped = true;
                nav.ResetPath();
                sitWaitTimer = 0f;
                animationHandler.SetSitPhase(1); // SitStart
                break;

            case AnimalState.Bark:
                nav.isStopped = true;
                nav.ResetPath();
                animationHandler.SetAnimation(PetAnimation.Bark);

                // [옵션] 입장 즉시 1회 재생을 원하면 아래 한 줄의 주석을 해제하세요.
                // if (Time.time >= nextBarkAt) { PlayBarkSound(); nextBarkAt = Time.time + barkSoundCooldown; }

                // 사람 반응은 즉시 가능
                if (currentBarkTarget != null)
                {
                    AISurpriseHandler surprise = currentBarkTarget.GetComponent<AISurpriseHandler>();
                    if (surprise != null) surprise.TriggerSurprise();
                }
                break;
        }
    }

    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleToWalkDelay)
        {
            idleTimer = 0f;
            ChangeState(AnimalState.FreeWalk);
        }
    }

    private void UpdateWalk()
    {
        behaviourTimer += Time.deltaTime;

        if (!nav.pathPending && nav.remainingDistance <= 0.3f)
        {
            ChangeState(AnimalState.Idle);
        }

        if (behaviourTimer >= checkInterval)
        {
            MoveRandomPoint();
            behaviourTimer = 0f;
        }
    }

    private void UpdateFollow()
    {
        if (Vector3.Distance(transform.position, player.position) > callDistance)
        {
            nav.SetDestination(player.position);
        }
        else
        {
            ChangeState(AnimalState.Idle);
        }
    }

    private void UpdateLeashFollow()
    {
        Transform leashTarget = leashTargetTransform != null ? leashTargetTransform : player;
        float distance = Vector3.Distance(transform.position, leashTarget.position);

        float playerSpeed = (leashTarget.position - previousPlayerPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        bool isPlayerMoving = playerSpeed > playerMoveThreshold;
        previousPlayerPos = leashTarget.position;

        if (distance <= leashFollowDistance && !isPlayerMoving)
        {
            nav.isStopped = true;
            nav.ResetPath();
            animationHandler.SetAnimation(PetAnimation.Idle);
            return;
        }

        float baseSpeed = 2f;
        float maxSpeed = 6f;
        float speedMultiplier = Mathf.Clamp01((distance - leashFollowDistance) / leashFollowDistance);
        nav.speed = Mathf.Lerp(baseSpeed, maxSpeed, speedMultiplier);

        if (distance > leashFollowDistance)
        {
            Vector3 dir = (transform.position - leashTarget.position).normalized;
            Vector3 clamped = leashTarget.position + dir * leashFollowDistance * 0.9f;

            if (NavMesh.SamplePosition(clamped, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                nav.isStopped = false;
                nav.SetDestination(hit.position);
                animationHandler.SetAnimation(PetAnimation.Walk);
            }
        }
        else
        {
            leashWalkTimer += Time.deltaTime;

            if (!nav.pathPending && nav.remainingDistance <= 0.3f)
            {
                if (leashWalkTimer >= checkInterval)
                {
                    MoveRandomPointInLeashArea();
                    leashWalkTimer = 0f;
                }
            }
        }
    }

    private void WaitForPatting()
    {
        sitWaitTimer += Time.deltaTime;

        if (sitWaitTimer > 2f && sitWaitTimer < 2.1f)
        {
            animationHandler.SetSitPhase(2); // SitLoop
        }

        if (sitWaitTimer > 10f)
        {
            nav.isStopped = true;
            nav.ResetPath();
            animationHandler.SetSitPhase(3); // SitEnd
            StartCoroutine(WaitAndFreeWalk(1.2f));
        }
    }

    private IEnumerator WaitAndFreeWalk(float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeState(AnimalState.FreeWalk);
    }

    private void UpdateRotation()
    {
        if (nav.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = nav.velocity.normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
            }
        }
    }

    private void MoveRandomPoint()
    {
        for (int i = 0; i < 10; i++) // 최대 10회 시도
        {
            Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized * walkRadius;
            Vector3 targetPos = transform.position + new Vector3(circle.x, 0f, circle.y);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
            {
                if ((hit.position - transform.position).magnitude >= 1f)
                {
                    nav.SetDestination(hit.position);
                    return;
                }
            }
        }
    }

    private void MoveRandomPointInLeashArea()
    {
        Transform leashCenter = leashTargetTransform != null ? leashTargetTransform : player;

        Vector3 rndDir = UnityEngine.Random.insideUnitSphere;
        rndDir.y = 0;
        rndDir.Normalize();

        float radius = UnityEngine.Random.Range(leashFollowDistance * 0.3f, leashFollowDistance * 0.95f);
        Vector3 targetPos = leashCenter.position + rndDir * radius;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            if (Vector3.Distance(leashCenter.position, hit.position) <= leashFollowDistance)
            {
                nav.SetDestination(hit.position);
            }
        }
    }

    public void SetLeashed(bool on)
    {
        isLeashed = on;
        Debug.Log($"[SetLeashed] 줄 상태 변경됨: {on}");
        if (on)
        {
            if (Vector3.Distance(transform.position, player.position) > leashFollowDistance)
            {
                Vector3 dir = (transform.position - player.position).normalized;
                Vector3 clamped = player.position + dir * leashFollowDistance * 0.9f;

                if (NavMesh.SamplePosition(clamped, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    nav.SetDestination(hit.position);
                }
            }

            ChangeState(AnimalState.LeashFollow);
        }
        else
        {
            ChangeState(AnimalState.Idle);
        }
    }

    public void OnBallSoundDetected(GameObject ball)
        => Debug.Log($"[AnimalLogic] OnBallSoundDetected 실행됨: {ball.name}");

    public void OnBallSpawned(GameObject ball) => fetchHandler.OnBallSpawned(ball);
    public void OnFeedSpawned(GameObject feed) => feedHandler.OnFeedSpawned(feed);

    public NavMeshAgent Agent => nav;
    public Animator animator => anim;
    public Transform Player => player;
    public Transform Mouth => mouthPos;
    public AnimalAnimation AnimationHandler => animationHandler;
    public float EatDuration => eatDuration;
    public AnimalState CurrentState => currentState;
    public void SetState(AnimalState state) => ChangeState(state);
    public bool IsLeashed => isLeashed;
}