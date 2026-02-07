using System;

using UnityEngine;

namespace UI.Themes
{
    public abstract class Element : MonoBehaviour
    {
        [SerializeField] protected ElementType _Type;
        [SerializeField] protected string Key;

        protected virtual void Start()
        {
            Drawer.AddListener(LoadData);

            LoadData();
        }
        protected virtual void OnDestroy()
        {
            Drawer.RemoveListener(LoadData);
        }

        public virtual string GetKey() => Key;
        public ElementType GetElementType() => _Type;

        public abstract void SetData(Data data);

        protected virtual void LoadData()
        {
            if (Drawer.TryGetData(GetKey(), out var data))
                SetData(data);
        }


        #region DATA
        [Serializable]
        public abstract class Data
        {

        }
        #endregion
    }
}