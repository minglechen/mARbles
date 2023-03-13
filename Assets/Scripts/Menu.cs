using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace Menu
{
    /// <summary>
    /// Menu that is added to a scene to surface tracking data and visualize trackables in order to aid in debugging.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class ARMenu : MonoBehaviour
    {

        [SerializeField]
        Button m_DisplayInfoMenuButton;
        

        [SerializeField]
        Button m_DisplayConfigurationsMenuButton;


        [SerializeField]
        Button m_DisplayDebugOptionsMenuButton;


        [SerializeField]
        GameObject m_InfoMenu;
        

        [SerializeField]
        GameObject m_DebugOptionsMenu;

        
        [SerializeField]
         DebugSlider m_SoundEffectsButton;


        [SerializeField]
        DebugSlider m_ShowDebugButton;

        [SerializeField]
        private GameObject _levelMenuRoot;

        [SerializeField] private LevelButton _levelButton;
        
        
        [SerializeField]
        GameObject m_Toolbar;

        [SerializeField]
        Font m_MenuFont;

        private MarbleControl _marbleControl;
        private bool _restartSelected;
        private List<LevelButton> _levelButtons = new List<LevelButton>();
        bool m_ConfigMenuSetup;

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
            if(m_DisplayInfoMenuButton == null && m_DisplayConfigurationsMenuButton == null && m_DisplayDebugOptionsMenuButton == null && m_Toolbar == null)
            {
                return false;
            }
            else if(m_DisplayInfoMenuButton == null || m_DisplayConfigurationsMenuButton == null || m_DisplayDebugOptionsMenuButton == null || m_SoundEffectsButton == null ||
                m_ShowDebugButton == null || m_InfoMenu == null || m_DebugOptionsMenu == null || m_MenuFont == null || m_Toolbar == null)
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
            m_DisplayInfoMenuButton.onClick.AddListener(delegate { ShowMenu(m_InfoMenu); });
            m_DisplayConfigurationsMenuButton.onClick.AddListener(delegate { RestartLevel(); });
            m_DisplayDebugOptionsMenuButton.onClick.AddListener(delegate { ShowMenu(m_DebugOptionsMenu); });
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
                var rect = m_Toolbar.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.eulerAngles = new Vector3(rect.eulerAngles.x, rect.eulerAngles.y, 90);
                rect.anchoredPosition = new Vector2(0, 20);
                var infoMenuButtonRect = m_DisplayInfoMenuButton.GetComponent<RectTransform>();
                var configurationsMenuButtonRect = m_DisplayConfigurationsMenuButton.GetComponent<RectTransform>();
                var debugOptionsMenuButtonRect = m_DisplayDebugOptionsMenuButton.GetComponent<RectTransform>();
                infoMenuButtonRect.localEulerAngles =  new Vector3(infoMenuButtonRect.localEulerAngles.x, infoMenuButtonRect.localEulerAngles.y, -90);
                configurationsMenuButtonRect.localEulerAngles = new Vector3(configurationsMenuButtonRect.localEulerAngles.x, configurationsMenuButtonRect.localEulerAngles.y, -90);
                debugOptionsMenuButtonRect.localEulerAngles = new Vector3(debugOptionsMenuButtonRect.localEulerAngles.x, debugOptionsMenuButtonRect.localEulerAngles.y, -90);

               var infoMenuRect = m_InfoMenu.GetComponent<RectTransform>();
               infoMenuRect.anchorMin = new Vector2(0.5f, 0);
               infoMenuRect.anchorMax = new Vector2(0.5f, 0);
               infoMenuRect.pivot = new Vector2(0.5f, 0);
               infoMenuRect.anchoredPosition = new Vector2(0, 150);
               

               var debugOptionsMenuRect = m_DebugOptionsMenu.GetComponent<RectTransform>();
               debugOptionsMenuRect.anchorMin = new Vector2(0.5f, 0);
               debugOptionsMenuRect.anchorMax = new Vector2(0.5f, 0);
               debugOptionsMenuRect.pivot = new Vector2(0.5f, 0);
               debugOptionsMenuRect.anchoredPosition = new Vector2(0, 150);
            }
        }

        void ConfigureButtons()
        {
            if(m_SoundEffectsButton)
            {
                m_SoundEffectsButton.interactable = true;
                m_SoundEffectsButton.onValueChanged.AddListener(delegate {ToggleOriginVisibility();});
            }

            if(m_ShowDebugButton)
            {
                m_ShowDebugButton.interactable = true;
                m_ShowDebugButton.onValueChanged.AddListener(delegate {TogglePlanesVisibility();});
            }
            
        }

        void ConfigureLevels()
        {
            var pos = new Vector2(-110, 110);
            const int xOffset = 110;
            const int yOffset = -110;
            for (int i = 0; i < _marbleControl.levels.Count; i++)
            {
                var button = Instantiate(_levelButton, _levelMenuRoot.transform);
                button.Menu = this;
                if (i == 0)
                {
                    button.GetComponent<RectTransform>().anchoredPosition = pos;
                    button.HighlightButton(true);
                }
                else
                {
                    var rowStart = i % 3 == 0 ? -2 : 1;
                    pos += new Vector2(rowStart * xOffset, i / 3 * yOffset);
                    button.GetComponent<RectTransform>().anchoredPosition = pos;
                }

                var level = i;
                button.SetText($"{i+1}");
                button.GetComponent<Button>().onClick.AddListener(delegate {_marbleControl.SelectLevel(level);});
                _levelButtons.Add(button);
            }
        }

        void RestartLevel()
        {
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
                m_InfoMenu.SetActive(false);
                m_DebugOptionsMenu.SetActive(false);

                menu.SetActive(true);
            }
        }

        void ToggleOriginVisibility()
        {
        }

        void TogglePlanesVisibility()
        {
        }
        
    }
}
