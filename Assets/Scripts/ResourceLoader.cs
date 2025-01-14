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

    private Dictionary<string, AsyncOperationHandle> handlesDictionary;//存储句柄，方便卸载资源

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

    #region 加载资源
    /// <summary>
    /// 异步初始化方法，加载所有资源
    /// </summary>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _norFont = await LoadFontAsync("思源宋体", cancellationToken);
        _tmpFont = await LoadTMPFontAsync("思源宋体tmp", cancellationToken);

        //await LoadGameObjectsToDictionaryAsync("Level", levelsDictionary, cancellationToken);
        //await LoadGameObjectsToDictionaryAsync("BoxLevel", boxLevelsDictionary, cancellationToken);
        await LoadSpritesToDictionaryAsync("UnlockImage", unlockImageDictionary, cancellationToken);
        await LoadSpritesToDictionaryAsync("BoxBoardImage", boxBoardSpriteDictionary, cancellationToken);
        await LoadSpritesToDictionaryAsync("BoomImage", boomSpriteDictionary, cancellationToken);
        await LoadGameObjectsToDictionaryAsync("Fx", fxGameObjectDictionary, cancellationToken);
    }

    /// <summary>
    /// 异步初始化，带进度条
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="onProgress"></param>
    /// <returns></returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default, Action<float> onProgress = null)
    {
        // 计算总资源数量
        int totalResources = 102; // 字体资源
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

        // 加载字体资源
        _norFont = await LoadFontAsync("思源宋体", cancellationToken);
        loadedResources++;
        onProgress?.Invoke((float)loadedResources / totalResources);

        _tmpFont = await LoadTMPFontAsync("思源宋体tmp", cancellationToken);
        loadedResources++;
        onProgress?.Invoke((float)loadedResources / totalResources);
        Debug.Log("获取资源数量完毕");

        // 加载其他资源
        //根据当前关卡数，加载资源

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

        //加载场景资源
        _loadedScene = await LoadSceneResourceAsync("Home", cancellationToken);

        loadedResources++;
        onProgress?.Invoke((float)loadedResources / totalResources);
    }

    /// <summary>
    /// 获取资源组中的资源数量
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
    /// 异步加载指定资源组的所有精灵到字典中，增加超时机制
    /// </summary>
    private async Task LoadSpritesToDictionaryAsync(string groupName, Dictionary<string, Sprite> targetDictionary, 
        CancellationToken cancellationToken, Action onResourceLoaded = null)
    {
        var loadHandle = Addressables.LoadAssetsAsync<Sprite>(groupName, null);
        _handles.Add(loadHandle); // 记录加载句柄
        handlesDictionary.Add(groupName, loadHandle);

        try
        {
            // 设置超时时间为 10 秒
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                int resourceCount = loadHandle.Result.Count; // 获取加载的资源数量
                Debug.Log($"成功加载 {resourceCount} 个 Sprite 资源，资源组: {groupName}");

                foreach (var sprite in loadHandle.Result)
                {
                    string key = sprite.name.ToLower();
                    if (!targetDictionary.ContainsKey(key))
                    {
                        targetDictionary[key] = sprite;
                    }

                    // 更新加载进度
                    onResourceLoaded?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"未能加载到任何 Sprite，请检查 Addressables/{groupName} 资源组中的资源");
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"加载 {groupName} 资源组超时，请检查资源是否正确配置");
            Addressables.Release(loadHandle); // 释放加载句柄
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"加载 {groupName} 资源组被取消");
            Addressables.Release(loadHandle); // 释放加载句柄
        }
    }

    /// <summary>
    /// 异步加载指定资源组的所有游戏物体到字典中，增加超时机制
    /// </summary>
    private async Task LoadGameObjectsToDictionaryAsync(string groupName, Dictionary<string, GameObject> targetDictionary, 
        CancellationToken cancellationToken, Action onResourceLoaded = null)
    {
        var loadHandle = Addressables.LoadAssetsAsync<GameObject>(groupName, null);
        _handles.Add(loadHandle); // 记录加载句柄
        handlesDictionary.Add(groupName, loadHandle);

        try
        {
            // 设置超时时间为 10 秒
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                int resourceCount = loadHandle.Result.Count; // 获取加载的资源数量
                Debug.Log($"成功加载 {resourceCount} 个 GameObject 资源，资源组: {groupName}");

                foreach (var gameObject in loadHandle.Result)
                {
                    string key = gameObject.name;
                    if (!targetDictionary.ContainsKey(key))
                    {
                        targetDictionary[key] = gameObject;
                    }

                    // 更新加载进度
                    onResourceLoaded?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"未能加载到任何 GameObject，请检查 Addressables/{groupName} 资源组中的资源");
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"加载 {groupName} 资源组超时，请检查资源是否正确配置");
            Addressables.Release(loadHandle); // 释放加载句柄
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"加载 {groupName} 资源组被取消");
            Addressables.Release(loadHandle); // 释放加载句柄
        }
    }

    /// <summary>
    /// 根据关卡数加载关卡
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public async Task LoadLevelGroup(int num)
    {
        string groupName = "Level" + num;

        // 检查资源组是否已经加载
        if (handlesDictionary.ContainsKey(groupName))
        {
            Debug.Log($"资源组 {groupName} 已经加载过，跳过加载。");
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

                Debug.Log($"资源组 {groupName} 加载成功!");
            }
            else
            {
                Debug.LogError($"资源组 {groupName} 加载失败!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"资源组 {groupName} 加载失败: {e.Message}");
        }
    }

    public async Task LoadBoxLevelGroup(int num)
    {
        string groupName = "BoxLevel" + num;

        // 检查资源组是否已经加载
        if (handlesDictionary.ContainsKey(groupName))
        {
            Debug.Log($"资源组 {groupName} 已经加载过，跳过加载。");
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

                Debug.Log($"资源组 {groupName} 加载成功!");
            }
            else
            {
                Debug.LogError($"资源组 {groupName} 加载失败!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"资源组 {groupName} 加载失败: {e.Message}");
        }
    }

    public async Task LoadLevelResources(int num)
    {
        await LoadLevelGroup(num);
        await LoadBoxLevelGroup(num);
    }
    /// <summary>
    /// 异步加载单个 Font 类型字体，增加超时机制
    /// </summary>
    private async Task<Font> LoadFontAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("传入的名字不能为空");
            return null;
        }

        string key = name.ToLower();
        var loadHandle = Addressables.LoadAssetAsync<Font>(key);
        _handles.Add(loadHandle); // 记录加载句柄
        handlesDictionary.Add(name, loadHandle);

        try
        {
            // 设置超时时间为 10 秒
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"成功加载字体资源: {name}");
                return loadHandle.Result;
            }
            else
            {
                Debug.LogError($"未能加载到名字为 '{name}' 的 Font 类型字体");
                return null;
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"加载字体 '{name}' 超时，请检查资源是否正确配置");
            Addressables.Release(loadHandle); // 释放加载句柄
            return null;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"加载字体 '{name}' 被取消");
            Addressables.Release(loadHandle); // 释放加载句柄
            return null;
        }
    }

    /// <summary>
    /// 异步加载单个 TMP_FontAsset 类型字体，增加超时机制
    /// </summary>
    private async Task<TMP_FontAsset> LoadTMPFontAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("传入的名字不能为空");
            return null;
        }

        string key = name.ToLower();
        var loadHandle = Addressables.LoadAssetAsync<TMP_FontAsset>(key);
        _handles.Add(loadHandle); // 记录加载句柄
        handlesDictionary.Add(name, loadHandle);

        try
        {
            // 设置超时时间为 10 秒
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"成功加载 TMP 字体资源: {name}");
                return loadHandle.Result;
            }
            else
            {
                Debug.LogError($"未能加载到名字为 '{name}' 的 TMP_FontAsset 类型字体");
                return null;
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError($"加载 TMP 字体 '{name}' 超时，请检查资源是否正确配置");
            Addressables.Release(loadHandle); // 释放加载句柄
            return null;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"加载 TMP 字体 '{name}' 被取消");
            Addressables.Release(loadHandle); // 释放加载句柄
            return null;
        }
    }

    /// <summary>
    /// 异步加载场景资源
    /// </summary>
    private async Task<SceneInstance> LoadSceneResourceAsync(string name, CancellationToken cancellationToken)
    {
        var loadHandle = Addressables.LoadSceneAsync(name, LoadSceneMode.Single); // 修正：使用 LoadSceneAsync 加载场景
        _handles.Add(loadHandle); // 记录加载句柄
        handlesDictionary.Add(name, loadHandle);
        try
        {
            // 设置超时时间为 10 秒
            await loadHandle.WaitForCompletionWithTimeout(10000, cancellationToken);

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"成功加载场景资源: {name}");

                return loadHandle.Result; // 返回加载的场景实例
            }
            else
            {
                Debug.LogError($"未能加载到场景资源: {name}");
                Addressables.Release(loadHandle); // 释放加载句柄
                return default;
            }
        }
        catch (Exception ex) when (ex is TimeoutException || ex is OperationCanceledException)
        {
            Debug.LogError($"加载场景资源 '{name}' 失败: {ex.Message}");
            Addressables.Release(loadHandle); // 释放加载句柄
            return default;
        }
    }
    #endregion

    #region 释放资源
    /// <summary>
    /// 释放所有加载的资源
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
            Debug.Log($"字典中没有存储资源组{levelNum}");
            return;
        }

        string boxLevelNum = "BoxLevel" + num.ToString();
        Addressables.Release(handlesDictionary[levelNum]);
        Addressables.Release(handlesDictionary[boxLevelNum]);

        handlesDictionary.Remove(levelNum);
        handlesDictionary.Remove(boxLevelNum);
        Debug.Log($"资源组{levelNum}移除成功！");
    }

    #endregion

    #region 获取资源

    /// <summary>
    /// 激活加载的场景
    /// </summary>
    public async Task ActivateLoadedScene()
    {
        if (_loadedScene.Scene.IsValid())
        {
            // 等待场景加载完成
            while (!_loadedScene.Scene.isLoaded)
            {
                await Task.Yield(); // 等待一帧
            }

            // 设置加载的场景为活动场景
            SceneManager.SetActiveScene(_loadedScene.Scene);
            Debug.Log($"场景 '{_loadedScene.Scene.name}' 已激活！");
        }
        else
        {
            Debug.LogError("加载的场景无效，无法激活。");
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Home")
        {
            Debug.Log($"场景 '{scene.name}' 已加载完成。");

            // 激活场景
            SceneManager.SetActiveScene(scene);
            Debug.Log($"场景 '{scene.name}' 已激活！");
        }
    }

    /// <summary>
    /// 获取 UnlockImage 中指定名字的 Sprite
    /// </summary>
    public Sprite GetUnlockImageSprite(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("传入的名字不能为空");
            return null;
        }

        string key = name.ToLower();
        if (unlockImageDictionary.TryGetValue(key, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogError($"未找到名字为 '{name}' 的 UnlockImage Sprite，请检查 Addressables/UnlockImage 资源组中的资源");
        return null;
    }

    /// <summary>
    /// 根据颜色关键字获取匹配的 BoomImage Sprite
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
        Debug.LogError($"未找到包含关键字 '{colorKey}' 的 Sprite");
        return null;
    }

    /// <summary>
    /// 根据颜色和孔数获取 BoxBoard Sprite
    /// </summary>
    public Sprite GetBoxBoardSprite(ScrewColor color, int count)
    {
        if (count < 2 || count > 4)
        {
            Debug.LogError("无效的孔数，仅支持 2-4");
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

        Debug.LogError($"未找到包含关键字 '{partialKey}' 的 Sprite");
        return null;
    }

    /// <summary>
    /// 获取 Fx 文件夹中指定名字的游戏物体
    /// </summary>
    public GameObject GetFxGameObject(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("传入的名字不能为空");
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
                Debug.Log("名字一致");
            }
        }
        
        Debug.LogError($"未找到名字为 '{name}' 的游戏物体，请检查 Addressables/Fx 资源组中的资源");
        return null;
    }

    /// <summary>
    /// 获取链对象
    /// </summary>
    public GameObject GetChain()
    {
        return fxGameObjectDictionary["chain"];
    }

    /// <summary>
    /// 获取 Levels 中指定名字的游戏物体
    /// </summary>
    public GameObject GetLevelsGameObject(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("传入的名字不能为空");
            return null;
        }
        if (levelsDictionary.TryGetValue(name, out GameObject gameObject))
        {
            return gameObject;
        }

        Debug.LogError($"未找到名字为 '{name}' 的游戏物体，请检查 Addressables/Levels 资源组中的资源");
        return null;
    }

    /// <summary>
    /// 获取 BoxLevels 中指定名字的游戏物体
    /// </summary>
    public GameObject GetBoxLevelsGameObject(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("传入的名字不能为空");
            return null;
        }

        if (boxLevelsDictionary.TryGetValue(name, out GameObject gameObject))
        {
            return gameObject;
        }

        Debug.LogError($"未找到名字为 '{name}' 的游戏物体，请检查 Addressables/BoxLevels 资源组中的资源");
        return null;
    }
    /// <summary>
    /// 获取颜色关键字
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
    /// 扩展方法：为 AsyncOperationHandle 增加超时检测
    /// </summary>
    public static async Task WaitForCompletionWithTimeout(this AsyncOperationHandle handle, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        var startTime = DateTime.Now;
        while (!handle.IsDone)
        {
            if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMilliseconds)
            {
                throw new TimeoutException("异步任务超时");
            }
            cancellationToken.ThrowIfCancellationRequested(); // 检查是否取消
            await Task.Yield();
        }
    }

    /// <summary>
    /// 扩展方法：为 AsyncOperationHandle<T> 增加超时检测
    /// </summary>
    public static async Task WaitForCompletionWithTimeout<T>(this AsyncOperationHandle<T> handle, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        var startTime = DateTime.Now;
        while (!handle.IsDone)
        {
            if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMilliseconds)
            {
                throw new TimeoutException("异步任务超时");
            }
            cancellationToken.ThrowIfCancellationRequested(); // 检查是否取消
            await Task.Yield();
        }
    }
}