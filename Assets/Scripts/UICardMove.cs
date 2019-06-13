using SLua;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UICardMoveEaseType
{
    easeInQuad,
    easeOutQuad,
    easeInOutQuad,
    easeInCubic,
    easeOutCubic,
    easeInOutCubic,
    easeInQuart,
    easeOutQuart,
    easeInOutQuart,
    easeInQuint,
    easeOutQuint,
    easeInOutQuint,
    easeInSine,
    easeOutSine,
    easeInOutSine,
    easeInExpo,
    easeOutExpo,
    easeInOutExpo,
    easeInCirc,
    easeOutCirc,
    easeInOutCirc,
    linear,
    spring,
    /* GFX47 MOD START */
    //bounce,
    easeInBounce,
    easeOutBounce,
    easeInOutBounce,
    /* GFX47 MOD END */
    easeInBack,
    easeOutBack,
    easeInOutBack,
    /* GFX47 MOD START */
    //elastic,
    easeInElastic,
    easeOutElastic,
    easeInOutElastic,
    /* GFX47 MOD END */
    punch
}
[SLua.CustomLuaClass]
public class UICardMove : MonoBehaviour
{
    public enum Movement
    {
        Horizontal,
        Vertical,
        InverseHorizontal,
        InverseVertical
    }

    struct InFocalInfo
    {
        public LuaFunction InFocalEvent;
        public bool bInFocal;
    }

    public float frameRate = 60;
    public Movement movement = Movement.Horizontal;

    public float allFrame = 80;
    public float leftMoveThresholdFrame = 70;
    public float leftMoveSpringThresholdFrame = 40;
    public float rightMoveThresholdFrame = 30;
    public float rightMoveSpringThresholdFrame = 50;
    public float middleMoveSpringThresholdFrame = 45;
    public float fewCountMoveSpringThresholdFrame = 50;
    public float frameStep = 10;
    public float relativeMoveCoefficient = 190.0f;
    public float springStrength = 10.0f;
    public bool outRangeDeactive = false;

    [DoNotToLua]
    public bool fixIpad = false;
    public List<GameObject> cards = new List<GameObject>();

    public Vector2 focalAreaLow = new Vector2();
    public Vector2 focalAreaMid = new Vector2();
    public Vector2 focalAreaHight = new Vector2();
    private Vector2 currFocalArea =  new Vector2();

    private float[] cardPlayAniNowTime;
    private bool m_bEnableSpring = true;
    private bool m_bInitProcess = false;
    private bool m_bSpringing = false;

    private Dictionary<int, InFocalInfo> cardInFocalEvents = new Dictionary<int, InFocalInfo>();
    public AnimationClip m_AnimationClip;
    private List<UIPanel> panelsCache = new List<UIPanel>();

    private float allFrameTime = 0.0f;
    private float rightMoveSpringThresholdFrameTime = 0.0f;
    private float rightMoveThresholdFrameTime = 0.0f;
    private float leftMoveSpringThresholdFrameTime = 0.0f;
    private float leftMoveThresholdFrameTime = 0.0f;
    private float middleMoveSpringThresholdFrameTime = 0.0f;
    private float relativeMoveDeltaCoefficient = 0.0f;

    private int nUICardMovePanelDepth = 0;

    public float firstShowSpringTime = 1.5f;
    private bool bUseFirstShowSpring = true;
    private bool bFirstShowSpringMoving = false;
    private float fFirstShowSpringCurrentValue = 0.0f;
    private iTween.EaseType iTweenEaseType = iTween.EaseType.easeInOutExpo;

    private float fCurrentMoveTickValue = 0.0f;
    private float fOldMoveTickValue = 0.0f;
    private float frameStepTime = 10;

    private int nCenterIndex = 0;
    private LuaFunction fzCenterOnCallback;
    private bool bIsInitProcess = false;

    public delegate void OnTickCallback();
    public OnTickCallback onTickCallback;

