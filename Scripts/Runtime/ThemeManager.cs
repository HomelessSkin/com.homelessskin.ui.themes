using UnityEngine;

namespace UI.Themes
{
    public class ThemeManager : MonoBehaviour
    {
        [SerializeField] DrawerSettings DrawerSettings;

        void Awake()
        {
            if (DrawerSettings)
                Drawer.Prepare(DrawerSettings);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!DrawerSettings)
                DrawerSettings = Resources.Load<DrawerSettings>("Serializables/DrawerSettings");
        }
#endif
    }
}