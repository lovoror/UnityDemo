//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the widget's size.
/// </summary>

[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Tween/Tween SpriteAmont.cs")]
public class TweenSpriteAmont : UITweener
{
    public float from = 0.0f;
    public float to = 1f;

    UISprite mUISprite;

    public UISprite cachedWidget { get { if (mUISprite == null) mUISprite = GetComponent<UISprite>(); return mUISprite; } }

	[System.Obsolete("Use 'value' instead")]
    public float spriteAmont { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

    public float value 
    { 
        get 
        {
            if (cachedWidget == null)
            {
                enabled = false;
                return 0f;
            }
            else
            {
                return cachedWidget.fillAmount; 
            }
            
            
        } 
        set 
        {
            if (cachedWidget == null)
            {
                enabled = false;
            }
            else
            {
                cachedWidget.fillAmount = value; 
            }
        } 
      }

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished)
	{
        value = (from * (1f - factor) + to * factor);
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

    static public TweenSpriteAmont Begin(UIWidget widget, float duration, int width)
	{
        TweenSpriteAmont comp = UITweener.Begin<TweenSpriteAmont>(widget.gameObject, duration);
		comp.from = widget.width;
		comp.to = width;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }
}
