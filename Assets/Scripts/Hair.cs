using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hair : MonoBehaviour
{
    public MeshRenderer m_meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool DebugDraw = true;
    public Transform ScalpProvider;
    private Vector3[] GetVertices()
    {
        return null;
    }
    bool ValidateImpl(bool value)
    {
        return true;
    }
    private void OnDrawGizmos()
    {
        if (!DebugDraw || GetVertices() == null || !ValidateImpl(false))
        {
            return;
        }
        
    }
}
