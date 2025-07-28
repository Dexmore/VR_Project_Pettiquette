using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HelpUISetting : MonoBehaviour
{
    [Header("UI Sprite Setting")]
    public Image ImageSetting;
    public Sprite[] images;

    [Header("UI Text Setting")]
    public TextMeshProUGUI textSetting;

    private string[] texts;

    [Header("Voice Narration")]
    public AudioSource audioSource;
    public AudioClip[] voiceClips;
    public Button muteVoiceButton;
    public TextMeshProUGUI muteButtonText;

    private bool isMuted = false;
    private const string MutePrefKey = "HelpUI_Mute";

    [Header("Navigation Buttons")]
    public Button prevButton;
    public Button nextButton;

    [Header("UI Button")]
    public Button button;

    [Header("���� ���")]
    public GameObject closeTarget;

    private int currentIndex = 0;

    private void Start()
    {
        // Load mute state from PlayerPrefs
        isMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;

        texts = new string[]
        {
            "����\n�������� ����� ��\n�ִ� ���ִ� �����̿���!\n\n��� �ֳ���?\n1. ���̸� ��(��Ʈ�ѷ�)���� ����.\n2. ��׸� ���� �ø��� ��ư�� �� ������!",
            "��\n�������� �躯�� �ϸ�\n�� ������ �����ϰ� ������ �� �־��!\n\n��� ġ����?\n1. ���� ��(��Ʈ�ѷ�)���� ����.\n2. �躯�� ���� ��¦ ���� ���,\n�ڵ����� ������ ġ������!",
            "�Ը���\n�������� ���ų� ������� ��,\n�ٸ� ����̳� ������ ��ȣ�ϱ� ����\n�� �Ը����� �����ؿ�!\n\n��� �����ϳ���?\n1. �Ը����� ��(��Ʈ�ѷ�)���� ����.\n2. ������ �󱼿� ��¦ ���� ���,\n�ڵ����� ����ſ�!",
            "��\n�������� ����� �� ����� �� �־��!\n1. ���������� �������� ����.\n2. ������ ��� ������ ������ ������ �ָ鼭 ������ �ؿ�!\n3. ������ �ϸ鼭 ��ư�� ����.\n4. �������� �����ɴϴ�!",
            "����\n�������� ��å�� �� �ʼ� �������Դϴ�!\n1. ���������� �������� ����.\n2. ������ �� ������ ��� �ڵ����� ������ �˴ϴ�!",
            "��׸�\n�������� ��Ḧ ��� �׸��Դϴ�!\n1. ���������� �������� ����.\n2. ���ϴ� ��ġ�� ������ �ξ��."
        };

        prevButton.onClick.AddListener(OnPrevHelp);
        nextButton.onClick.AddListener(OnNextHelp);
        muteVoiceButton.onClick.AddListener(OnToggleMute);

        UpdateHelpUI();

        button.onClick.AddListener(() =>
        {
            if (closeTarget != null)
            {
                closeTarget.gameObject.SetActive(false);
                UIManager.Instance.CloseAllSubCanvas();
            }
        });
    }

    private void UpdateHelpUI()
    {
        if (ImageSetting != null && images.Length > currentIndex)
            ImageSetting.sprite = images[currentIndex];

        if (textSetting != null && texts.Length > currentIndex)
            textSetting.text = texts[currentIndex];

        UpdateVoicePlayback();
        UpdateMuteButtonText();
        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < texts.Length - 1;
    }

    private void UpdateVoicePlayback()
    {
        if (audioSource == null || voiceClips.Length <= currentIndex || voiceClips[currentIndex] == null)
            return;

        audioSource.Stop();  // ���� ��� ����

        audioSource.clip = voiceClips[currentIndex];
        audioSource.volume = AudioManager.Instance != null
            ? AudioManager.Instance.GetSFXVolume(SFXCategory.Voice)
            : 1f;

        audioSource.mute = isMuted;

        if (!isMuted)
            audioSource.Play();
    }


    public void OnNextHelp()
    {
        if (currentIndex < texts.Length - 1)
        {
            currentIndex++;
            UpdateHelpUI();
        }
    }

    public void OnPrevHelp()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateHelpUI();
        }
    }

    public void OnToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt(MutePrefKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateHelpUI(); // ���� ���¿� �°� ��ü ����
    }


    private void UpdateMuteButtonText()
    {
        if (muteButtonText != null)
            muteButtonText.text = isMuted ? "����" : "���Ұ�";
    }
}