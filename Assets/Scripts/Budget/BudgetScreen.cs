using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private ScreenVisabilityHandler _screenVisabilityHandler;

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
    }

    private void Start()
    {
        _addBudgetLimit.gameObject.SetActive(false);
        _editBudgetLimit.gameObject.SetActive(false);
        _logSpending.gameObject.SetActive(false);
        _budgetHistory.gameObject.SetActive(false);

        //_screenVisabilityHandler.DisableScreen();

        foreach (var plane in _planes)
        {
            plane.Disable();
        }

        _emptyPlane.gameObject.SetActive(ArePlanesAvailable());
    }

    private bool ArePlanesAvailable()
    {
        return _planes.All(plane => !plane.IsActive);
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
        }
        else
        {
            _planes[1].Enable(data);
        }

        _addBudgetButton.interactable = ArePlanesAvailable();
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

        _addBudgetButton.interactable = ArePlanesAvailable();
    }

    private void Delete(BudgetData data)
    {
        if (data.IsYearly)
        {
            _planes[0].Disable();
        }
        else
        {
            _planes[1].Disable();
        }
        
        _addBudgetButton.interactable = ArePlanesAvailable();
        _emptyPlane.gameObject.SetActive(ArePlanesAvailable());
    }
}