using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace Menu
{
    /// <summary>
    /// Main menu for level select, restart level, and options.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class ARMenu : MonoBehaviour
    {
        
        public Button displayInfoMenuButton;
        
        public Button restartLevelButton;

        public Button displayOptionsMenuButton;

        public GameObject infoMenu;
        
        public GameObject optionsMenu;
        
        public DebugSlider soundEffectsButton;
        
        public DebugSlider showDebugButton;
         
        public GameObject levelMenuRoot;
        
        public  LevelButton levelButton;
        
        public GameObject toolbar;
        
        public Font menuFont;

        private MarbleControl _marbleControl;
        private bool _restartSelected;
        private readonly List<LevelButton> _levelButtons = new List<LevelButton>();

        void Start()
        {
            _marbleControl = GameObject.FindWithTag("GameController").GetComponent<MarbleControl>();
            if(!CheckMenuConfigured())
            {
                enabled = false;
                Debug.LogError($"The menu has not been configured correctly and will currently be disabled. For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.");
            }
            else
            {
                ConfigureMenuPosition();
            }
            ConfigureLevels();
        }

        public void ClearLevelButtonHighlights(LevelButton button)
        {
            foreach (var levelButton in _levelButtons)
            {
                if (button == levelButton) continue;
                levelButton.HighlightButton(false);
            }
        }
        bool CheckMenuConfigured()
        {
            if(displayInfoMenuButton == null && restartLevelButton == null && displayOptionsMenuButton == null && toolbar == null)
            {
                return false;
            }
            else if(displayInfoMenuButton == null || restartLevelButton == null || displayOptionsMenuButton == null || soundEffectsButton == null ||
                showDebugButton == null || infoMenu == null || optionsMenu == null || menuFont == null || toolbar == null)
            {
                Debug.LogWarning("The menu has not been fully configured so some functionality will be disabled. For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu");
            }

            return true;
        }

        void OnEnable()
        {
            InitMenu();
            ConfigureButtons();
        }
        
        void InitMenu()
        {
            displayInfoMenuButton.onClick.AddListener(delegate { ShowMenu(infoMenu); });
            restartLevelButton.onClick.AddListener(delegate { RestartLevel(); });
            displayOptionsMenuButton.onClick.AddListener(delegate { ShowMenu(optionsMenu); });
            Canvas menu = GetComponent<Canvas>();
#if UNITY_IOS || UNITY_ANDROID
            menu.renderMode = RenderMode.ScreenSpaceOverlay;
#else
            var rectTransform = GetComponent<RectTransform>();
            menu.renderMode = RenderMode.WorldSpace;
            menu.worldCamera  = m_CameraAR;
            m_CameraFollow = true;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 575);
#endif
        }

        // Set up code from AR Foundation
        void ConfigureMenuPosition()
        {
            float screenWidthInInches = Screen.width / Screen.dpi;

            if(screenWidthInInches < 5)
            {
                var rect = toolbar.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.eulerAngles = new Vector3(rect.eulerAngles.x, rect.eulerAngles.y, 90);
                rect.anchoredPosition = new Vector2(0, 20);
                var infoMenuButtonRect = displayInfoMenuButton.GetComponent<RectTransform>();
                var configurationsMenuButtonRect = restartLevelButton.GetComponent<RectTransform>();
                var debugOptionsMenuButtonRect = displayOptionsMenuButton.GetComponent<RectTransform>();
                infoMenuButtonRect.localEulerAngles =  new Vector3(infoMenuButtonRect.localEulerAngles.x, infoMenuButtonRect.localEulerAngles.y, -90);
                configurationsMenuButtonRect.localEulerAngles = new Vector3(configurationsMenuButtonRect.localEulerAngles.x, configurationsMenuButtonRect.localEulerAngles.y, -90);
                debugOptionsMenuButtonRect.localEulerAngles = new Vector3(debugOptionsMenuButtonRect.localEulerAngles.x, debugOptionsMenuButtonRect.localEulerAngles.y, -90);

               var infoMenuRect = infoMenu.GetComponent<RectTransform>();
               infoMenuRect.anchorMin = new Vector2(0.5f, 0);
               infoMenuRect.anchorMax = new Vector2(0.5f, 0);
               infoMenuRect.pivot = new Vector2(0.5f, 0);
               infoMenuRect.anchoredPosition = new Vector2(0, 150);
               

               var debugOptionsMenuRect = optionsMenu.GetComponent<RectTransform>();
               debugOptionsMenuRect.anchorMin = new Vector2(0.5f, 0);
               debugOptionsMenuRect.anchorMax = new Vector2(0.5f, 0);
               debugOptionsMenuRect.pivot = new Vector2(0.5f, 0);
               debugOptionsMenuRect.anchoredPosition = new Vector2(0, 150);
            }
        }

        void ConfigureButtons()
        {
            if(soundEffectsButton)
            {
                soundEffectsButton.interactable = true;
                soundEffectsButton.onValueChanged.AddListener(delegate {_marbleControl.ToggleSound();});
            }

            if(showDebugButton)
            {
                showDebugButton.interactable = true;
                showDebugButton.onValueChanged.AddListener(delegate {_marbleControl.ToggleDebug();});
            }
            
        }

        void ConfigureLevels()
        {
            var anchoredPosition = new Vector2(-110, 110);
            const int xOffset = 110;
            const int yOffset = -110;
            for (int i = 0; i < _marbleControl.levels.Count; i++)
            {
                var button = Instantiate(levelButton, levelMenuRoot.transform);
                button.Menu = this;
                if (i == 0)
                {
                    button.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
                    button.HighlightButton(true);
                }
                else
                {
                    var columnIndex = i % 3;
                    var rowIndex = i / 3;
                    button.GetComponent<RectTransform>().anchoredPosition = anchoredPosition + new Vector2(columnIndex * xOffset, rowIndex * yOffset);
                }

                var level = i;
                button.SetText($"{i+1}");
                button.GetComponent<Button>().onClick.AddListener(delegate {_marbleControl.SelectLevel(level);});
                _levelButtons.Add(button);
            }
        }

        void RestartLevel()
        {
            //Clear any currently open menus.
            infoMenu.SetActive(false);
            optionsMenu.SetActive(false);
            if (_restartSelected)
            {
                _marbleControl.RestartLevel();
                _restartSelected = false;
            }
            else
            {
                _restartSelected = true;
            }
        }
        void ShowMenu(GameObject menu)
        {
            if(menu.activeSelf)
            {
                menu.SetActive(false);
            }
            else
            {
                //Clear any currently open menus.
                infoMenu.SetActive(false);
                optionsMenu.SetActive(false);
                _restartSelected = false;
                menu.SetActive(true);
            }
        }
    }
}