    public bool bCanDragMove
    {
        get
        {
            return m_bCanDragMove;
        }
        set
        {
            m_bCanDragMove = value;
        }
    }
    private bool m_bCanDragMove = true;
    private enum SpringDirection
    {
        Spring_None,
        Spring_To_Min,
        Spring_To_Mid,
        Spring_To_FewCount,
        Spring_To_Max,
        Spring_To_Select,
    }

    private SpringDirection springDirection = SpringDirection.Spring_None;

    // Update is called once per frame
    void LateUpdate()
    {
        SpringCard();
    }

    public void Add(GameObject go, LuaFunction callback = null)
    {
        cards.Add(go);

        if (!ReferenceEquals(callback, null))
        {
            var info = new InFocalInfo();
            info.InFocalEvent = callback;
            info.bInFocal = false;
            cardInFocalEvents.Add(go.GetHashCode(), info);
        }
    }

    public void AddReverse(GameObject go, LuaFunction callback = null)
    {
        cards.Insert(0, go);

        if (!ReferenceEquals(callback, null))
        {
            var info = new InFocalInfo();
            info.InFocalEvent = callback;
            info.bInFocal = false;
            cardInFocalEvents.Add(go.GetHashCode(), info);
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Destroy(cards[i]);
        }
        cards.Clear();
        cardInFocalEvents.Clear();

        panelsCache.Clear();

        m_bInitProcess = false;
    }

    [ContextMenu("ClearDontDestroy")]
    public void ClearDontDestroy()
    {
        cardInFocalEvents.Clear();
        cards.Clear();
        panelsCache.Clear();

        m_bInitProcess = false;
    }

    [ContextMenu("InitProcess")]
    public void InitProcess()
    {
        if (cards.Count < 1)
        {
            m_bInitProcess = false;
            return;
        }
        else
        {
            m_bInitProcess = true;
            m_bEnableSpring = true;

            fCurrentMoveTickValue = 0.0f;
            fOldMoveTickValue = 0.0f;
        }

        UIPanel uiPanel = GetComponent<UIPanel>();
        if (uiPanel != null)
        {
            nUICardMovePanelDepth = uiPanel.depth;
        }
        else
        {
            Debug.LogError("没有找到UIPanel组件");
        }

        var curQuality = 1;
        if (curQuality >= 3)
            currFocalArea = focalAreaHight;
        else if (curQuality == 2)
            currFocalArea = focalAreaMid;
        else
            currFocalArea = focalAreaLow;

        cardPlayAniNowTime = null;
        cardPlayAniNowTime = new float[cards.Count];

        allFrameTime = allFrame / frameRate;
        rightMoveSpringThresholdFrameTime = rightMoveSpringThresholdFrame / frameRate;
        rightMoveThresholdFrameTime = rightMoveThresholdFrame / frameRate;
        leftMoveSpringThresholdFrameTime = leftMoveSpringThresholdFrame / frameRate;
        leftMoveThresholdFrameTime = leftMoveThresholdFrame / frameRate;
        middleMoveSpringThresholdFrameTime = middleMoveSpringThresholdFrame / frameRate;

        //先做个简单的，手指滑动所有卡牌的速度相同。
        relativeMoveDeltaCoefficient = relativeMoveCoefficient / (frameStep / frameRate);

        frameStepTime = frameStep / frameRate;
        int nActiveCardNum = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            panelsCache.Add(cards[i].gameObject.GetComponent<UIPanel>());

            float initTime = (i - cards.Count + 1) * frameStep / frameRate;
            cardPlayAniNowTime[i] = initTime;

            if (outRangeDeactive)
            {
                if ((cardPlayAniNowTime[i] > allFrameTime) || (cardPlayAniNowTime[i] < 0))
                {
                    cards[i].gameObject.SetActive(false);
                }
                else
                {
                    nActiveCardNum++;
                    cards[i].gameObject.SetActive(true);
                    panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                }
            }
            else
            {
                nActiveCardNum++;
                cards[i].gameObject.SetActive(true);
                panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
            }
            cards[i].gameObject.transform.SetParent(this.gameObject.transform);
        }
        bUseFirstShowSpring = true;
        StopFirstShowSpring();
        SpringCard();

