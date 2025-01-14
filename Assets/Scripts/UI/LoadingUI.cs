using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    private Transform barTrans;
    private Image progressBar;
    private Text progressText;
    private Text loadingText;
    public AssetReference chineseFontReference;
    public Font ChineseFont;
    AsyncOperationHandle<Font> fontHandle;

    protected void Awake()
    {
        YLocalization.lanaguage = YLocalization.Lanaguage.Chinese;
        barTrans = transform.Find("BG/bar");
        progressBar = barTrans.Find("slider").GetComponent<Image>();
        progressText = barTrans.Find("per").GetComponent<Text>();
        loadingText = barTrans.Find("Loading").GetComponent<Text>();
        DontDestroyOnLoad(transform.parent);
    }

    private async void Start()
    {
        GameDataManager.Initialize();//��������
        if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
        {
            if (loadingText != null)
            {
                loadingText.font = ChineseFont;
            }
            if (progressText != null)
            {
                progressText.font = ChineseFont;
            }
        }

        Debug.Log("��ʼ������Դ");
        await ResourceLoader.Instance.InitializeAsync(default, count =>
        {
            progressText.text = $"{(int)(count * 100)}%";
            progressBar.fillAmount = count;
        });
        AnimationUtility.FadeOut(barTrans.parent, 1f, () =>
        {
            barTrans.parent.gameObject.SetActive(false);
        });
    }
    private async Task LoadChineseFontAsync()
    {
        if (chineseFontReference != null)
        {
            try
            {
                // �첽����������Դ
                fontHandle = chineseFontReference.LoadAssetAsync<Font>();

                // �ȴ��������
                Font loadedFont = await fontHandle.Task;

                // �����帳ֵ��UI Text���
                if (loadingText != null)
                {
                    loadingText.font = loadedFont;
                }
                if (progressText != null)
                {
                    progressText.font = loadedFont;
                }


            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load Chinese font: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Chinese font reference is not set.");
        }
    }
}
