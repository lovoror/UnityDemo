using UnityEngine;

public class UIPixelClearFlag : MonoBehaviour
{
    private bool _isClear = false;

    [HideInInspector]
    public bool isClear {
        get { return _isClear; }
        set { _isClear = value; }
    } 
}
