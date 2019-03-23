using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isPass;             // 判断是否遍历过

    private int id;                 // 编号
    private string path;            // 路径
    private string name;            // 资源名
    private string md5;             // 文件的MD5码
    private string metaName;        // meta文件名
    private string metaMD5;         // meta文件md5码
    private string assetBundleName; // 所属的assetBundle包名

    protected int inDegree;         // 入度
    protected int outDegree;        // 出度
    protected List<Node> parent;    // 父节点列表
    protected List<Node> children;  // 子节点列表


}
