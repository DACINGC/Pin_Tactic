using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using TMPro;
using System;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ResourceLoader
{
    private static readonly ResourceLoader _instance = new ResourceLoader();
    public static ResourceLoader Instance => _instance;

    private Dictionary<string, Sprite> boomSpriteDictionary;
    private Dictionary<string, Sprite> boxBoardSpriteDictionary;
    private Dictionary<string, GameObject> fxGameObjectDictionary;
    private Dictionary<string, Sprite> unlockImageDictionary;

    private Dictionary<string, GameObject> levelsDictionary;
    private Dictionary<string, GameObject> boxLevelsDictionary;

    private Font _norFont;
    private TMP_FontAsset _tmpFont;
    private SceneInstance _loadedScene;

    private Dictionary<string, AsyncOperationHandle> handlesDictionary;//�洢���������ж����Դ

    private List<AsyncOperationHandle> _handles;

    public Font NorFont => _norFont;
    public TMP_FontAsset TmpFont => _tmpFont;
    public SceneInstance SceneResource => _loadedScene;

    public Dictionary<string, GameObject> Levels => levelsDictionary;
    public Dictionary<string, GameObject> BoxLevels => boxLevelsDictionary;

    private ResourceLoader()
    {
        boomSpriteDictionary = new Dictionary<string, Sprite>();
        boxBoardSpriteDictionary = new Dictionary<string, Sprite>();
        fxGameObjectDictionary = new Dictionary<string, GameObject>();
        unlockImageDictionary = new Dictionary<string, Sprite>();
        levelsDictionary = new Dictionary<string, GameObject>();
        boxLevelsDictionary = new Dictionary<string, GameObject>();
        handlesDictionary = new Dictionary<string, AsyncOperationHandle>();

        _handles = new List<AsyncOperationHandle>();
    }

    #region ������Դ
    /// <summary>
    /// �첽��ʼ������������������Դ
    /// </summary>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _norFont = await LoadFontAsync("˼Դ����", cancellationToken);
        _tmpFont = await LoadTMPFontAsync("˼Դ����tmp", cancellationToken);

        //await LoadGameObjectsToDictionaryAsync("Level", levelsDictionary, cancellationToken);
        //await LoadGameObjectsToDictionaryAsync("BoxLevel", boxLevelsDictionary, cancellationToken);
        await LoadSpritesToDictionaryAsync("UnlockImage", unlockImageDictionary, cancellationToken);
        await LoadSpritesToDictionaryAsync("BoxBoardImage", boxBoardSpriteDictionary, cancellationToken);
        await LoadSpritesToDictionaryAsync("BoomImage", boomSpriteDictionary, cancellationToken);
        await LoadGameObjectsToDictionaryAsync("Fx", fxGameObjectDictionary, cancellationToken);
    }

    /// <summary>
    /// �첽��ʼ������������
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="onProgress"></param>
    /// <returns></returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default, Action<float> onProgress = null)
    {
        // ��������Դ����
        int totalResources = 102; // ������Դ
        int loadedResources = 0;
        onProgress?.Invoke((float)loadedResources / totalResources);

        totalResources += await GetGameObjectResourceCountAsync("Fx");
        loadedResources += 5;
        onProgress?.Invoke((float)loadedResources / totalResources);

        totalResources += await GetSpriteResourceCountAsync("UnlockImage");
        loadedResources += 25;
        onProgress?.Invoke((float)loadedResources / totalResources);

        totalResources += await GetSpriteResourceCountAsync("BoxBoardImage");
        loadedResources += 30;
        onProgress?.Invoke((float)loadedResources / totalResources);

        totalResources += await GetSpriteResourceCountAsync("BoomImage");
        loadedResources += 20;
        onProgress?.Invoke((float)loadedResources / totalResources);

        // ����������Դ
        _norFont = await LoadFontAsync("˼Դ����", cancellationToken);
        loadedResources++;
        onProgress?.Invoke((float)loadedResources / totalResources);

        _tmpFont = await LoadTMPFontAsync("˼Դ����tmp", cancellationToken);
        loadedResources++;
        onProgress?.Invoke((float)loadedResources / totalResources);
        Debug.Log("��ȡ��Դ�������");

        // ����������Դ
        //���ݵ�ǰ�ؿ�����������Դ

        int num = GameDataManager.CurrentGameData.levelNum;
        if (num % 10 != 0)
            num = NameUtility.SetLastDigitToZero(num + 10);

        await LoadLevelGroup(num);
        loadedResources += 10;
        onProgress?.Invoke((float)loadedResources / totalResources);

        await LoadBoxLevelGroup(num);
        loadedResources += 10;
        onProgress?.Invoke((float)loadedResources / totalResources);

        await LoadSpritesToDictionaryAsync("UnlockImage", unlockImageDictionary, cancellationToken, () =>
        {
            loadedResources++;
            onProgress?.Invoke((float)loadedResources / totalResources);
        });

        await LoadSpritesToDictionaryAsync("BoxBoardImage", boxBoardSpriteDictionary, cancellationToken, () =>
        {
            loadedResources++;
            onProgress?.Invoke((float)loadedResources / totalResources);
        });

        await LoadSpritesToDictionaryAsync("BoomImage", boomSpriteDictionary, cancellationToken, () =>
        {
            loadedResources++;
            onProgress?.Invoke((float)loadedResources / totalResources);
        });

        await LoadGameObjectsToDictionaryAsync("Fx", fxGameObjectDictionary, cancellationToken, () =>
        {
            loadedResources++;
            onProgress?.Invoke((float)loadedResources / totalResources);
        });

        //���س�����Դ
        _loadedScene = await LoadSceneResourceAsync("Home", cancellationToken);

        loadedResources++;
        onProgress?.Invoke((float)loadedResources / totalResources);
    }

    /// <summary>
    /// ��ȡ��Դ���е���Դ����
    /// </summary>
    private async Task<int> GetSpriteResourceCountAsync(string groupName)
    {
        var loadHandle = Addressables.LoadAssetsAsync<Sprite>(groupName, null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            return loadHandle.Result.Count;
        }

        return 0;
    }
    private async Task<int> GetGameObjectResourceCountAsync(string groupName)
    {
        var loadHandle = Addressables.LoadAssetsAsync<GameObject>(groupName, null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            return loadHandle.Result.Count;
        }

        return 0;
    }


    /// <summary>
    /// �첽����ָ����Դ������о��鵽�ֵ��У����ӳ�ʱ����
    /// </summary>
    private async Task LoadSpritesToDictionaryAsync(string groupName, Dictionary<string, Sprite> targetDictionary, 
        CancellationToken cancellationToken, Action onResourceLoaded = null)
    {
        var loadHandle = Addressables.LoadAssetsAsync<Sprite>(groupName, null);
        _handles.Add(loadHandle); // ��¼���ؾ��
        handlesDictionary.Add(groupName, loadHandle);

        try
        {
            // ���ó�ʱʱ��Ϊ 10 ��
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                int resourceCount = loadHandle.Result.Count; // ��ȡ���ص���Դ����
                Debug.Log($"�ɹ����� {resourceCount} �� Sprite ��Դ����Դ��: {groupName}");

                foreach (var sprite in loadHandle.Result)
                {
                    string key = sprite.name.ToLower();
                    if (!targetDictionary.ContainsKey(key))
                    {
                        targetDictionary[key] = sprite;
                    }

                    // ���¼��ؽ���
                    onResourceLoaded?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"δ�ܼ��ص��κ� Sprite������ Addressables/{groupName} ��Դ���е���Դ");
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"���� {groupName} ��Դ�鳬ʱ��������Դ�Ƿ���ȷ����");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"���� {groupName} ��Դ�鱻ȡ��");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
        }
    }

    /// <summary>
    /// �첽����ָ����Դ���������Ϸ���嵽�ֵ��У����ӳ�ʱ����
    /// </summary>
    private async Task LoadGameObjectsToDictionaryAsync(string groupName, Dictionary<string, GameObject> targetDictionary, 
        CancellationToken cancellationToken, Action onResourceLoaded = null)
    {
        var loadHandle = Addressables.LoadAssetsAsync<GameObject>(groupName, null);
        _handles.Add(loadHandle); // ��¼���ؾ��
        handlesDictionary.Add(groupName, loadHandle);

        try
        {
            // ���ó�ʱʱ��Ϊ 10 ��
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                int resourceCount = loadHandle.Result.Count; // ��ȡ���ص���Դ����
                Debug.Log($"�ɹ����� {resourceCount} �� GameObject ��Դ����Դ��: {groupName}");

                foreach (var gameObject in loadHandle.Result)
                {
                    string key = gameObject.name;
                    if (!targetDictionary.ContainsKey(key))
                    {
                        targetDictionary[key] = gameObject;
                    }

                    // ���¼��ؽ���
                    onResourceLoaded?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"δ�ܼ��ص��κ� GameObject������ Addressables/{groupName} ��Դ���е���Դ");
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"���� {groupName} ��Դ�鳬ʱ��������Դ�Ƿ���ȷ����");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"���� {groupName} ��Դ�鱻ȡ��");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
        }
    }

    /// <summary>
    /// ���ݹؿ������عؿ�
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public async Task LoadLevelGroup(int num)
    {
        string groupName = "Level" + num;

        // �����Դ���Ƿ��Ѿ�����
        if (handlesDictionary.ContainsKey(groupName))
        {
            Debug.Log($"��Դ�� {groupName} �Ѿ����ع����������ء�");
            return;
        }

        try
        {
            var _loadHandle = Addressables.LoadAssetsAsync<GameObject>(groupName, null);
            await _loadHandle.Task;

            handlesDictionary.Add(groupName, _loadHandle);

            if (_loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<GameObject> loadedAssets = _loadHandle.Result;
                foreach (var asset in loadedAssets)
                {
                    levelsDictionary[asset.name] = asset;
                }

                Debug.Log($"��Դ�� {groupName} ���سɹ�!");
            }
            else
            {
                Debug.LogError($"��Դ�� {groupName} ����ʧ��!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"��Դ�� {groupName} ����ʧ��: {e.Message}");
        }
    }

    public async Task LoadBoxLevelGroup(int num)
    {
        string groupName = "BoxLevel" + num;

        // �����Դ���Ƿ��Ѿ�����
        if (handlesDictionary.ContainsKey(groupName))
        {
            Debug.Log($"��Դ�� {groupName} �Ѿ����ع����������ء�");
            return;
        }

        try
        {
            var _loadHandle = Addressables.LoadAssetsAsync<GameObject>(groupName, null);
            await _loadHandle.Task;
            handlesDictionary.Add(groupName, _loadHandle);

            if (_loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<GameObject> loadedAssets = _loadHandle.Result;
                foreach (var asset in loadedAssets)
                {
                    boxLevelsDictionary[asset.name] = asset;
                }

                Debug.Log($"��Դ�� {groupName} ���سɹ�!");
            }
            else
            {
                Debug.LogError($"��Դ�� {groupName} ����ʧ��!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"��Դ�� {groupName} ����ʧ��: {e.Message}");
        }
    }

    public async Task LoadLevelResources(int num)
    {
        await LoadLevelGroup(num);
        await LoadBoxLevelGroup(num);
    }
    /// <summary>
    /// �첽���ص��� Font �������壬���ӳ�ʱ����
    /// </summary>
    private async Task<Font> LoadFontAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("��������ֲ���Ϊ��");
            return null;
        }

        string key = name.ToLower();
        var loadHandle = Addressables.LoadAssetAsync<Font>(key);
        _handles.Add(loadHandle); // ��¼���ؾ��
        handlesDictionary.Add(name, loadHandle);

        try
        {
            // ���ó�ʱʱ��Ϊ 10 ��
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"�ɹ�����������Դ: {name}");
                return loadHandle.Result;
            }
            else
            {
                Debug.LogError($"δ�ܼ��ص�����Ϊ '{name}' �� Font ��������");
                return null;
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"�������� '{name}' ��ʱ��������Դ�Ƿ���ȷ����");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
            return null;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"�������� '{name}' ��ȡ��");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
            return null;
        }
    }

    /// <summary>
    /// �첽���ص��� TMP_FontAsset �������壬���ӳ�ʱ����
    /// </summary>
    private async Task<TMP_FontAsset> LoadTMPFontAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("��������ֲ���Ϊ��");
            return null;
        }

        string key = name.ToLower();
        var loadHandle = Addressables.LoadAssetAsync<TMP_FontAsset>(key);
        _handles.Add(loadHandle); // ��¼���ؾ��
        handlesDictionary.Add(name, loadHandle);

        try
        {
            // ���ó�ʱʱ��Ϊ 10 ��
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"�ɹ����� TMP ������Դ: {name}");
                return loadHandle.Result;
            }
            else
            {
                Debug.LogError($"δ�ܼ��ص�����Ϊ '{name}' �� TMP_FontAsset ��������");
                return null;
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"���� TMP ���� '{name}' ��ʱ��������Դ�Ƿ���ȷ����");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
            return null;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"���� TMP ���� '{name}' ��ȡ��");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
            return null;
        }
    }

    /// <summary>
    /// �첽���س�����Դ
    /// </summary>
    private async Task<SceneInstance> LoadSceneResourceAsync(string name, CancellationToken cancellationToken)
    {
        var loadHandle = Addressables.LoadSceneAsync(name, LoadSceneMode.Single); // ������ʹ�� LoadSceneAsync ���س���
        _handles.Add(loadHandle); // ��¼���ؾ��
        handlesDictionary.Add(name, loadHandle);
        try
        {
            // ���ó�ʱʱ��Ϊ 10 ��
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"�ɹ����س�����Դ: {name}");

                return loadHandle.Result; // ���ؼ��صĳ���ʵ��
            }
            else
            {
                Debug.LogError($"δ�ܼ��ص�������Դ: {name}");
                Addressables.Release(loadHandle); // �ͷż��ؾ��
                return default;
            }
        }
        catch (Exception ex) when (ex is TimeoutException || ex is OperationCanceledException)
        {
            Debug.LogError($"���س�����Դ '{name}' ʧ��: {ex.Message}");
            Addressables.Release(loadHandle); // �ͷż��ؾ��
            return default;
        }
    }
    #endregion

    #region �ͷ���Դ
    /// <summary>
    /// �ͷ����м��ص���Դ
    /// </summary>
    public void ReleaseResources()
    {
        foreach (var handle in _handles)
        {
            Addressables.Release(handle);
        }
        _handles.Clear();
    }

    public void ReleaseLevelResources(int num)
    {
        string levelNum = "Level" + num.ToString();
        if (!handlesDictionary.ContainsKey(levelNum))
        {
            Debug.Log($"�ֵ���û�д洢��Դ��{levelNum}");
            return;
        }

        string boxLevelNum = "BoxLevel" + num.ToString();
        Addressables.Release(handlesDictionary[levelNum]);
        Addressables.Release(handlesDictionary[boxLevelNum]);

        handlesDictionary.Remove(levelNum);
        handlesDictionary.Remove(boxLevelNum);
        Debug.Log($"��Դ��{levelNum}�Ƴ��ɹ���");
    }

    #endregion

    #region ��ȡ��Դ

    /// <summary>
    /// ������صĳ���
    /// </summary>
    public async Task ActivateLoadedScene()
    {
        if (_loadedScene.Scene.IsValid())
        {
            // �ȴ������������
            while (!_loadedScene.Scene.isLoaded)
            {
                await Task.Yield(); // �ȴ�һ֡
            }

            // ���ü��صĳ���Ϊ�����
            SceneManager.SetActiveScene(_loadedScene.Scene);
            Debug.Log($"���� '{_loadedScene.Scene.name}' �Ѽ��");
        }
        else
        {
            Debug.LogError("���صĳ�����Ч���޷����");
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Home")
        {
            Debug.Log($"���� '{scene.name}' �Ѽ�����ɡ�");

            // �����
            SceneManager.SetActiveScene(scene);
            Debug.Log($"���� '{scene.name}' �Ѽ��");
        }
    }

    /// <summary>
    /// ��ȡ UnlockImage ��ָ�����ֵ� Sprite
    /// </summary>
    public Sprite GetUnlockImageSprite(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("��������ֲ���Ϊ��");
            return null;
        }

        string key = name.ToLower();
        if (unlockImageDictionary.TryGetValue(key, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogError($"δ�ҵ�����Ϊ '{name}' �� UnlockImage Sprite������ Addressables/UnlockImage ��Դ���е���Դ");
        return null;
    }

    /// <summary>
    /// ������ɫ�ؼ��ֻ�ȡƥ��� BoomImage Sprite
    /// </summary>
    public Sprite GetBoomSpriteByColor(ScrewColor color)
    {
        string colorKey = GetColorKey(color);
        foreach (var kvp in boomSpriteDictionary)
        {
            if (kvp.Key.Contains(colorKey))
            {
                return kvp.Value;
            }
        }
        Debug.LogError($"δ�ҵ������ؼ��� '{colorKey}' �� Sprite");
        return null;
    }

    /// <summary>
    /// ������ɫ�Ϳ�����ȡ BoxBoard Sprite
    /// </summary>
    public Sprite GetBoxBoardSprite(ScrewColor color, int count)
    {
        if (count < 2 || count > 4)
        {
            Debug.LogError("��Ч�Ŀ�������֧�� 2-4");
            return null;
        }

        string partialKey = $"{color.ToString().ToLower()}{count}";

        foreach (var kvp in boxBoardSpriteDictionary)
        {
            if (kvp.Key.Contains(partialKey))
            {
                return kvp.Value;
            }
        }

        Debug.LogError($"δ�ҵ������ؼ��� '{partialKey}' �� Sprite");
        return null;
    }

    /// <summary>
    /// ��ȡ Fx �ļ�����ָ�����ֵ���Ϸ����
    /// </summary>
    public GameObject GetFxGameObject(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("��������ֲ���Ϊ��");
            return null;
        }

        if (fxGameObjectDictionary.TryGetValue(name, out GameObject gameObject))
        {
            return gameObject;
        }

        foreach (string fxname in fxGameObjectDictionary.Keys)
        {
            Debug.Log(fxname);
            if (fxname == name)
            {
                Debug.Log("����һ��");
            }
        }
        
        Debug.LogError($"δ�ҵ�����Ϊ '{name}' ����Ϸ���壬���� Addressables/Fx ��Դ���е���Դ");
        return null;
    }

    /// <summary>
    /// ��ȡ������
    /// </summary>
    public GameObject GetChain()
    {
        return fxGameObjectDictionary["chain"];
    }

    /// <summary>
    /// ��ȡ Levels ��ָ�����ֵ���Ϸ����
    /// </summary>
    public GameObject GetLevelsGameObject(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("��������ֲ���Ϊ��");
            return null;
        }
        if (levelsDictionary.TryGetValue(name, out GameObject gameObject))
        {
            return gameObject;
        }

        Debug.LogError($"δ�ҵ�����Ϊ '{name}' ����Ϸ���壬���� Addressables/Levels ��Դ���е���Դ");
        return null;
    }

    /// <summary>
    /// ��ȡ BoxLevels ��ָ�����ֵ���Ϸ����
    /// </summary>
    public GameObject GetBoxLevelsGameObject(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("��������ֲ���Ϊ��");
            return null;
        }

        if (boxLevelsDictionary.TryGetValue(name, out GameObject gameObject))
        {
            return gameObject;
        }

        Debug.LogError($"δ�ҵ�����Ϊ '{name}' ����Ϸ���壬���� Addressables/BoxLevels ��Դ���е���Դ");
        return null;
    }
    /// <summary>
    /// ��ȡ��ɫ�ؼ���
    /// </summary>
    private string GetColorKey(ScrewColor color)
    {
        return color.ToString().ToLower();
    }
    #endregion
}

public static class TaskExtensions
{
    /// <summary>
    /// ��չ������Ϊ AsyncOperationHandle ���ӳ�ʱ���
    /// </summary>
    public static async Task WaitForCompletionWithTimeout(this AsyncOperationHandle handle, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        var startTime = DateTime.Now;
        while (!handle.IsDone)
        {
            if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMilliseconds)
            {
                throw new TimeoutException("�첽����ʱ");
            }
            cancellationToken.ThrowIfCancellationRequested(); // ����Ƿ�ȡ��
            await Task.Yield();
        }
    }

    /// <summary>
    /// ��չ������Ϊ AsyncOperationHandle<T> ���ӳ�ʱ���
    /// </summary>
    public static async Task WaitForCompletionWithTimeout<T>(this AsyncOperationHandle<T> handle, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        var startTime = DateTime.Now;
        while (!handle.IsDone)
        {
            if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMilliseconds)
            {
                throw new TimeoutException("�첽����ʱ");
            }
            cancellationToken.ThrowIfCancellationRequested(); // ����Ƿ�ȡ��
            await Task.Yield();
        }
    }
}