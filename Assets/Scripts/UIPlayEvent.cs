using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SLua;

[AddComponentMenu("NGUI/Interaction/Play Event")]
[SLua.CustomLuaClass]
public class UIPlayEvent : MonoBehaviour
{
//#if UNITY_EDITOR      // 先解决宕机问题，后续最好加入NonSerialized并把所有资源刷一遍
    [SLua.DoNotToLua]
    public byte[] valueGuid = new byte[16];
//#endif

    public enum Trigger
    {
        OnClick,
        OnMouseOver,
        OnMouseOut,
        OnPress,
        OnRelease,
        Custom,
        OnEnable,
        OnDisable,
        OnDragStart,
        OnDrag,
    }

    public string soundEvent = string.Empty;
    public string strBankName = "UI";
    public string strStateName = string.Empty;
    public string strStateGourpName = string.Empty;

    public int nAudioID;

    public bool bForcePlayOnDestroy = false;

    public Trigger trigger = Trigger.OnClick;

    bool mIsOver = false;

    //拖拽音效间隔时间
    [SLua.DoNotToLua]
    public float DragSoundInterval = 1.0f;

    private float mfLastDragSoundTime;
    bool canPlay
    {
        get
        {
            if (!enabled) return false;
            UIButton btn = GetComponent<UIButton>();
            return (btn == null || btn.isEnabled);
        }
    }

    private void Start()
    {
        mfLastDragSoundTime = UnityEngine.Time.realtimeSinceStartup;
    }
    void OnEnable()
    {

    }

    void OnDisable()
    {
        if (trigger == Trigger.OnDisable)
            Play();
    }

    void OnHover(bool isOver)
    {
        if (trigger == Trigger.OnMouseOver)
        {
            if (mIsOver == isOver) return;
            mIsOver = isOver;
        }

        if (canPlay && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
            Play();
    }

    void OnPress(bool isPressed)
    {
        if (trigger == Trigger.OnPress)
        {
            if (mIsOver == isPressed) return;
            mIsOver = isPressed;
        }

        if (canPlay && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
            Play();
    }

    void OnClick()
    {
        if (canPlay && trigger == Trigger.OnClick)
            Play();
    }

    void OnSelect(bool isSelected)
    {
        if (canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
            OnHover(isSelected);
    }
    void OnDragStart()
    {
        if(canPlay &&trigger == Trigger.OnDragStart)
        {
            Play();
        }
    }
    private void OnDrag(Vector2 delta)
    {
        if (canPlay && trigger == Trigger.OnDrag)
        {
            var currentTime = UnityEngine.Time.realtimeSinceStartup;
            if(currentTime >= mfLastDragSoundTime + DragSoundInterval)
            Play();
        }
    }
    public void Play()
    {

    }

    private void OnDestroy()
    {

    }
}
