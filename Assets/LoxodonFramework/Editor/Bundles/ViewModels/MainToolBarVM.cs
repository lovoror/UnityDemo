using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Bundles.Editors
{
    [System.Serializable]
    public class MainToolBarVM : AbstractViewModel
    {
        private const string PREFIX = "Loxodon::Framework::Bundle::";
        private const string CURRENT_MENU_INDEX_KEY = "CURR_MENU_INDEX";

        [SerializeField]
        private int currentMenuIndex = -1;
        [SerializeField]
        private string[] menus;

        public MainToolBarVM()
        {
        }

        public string[] Menus
        {
            get { return menus; }
            set { menus = value; }
        }

        public int CurrentMenuIndex
        {
            get
            {
                if (currentMenuIndex == -1)
                    currentMenuIndex = EditorPrefs.GetInt(PREFIX + CURRENT_MENU_INDEX_KEY, 0);

                return currentMenuIndex;
            }
            set
            {
                if (currentMenuIndex == value)
                    return;

                currentMenuIndex = value;
                EditorPrefs.SetInt(PREFIX + CURRENT_MENU_INDEX_KEY, currentMenuIndex);
            }
        }
    }
}
