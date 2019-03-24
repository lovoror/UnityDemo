using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public bool isPass;             // 判断是否遍历过
    public int id;                  // 编号
    public string path;             // 路径
    public string name;             // 资源名
    public string md5;              // 文件的MD5码
    public string metaName;         // meta文件名
    public string metaMD5;          // meta文件md5码
    public string assetBundleName;  // 所属的assetBundle包名
    public int inDegree;            // 入度
    public int outDegree;           // 出度
    public List<Node> parent;       // 父节点列表
    public List<Node> children;     // 子节点列表

    public bool AddChildNode(Node node)
    {
        if (node == null)
        {
            Debug.LogError("child node is null");
            return false;
        }
        int i;
        for (i = 0; i < outDegree; i++)
        {
            if (children[i].id == node.id)
            {
                Debug.LogError("child node is exist");
                return false;
            }
        }

        // find position for insert
        for (i = 0; i < outDegree; i++)
        {
            if(node.id < children[i].id)
            {
                break;
            }
        }
        if (node.AddParentNode(this))
        {
            children.Insert(i, node);
            outDegree++;
            return true;
        }
        Debug.LogError("add node failed");
        return false;
    }
    public bool AddParentNode(Node node)
    {
        if (node == null)
        {
            Debug.LogError("parent node is null");
            return false;
        }
        int i;
        for (i = 0; i < inDegree; i++)
        {
            if (parent[i].id == node.id)
            {
                Debug.LogError("parent node is exist");
                return false;
            }
        }
        for (i = 0; i < inDegree; i++)
        {
            if (node.id < parent[i].id)
            {
                break;
            }
        }
        parent.Insert(i, node);
        inDegree++;
        return true;
    }
}
