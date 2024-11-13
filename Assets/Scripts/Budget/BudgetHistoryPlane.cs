using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BudgetHistoryPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _amount;

    public bool IsActive { get; private set; }
    
    public void Enable(SpendingData data)
    {
        gameObject.SetActive(true);
        _name.text = data.Name;
        _amount.text = "$" + data.Amount;
        IsActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }
}
