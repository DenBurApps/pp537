using System;
using System.Collections.Generic;
using System.IO;
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
    private string savePath;

    public event Action FirstPlaneActive;
    public event Action FirstPlaneDeleted;
    public event Action SecondPlaneActive;
    public event Action SecondPlaneDeleted;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        savePath = Path.Combine(Application.persistentDataPath, "budgetData.json");
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
        
        Load();

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
        Save();
    }

    private void OpenBudgetHistory(BudgetData data)
    {
        _budgetHistory.Enable(data);
        Save();
    }

    private void EditBudget(BudgetData data)
    {
        _editBudgetLimit.EnableScreen(data);
        Save();
    }

    private void OpenLogSpending(BudgetData data)
    {
        _logSpending.Enable(data);
        Save();
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
        Save();
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
        Save();
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

    private void Save()
    {
        try
        {
            List<BudgetData> budgetDataList = _planes
                .Where(plane => plane.IsActive)
                .Select(plane => plane.Data)
                .ToList();
            
            BudgetDataListWrapper dataWrapper = new BudgetDataListWrapper(budgetDataList);

            string json = JsonUtility.ToJson(dataWrapper, true);
            File.WriteAllText(savePath, json);

            Debug.Log("Budget data saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save budget data: {e.Message}");
        }
    }
    
    private void Load()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                BudgetDataListWrapper dataWrapper = JsonUtility.FromJson<BudgetDataListWrapper>(json);
                List<BudgetData> loadedData = dataWrapper?.Data ?? new List<BudgetData>();
                
                foreach (var plane in _planes)
                {
                    plane.Disable();
                }
                
                foreach (var data in loadedData)
                {
                    EnableBudgetPlane(data);
                }

                Debug.Log("Budget data loaded successfully.");
            }
            else
            {
                Debug.Log("No saved budget data found.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load budget data: {e.Message}");
        }
    }

    [Serializable]
    public class BudgetDataListWrapper
    {
        public List<BudgetData> Data;

        public BudgetDataListWrapper(List<BudgetData> data)
        {
            Data = data;
        }
    }
}