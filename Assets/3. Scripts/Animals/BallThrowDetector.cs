using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class BallThrowDetector : MonoBehaviour
{
    [Header("옵션")]
    public AnimalLogic dog;
    public float throwVelocityThreshold = 1.5f;
    public float throwDistanceThreshold = 2f;

    [Header("씬 제한 옵션")]
    public bool restrictToSpecificScene = false; // ✅ 기본 false: 모든 씬에서 동작
    public string allowedSceneName = "WalkScene";

    private Rigidbody rb;
    private Transform player;
    private XRGrabInteractable interactable;

    private bool hasThrown = false;
    private bool isHeld = true;
    private bool releasedThisFrame = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<XRGrabInteractable>();

        interactable.selectEntered.AddListener(args =>
        {
            isHeld = true;
            hasThrown = false;
            releasedThisFrame = false;
            EnsurePlayerRef();
        });

        interactable.selectExited.AddListener(args =>
        {
            isHeld = false;
            releasedThisFrame = true;
        });

        EnsurePlayerRef();
    }

    private void EnsurePlayerRef()
    {
        if (dog != null && dog.Player != null) { player = dog.Player; return; }
        if (player == null && Camera.main != null) player = Camera.main.transform;
    }

    private void Update()
    {
        if (hasThrown || player == null) return;

        if (releasedThisFrame) { releasedThisFrame = false; return; }

        float velocity = rb.velocity.magnitude;
        float distance = Vector3.Distance(transform.position, player.position);

        if (!isHeld && (velocity > throwVelocityThreshold || distance > throwDistanceThreshold))
        {
            hasThrown = true;
            Debug.Log("[BallThrowDetector] 공 던짐 감지됨");

            if (restrictToSpecificScene && SceneManager.GetActiveScene().name != allowedSceneName)
            {
                Debug.Log($"[BallThrowDetector] 현재 씬({SceneManager.GetActiveScene().name})이 허용 씬({allowedSceneName})이 아니라서 통지 생략");
                return;
            }

            NotifyDog();
        }
    }

    private void NotifyDog()
    {
        // 1) 지정된 강아지에게 우선 통지
        if (dog != null)
        {
            Debug.Log($"[BallThrowDetector] 지정된 강아지에 통지: {dog.name}");
            dog.OnBallSoundDetected(gameObject);
            return;
        }

        // 2) 씬 내 강아지 자동 탐색 → 가장 가까운 한 마리에게 통지
        var dogs = FindObjectsOfType<AnimalLogic>(includeInactive: false);
        if (dogs.Length == 0)
        {
            Debug.Log("[BallThrowDetector] 씬에서 AnimalLogic을 찾지 못함");
            return;
        }

        AnimalLogic nearest = null;
        float best = float.MaxValue;
        foreach (var d in dogs)
        {
            float dist = Vector3.Distance(transform.position, d.transform.position);
            if (dist < best) { best = dist; nearest = d; }
        }

        if (nearest != null)
        {
            Debug.Log($"[BallThrowDetector] 가장 가까운 강아지에 통지: {nearest.name} (거리 {best:F2}m)");
            nearest.OnBallSoundDetected(gameObject);
        }
    }
}