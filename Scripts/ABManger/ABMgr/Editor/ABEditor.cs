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
        //BuildTarget ѡ��build������AB��Ҫʹ�õ�ƽ̨
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
    }


    [MenuItem("Tools/AssetBundle/Clear Unuse AssetBundles Name")]
    public static void ClearUnuseABName()
    {
        //AssetDatabase.RemoveUnusedAssetBundleNames(); //�Ƴ����õ�ABName

        //ǿ��ɾ�����е�abName
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < abNames.Length; i++) AssetDatabase.RemoveAssetBundleName(abNames[i], true);
    }


    [MenuItem("Tools/AssetBundle/Build AssetBundles Name")]
    public static void Assetbundleseditor()
    {
        string abRootPath = Application.dataPath + "/ABResources";
        DirectoryInfo abDirector = new DirectoryInfo(abRootPath);
        //ֻ������ļ��У��������
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
        //��ȡAssets֮���·��
        //AssetImporter.GetAtPath������unity���̵����·��
        //����ҪAssets��ͷ
        string filePath = fileInfo.FullName.Substring(index);
        //ͨ��AssetImporterָ��Ҫ��ǵ��ļ�
        AssetImporter importer = AssetImporter.GetAtPath(filePath);
        //���ֳ����ļ�����Դ�ļ���׺��
        if (fileInfo.Extension == ".unity")
            importer.assetBundleVariant = "u3d";
        else
            importer.assetBundleVariant = "ab";
        //������
        string bundleName = string.Empty;
        //��Ҫ�õ�����Ŀ¼����һ��Ŀ¼����
        //����=����Ŀ¼��+��һ��Ŀ¼��
        int indexScenes = fileInfo.FullName.IndexOf(assetName) + assetName.Length + 1;
        string bundlePath = fileInfo.FullName.Substring(indexScenes);
        //�滻win·����ķ�б��
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
