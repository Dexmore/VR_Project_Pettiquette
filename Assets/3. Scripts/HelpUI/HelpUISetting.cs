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

    [Header("닫을 대상")]
    public GameObject closeTarget;

    private int currentIndex = 0;

    private void Start()
    {
        // Load mute state from PlayerPrefs
        isMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;

        texts = new string[]
        {
            "먹이\n강아지가 배고플 때\n주는 맛있는 간식이에요!\n\n어떻게 주나요?\n1. 먹이를 손(컨트롤러)으로 들어요.\n2. 밥그릇 위에 올리고 버튼을 꾹 눌러요!",
            "삽\n강아지가 배변을 하면\n이 삽으로 깨끗하게 정리할 수 있어요!\n\n어떻게 치울까요?\n1. 삽을 손(컨트롤러)으로 들어요.\n2. 배변물 위에 살짝 갖다 대면,\n자동으로 말끔히 치워져요!",
            "입마개\n강아지가 놀라거나 흥분했을 때,\n다른 사람이나 동물을 보호하기 위해\n이 입마개를 착용해요!\n\n어떻게 착용하나요?\n1. 입마개를 손(컨트롤러)으로 들어요.\n2. 강아지 얼굴에 살짝 갖다 대면,\n자동으로 착용돼요!",
            "공\n강아지와 놀아줄 때 사용할 수 있어요!\n1. 오른손으로 아이템을 들어요.\n2. 손으로 들고 앞으로 던지는 느낌을 주면서 스윙을 해요!\n3. 스윙을 하면서 버튼을 떼요.\n4. 강아지가 가져옵니다!",
            "목줄\n강아지와 산책갈 때 필수 아이템입니다!\n1. 오른손으로 아이템을 들어요.\n2. 강아지 목에 가져다 대면 자동으로 착용이 됩니다!",
            "밥그릇\n강아지의 사료를 담는 그릇입니다!\n1. 오른손으로 아이템을 들어요.\n2. 원하는 위치에 가져다 두어요."
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

        audioSource.Stop();  // 현재 재생 중지

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

        UpdateHelpUI(); // 현재 상태에 맞게 전체 갱신
    }


    private void UpdateMuteButtonText()
    {
        if (muteButtonText != null)
            muteButtonText.text = isMuted ? "해제" : "음소거";
    }
}