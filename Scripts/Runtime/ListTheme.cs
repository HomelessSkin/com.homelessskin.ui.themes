using Core;

using UnityEngine;

namespace UI.Themes
{
    public class ListTheme : ScrollItem
    {
        [SerializeField] MenuButton SelectButton;

        public override void Init(int index, IStorage.Data data)
        {
            base.Init(index, data);

            //SelectButton.AddListener(() => Drawer.SetTheme(index));
        }
    }
}