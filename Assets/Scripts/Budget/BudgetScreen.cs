using System;
using System.Linq;
using Exams;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class BudgetScreen : MonoBehaviour
{
    [SerializeField] BudgetPlane[] _planes;
    [SerializeField] private Button _addBudgetButton;
    [SerializeField] private Menu _menu;
    [SerializeField] private AddBudgetLimit _addBudgetLimit;
    [SerializeField] private EditBudgetLimit _editBudgetLimit;
    [SerializeField] private LogSpending _logSpending;
    [SerializeField] private BudgetHistory _budgetHistory;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private MainScreenView _mainScreenView;
    [SerializeField] private ScheduleScreen.ScheduleScreen _scheduleScreen;
    [SerializeField] private ExamsScreen _examsScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action FirstPlaneActive;
    public event Action FirstPlaneDeleted;
    public event Action SecondPlaneActive;
    public event Action SecondPlaneDeleted;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _addBudgetLimit.DataSaved += EnableBudgetPlane;

        _editBudgetLimit.DataSaved += EnableBudgetPlane;
        _editBudgetLimit.Deleted += Delete;

        foreach (var plane in _planes)
        {
            plane.LogSpendingClicked += OpenLogSpending;
            plane.HistoryClicked += OpenBudgetHistory;
            plane.EditLimitClicked += EditBudget;
        }

        _addBudgetButton.onClick.AddListener(AddBudget);
        _logSpending.Saved += UpdateData;

        _menu.ScheduleClicked += ScheduleOpen;
        _menu.ExamsClicked += ExamsOpen;
        _menu.MainScreenClicked += MainScreenOpen;
    }

    private void OnDisable()
    {
        _addBudgetLimit.DataSaved -= EnableBudgetPlane;

        _editBudgetLimit.DataSaved -= EnableBudgetPlane;
        _editBudgetLimit.Deleted -= Delete;

        foreach (var plane in _planes)
        {
            plane.LogSpendingClicked -= OpenLogSpending;
            plane.HistoryClicked -= OpenBudgetHistory;
            plane.EditLimitClicked -= EditBudget;
        }

        _addBudgetButton.onClick.RemoveListener(AddBudget);
        _logSpending.Saved -= UpdateData;
        
        _menu.ScheduleClicked -= ScheduleOpen;
        _menu.ExamsClicked -= ExamsOpen;
        _menu.MainScreenClicked -= MainScreenOpen;
    }

    private void Start()
    {
        _addBudgetLimit.gameObject.SetActive(false);
        _editBudgetLimit.gameObject.SetActive(false);
        _logSpending.gameObject.SetActive(false);
        _budgetHistory.gameObject.SetActive(false);

        _screenVisabilityHandler.DisableScreen();

        foreach (var plane in _planes)
        {
            plane.Disable();
        }

        _emptyPlane.gameObject.SetActive(ArePlanesAvailable());
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    private bool ArePlanesAvailable()
    {
        return _planes.All(plane => !plane.IsActive);
    }

    private bool AllPlanesActive()
    {
        return _planes.Any(plane => !plane.IsActive);
    }

    private void AddBudget()
    {
        _addBudgetLimit.gameObject.SetActive(true);
    }

    private void EnableBudgetPlane(BudgetData data)
    {
        if (data.IsYearly)
        {
            _planes[0].Enable(data);
            FirstPlaneActive?.Invoke();
        }
        else
        {
            _planes[1].Enable(data);
            SecondPlaneActive?.Invoke();
        }

        _addBudgetButton.interactable = AllPlanesActive();
        _emptyPlane.gameObject.SetActive(ArePlanesAvailable());
    }

    private void OpenBudgetHistory(BudgetData data)
    {
        _budgetHistory.Enable(data);
    }

    private void EditBudget(BudgetData data)
    {
        _editBudgetLimit.EnableScreen(data);
    }

    private void OpenLogSpending(BudgetData data)
    {
        _logSpending.Enable(data);
    }

    private void UpdateData(BudgetData data)
    {
        if (data.IsYearly)
        {
            _planes[0].SetData(data);
        }
        else
        {
            _planes[1].SetData(data);
        }

        _addBudgetButton.interactable = AllPlanesActive();
    }

    private void Delete(BudgetData data)
    {
        if (data.IsYearly)
        {
            _planes[0].Disable();
            FirstPlaneDeleted?.Invoke();
        }
        else
        {
            _planes[1].Disable();
            SecondPlaneDeleted?.Invoke();
        }

        _addBudgetButton.interactable = AllPlanesActive();
        _emptyPlane.gameObject.SetActive(ArePlanesAvailable());
    }

    private void MainScreenOpen()
    {
        _screenVisabilityHandler.DisableScreen();
        _mainScreenView.Enable();
    }

    private void ScheduleOpen()
    {
        _screenVisabilityHandler.DisableScreen();
        _scheduleScreen.Enable();
    }

    private void ExamsOpen()
    {
        _examsScreen.Enable();
        _screenVisabilityHandler.DisableScreen();
    }
}