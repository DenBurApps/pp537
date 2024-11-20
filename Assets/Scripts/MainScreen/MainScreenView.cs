using System;
using UnityEngine;
using UnityEngine.UI;

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

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
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
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
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
    }
}