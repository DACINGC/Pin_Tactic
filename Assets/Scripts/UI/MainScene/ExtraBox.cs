using UnityEngine.UI;

public class ExtraBox : BaseUI
{
    private Button coinButton;
    private Button freeButton;
    private Button closeButton;

    protected override void Awake()
    {
        base.Awake();
        closeButton = tableTransform.Find("Button Close").GetComponent<Button>();
        coinButton = tableTransform.Find("Button Gold").GetComponent<Button>();
        freeButton = tableTransform.Find("Button Free").GetComponent<Button>();

        closeButton.onClick.AddListener(CloseEvent);
        coinButton.onClick.AddListener(CoinEvent);
        freeButton.onClick.AddListener(FreeEvent);
    }


    private void CloseEvent()
    {
        UIManager.Instance.HideUI<ExtraBox>();
    }

    private void CoinEvent()
    {
        if (GameDataManager.DecreaseCoinCount(100))
        {
            //增加数量
            GameDataManager.AddItemCount(ItemType.DoubleBox, 1);
            MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.DoubleBox);

            UIManager.Instance.HideUI<ExtraBox>();
        }
    }

    private void FreeEvent()
    {
        GameDataManager.AddItemCount(ItemType.DoubleBox, 1);
        MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.DoubleBox);

        UIManager.Instance.HideUI<ExtraBox>();
    }
}
