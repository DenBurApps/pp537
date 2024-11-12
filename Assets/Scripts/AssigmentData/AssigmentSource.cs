using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AssigmentData
{
    public class AssigmentSource : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Button _deleteButton;

        public event Action<AssigmentSource> Deleted; 
        
        public AssigmentSourceData Data { get; private set; }
        public bool IsActive { get; private set; }

        public void Enable(AssigmentSourceData sourceData)
        {
            gameObject.SetActive(true);
            IsActive = true;

            if (sourceData == null)
                throw new ArgumentNullException(nameof(sourceData));

            Data = sourceData;
            _name.text = Data.Name;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }
        
    }

    [Serializable]
    public class AssigmentSourceData
    {
        public string Name;
    }
}