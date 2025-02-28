using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MainScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class MainScreenView : MonoBehaviour
    {
        [SerializeField] private Menu _menu;
        [SerializeField] private ScheduleScreen.ScheduleScreen _scheduleScreen;
        [SerializeField] private Exams.ExamsScreen _examsScreen;
        [SerializeField] private BudgetScreen _budgetScreen;
        [SerializeField] private AllAssigments.AllAssigmentsScreen _allAssigmentsScreen;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Settings _settings;

        [Header("Animation Settings")]
        [SerializeField] private float _transitionDuration = 0.5f;
        [SerializeField] private Ease _transitionEase = Ease.OutQuad;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _canvasGroup = GetComponent<CanvasGroup>();

            // Ensure CanvasGroup component exists
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void OnEnable()
        {
            _menu.ScheduleClicked += OpenSchedule;
            _menu.ExamsClicked += OpenExams;
            _menu.BudgetClicked += OpenBudget;
            _allAssigmentsScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
            _settingsButton.onClick.AddListener(OpenSettings);
            _settings.SettingsClosed += Enable;
        }

        private void OnDisable()
        {
            _menu.ScheduleClicked -= OpenSchedule;
            _menu.ExamsClicked -= OpenExams;
            _menu.BudgetClicked -= OpenBudget;
            _allAssigmentsScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
            _settingsButton.onClick.RemoveListener(OpenSettings);
            _settings.SettingsClosed -= Enable;
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.transform.localScale = Vector3.one * 0.9f;

                _canvasGroup.DOFade(1f, _transitionDuration)
                    .SetEase(_transitionEase);

                _canvasGroup.transform.DOScale(1f, _transitionDuration)
                    .SetEase(_transitionEase);
            }
        }

        public void Disable()
        {
            // Animate screen disable with fade and scale
            if (_canvasGroup != null)
            {
                _canvasGroup.DOFade(0f, _transitionDuration)
                    .SetEase(_transitionEase)
                    .OnComplete(() => 
                    {
                        _screenVisabilityHandler.DisableScreen();
                        _canvasGroup.transform.localScale = Vector3.one;
                    });

                _canvasGroup.transform.DOScale(0.9f, _transitionDuration)
                    .SetEase(_transitionEase);
            }
            else
            {
                _screenVisabilityHandler.DisableScreen();
            }
        }

        private void OpenExams()
        {
            Disable();
            _examsScreen.Enable();
        }

        private void OpenBudget()
        {
            Disable();
            _budgetScreen.Enable();
        }

        private void OpenSchedule()
        {
            Disable();
            _scheduleScreen.Enable();
        }

        private void OpenSettings()
        {
            _settings.ShowSettings();
            Disable();
        }

        // Optional: Method to cancel any running animations
        private void CancelAnimations()
        {
            _canvasGroup?.DOKill();
        }

        private void OnDestroy()
        {
            CancelAnimations();
        }
    }
}