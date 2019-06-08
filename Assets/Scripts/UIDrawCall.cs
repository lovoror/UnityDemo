using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MiniNGUI
{
    public class UIDrawCall : MonoBehaviour
    {
        const int geomSize = 1000;
        public static List<Vector3> verts = new List<Vector3>(geomSize);
        public Material mMaterial;
        public Texture mTexture;
        public Shader mShader;
        Mesh mMesh;
        MeshFilter mMeshFilter;
        public MeshRenderer mRenderer;

        public void UpdateGeometry()
        {
            if (mMesh == null)
            {
                mMesh = new Mesh
                {
                    hideFlags = HideFlags.DontSave,
                    name = (mMaterial != null) ? "[NGUI]" + mMaterial.name : "[NGUI] Mesh"
                };
                mMesh.MarkDynamic();
            }
        }

    }

}
