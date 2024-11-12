using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssigmentStep : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;

    [SerializeField] private Image _toggleImage;
    [SerializeField] private Sprite _selectedToggleSprite;
    [SerializeField] private Sprite _unselectedToggleSprite;

    [SerializeField] private TMP_Text _name;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _selectButton;

    public event Action<AssigmentStep> Deleted;

    public bool IsActive { get; private set; }
    public AssigmentStepData Data { get; private set; }

    private void OnEnable()
    {
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        _selectButton.onClick.AddListener(OnToggleButtonClicked);
    }

    private void OnDisable()
    {
        _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
        _selectButton.onClick.RemoveListener(OnToggleButtonClicked);
    }

    public void Enable(AssigmentStepData data)
    {
        gameObject.SetActive(true);
        IsActive = true;

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        Data = data;
        _name.text = Data.Name;

        ToggleSelection();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }

    private void ToggleSelection()
    {
        if (Data.IsSelected)
        {
            _name.color = _selectedColor;
            _toggleImage.sprite = _selectedToggleSprite;
        }
        else
        {
            _name.color = _defaultColor;
            _toggleImage.sprite = _unselectedToggleSprite;
        }
    }

    private void OnToggleButtonClicked()
    {
        if (Data != null)
        {
            if (Data.IsSelected)
            {
                Data.IsSelected = true;
            }
            else
            {
                Data.IsSelected = false;
            }
            
            ToggleSelection();
        }
    }

    private void OnDeleteButtonClicked() => Deleted?.Invoke(this);
}

[Serializable]
public class AssigmentStepData
{
    public string Name;
    public bool IsSelected;

    public AssigmentStepData(string name, bool isSelected)
    {
        Name = name;
        IsSelected = isSelected;
    }
}