using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BudgetPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _maxAmount;
    [SerializeField] private Button _logSpendingButton;
    [SerializeField] private Button _editLimitButton;
    [SerializeField] private Button _historyButton;

    public event Action<BudgetData> EditLimitClicked;
    public event Action<BudgetData> LogSpendingClicked;
    public event Action<BudgetData> HistoryClicked;
    
    public bool IsActive { get; private set; }
    public BudgetData Data { get; private set; }

    private void OnEnable()
    {
        _logSpendingButton.onClick.AddListener(OnLogSpendingClicked);
        _historyButton.onClick.AddListener(OnHistoryClicked);
        _editLimitButton.onClick.AddListener(OnEditClicked);
    }

    private void OnDisable()
    {
        _logSpendingButton.onClick.RemoveListener(OnLogSpendingClicked);
        _historyButton.onClick.RemoveListener(OnHistoryClicked);
        _editLimitButton.onClick.RemoveListener(OnEditClicked);
    }

    public void Enable(BudgetData data)
    {
        gameObject.SetActive(true);
        Data = data;

        _name.text = Data.IsYearly ? "Yearly budget" : "Semester budget";
        _maxAmount.text = "$" + Data.SpendAmount + "/" + Data.Amount;
        IsActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }
    
    public void SetData(BudgetData data)
    {
        Data = data;

        _name.text = Data.IsYearly ? "Yearly budget" : "Semester budget";
        _maxAmount.text = "$" + Data.SpendAmount + "/" + Data.Amount;
    }

    private void OnLogSpendingClicked() => LogSpendingClicked?.Invoke(Data);
    private void OnHistoryClicked() => HistoryClicked?.Invoke(Data);
    private void OnEditClicked() => EditLimitClicked?.Invoke(Data);
}

[Serializable]
public class BudgetData
{
    public int Amount;
    public string StartDate;
    public string EndDate;
    public int SpendAmount;
    public List<SpendingData> SpendingDatas;
    public bool IsYearly;

    public BudgetData(int amount, string name, string startDate, string endDate, bool isYearly)
    {
        Amount = amount;
        StartDate = startDate;
        EndDate = endDate;
        SpendingDatas = new List<SpendingData>();
        IsYearly = isYearly;
    }

    public void AddSpendAmount(SpendingData data)
    {
        if (SpendAmount + data.Amount < Amount)
        {
            SpendingDatas.Add(data);
            SpendAmount += data.Amount;
        }
    }
}

[Serializable]
public class SpendingData
{
    public int Amount;
    public string Name;

    public SpendingData(int amount, string name)
    {
        Amount = amount;
        Name = name;
    }
}
