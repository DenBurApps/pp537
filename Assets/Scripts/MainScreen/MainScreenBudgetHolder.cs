using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class MainScreenBudgetHolder : MonoBehaviour
    {
        [SerializeField] private BudgetScreen _budgetScreen;
        [SerializeField] private GameObject _yearlyBudget;
        [SerializeField] private GameObject _semesterBudget;
        [SerializeField] private Button _manageBudget;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private MainScreenView _view;

        private bool _yearlyInputed;
        private bool _semesterInputed;

        private void OnEnable()
        {
            _yearlyInputed = false;
            _semesterInputed = false;
            _yearlyBudget.gameObject.SetActive(false);
            _semesterBudget.gameObject.SetActive(false);
            ToggleEmptyPlane();

            _budgetScreen.FirstPlaneActive += YearlyInputed;
            _budgetScreen.SecondPlaneActive += SemesterInputed;
            _budgetScreen.FirstPlaneDeleted += YearlyDeleted;
            _budgetScreen.SecondPlaneDeleted += SemesterDeleted;
            
            _manageBudget.onClick.AddListener(OpenBudget);
        }

        private void OnDisable()
        {
            _budgetScreen.FirstPlaneActive -= YearlyInputed;
            _budgetScreen.SecondPlaneActive -= SemesterInputed;
            _budgetScreen.FirstPlaneDeleted -= YearlyDeleted;
            _budgetScreen.SecondPlaneDeleted -= SemesterDeleted;
            
            _manageBudget.onClick.RemoveListener(OpenBudget);
        }

        private void OpenBudget()
        {
            _budgetScreen.Enable();
            _view.Disable();
        }

        private void YearlyInputed()
        {
            _yearlyInputed = true;
            _yearlyBudget.gameObject.SetActive(true);
            ToggleEmptyPlane();
        }

        private void SemesterInputed()
        {
            _semesterInputed = true;
            _semesterBudget.gameObject.SetActive(true);
            ToggleEmptyPlane();
        }

        private void YearlyDeleted()
        {
            _yearlyBudget.gameObject.SetActive(false);
            _yearlyInputed = false;
            ToggleEmptyPlane();
        }

        private void SemesterDeleted()
        {
            _semesterBudget.gameObject.SetActive(false);
            _semesterInputed = false;
            ToggleEmptyPlane();
        }

        private void ToggleEmptyPlane()
        {
            if (_yearlyInputed || _semesterInputed)
            {
                _emptyPlane.gameObject.SetActive(false);
            }
            else
            {
                _emptyPlane.gameObject.SetActive(true);
            }
        }
    }
}