using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewSliderBase : ViewBase
{
    private Slider slider;
    public Slider Slider => slider;

    [SerializeField] TextMeshProUGUI text;

    public override void Awake()
    {
        slider = GetComponent<Slider>();

        base.Awake();
    }

    public void Start()
    {
        text.text = slider.value.ToString();
        slider.onValueChanged.AddListener((e) => 
        {
            text.text = e.ToString();
        });
    }

    public override void Close()
    {
        base.Close();
    }

    public override void OnChanged()
    {
        base.OnChanged();
    }

    public override void Open(ControllerBase ctrl)
    {
        base.Open(ctrl);
    }

    public override void PointerClick()
    {
        base.PointerClick();
    }

    public override void PointerEnter()
    {
        base.PointerEnter();
    }

    public override void PointerExit()
    {
        base.PointerExit();
    }
}
