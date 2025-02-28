using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AddBudgetLimit : MonoBehaviour
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
    [SerializeField] private Button _backButton;

    [Header("Animation Settings")]
    [SerializeField] private float _enterDuration = 0.5f;
    [SerializeField] private float _exitDuration = 0.3f;
    [SerializeField] private Ease _enterEase = Ease.OutBack;
    [SerializeField] private Ease _exitEase = Ease.InBack;

    public event Action<BudgetData> DataSaved;

    private string _name;
    private int _maxAmount;
    private string _startDate;
    private string _endDate;
    private Button _currentButton;
    private Button _currentTypeButton;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        transform.DOScale(1f, _enterDuration)
            .SetEase(_enterEase);

        _amountInput.onValueChanged.AddListener(SetAmount);
        _startDateButton.onClick.AddListener(() => OnButtonClicked(_startDateButton));
        _endDateButton.onClick.AddListener(() => OnButtonClicked(_endDateButton));
        _setLimit.onClick.AddListener(SaveData);
        _semesterBudget.onClick.AddListener((() => OnTypeButtonClicked(_semesterBudget)));
        _yearlyBudget.onClick.AddListener((() => OnTypeButtonClicked(_yearlyBudget)));

        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
        
        _cancel.onClick.AddListener(CancelClicked);
        _backButton.onClick.AddListener(CancelClicked);
        
        ResetData();

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

        _cancel.onClick.RemoveListener(CancelClicked);
        _backButton.onClick.RemoveListener(CancelClicked);
        
        _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
    }

    private void SetAmount(string amount)
    {
        if (int.TryParse(amount, out int parsedAmount))
        {
            _maxAmount = parsedAmount;
            _amountInput.transform.DOShakeScale(0.3f, 0.5f);
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
        
        _datePicker.gameObject.transform.localScale = Vector3.zero;
        _datePicker.gameObject.SetActive(true);
        _datePicker.gameObject.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void OnTypeButtonClicked(Button button)
    {
        if (_currentTypeButton != null)
        {
            _currentTypeButton.image.DOColor(_defaultColor, 0.2f);
        }

        _currentTypeButton = button;
        _currentTypeButton.image.DOColor(_selectedColor, 0.2f);
        
        _currentTypeButton.transform.DOShakeScale(0.3f, 0.5f);

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
            _dateStart.transform.DOShakePosition(0.3f, 3f);
        }
        else if (_currentButton == _endDateButton)
        {
            _endDate = selectedDate;
            _dateEnd.text = selectedDate;
            _dateEnd.transform.DOShakePosition(0.3f, 3f);
        }

        _datePicker.gameObject.transform.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() => _datePicker.gameObject.SetActive(false));

        ValidateInput();
    }

    private void SaveData()
    {
        bool isYearly = _currentTypeButton == _yearlyBudget;

        var budgetData = new BudgetData(_maxAmount, _name, _startDate, _endDate, isYearly);
        
        _setLimit.transform.DOShakeScale(0.3f, 0.5f);

        transform.DOScale(0f, _exitDuration)
            .SetEase(_exitEase)
            .OnComplete(() => 
            {
                DataSaved?.Invoke(budgetData);
                gameObject.SetActive(false);
            });
    }

    private void ResetData()
    {
        _maxAmount = 0;
        _startDate = string.Empty;
        _endDate = string.Empty;
        
        _dateStart.text = string.Empty;
        _dateEnd.text = string.Empty;

        if (_currentTypeButton != null)
            _currentTypeButton.image.color = _selectedColor;

        _yearlyBudget.image.color = _defaultColor;
        _semesterBudget.image.color = _defaultColor;
        
        _currentTypeButton = null;
        _datePicker.gameObject.SetActive(false);
    }

    private void CancelClicked()
    {
        transform.DOScale(0f, _exitDuration)
            .SetEase(_exitEase)
            .OnComplete(() => 
            {
                ResetData();
                gameObject.SetActive(false);
            });
    }

    private void ValidateInput()
    {
        bool isValid = _maxAmount > 0 && !string.IsNullOrEmpty(_startDate) &&
                       !string.IsNullOrEmpty(_endDate) && _currentTypeButton != null;
        
        _setLimit.interactable = isValid;
        if (isValid)
        {
            _setLimit.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f);
        }
    }
}