using System;
using System.Collections;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditBudgetLimit : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private TMP_InputField _amountInput;
    [SerializeField] private Button _semesterBudget;
    [SerializeField] private Button _yearlyBudget;
    [SerializeField] private Button _startDateButton;
    [SerializeField] private Button _endDateButton;
    [SerializeField] private TMP_Text _dateStart;
    [SerializeField] private TMP_Text _dateEnd;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private Button _setLimit;
    [SerializeField] private Button _cancel;
    [SerializeField] private Button _deleteButton;

    public event Action<BudgetData> DataSaved;
    public event Action<BudgetData> Deleted;

    private string _name;
    private int _maxAmount;
    private string _startDate;
    private string _endDate;
    private Button _currentButton;
    private Button _currentTypeButton;

    private BudgetData _currentData;
    
    private void OnEnable()
    {
        _amountInput.onValueChanged.AddListener(SetAmount);
        _startDateButton.onClick.AddListener(() => OnButtonClicked(_startDateButton));
        _endDateButton.onClick.AddListener(() => OnButtonClicked(_endDateButton));
        _setLimit.onClick.AddListener(SaveData);
        _semesterBudget.onClick.AddListener((() => OnTypeButtonClicked(_semesterBudget)));
        _yearlyBudget.onClick.AddListener((() => OnTypeButtonClicked(_yearlyBudget)));
        
        _deleteButton.onClick.AddListener(Delete);

        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);

        ValidateInput();
    }

    private void OnDisable()
    {
        _amountInput.onValueChanged.RemoveListener(SetAmount);
        _startDateButton.onClick.RemoveListener(() => OnButtonClicked(_startDateButton));
        _endDateButton.onClick.RemoveListener(() => OnButtonClicked(_endDateButton));
        _setLimit.onClick.RemoveListener(SaveData);
        _semesterBudget.onClick.RemoveListener((() => OnTypeButtonClicked(_semesterBudget)));
        _yearlyBudget.onClick.RemoveListener((() => OnTypeButtonClicked(_yearlyBudget)));

        _deleteButton.onClick.RemoveListener(Delete);
        
        _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
    }

    public void EnableScreen(BudgetData data)
    {
        gameObject.SetActive(true);
        _maxAmount = data.Amount;
        _amountInput.text = data.Amount.ToString();

        _startDate = data.StartDate;
        _endDate = data.EndDate;

        _dateStart.text = _startDate;
        _dateEnd.text = _endDate;

        if (data.IsYearly)
        {
            _currentTypeButton = _yearlyBudget;
            _yearlyBudget.image.color = _selectedColor;
        }
        else
        {
            _currentTypeButton = _semesterBudget;
            _semesterBudget.image.color = _selectedColor;
        }

        _currentData = data;
        ValidateInput();
    }

    private void SetAmount(string amount)
    {
        if (int.TryParse(amount, out int parsedAmount))
        {
            _maxAmount = parsedAmount;
        }
        else
        {
            _maxAmount = 0;
        }

        ValidateInput();
    }

    private void OnButtonClicked(Button button)
    {
        _currentButton = button;
        _datePicker.gameObject.SetActive(true);
    }

    private void OnTypeButtonClicked(Button button)
    {
        if (_currentTypeButton != null)
        {
            _currentTypeButton.image.color = _defaultColor;
        }

        _currentTypeButton.image.color = _selectedColor;
        _currentTypeButton = button;
        ValidateInput();
    }

    private void SetDate()
    {
        if (_currentButton == null) return;

        var selection = _datePicker.Content.Selection;
        string selectedDate = selection.Count > 0 ? selection.GetItem(0).ToString("dd.MM.yyyy") : "";

        if (_currentButton == _startDateButton)
        {
            _startDate = selectedDate;
            _dateStart.text = selectedDate;
        }
        else if (_currentButton == _endDateButton)
        {
            _endDate = selectedDate;
            _dateEnd.text = selectedDate;
        }

        _datePicker.gameObject.SetActive(false);
        ValidateInput();
    }

    private void SaveData()
    {
        bool isYearly;

        if (_currentTypeButton == _yearlyBudget)
        {
            isYearly = true;
        }
        else
        {
            isYearly = false;
        }

        var budgetData = new BudgetData(_maxAmount, _name, _startDate, _endDate, isYearly);
        DataSaved?.Invoke(budgetData);
    }

    private void ValidateInput()
    {
        _setLimit.interactable = _maxAmount > 0 && !string.IsNullOrEmpty(_startDate) &&
                                 !string.IsNullOrEmpty(_endDate) && _currentTypeButton != null;
    }

    private void Delete()
    {
        Deleted?.Invoke(_currentData);
        gameObject.SetActive(false);
    }
}
