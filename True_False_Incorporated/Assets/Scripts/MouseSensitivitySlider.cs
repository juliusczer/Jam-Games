using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MouseSensitivitySlider : MonoBehaviour {
    private const string PrefsKey = "MouseSensitivity";

    public PlayerMovement playerMovement;
    public float minSensitivity = 0.5f;
    public float maxSensitivity = 10f;
    public float defaultSensitivity = 2.5f;

    private Slider slider;

    void Awake() {
        slider = GetComponent<Slider>();
        slider.minValue = minSensitivity;
        slider.maxValue = maxSensitivity;
    }

    void Start() {
        if (playerMovement == null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                playerMovement = player.GetComponent<PlayerMovement>();
            }
        }

        float startingValue = playerMovement != null ? playerMovement.mouseSensitivity : defaultSensitivity;
        float saved = PlayerPrefs.GetFloat(PrefsKey, startingValue);

        slider.value = saved;
        ApplySensitivity(saved);

        slider.onValueChanged.AddListener(ApplySensitivity);
    }

    void ApplySensitivity(float value) {
        if (playerMovement != null) {
            playerMovement.mouseSensitivity = value;
        }

        PlayerPrefs.SetFloat(PrefsKey, value);
        PlayerPrefs.Save();
    }

    void OnDestroy() {
        slider.onValueChanged.RemoveListener(ApplySensitivity);
    }
}