        if (fixIpad)
        {
            FixIpad();
        }
    }

    public void InitCenterOn(int idx, LuaFunction callback)
    {
        nCenterIndex = idx;
        fzCenterOnCallback = callback;
        InitProcess();
    }

    public void SetUICardMoveEaseType(UICardMoveEaseType type)
    {
        switch (type)
        {
            case UICardMoveEaseType.easeInQuad:
                iTweenEaseType = iTween.EaseType.easeInQuad;
                break;
            case UICardMoveEaseType.easeOutQuad:
                iTweenEaseType = iTween.EaseType.easeOutQuad;
                break;
            case UICardMoveEaseType.easeInOutQuad:
                iTweenEaseType = iTween.EaseType.easeInOutQuad;
                break;
            case UICardMoveEaseType.easeInCubic:
                iTweenEaseType = iTween.EaseType.easeInCubic;
                break;
            case UICardMoveEaseType.easeOutCubic:
                iTweenEaseType = iTween.EaseType.easeOutCubic;
                break;
            case UICardMoveEaseType.easeInOutCubic:
                iTweenEaseType = iTween.EaseType.easeInOutCubic;
                break;
            case UICardMoveEaseType.easeInQuart:
                iTweenEaseType = iTween.EaseType.easeInQuart;
                break;
            case UICardMoveEaseType.easeOutQuart:
                iTweenEaseType = iTween.EaseType.easeOutQuart;
                break;
            case UICardMoveEaseType.easeInOutQuart:
                iTweenEaseType = iTween.EaseType.easeInOutQuart;
                break;
            case UICardMoveEaseType.easeInQuint:
                iTweenEaseType = iTween.EaseType.easeInQuint;
                break;
            case UICardMoveEaseType.easeOutQuint:
                iTweenEaseType = iTween.EaseType.easeOutQuint;
                break;
            case UICardMoveEaseType.easeInOutQuint:
                iTweenEaseType = iTween.EaseType.easeInOutQuint;
                break;
            case UICardMoveEaseType.easeInSine:
                iTweenEaseType = iTween.EaseType.easeInSine;
                break;
            case UICardMoveEaseType.easeOutSine:
                iTweenEaseType = iTween.EaseType.easeOutSine;
                break;
            case UICardMoveEaseType.easeInOutSine:
                iTweenEaseType = iTween.EaseType.easeInOutSine;
                break;
            case UICardMoveEaseType.easeInExpo:
                iTweenEaseType = iTween.EaseType.easeInExpo;
                break;
            case UICardMoveEaseType.easeOutExpo:
                iTweenEaseType = iTween.EaseType.easeOutExpo;
                break;
            case UICardMoveEaseType.easeInOutExpo:
                iTweenEaseType = iTween.EaseType.easeInOutExpo;
                break;
            case UICardMoveEaseType.easeInCirc:
                iTweenEaseType = iTween.EaseType.easeInCirc;
                break;
            case UICardMoveEaseType.easeOutCirc:
                iTweenEaseType = iTween.EaseType.easeOutCirc;
                break;
            case UICardMoveEaseType.easeInOutCirc:
                iTweenEaseType = iTween.EaseType.easeInOutCirc;
                break;
            case UICardMoveEaseType.linear:
                iTweenEaseType = iTween.EaseType.linear;
                break;
            case UICardMoveEaseType.spring:
                iTweenEaseType = iTween.EaseType.spring;
                break;
            case UICardMoveEaseType.easeInBounce:
                iTweenEaseType = iTween.EaseType.easeInBounce;
                break;
            case UICardMoveEaseType.easeOutBounce:
                iTweenEaseType = iTween.EaseType.easeOutBounce;
                break;
            case UICardMoveEaseType.easeInOutBounce:
                iTweenEaseType = iTween.EaseType.easeInOutBounce;
                break;
            case UICardMoveEaseType.easeInBack:
                iTweenEaseType = iTween.EaseType.easeInBack;
                break;
            case UICardMoveEaseType.easeOutBack:
                iTweenEaseType = iTween.EaseType.easeOutBack;
                break;
            case UICardMoveEaseType.easeInOutBack:
                iTweenEaseType = iTween.EaseType.easeInOutBack;
                break;
            case UICardMoveEaseType.easeInElastic:
                iTweenEaseType = iTween.EaseType.easeInElastic;
                break;
            case UICardMoveEaseType.easeOutElastic:
                iTweenEaseType = iTween.EaseType.easeOutElastic;
                break;
            case UICardMoveEaseType.easeInOutElastic:
                iTweenEaseType = iTween.EaseType.easeInOutElastic;
                break;
            case UICardMoveEaseType.punch:
                iTweenEaseType = iTween.EaseType.punch;
                break;
            default:
                iTweenEaseType = iTween.EaseType.linear;
                break;
        }
    }

    public void Process(Vector3 relative)
    {
        if (!m_bInitProcess)
        {
            return;
        }

        if (!m_bCanDragMove)
        {
            return;
        }

        if (m_bSpringing)
        {
            return;
        }

        float deltaTime = 0.0f;
        if (movement == Movement.Horizontal)
        {
            deltaTime = relative.x / relativeMoveDeltaCoefficient;
        }
        else if (movement == Movement.Vertical)
        {
            deltaTime = relative.y / relativeMoveDeltaCoefficient;
        }
        else if (movement == Movement.InverseHorizontal)
        {
            deltaTime = relative.x / -relativeMoveDeltaCoefficient;
        }
        else if (movement == Movement.InverseVertical)
        {
            deltaTime = relative.y / -relativeMoveDeltaCoefficient;
        }

        //向右移动
        if (deltaTime > 0.0f)
        {
            float nowTime = cardPlayAniNowTime[cardPlayAniNowTime.Length - 1];
            nowTime -= deltaTime;

            if (nowTime < rightMoveThresholdFrameTime)
            {
                return;
            }
            else
            {
                fCurrentMoveTickValue -= deltaTime;

                if (Mathf.Abs(fCurrentMoveTickValue - fOldMoveTickValue) > frameStepTime)
                {
                    fOldMoveTickValue = fCurrentMoveTickValue;

                    if (onTickCallback != null)
                    {
                        onTickCallback();
                    }
                }
            }

            int nActiveCardNum = 0;
            //for (int i = cards.Count - 1; i >= 0; i--)
            for (int i = 0; i < cards.Count; i++)
            {
                nowTime = cardPlayAniNowTime[i];
                nowTime -= deltaTime;

                cardPlayAniNowTime[i] = nowTime;

                if (cardPlayAniNowTime[i] > allFrameTime)
                {
                    SetSampleAnimation(m_AnimationClip, cards[i], allFrameTime);
                }
                else if (cardPlayAniNowTime[i] < 0.0f)
                {
                    SetSampleAnimation(m_AnimationClip, cards[i], 0.0f);
                }
                else
                {
                    SetSampleAnimation(m_AnimationClip, cards[i], nowTime);
                }

                if (outRangeDeactive)
                {
                    if ((cardPlayAniNowTime[i] > allFrameTime) || (cardPlayAniNowTime[i] < 0))
                    {
                        cards[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        nActiveCardNum++;
                        cards[i].gameObject.SetActive(true);
                        panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                    }
                }
                else
                {
                    nActiveCardNum++;
                    cards[i].gameObject.SetActive(true);
                    panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                }
            }
        }

        //向左移动
        if (deltaTime < 0.0f)
        {
            float nowTime = cardPlayAniNowTime[0];
            nowTime -= deltaTime;

            if (nowTime > leftMoveThresholdFrameTime)
            {
                return;
            }
            else
            {
                fCurrentMoveTickValue -= deltaTime;

                if (Mathf.Abs(fCurrentMoveTickValue - fOldMoveTickValue) > frameStepTime)
                {
                    fOldMoveTickValue = fCurrentMoveTickValue;

                    if (onTickCallback != null)
                    {
                        onTickCallback();
                    }
                }
            }

            int nActiveCardNum = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                nowTime = cardPlayAniNowTime[i];
                nowTime -= deltaTime;

                cardPlayAniNowTime[i] = nowTime;

                if (cardPlayAniNowTime[i] > allFrameTime)
                {
                    SetSampleAnimation(m_AnimationClip, cards[i], allFrameTime);
                }
                else if (cardPlayAniNowTime[i] < 0.0f)
                {
                    SetSampleAnimation(m_AnimationClip, cards[i], 0.0f);
                }
                else
                {
                    SetSampleAnimation(m_AnimationClip, cards[i], nowTime);
                }

                if (outRangeDeactive)
                {
                    if ((cardPlayAniNowTime[i] > allFrameTime) || (cardPlayAniNowTime[i] < 0))
                    {
                        cards[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        nActiveCardNum++;
                        cards[i].gameObject.SetActive(true);
                        panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                    }
                }
                else
                {
                    nActiveCardNum++;
                    cards[i].gameObject.SetActive(true);
                    panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                }
            }
        }
    }

    private void SpringCard()
    {
        if (m_bInitProcess)
        {
            springDirection = SpringDirection.Spring_None;

            if (nCenterIndex > 0)
            {
                springDirection = SpringDirection.Spring_To_Select;
            }
            else if (cards.Count < (rightMoveSpringThresholdFrame - leftMoveSpringThresholdFrame) / frameStep)
            {
                if ((cardPlayAniNowTime[cardPlayAniNowTime.Length - 1] > middleMoveSpringThresholdFrameTime + (cards.Count - 1) * frameStepTime / 2.0f + 0.01f)
                    || (cardPlayAniNowTime[cardPlayAniNowTime.Length - 1] < middleMoveSpringThresholdFrameTime + (cards.Count - 1) * frameStepTime / 2.0f - 0.01f))
                {
                    springDirection = SpringDirection.Spring_To_Mid;
                }
            }
            else
            {
                if (cards.Count == 2)
                {
                    springDirection = SpringDirection.Spring_To_Min;
                }
                else
                {
                    if (cardPlayAniNowTime[cardPlayAniNowTime.Length - 1] < rightMoveSpringThresholdFrameTime - 0.01f)
                    {
                        springDirection = SpringDirection.Spring_To_Max;
                    }
                    else if (cardPlayAniNowTime[0] > leftMoveSpringThresholdFrameTime + 0.01f)
                    {
                        springDirection = SpringDirection.Spring_To_Min;
                    }
                }
            }

            if (m_bEnableSpring)
            {
                if (springDirection == SpringDirection.Spring_None)
                {
                    m_bSpringing = false;
                    return;
                }

                float target = 0.0f;
                float before = 0.0f;
                float after = 0.0f;
                float delta = RealTime.deltaTime;
                float difference = 0.0f;
                switch (springDirection)
                {
                    case SpringDirection.Spring_To_Min:
                        target = leftMoveSpringThresholdFrameTime;
                        before = cardPlayAniNowTime[0];
                        break;
                    case SpringDirection.Spring_To_Max:
                        target = rightMoveSpringThresholdFrameTime;
                        before = cardPlayAniNowTime[cardPlayAniNowTime.Length - 1];
                        break;
                    case SpringDirection.Spring_To_Mid:
                        target = middleMoveSpringThresholdFrameTime + (cards.Count - 2) * frameStepTime / 2.0f;
                        before = cardPlayAniNowTime[cardPlayAniNowTime.Length - 1];
                        break;
                    case SpringDirection.Spring_To_Select:
                        if (nCenterIndex < 0 || cardPlayAniNowTime.Length - nCenterIndex < 0)
                        {
                            Debug.LogError("The index of choices exceeds the total");
                            return;
                        }
                        target = middleMoveSpringThresholdFrameTime;
                        before = cardPlayAniNowTime[cardPlayAniNowTime.Length - nCenterIndex];
                        break;
                }
                if (bUseFirstShowSpring)
                {
                    if (bFirstShowSpringMoving)
                    {
                        after = fFirstShowSpringCurrentValue;
                    }
                    else
                    {
                        FirstShowSpring(before, target, firstShowSpringTime);
                    }
                }
                else
                {
                    after = NGUIMath.SpringLerp(before, target, springStrength, delta);
                    if (Mathf.Abs(after - target) < 0.005f)
                    {
                        after = target;
                    }
                }

                if (!bUseFirstShowSpring && Mathf.Abs(after - before) < 0.001f)
                {
                    springDirection = SpringDirection.Spring_None;

                    m_bEnableSpring = false;

                    if (bFirstShowSpringMoving)
                    {
                        StopFirstShowSpring();
                    }

                    m_bSpringing = false;
                }
                else
                {
                    m_bSpringing = true;
                }

                difference = after - before;

                if (bUseFirstShowSpring == false)
                {
                    fCurrentMoveTickValue += difference;

                    if (Mathf.Abs(fCurrentMoveTickValue - fOldMoveTickValue) > frameStepTime)
                    {
                        fOldMoveTickValue = fCurrentMoveTickValue;

                        if (onTickCallback != null)
                        {
                            onTickCallback();
                        }
                    }
                }

                int nActiveCardNum = 0;
                for (int i = 0; i < cardPlayAniNowTime.Length; i++)
                {
                    cardPlayAniNowTime[i] += difference;

                    if (cardPlayAniNowTime[i] > allFrameTime)
                    {
                        SetSampleAnimation(m_AnimationClip, cards[i], allFrameTime);
                    }
                    else if (cardPlayAniNowTime[i] < 0.0f)
                    {
                        SetSampleAnimation(m_AnimationClip, cards[i], 0.0f);
                    }
                    else
                    {
                        SetSampleAnimation(m_AnimationClip, cards[i], cardPlayAniNowTime[i]);
                    }

                    if (outRangeDeactive)
                    {
                        if ((cardPlayAniNowTime[i] > allFrameTime) || (cardPlayAniNowTime[i] < 0))
                        {
                            cards[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            nActiveCardNum++;
                            cards[i].gameObject.SetActive(true);
                            panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                        }
                    }
                    else
                    {
                        nActiveCardNum++;
                        cards[i].gameObject.SetActive(true);
                        panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                    }
                }
            }
        }
    }

    public void EnableSpring(bool bEnable)
    {
        m_bEnableSpring = bEnable;

        if (!m_bEnableSpring)
        {
            springDirection = SpringDirection.Spring_None;
        }

        bUseFirstShowSpring = false;

        if (bFirstShowSpringMoving)
        {
            StopFirstShowSpring();
        }
    }

    private void FirstShowSpring(float fFrom, float fTo, float fTime)
    {
        bFirstShowSpringMoving = true;
        fFirstShowSpringCurrentValue = fFrom;

        Hashtable args = null;
        args = iTween.Hash(new object[]
            {
                "from",
                fFrom,
                "to",
                 fTo,
                "time",
                fTime,
                "onupdate",
                "FirstShowSpringOnUpdate",
                "oncomplete",
                "OnFirstShowSpringComplete",
                "onupdatetarget",
                gameObject,
                "easetype",
                iTweenEaseType,
                "name",
                "FirstShowSpring"
            });
        iTween.StopByName(gameObject, "FirstShowSpring");
        iTween.ValueTo(gameObject, args);
    }

    private void OnFirstShowSpringComplete()
    {
        nCenterIndex = 0;
        if (!ReferenceEquals(fzCenterOnCallback, null))
        {
            var temp = fzCenterOnCallback;
            fzCenterOnCallback = null;
            temp.call();
        }
    }

    private void StopFirstShowSpring()
    {
        iTween.StopByName(gameObject, "FirstShowSpring");


        bFirstShowSpringMoving = false;
    }

    private void FirstShowSpringOnUpdate(float val)
    {
        fFirstShowSpringCurrentValue = val;
    }

    [ContextMenu("Execute")]
    public void MoveToMiddlePos(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        float deltaTime = 0.0f;

        for (int i = 0; i < cards.Count; i++)
        {
            if (go == cards[i])
            {
                deltaTime = cardPlayAniNowTime[i];
                break;
            }
        }

        float nowTime = 0.0f;
        deltaTime = middleMoveSpringThresholdFrameTime - deltaTime - frameStepTime / 2;

        fCurrentMoveTickValue -= deltaTime;

        if (Mathf.Abs(fCurrentMoveTickValue - fOldMoveTickValue) > frameStepTime)
        {
            fOldMoveTickValue = fCurrentMoveTickValue;

            if (onTickCallback != null)
            {
                onTickCallback();
            }
        }

        int nActiveCardNum = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            nowTime = cardPlayAniNowTime[i];
            nowTime += deltaTime;

            cardPlayAniNowTime[i] = nowTime;

            if (cardPlayAniNowTime[i] > allFrameTime)
            {
                SetSampleAnimation(m_AnimationClip, cards[i], allFrameTime);
            }
            else if (cardPlayAniNowTime[i] < 0.0f)
            {
                SetSampleAnimation(m_AnimationClip, cards[i], 0.0f);
            }
            else
            {
                SetSampleAnimation(m_AnimationClip, cards[i], nowTime);
            }

            if (outRangeDeactive)
            {
                if ((cardPlayAniNowTime[i] > allFrameTime) || (cardPlayAniNowTime[i] < 0))
                {
                    cards[i].gameObject.SetActive(false);
                }
                else
                {
                    nActiveCardNum++;
                    cards[i].gameObject.SetActive(true);
                    panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
                }
            }
            else
            {
                nActiveCardNum++;
                cards[i].gameObject.SetActive(true);
                panelsCache[i].depth = nUICardMovePanelDepth + nActiveCardNum;
            }
        }

        EnableSpring(true);
    }

    void SetSampleAnimation(AnimationClip clip, GameObject go, float time)
    {
        clip.SampleAnimation(go, time);

        if (cardInFocalEvents.ContainsKey(go.GetHashCode()))
        {
            var info = cardInFocalEvents[go.GetHashCode()];
            if (!info.bInFocal && time >= currFocalArea.x && time <= currFocalArea.y)
            {
                info.InFocalEvent.call(true, time);
                info.bInFocal = true;
                cardInFocalEvents[go.GetHashCode()] = info;
            }
            else if (info.bInFocal && (time < currFocalArea.x || time > currFocalArea.y))
            {
                info.InFocalEvent.call(false, time);
                info.bInFocal = false;
                cardInFocalEvents[go.GetHashCode()] = info;
            }
        }
    }

    void FixIpad()
    {
        float ratio = Screen.width / Screen.height;
        if (ratio < 1280.0f / 720.0f)
        {
            float scale = (Screen.height / (Screen.width / 1280.0f)) / 720.0f;
            transform.localScale = new UnityEngine.Vector3(scale, scale, 1);
            transform.localPosition = new UnityEngine.Vector3(1280 * (scale - 1) * 0.5f, 0, 0);
        }
    }
}
