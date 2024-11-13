using System;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _mainScreenButton;
    [SerializeField] private Button _scheduleButton;
    [SerializeField] private Button _examsButton;
    [SerializeField] private Button _budgetButton;

    public event Action MainScreenClicked;
    public event Action ScheduleClicked;
    public event Action ExamsClicked;
    public event Action BudgetClicked;

    private void OnEnable()
    {
        _mainScreenButton.onClick.AddListener(OnMainScreenClicked);
        _scheduleButton.onClick.AddListener(OnScheduleClicked);
        _examsButton.onClick.AddListener(OnExamsClicked);
        _budgetButton.onClick.AddListener(OnBudgetClicked);
    }

    private void OnDisable()
    {
        _mainScreenButton.onClick.RemoveListener(OnMainScreenClicked);
        _scheduleButton.onClick.RemoveListener(OnScheduleClicked);
        _examsButton.onClick.RemoveListener(OnExamsClicked);
        _budgetButton.onClick.RemoveListener(OnBudgetClicked);
    }

    private void OnMainScreenClicked() => MainScreenClicked?.Invoke();
    private void OnScheduleClicked() => ScheduleClicked?.Invoke();
    private void OnExamsClicked() => ExamsClicked?.Invoke();
    private void OnBudgetClicked() => BudgetClicked?.Invoke();
}
