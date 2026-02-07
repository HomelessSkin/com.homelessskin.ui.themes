using System;
using System.Collections.Generic;

using Core;

using UnityEngine;

namespace UI.Themes
{
    [Serializable]
    public abstract class PersonalizedStorage : ScrollBase, IPrefKey
    {
        public string DefaultPath;
        public string PrefKey;
        public string _Key => PrefKey;

        [Space]
        public IStorage.Data Default = new IStorage.Data();
        public IStorage.Data Current = new IStorage.Data();

        [Space]
        public Element[] Elements;

        protected abstract void LoadDefault();
        public virtual void SetData(IStorage.Data data)
        {
            Current = data;

            this.SavePrefString(data.Name);
        }
        public virtual void PickSaved()
        {
            LoadDefault();

            var saved = this.LoadPrefString();
            switch (saved)
            {
                case null:
                case "":
                case "Default":
                {
                    SetData(Default);
                }
                break;
                default:
                {
                    var found = false;
                    for (int m = 0; m < AllData.Count; m++)
                    {
                        var data = AllData[m];
                        if (data.Name == saved)
                        {
                            found = true;

                            SetData(data);

                            break;
                        }
                    }

                    if (!found)
                        goto case "Default";
                }
                break;
            }
        }

        public bool TryGetValue(string key, out Element.Data value) => TryGetValue<Element.Data>(key, out value);
        public bool TryGetValue<T>(string key, out T value) where T : Element.Data => TryGetCurrent<T>(key, out value) || TryGetDefault<T>(key, out value);
        public bool TryGetCurrent(string key, out Element.Data value) => TryGetCurrent<Element.Data>(key, out value);
        public bool TryGetCurrent<T>(string key, out T value) where T : Element.Data
        {
            value = null;
            if ((Current as Container).Map.TryGetValue(key, out var data))
            {
                value = data as T;

                return true;
            }

            return false;
        }
        public bool TryGetDefault(string key, out Element.Data value) => TryGetDefault<Element.Data>(key, out value);
        public bool TryGetDefault<T>(string key, out T value) where T : Element.Data
        {
            value = null;
            if ((Default as Container).Map.TryGetValue(key, out var data))
            {
                value = data as T;

                return true;
            }

            return false;
        }

        #region CONTAINER
        [Serializable]
        public class Container : IStorage.Data
        {
            public Dictionary<string, Element.Data> Map = new Dictionary<string, Element.Data>();
        }
        #endregion
    }
}