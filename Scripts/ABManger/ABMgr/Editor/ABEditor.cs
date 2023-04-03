using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABEditor
{
    [MenuItem("Tools/AssetBundle/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string dir = Application.streamingAssetsPath + "/ABRes";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        //BuildTarget 选择build出来的AB包要使用的平台
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
    }


    [MenuItem("Tools/AssetBundle/Clear Unuse AssetBundles Name")]
    public static void ClearUnuseABName()
    {
        //AssetDatabase.RemoveUnusedAssetBundleNames(); //移除不用的ABName

        //强制删除所有的abName
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < abNames.Length; i++) AssetDatabase.RemoveAssetBundleName(abNames[i], true);
    }


    [MenuItem("Tools/AssetBundle/Build AssetBundles Name")]
    public static void Assetbundleseditor()
    {
        string abRootPath = Application.dataPath + "/ABResources";
        DirectoryInfo abDirector = new DirectoryInfo(abRootPath);
        //只获得子文件夹，不获得孙
        DirectoryInfo[] subDirs = abDirector.GetDirectories();

        foreach (DirectoryInfo subDir in subDirs)
        {
            FindFile(subDir, subDir.Name);
        }
    }

    private static void FindFile(DirectoryInfo subDir, string name)
    {
        FileInfo[] fileInfos = subDir.GetFiles();
        foreach (var item in fileInfos)
        {
            SetFileABName(item, name);
        }

        DirectoryInfo[] directories = subDir.GetDirectories();
        foreach (var item in directories)
        {
            FindFile(item, item.Name);
        }
    }

    private static void SetFileABName(FileInfo fileInfo, string assetName)
    {
        if (fileInfo.Extension == ".meta") return;

        int index = fileInfo.FullName.IndexOf("Assets");
        //截取Assets之后的路径
        //AssetImporter.GetAtPath必须是unity工程的相对路径
        //所以要Assets开头
        string filePath = fileInfo.FullName.Substring(index);
        //通过AssetImporter指定要标记的文件
        AssetImporter importer = AssetImporter.GetAtPath(filePath);
        //区分场景文件和资源文件后缀名
        if (fileInfo.Extension == ".unity")
            importer.assetBundleVariant = "u3d";
        else
            importer.assetBundleVariant = "ab";
        //包名称
        string bundleName = string.Empty;
        //需要拿到场景目录下面一级目录名称
        //包名=场景目录名+下一级目录名
        int indexScenes = fileInfo.FullName.IndexOf(assetName) + assetName.Length + 1;
        string bundlePath = fileInfo.FullName.Substring(indexScenes);
        //替换win路径里的反斜杠
        bundlePath = bundlePath.Replace(@"\", "/");
        Debug.Log(bundlePath);
        if (bundlePath.Contains("/"))
        {
            string[] strArr = bundlePath.Split('/');
            bundleName = assetName + "/" + strArr[0];
        }
        else
        {
            bundleName = assetName + "/" + assetName;
        }
        importer.assetBundleName = bundleName;
    }
}
