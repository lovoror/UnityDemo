using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDragRole : MonoBehaviour {

    public  GameObject updateGo=null;
    private float xbias = 0;
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //updateGo = player.transform.GetChild(0).gameObject ;    
    }
    void OnDrag(Vector2 vos)
    {
        if (updateGo == null)
        {
            return;
        }
        Quaternion x = Quaternion.Euler(new Vector3(xbias, 0, 0));
        //updateGo.transform.localRotation = Quaternion.Euler(new Vector3(-90, updateGo.transform.localRotation.eulerAngles.y - vos.x, 0)); //modify LG 打桩测试
        updateGo.transform.localRotation = Quaternion.Euler(new Vector3(0, updateGo.transform.localRotation.eulerAngles.y - vos.x, 0));
        updateGo.transform.localRotation = x * updateGo.transform.localRotation;
    }

    public GameObject GetPlayer(uint index)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player.transform.GetChild((int)index).gameObject;
    }


    private void SetPlayer(uint index)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = GetPlayer((uint)i);
            if (i == index)
            {
                go.transform.localRotation = Quaternion.Euler(new Vector3(0, 30, 0));
            }
            go.gameObject.SetActive(i == index);
        }

        CreateRoleMovieCtrl.instance.PlayeAnimation((int)(110001 + index));
    }

    //public void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.A))
    //    {
    //        CreateRoleMovieCtrl.instance.OnClickZoom();
    //    }
    //    if(Input.GetKeyUp(KeyCode.Keypad1))
    //    {
    //        SetPlayer(0);
    //    }
    //    if (Input.GetKeyUp(KeyCode.Keypad2))
    //    {
    //        SetPlayer(1);
    //    }
    //    if (Input.GetKeyUp(KeyCode.Keypad3))
    //    {
    //        SetPlayer(2);
    //    }
    //}
}
