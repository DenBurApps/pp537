using System;
using System.Collections;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems; // Added DOTween namespace

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
    [SerializeField] private Button _backButton;
    
    // Animation parameters
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private float _buttonScaleFactor = 1.1f;
    [SerializeField] private float _panelFadeInDuration = 0.5f;
    [SerializeField] private float _panelFadeOutDuration = 0.3f;
    [SerializeField] private Ease _buttonEase = Ease.OutBack;
    [SerializeField] private Ease _fadeEase = Ease.InOutSine;
    
    [SerializeField] private CanvasGroup _canvasGroup;

    public event Action<BudgetData> DataSaved;
    public event Action<BudgetData> Deleted;

    private string _name;
    private int _maxAmount;
    private string _startDate;
    private string _endDate;
    private Button _currentButton;
    private Button _currentTypeButton;

    private BudgetData _currentData;
    private Sequence _currentSequence;
    
    private void Awake()
    {
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
    
    private void OnEnable()
    {
        _amountInput.onValueChanged.AddListener(SetAmount);
        _startDateButton.onClick.AddListener(() => OnButtonClicked(_startDateButton));
        _endDateButton.onClick.AddListener(() => OnButtonClicked(_endDateButton));
        _setLimit.onClick.AddListener(SaveData);
        _semesterBudget.onClick.AddListener((() => OnTypeButtonClicked(_semesterBudget)));
        _yearlyBudget.onClick.AddListener((() => OnTypeButtonClicked(_yearlyBudget)));
        
        _deleteButton.onClick.AddListener(Delete);
        _cancel.onClick.AddListener(Cancel);
        _backButton.onClick.AddListener(Cancel);

        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);

        AddButtonAnimations();

        ValidateInput();
        
        AnimatePanelShow();
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
        _cancel.onClick.RemoveListener(Cancel);
        _backButton.onClick.RemoveListener(Cancel);
        
        _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
        
        DOTween.Kill(transform);
        RemoveButtonAnimations();
    }

    public void EnableScreen(BudgetData data)
    {
        _canvasGroup.alpha = 0;
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
            _semesterBudget.image.color = _defaultColor;
        }
        else
        {
            _currentTypeButton = _semesterBudget;
            _semesterBudget.image.color = _selectedColor;
            _yearlyBudget.image.color = _defaultColor;
        }

        _currentData = data;
        ValidateInput();
        _datePicker.gameObject.SetActive(false);
        
        AnimatePanelShow();
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
        
        if (!_datePicker.gameObject.activeSelf)
        {
            _datePicker.gameObject.SetActive(true);
            _datePicker.GetComponent<CanvasGroup>()?.DOFade(0, 0);
            _datePicker.GetComponent<CanvasGroup>()?.DOFade(1, _panelFadeInDuration).SetEase(_fadeEase);
            _datePicker.transform.DOScale(1.1f, 0);
            _datePicker.transform.DOScale(1f, _animationDuration).SetEase(_buttonEase);
        }
    }

    private void OnTypeButtonClicked(Button button)
    {
        if (_currentTypeButton != null)
        {
            _currentTypeButton.image.DOColor(_defaultColor, _animationDuration);
        }

        _currentTypeButton = button;
        _currentTypeButton.image.DOColor(_selectedColor, _animationDuration);
        
        _currentTypeButton.transform.DOPunchScale(Vector3.one * 0.1f, _animationDuration, 3, 0.5f);
        
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
            AnimateTextChange(_dateStart);
        }
        else if (_currentButton == _endDateButton)
        {
            _endDate = selectedDate;
            _dateEnd.text = selectedDate;
            AnimateTextChange(_dateEnd);
        }

        if (_datePicker.gameObject.activeSelf)
        {
            CanvasGroup datePickerCanvas = _datePicker.GetComponent<CanvasGroup>();
            if (datePickerCanvas != null)
            {
                datePickerCanvas.DOFade(0, _panelFadeOutDuration).SetEase(_fadeEase)
                    .OnComplete(() => _datePicker.gameObject.SetActive(false));
            }
            else
            {
                _datePicker.transform.DOScale(0.9f, _panelFadeOutDuration).SetEase(_fadeEase)
                    .OnComplete(() => _datePicker.gameObject.SetActive(false));
            }
        }
        else
        {
            _datePicker.gameObject.SetActive(false);
        }
        
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
        
        AnimatePanelHide(() => {
            DataSaved?.Invoke(budgetData);
            gameObject.SetActive(false);
        });
    }

    private void ValidateInput()
    {
        bool isValid = _maxAmount > 0 && !string.IsNullOrEmpty(_startDate) &&
                      !string.IsNullOrEmpty(_endDate) && _currentTypeButton != null;
        
        if (isValid != _setLimit.interactable)
        {
            _setLimit.interactable = isValid;
            
            if (isValid)
            {
                _setLimit.transform.DOPunchScale(Vector3.one * 0.1f, _animationDuration, 3, 0.5f);
                Color targetColor = _setLimit.colors.normalColor;
                targetColor.a = 1f;
                _setLimit.image.DOColor(targetColor, _animationDuration);
            }
            else
            {
                Color targetColor = _setLimit.colors.disabledColor;
                _setLimit.image.DOColor(targetColor, _animationDuration);
            }
        }
    }

    private void Delete()
    {
        AnimatePanelHide(() => {
            Deleted?.Invoke(_currentData);
            gameObject.SetActive(false);
        });
    }

    private void Cancel()
    {
        AnimatePanelHide(() => {
            gameObject.SetActive(false);
        });
    }
    
    #region Animation Methods
    
    private void AnimatePanelShow()
    {
        DOTween.Kill(transform);
        
        transform.localScale = Vector3.one * 0.95f;
        
        _currentSequence = DOTween.Sequence();
        
        _currentSequence.Append(_canvasGroup.DOFade(1, _panelFadeInDuration).SetEase(_fadeEase));
        
        _currentSequence.Join(transform.DOScale(1f, _panelFadeInDuration).SetEase(_buttonEase));
        
        _currentSequence.Play();
    }
    
    private void AnimatePanelHide(Action onComplete = null)
    {
        DOTween.Kill(transform);
        
        _currentSequence = DOTween.Sequence();
        
        _currentSequence.Append(_canvasGroup.DOFade(0, _panelFadeOutDuration).SetEase(_fadeEase));
        
        _currentSequence.Join(transform.DOScale(0.95f, _panelFadeOutDuration).SetEase(_fadeEase));
        
        if (onComplete != null)
        {
            _currentSequence.OnComplete(() => onComplete.Invoke());
        }
        
        _currentSequence.Play();
    }
    
    private void AnimateTextChange(TMP_Text textComponent)
    {
        textComponent.transform.DOPunchScale(Vector3.one * 0.2f, _animationDuration, 2, 0.5f);
        
        Color originalColor = textComponent.color;
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(textComponent.DOColor(Color.green, _animationDuration * 0.5f));
        textSequence.Append(textComponent.DOColor(originalColor, _animationDuration * 0.5f));
        textSequence.Play();
    }
    
    private void AddButtonAnimations()
    {
        AddButtonHoverAnimation(_setLimit);
        AddButtonHoverAnimation(_cancel);
        AddButtonHoverAnimation(_deleteButton);
        AddButtonHoverAnimation(_backButton);
        AddButtonHoverAnimation(_startDateButton);
        AddButtonHoverAnimation(_endDateButton);
        AddButtonHoverAnimation(_semesterBudget);
        AddButtonHoverAnimation(_yearlyBudget);
    }
    
    private void AddButtonHoverAnimation(Button button)
    {
        if (button == null) return;
        
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            if (button.interactable)
            {
                button.transform.DOScale(_buttonScaleFactor, _animationDuration).SetEase(_buttonEase);
            }
        });
        trigger.triggers.Add(entryEnter);
        
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            button.transform.DOScale(1f, _animationDuration).SetEase(_buttonEase);
        });
        trigger.triggers.Add(entryExit);
        
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => {
            if (button.interactable)
            {
                button.transform.DOScale(0.95f, _animationDuration * 0.5f).SetEase(_buttonEase);
            }
        });
        trigger.triggers.Add(entryDown);
        
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => {
            if (button.interactable)
            {
                button.transform.DOScale(_buttonScaleFactor, _animationDuration * 0.5f).SetEase(_buttonEase);
            }
        });
        trigger.triggers.Add(entryUp);
    }
    
    private void RemoveButtonAnimations()
    {
        Button[] buttons = { _setLimit, _cancel, _deleteButton, _backButton, 
                            _startDateButton, _endDateButton, _semesterBudget, _yearlyBudget };
        
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                DOTween.Kill(button.transform);
                button.transform.localScale = Vector3.one;
            }
        }
    }
    
    #endregion
}