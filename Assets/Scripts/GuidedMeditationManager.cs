using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GuidedMeditationManager : MonoBehaviour
{
    public TMP_Dropdown durationDropdown;
    public TMP_Dropdown meditationTypeDropdown;
    public Button startButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button exitButton;
    public TMP_Text countdownText;
    public TMP_Text meditationTypeText;
    public TMP_Text summaryText;
    public AudioSource gratitudeAudioSource;
    public AudioSource sleepAudioSource;
    public AudioSource stressReliefAudioSource;
    public AudioSource focusAudioSource;

    public GameObject currentMeditationMenu;
    public GameObject summaryMenu;

    private int durationInSeconds;
    private string selectedMeditationType;
    private bool isPaused = false;
    private int remainingSeconds;
    private float audioPlayPosition = 0;

    void Start()
    {
        startButton.onClick.AddListener(StartMeditation);
        pauseButton.onClick.AddListener(PauseMeditation);
        resumeButton.onClick.AddListener(ResumeMeditation);
        exitButton.onClick.AddListener(ExitMeditation);
    }

    void StartMeditation()
    {
        durationInSeconds = GetDurationInSeconds(durationDropdown.value);
        selectedMeditationType = meditationTypeDropdown.options[meditationTypeDropdown.value].text + " Meditation";
        meditationTypeText.text = selectedMeditationType;
        summaryText.text = selectedMeditationType;

        PlayMeditationAudio(meditationTypeDropdown.options[meditationTypeDropdown.value].text);
        remainingSeconds = durationInSeconds;
        StartCoroutine(CountdownTimer());
    }

    int GetDurationInSeconds(int dropdownIndex)
    {
        switch (dropdownIndex)
        {
            case 0: return 300;  // 5 minutes
            case 1: return 600;  // 10 minutes
            case 2: return 900;  // 15 minutes
            case 3: return 1200; // 20 minutes
            default: return 300;
        }
    }

    void PlayMeditationAudio(string meditationType)
    {
        StopAllAudioSources();
        AudioSource selectedAudioSource = GetSelectedAudioSource(meditationType);

        if (selectedAudioSource != null)
        {
            selectedAudioSource.time = audioPlayPosition;  // Set the playback position
            selectedAudioSource.Play();
        }
        else
        {
            Debug.LogError($"No audio source found for meditation type: {meditationType}");
        }
    }

    IEnumerator CountdownTimer()
    {
        while (remainingSeconds > 0)
        {
            if (!isPaused)
            {
                countdownText.text = $"{remainingSeconds / 60:D2}:{remainingSeconds % 60:D2}";
                yield return new WaitForSeconds(1);
                remainingSeconds--;
            }
            else
            {
                yield return null;
            }
        }

        if (remainingSeconds <= 0)
        {
            EndMeditation();
        }
    }

    void PauseMeditation()
    {
        isPaused = true;
        audioPlayPosition = GetSelectedAudioSource(selectedMeditationType.Replace(" Meditation", "")).time;  // Store the current playback position
        StopAllAudioSources();
    }

    void ResumeMeditation()
    {
        isPaused = false;
        PlayMeditationAudio(meditationTypeDropdown.options[meditationTypeDropdown.value].text);
    }

    void ExitMeditation()
    {
        StopAllAudioSources();
        EndMeditation();
    }

    void EndMeditation()
    {
        StopAllAudioSources();
        currentMeditationMenu.SetActive(false);
        summaryMenu.SetActive(true);
    }

    void StopAllAudioSources()
    {
        gratitudeAudioSource.Stop();
        sleepAudioSource.Stop();
        stressReliefAudioSource.Stop();
        focusAudioSource.Stop();
    }

    AudioSource GetSelectedAudioSource(string meditationType)
    {
        switch (meditationType)
        {
            case "Gratitude":
                return gratitudeAudioSource;
            case "Sleep":
                return sleepAudioSource;
            case "Stress Relief":
                return stressReliefAudioSource;
            case "Focus":
                return focusAudioSource;
            default:
                return null;
        }
    }
}
