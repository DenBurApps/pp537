using System;
using System.Collections.Generic;
using System.Linq;
using AssigmentData;
using UnityEngine;

namespace AssigmentDataInputScreen
{
    public class AssigmentSourceHolder : MonoBehaviour
    {
        [SerializeField] private List<AssigmentSource> _assigmentSources;

        public event Action AllPlanesDisabled;
        
        private void OnEnable()
        {
            DisableAllSteps();
        }

        private void OnDisable()
        {
            foreach (var source in _assigmentSources)
            {
                source.Deleted -= DisableSource;
            }
        }

        public void EnableSource()
        {
            var source = _assigmentSources.FirstOrDefault(source => source.IsActive == false);

            if (source!)
            {
                source.Enable();
                source.Deleted += DisableSource;
            }
        }

        public List<AssigmentSourceData> GetDatas()
        {
            return _assigmentSources
                .Where(source => source.Data != null)
                .Select(source => source.Data)
                .ToList();
        }

        private void DisableSource(AssigmentSource source)
        {
            source.Disable();

            if (AreAllStepsDisabled())
            {
                AllPlanesDisabled?.Invoke();
            }
        }

        private void DisableAllSteps()
        {
            foreach (var source in _assigmentSources)
            {
                source.Disable();
            }
        }
        
        private bool AreAllStepsDisabled()
        {
            return _assigmentSources.All(source => !source.IsActive);
        }
    }
}
