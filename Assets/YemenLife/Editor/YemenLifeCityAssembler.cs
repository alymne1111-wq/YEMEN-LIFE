using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public static class YemenLifeCityAssembler
{
    const string KitRoot = "Assets/YemenLife/Art/CityKit/models";

    [MenuItem("YEMEN LIFE/Build City From Imported GLB Kit")]
    public static void Build()
    {
        string[] names = {
            "casa_simples", "casa_sobrado", "casas_geminadas", "loja_esquina",
            "mercado", "restaurante", "hospital", "policia", "prefeitura",
            "escola", "asfalto_reta", "asfalto_cruzamento", "asfalto_curva",
            "asfalto_t", "calcada", "carro", "taxi", "onibus", "poste_luz",
            "semaforo", "arvores_mata", "morro", "grama"
        };

        var guids = AssetDatabase.FindAssets("t:Model", new[] { KitRoot });
        var assets = guids.Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g)))
                          .Where(x => x != null).ToArray();

        GameObject Find(string partial)
        {
            return assets.FirstOrDefault(a => a.name.ToLower().Contains(partial.ToLower()));
        }

        var house = Find("casa_simples") ?? Find("casa_sobrado");
        var shop = Find("loja_esquina") ?? Find("mercado");
        var road = Find("asfalto_reta");
        var cross = Find("asfalto_cruzamento");
        var police = Find("policia");
        var market = Find("mercado");
        var car = Find("carro");
        var taxi = Find("taxi");
        var tree = Find("arvores_mata");
        var lamp = Find("poste_luz");
        var hill = Find("morro");

        if (house == null || road == null)
        {
            EditorUtility.DisplayDialog("YEMEN LIFE",
                "لم أجد مجسمات GLB بعد. فك ضغط حزمة المدينة وانسخ مجلد models إلى:\n" + KitRoot,
                "حسناً");
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "YemenLife_Main";
        var root = new GameObject("YEMEN LIFE - City");

        // Compact playable district assembled from actual imported kit models.
        // Grid is intentionally modular so it can be expanded after performance testing.
        const int blocks = 9;
        const float tile = 2f;
        for (int z = 0; z < blocks; z++)
        for (int x = 0; x < blocks; x++)
        {
            var roadPrefab = (x % 3 == 1 || z % 3 == 1) ? road : null;
            if (roadPrefab != null)
                Place(roadPrefab, new Vector3(x * tile, 0, z * tile), 0, root.transform);

            if (x % 3 != 1 && z % 3 != 1)
            {
                var building = ((x + z) % 4 == 0 && market != null) ? market :
                               ((x + z) % 3 == 0 && shop != null) ? shop : house;
                Place(building, new Vector3(x * tile, 0, z * tile), ((x + z) % 4) * 90, root.transform);
            }
        }

        if (cross != null)
            Place(cross, new Vector3(3 * tile, 0.01f, 3 * tile), 0, root.transform);
        if (police != null)
            Place(police, new Vector3(8 * tile, 0, 2 * tile), 0, root.transform);
        if (car != null) Place(car, new Vector3(2, 0.1f, 3), 90, root.transform);
        if (taxi != null) Place(taxi, new Vector3(5, 0.1f, 3), 90, root.transform);

        for (int i = 0; i < 10; i++)
        {
            if (tree != null) Place(tree, new Vector3((i % 5) * 5 - 2, 0, (i / 5) * 7 - 3), 0, root.transform);
            if (lamp != null) Place(lamp, new Vector3(i * 3, 0, -2), 0, root.transform);
        }
        if (hill != null) Place(hill, new Vector3(12, 0, 18), 0, root.transform);

        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player - TEMP placeholder";
        player.transform.position = new Vector3(2, 1, 2);
        player.transform.SetParent(root.transform);

        var camera = Camera.main;
        if (camera != null)
        {
            camera.transform.position = new Vector3(12, 15, -16);
            camera.transform.LookAt(new Vector3(8, 0, 8));
        }

        EditorSceneManager.SaveScene(scene, "Assets/YemenLife/Scenes/YemenLife_Main.unity");
        EditorBuildSettings.scenes = new[] {
            new EditorBuildSettingsScene("Assets/YemenLife/Scenes/YemenLife_Main.unity", true)
        };
        EditorUtility.DisplayDialog("YEMEN LIFE",
            "تم إنشاء مشهد أولي باستخدام المجسمات المستوردة. هذه ليست لعبة نهائية بعد؛ يلزم إضافة أنظمة اللعب والاختبار وتحسين تخطيط المدينة.",
            "حسناً");
    }

    static void Place(GameObject prefab, Vector3 pos, float yaw, Transform parent)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(0, yaw, 0);
    }
}
