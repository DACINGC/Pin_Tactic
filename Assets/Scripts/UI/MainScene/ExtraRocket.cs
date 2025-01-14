using UnityEngine.UI;

public class ExtraRocket : BaseUI
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
        UIManager.Instance.HideUI<ExtraRocket>();
    }

    private void CoinEvent()
    {
        if (GameDataManager.DecreaseCoinCount(100))
        {
            GameDataManager.AddItemCount(ItemType.Rocket, 1);
            MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Rocket);
            UIManager.Instance.HideUI<ExtraRocket>();
        }
    }

    private void FreeEvent()
    {
        GameDataManager.AddItemCount(ItemType.Rocket, 1);
        MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Rocket);
        UIManager.Instance.HideUI<ExtraRocket>();
    }
}
