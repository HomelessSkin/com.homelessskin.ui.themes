using Core;

using UnityEngine;

namespace UI
{
    public class ListLocalization : ScrollItem
    {
        [SerializeField] MenuButton SelectButton;

        public override void Init(int index, IStorage.Data data)
        {
            base.Init(index, data);

            //SelectButton.AddListener(() => manager.SelectLanguage(data.Name));
        }
    }
}