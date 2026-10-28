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

            //text.transform.DOPunchScale(new Vector3(1,1,1) , 0.3f);
            text.transform.DOScale(1.1f, 0.1f).OnComplete(() => 
            {
                text.transform.DOScale(1f, 0.1f).SetDelay(1000);
            });
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
