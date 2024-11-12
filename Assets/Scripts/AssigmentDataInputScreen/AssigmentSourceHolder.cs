using System.Collections.Generic;
using System.Linq;
using AssigmentData;
using UnityEngine;

namespace AssigmentDataInputScreen
{
    public class AssigmentSourceHolder : MonoBehaviour
    {
        [SerializeField] private List<AssigmentSource> _assigmentSources;

        private void OnEnable()
        {
            DisableAllSteps();
        }

        private void OnDisable()
        {
            foreach (var source in _assigmentSources)
            {
                source.Deleted -= DisableStep;
            }
        }

        public void EnableStep()
        {
            var source = _assigmentSources.FirstOrDefault(source => source.IsActive == false);

            if (source!)
            {
                source.Enable();
                source.Deleted += DisableStep;
            }
        }

        private void DisableStep(AssigmentSource source)
        {
            source.Disable();
        }

        private void DisableAllSteps()
        {
            foreach (var source in _assigmentSources)
            {
                source.Disable();
            }
        }
    }
}
