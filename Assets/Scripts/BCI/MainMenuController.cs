using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BCI
{
    public class MainMenuController : MonoBehaviour
    {
        public Button leftButton;
        public Button rightButton;
        public Toggle armToggle;
        public Toggle legToggle;
        public Dropdown armDropdown;
        public Dropdown legDropdown;
        public Button startButton;

        public string[] armSceneNames = { "DoorTask", "ElevatorTask" };
        public string[] legSceneNames = { "BallTask", "TrashCanTask" };

        static readonly Color SelectedColour = new Color(0.38f, 0.69f, 0.42f);
        static readonly Color UnselectedColour = new Color(0.788f, 0.78f, 0.671f);
        static readonly Color SelectedBorderColour = new Color(0.35f, 0.16f, 0.05f);

        void Start()
        {
            leftButton.onClick.AddListener(() => SetMirrored(true));
            rightButton.onClick.AddListener(() => SetMirrored(false));
            armToggle.onValueChanged.AddListener(OnArmToggleChanged);
            legToggle.onValueChanged.AddListener(OnLegToggleChanged);
            armDropdown.onValueChanged.AddListener(_ => UpdateArmSelection());
            legDropdown.onValueChanged.AddListener(_ => UpdateLegSelection());
            startButton.onClick.AddListener(StartSession);

            RefreshButtonHighlights();

            if (armToggle.isOn)
                UpdateArmSelection();
            else if (legToggle.isOn)
                UpdateLegSelection();
        }

        void SetMirrored(bool isMirrored)
        {
            SessionSettings.IsMirrored = isMirrored;
            Debug.Log($"IsMirrored = {SessionSettings.IsMirrored}");
            RefreshButtonHighlights();
        }

        void RefreshButtonHighlights()
        {
            SetButtonColour(leftButton, SessionSettings.IsMirrored == true);
            SetButtonColour(rightButton, SessionSettings.IsMirrored == false);

        }

        void SetButtonColour(Button button, bool isSelected)
        {
            Color baseColour = isSelected ? SelectedColour : UnselectedColour;
            var colours = button.colors;
            colours.normalColor = baseColour;
            colours.selectedColor = Color.Lerp(baseColour, SelectedColour, 0.4f);
            colours.pressedColor = Color.Lerp(baseColour, Color.black, 0.15f);
            button.colors = colours;

            var outline = button.GetComponent<Outline>();
            if(outline != null)
            {
                outline.enabled = isSelected;
                outline.effectColor = SelectedBorderColour;
            }
        }

        void OnArmToggleChanged(bool isOn)
        {
            armDropdown.interactable = isOn;
            if (isOn)
                UpdateArmSelection();
        }

        void OnLegToggleChanged(bool isOn)
        {
            legDropdown.interactable = isOn;
            if (isOn)
                UpdateLegSelection();
        }

        void UpdateArmSelection()
        {
            if (armDropdown.value >= armSceneNames.Length)
            {
                Debug.LogError("UpdateArmSelection: dropdown value out of range for armSceneNames.");
                return;
            }
            SessionSettings.SelectedTaskScene = armSceneNames[armDropdown.value];
        }

        void UpdateLegSelection()
        {
            if (legDropdown.value >= legSceneNames.Length)
            {
                Debug.LogError("UpdateLegSelection: dropdown value out of range for legSceneNames.");
                return;
            }
            SessionSettings.SelectedTaskScene = legSceneNames[legDropdown.value];
        }


        void StartSession()
        {
            if (SessionSettings.IsMirrored == null || string.IsNullOrEmpty(SessionSettings.SelectedTaskScene))
            {
                Debug.LogWarning("MainMenuController: pick both a side and a task before starting.");
                return;
            }
            SceneManager.LoadScene(SessionSettings.SelectedTaskScene);
            Debug.Log("Start clicked");
        }
    }
}