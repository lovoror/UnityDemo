using UnityEngine;
using System.Collections;

public class UICardScrollView : UIScrollView
{
    public UICardMove cardMove = null;

    public override void Awake()
    {
        base.Awake();

        base.restrictWithinPanel = true;
    }

    public override void MoveRelative(Vector3 relative)
    {
        cardMove.Process(relative);
    }

    public override void DisableSpring()
    {
        base.DisableSpring();

        cardMove.EnableSpring(false);
    }

    public override bool RestrictWithinBounds(bool instant, bool horizontal, bool vertical)
    {
        cardMove.EnableSpring(true);
        return true;
    }
}
