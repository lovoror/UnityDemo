using UnityEngine;
using UnityEditor;

namespace Loxodon.Framework.Bundles.Editors
{
    public class MainToolBar : Panel
    {
        private readonly MainToolBarVM mainToolBarVM;
        private GUIContent[] menuContents;

        public MainToolBar(EditorWindow parent, MainToolBarVM mainToolBarVM) : base(parent)
        {
            this.mainToolBarVM = mainToolBarVM;
        }

        public MainToolBarVM MainToolBarVM { get { return this.mainToolBarVM; } }

        public override void OnEnable()
        {
            menuContents = new GUIContent[] { };
            if (mainToolBarVM.Menus != null)
            {
                var menus = mainToolBarVM.Menus;
                menuContents = new GUIContent[menus.Length];
                for (int i = 0; i < menus.Length; i++)
                {
                    menuContents[i] = new GUIContent(menus[i]);
                }
            }
            mainToolBarVM.CurrentMenuIndex = Mathf.Clamp(mainToolBarVM.CurrentMenuIndex, 0, menuContents.Length);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            mainToolBarVM.CurrentMenuIndex = GUILayout.SelectionGrid(mainToolBarVM.CurrentMenuIndex, this.menuContents, this.menuContents.Length, EditorStyles.toolbarButton, GUILayout.Width(100 * this.menuContents.Length));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
