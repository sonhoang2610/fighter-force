using EazyEngine.Space;
using EazyEngine.Tools;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpaceManagerWindow : OdinMenuEditorWindow
{
    [MenuItem("Space/GameManager %#d")]
    private static void OpenWindow()
    {
        GetWindow<SpaceManagerWindow>().Show();
    }
    [MenuItem("Space/Select Prefab %#g")]
    private static void Add()
    {
        GameManager.Instance.groupPrefabBullet[0].prefabBullet.Add((GameObject)Selection.activeObject);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;
        tree.Add("Space",null);

        string path = "Assets/SpaceJourney/Resources/Variants/Database/EnemyConfig.asset";
        var database =  AssetDatabase.LoadAssetAtPath<EazyEngine.Space.EazySpaceConfigDatabase>(path);

        string path1 = "Assets/SpaceJourney/Resources/Variants/DataBase/GameDatabase.asset";
        var database1 = AssetDatabase.LoadAssetAtPath<EazyEngine.Space.GameDatabase>(path1);
        string pathMainPlane = "Assets/SpaceJourney/Resources/Variants/DataBase/Plane/TableMainPlane.asset";
        var databaseMain = AssetDatabase.LoadAssetAtPath<EazyEngine.Space.TablePlane>(pathMainPlane);
        string pathMainspPlane = "Assets/SpaceJourney/Resources/Variants/DataBase/SupportPlane/TableSPPlane.asset";
        var databasespMain = AssetDatabase.LoadAssetAtPath<EazyEngine.Space.TablePlane>(pathMainspPlane);
        tree.AddObjectAtPath("Space/Database", database1);
        tree.AddObjectAtPath("Space/MainPlane", databaseMain);
        tree.AddObjectAtPath("Space/SupportPlane", databasespMain);
        tree.AddObjectAtPath("Space/Enemies/Small", database.smallEnemiesObs);
        for (int i = 0; i < databaseMain.info.Length; ++i)
        {
            tree.AddObjectAtPath("Space/MainPlane/Plane" + i, databaseMain.info[i]).AddIcon(databaseMain.info[i].iconGame.texture);
        }
        for (int i = 0; i < databasespMain.info.Length; ++i)
        {
            tree.AddObjectAtPath("Space/SupportPlane/Plane" + i, databasespMain.info[i]).AddIcon(databasespMain.info[i].iconGame.texture);
        }
        for (int i = 0; i < database.smallEnemiesObs.Length; ++i)
        {
            tree.AddObjectAtPath("Space/Enemies/Small/enemy" + i, database.smallEnemiesObs[i]).AddIcon(database.smallEnemiesObs[i].info.preview);
        }
        tree.AddObjectAtPath("Space/Enemies/Medium", database.mediumEnemiesObs);
        for (int i = 0; i < database.mediumEnemiesObs.Length; ++i)
        {
            tree.AddObjectAtPath("Space/Enemies/Medium/medium" + i, database.mediumEnemiesObs[i]).AddIcon(database.mediumEnemiesObs[i].info.preview);
        }
        tree.AddObjectAtPath("Space/Enemies/Boss", database.bosssObs);
        for (int i = 0; i < database.bosssObs.Length; ++i)
        {
            tree.AddObjectAtPath("Space/Enemies/Boss/Boss" + i, database.bosssObs[i]).AddIcon(database.bosssObs[i].info.preview);
        }
        tree.AddObjectAtPath("Space/Enemies/MiniBoss", database.miniBosssObs);
        for (int i = 0; i < database.miniBosssObs.Length; ++i)
        {
            tree.AddObjectAtPath("Space/Enemies/MiniBoss/" + i, database.miniBosssObs[i]).AddIcon(database.miniBosssObs[i].info.preview);
        }
        //   tree.EnumerateTree().AddThumbnailIcons();

        return tree;
    }


}