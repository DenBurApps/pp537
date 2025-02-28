using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exams;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    [Header("Animation Settings")] [SerializeField]
    private float _animationDuration = 0.3f;

    [SerializeField] private Ease _animationEase = Ease.OutQuad;

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
        _addBudgetLimit.gameObject.SetActive(true);
        _addBudgetLimit.transform.localScale = Vector3.zero;
        _addBudgetLimit.gameObject.SetActive(false);

        _editBudgetLimit.gameObject.SetActive(true);
        _editBudgetLimit.transform.localScale = Vector3.zero;
        _editBudgetLimit.gameObject.SetActive(false);

        _logSpending.gameObject.SetActive(true);
        _logSpending.transform.localScale = Vector3.zero;
        _logSpending.gameObject.SetActive(false);

        _budgetHistory.gameObject.SetActive(true);
        _budgetHistory.transform.localScale = Vector3.zero;
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
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, _animationDuration)
            .SetEase(_animationEase);
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
        _addBudgetLimit.transform.localScale = Vector3.zero;
        _addBudgetLimit.transform.DOScale(Vector3.one, _animationDuration)
            .SetEase(_animationEase);
    }

    private void EnableBudgetPlane(BudgetData data)
    {
        BudgetPlane targetPlane;
        if (data.IsYearly)
        {
            targetPlane = _planes[0];
            FirstPlaneActive?.Invoke();
        }
        else
        {
            targetPlane = _planes[1];
            SecondPlaneActive?.Invoke();
        }

        targetPlane.gameObject.SetActive(true);
        targetPlane.transform.localScale = Vector3.zero;
        targetPlane.Enable(data);
        targetPlane.transform.DOScale(Vector3.one, _animationDuration)
            .SetEase(_animationEase);

        _addBudgetButton.interactable = AllPlanesActive();
        _emptyPlane.gameObject.SetActive(ArePlanesAvailable());
        Save();
    }

    private void OpenBudgetHistory(BudgetData data)
    {
        _budgetHistory.gameObject.SetActive(true);
        _budgetHistory.transform.localScale = Vector3.zero;
        _budgetHistory.Enable(data);
        _budgetHistory.transform.DOScale(Vector3.one, _animationDuration)
            .SetEase(_animationEase);
        Save();
    }

    private void EditBudget(BudgetData data)
    {
        _editBudgetLimit.gameObject.SetActive(true);
        _editBudgetLimit.transform.localScale = Vector3.zero;
        _editBudgetLimit.EnableScreen(data);
        _editBudgetLimit.transform.DOScale(Vector3.one, _animationDuration)
            .SetEase(_animationEase);
        Save();
    }

    private void OpenLogSpending(BudgetData data)
    {
        _logSpending.gameObject.SetActive(true);
        _logSpending.transform.localScale = Vector3.zero;
        _logSpending.Enable(data);
        _logSpending.transform.DOScale(Vector3.one, _animationDuration)
            .SetEase(_animationEase);
        Save();
    }

    private void UpdateData(BudgetData data)
    {
        BudgetPlane targetPlane = data.IsYearly ? _planes[0] : _planes[1];

        targetPlane.SetData(data);
        targetPlane.transform.DOPunchScale(Vector3.one * 0.1f, _animationDuration, 2, 1f);

        _addBudgetButton.interactable = AllPlanesActive();
        Save();
    }

    private void Delete(BudgetData data)
    {
        BudgetPlane targetPlane;
        if (data.IsYearly)
        {
            targetPlane = _planes[0];
            FirstPlaneDeleted?.Invoke();
        }
        else
        {
            targetPlane = _planes[1];
            SecondPlaneDeleted?.Invoke();
        }

        targetPlane.transform.DOScale(Vector3.zero, _animationDuration)
            .SetEase(_animationEase)
            .OnComplete(() =>
            {
                targetPlane.Disable();
                _addBudgetButton.interactable = AllPlanesActive();
                _emptyPlane.gameObject.SetActive(ArePlanesAvailable());
            });

        Save();
    }

    private void MainScreenOpen()
    {
        transform.DOScale(Vector3.zero, _animationDuration)
            .SetEase(_animationEase)
            .OnComplete(() =>
            {
                _screenVisabilityHandler.DisableScreen();
                _mainScreenView.Enable();
            });
    }

    private void ScheduleOpen()
    {
        transform.DOScale(Vector3.zero, _animationDuration)
            .SetEase(_animationEase)
            .OnComplete(() =>
            {
                _screenVisabilityHandler.DisableScreen();
                _scheduleScreen.Enable();
            });
    }

    private void ExamsOpen()
    {
        transform.DOScale(Vector3.zero, _animationDuration)
            .SetEase(_animationEase)
            .OnComplete(() =>
            {
                _examsScreen.Enable();
                _screenVisabilityHandler.DisableScreen();
            });
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