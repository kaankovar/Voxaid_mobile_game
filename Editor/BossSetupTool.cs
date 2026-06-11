using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;

public class BossSetupWindow : EditorWindow
{
    public GameObject baseBossPrefab; 
    public Material customOverrideMaterial; 

    [MenuItem("Tools/Boss Fabrikası (Şablonlu)")]
    public static void ShowWindow()
    {
        GetWindow<BossSetupWindow>("Boss Fabrikası");
    }

    void OnGUI()
    {
        GUILayout.Label("1. Ana Boss Şablonunu (Prefab) Buraya Sürükle:", EditorStyles.boldLabel);
        baseBossPrefab = (GameObject)EditorGUILayout.ObjectField("Şablon Prefab", baseBossPrefab, typeof(GameObject), false);

        GUILayout.Space(10);
        GUILayout.Label("2. (Opsiyonel) Dışarıdan Hazırladığın Özel Materyal:", EditorStyles.boldLabel);
        customOverrideMaterial = (Material)EditorGUILayout.ObjectField("Özel Materyal", customOverrideMaterial, typeof(Material), false);

        GUILayout.Space(20);
        GUILayout.Label("3. Project Penceresinden Yürüme ve Slam FBX'lerini Seç.", EditorStyles.boldLabel);
        
        GUILayout.Space(20);
        if (GUILayout.Button("Seçili FBX'leri Şablonla Birleştir ve Yeni Boss Üret", GUILayout.Height(40)))
        {
            if (baseBossPrefab == null)
            {
                EditorUtility.DisplayDialog("Hata", "Lütfen önce Ana Boss Şablonunu (Prefab) kutuya sürükleyin!", "Tamam");
                return;
            }
            ProcessMultipleFBXForOneBoss();
        }
    }

    private void ProcessMultipleFBXForOneBoss()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Lütfen Boss'a ait model ve animasyon FBX dosyalarını seçin.");
            return;
        }

        string firstAssetPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
        string folderPath = Path.GetDirectoryName(firstAssetPath);
        string baseName = new DirectoryInfo(folderPath).Name; 
        
        
        if (!AssetDatabase.IsValidFolder("Assets/Animations")) AssetDatabase.CreateFolder("Assets", "Animations");
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");

        
        string controllerPath = "Assets/Animations/" + baseName + "_Controller.controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        controller.AddParameter("shoot", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        AnimatorState walkState = rootStateMachine.AddState("Walk");
        AnimatorState slamState = rootStateMachine.AddState("Charged_Ground_Slam");
        rootStateMachine.defaultState = walkState;

        AnimationClip walkClip = null;
        AnimationClip slamClip = null;
        GameObject mainModelFBX = null;
        GameObject walkModelFallback = null; 
        Dictionary<string, Material> extractedMaterials = new Dictionary<string, Material>();

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (!assetPath.ToLower().EndsWith(".fbx")) continue;
            string lowerFBXName = obj.name.ToLower();

            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (Object subAsset in allAssets)
            {
                if (subAsset is AnimationClip && !subAsset.name.StartsWith("__preview__"))
                {
                    AnimationClip originalClip = subAsset as AnimationClip;
                    AnimationClip newClip = new AnimationClip();
                    EditorUtility.CopySerialized(originalClip, newClip);

                    string clipLowerName = originalClip.name.ToLower();

                    if (clipLowerName.Contains("walk") || clipLowerName.Contains("yuru") || lowerFBXName.Contains("walk"))
                    {
                        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(newClip);
                        settings.loopTime = true;
                        AnimationUtility.SetAnimationClipSettings(newClip, settings);
                        walkClip = newClip;
                        newClip.name = "Walk";
                        walkModelFallback = obj as GameObject;
                    }
                    else if (clipLowerName.Contains("slam") || clipLowerName.Contains("attack") || lowerFBXName.Contains("slam"))
                    {
                        slamClip = newClip;
                        newClip.name = "Charged_Ground_Slam";
                    }
                    else newClip.name = originalClip.name;

                    
                    string clipPath = "Assets/Animations/" + baseName + "_" + newClip.name + ".anim";
                    AssetDatabase.CreateAsset(newClip, clipPath);
                }
                
                if (subAsset is Material && customOverrideMaterial == null)
                {
                    Material originalMat = subAsset as Material;
                    if (!originalMat.name.StartsWith("Default-") && !extractedMaterials.ContainsKey(originalMat.name))
                    {
                        Material newMat = new Material(originalMat);
                        
                        string matPath = "Assets/Materials/" + baseName + "_" + originalMat.name + ".mat";
                        AssetDatabase.CreateAsset(newMat, matPath);
                        extractedMaterials.Add(originalMat.name, newMat);
                    }
                }
            }

            if (!lowerFBXName.Contains("walk") && !lowerFBXName.Contains("slam") && !lowerFBXName.Contains("yuru") && !lowerFBXName.Contains("attack"))
            {
                mainModelFBX = obj as GameObject;
            }
        }

        if (walkClip != null) walkState.motion = walkClip;
        if (slamClip != null) slamState.motion = slamClip;

        AnimatorStateTransition walkToSlam = walkState.AddTransition(slamState);
        walkToSlam.AddCondition(AnimatorConditionMode.If, 0, "shoot");
        walkToSlam.hasExitTime = false;
        walkToSlam.hasFixedDuration = true;
        walkToSlam.duration = 0.1f;
        walkToSlam.exitTime = 0f;

        AnimatorStateTransition slamToWalk = slamState.AddTransition(walkState);
        slamToWalk.hasExitTime = true;
        slamToWalk.hasFixedDuration = true;
        slamToWalk.duration = 0.25f;

        if (mainModelFBX == null)
        {
            if (walkModelFallback != null) mainModelFBX = walkModelFallback;
            else mainModelFBX = selectedObjects[0] as GameObject;
        }

        if (mainModelFBX != null)
        {
            GameObject bossInstance = (GameObject)PrefabUtility.InstantiatePrefab(baseBossPrefab);
            
            Vector3 oldPosition = Vector3.zero;
            Quaternion oldRotation = Quaternion.identity;
            Vector3 oldScale = Vector3.one;
            GameObject objectToDestroy = null;

            Transform oldModelTransform = bossInstance.transform.Find("Meshy_AI_Animation_Walking_withSkin");
            
            if (oldModelTransform != null)
            {
                oldPosition = oldModelTransform.localPosition;
                oldRotation = oldModelTransform.localRotation;
                oldScale = oldModelTransform.localScale;
                objectToDestroy = oldModelTransform.gameObject;
            }
            else
            {
                Animator[] oldAnimators = bossInstance.GetComponentsInChildren<Animator>(true);
                foreach (Animator oldAnim in oldAnimators)
                {
                    if (oldAnim.gameObject != bossInstance) 
                    {
                        oldPosition = oldAnim.transform.localPosition;
                        oldRotation = oldAnim.transform.localRotation;
                        oldScale = oldAnim.transform.localScale;
                        objectToDestroy = oldAnim.gameObject;
                        break; 
                    }
                }
            }

            GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(mainModelFBX);
            
            modelInstance.transform.SetParent(bossInstance.transform);
            modelInstance.transform.localPosition = oldPosition;
            modelInstance.transform.localRotation = oldRotation;
            modelInstance.transform.localScale = oldScale;

            if (objectToDestroy != null) DestroyImmediate(objectToDestroy);

            Animator anim = modelInstance.GetComponent<Animator>();
            if (anim == null) anim = modelInstance.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller;

            Renderer[] childRenderers = modelInstance.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer rend in childRenderers)
            {
                Material[] sharedMats = rend.sharedMaterials;
                for (int i = 0; i < sharedMats.Length; i++)
                {
                    if (customOverrideMaterial != null)
                    {
                        sharedMats[i] = customOverrideMaterial;
                    }
                    else if (sharedMats[i] != null && extractedMaterials.ContainsKey(sharedMats[i].name))
                    {
                        sharedMats[i] = extractedMaterials[sharedMats[i].name];
                    }
                }
                rend.sharedMaterials = sharedMats;
            }

            
            string prefabPath = "Assets/Prefabs/" + baseName + "_GameReadyBoss.prefab";
            PrefabUtility.SaveAsPrefabAsset(bossInstance, prefabPath);
            
            DestroyImmediate(bossInstance);

            Debug.Log($"<color=green>[MÜKEMMEL!]</color> Dosyalar organize edildi: Prefab -> Prefabs, Animator/Animasyonlar -> Animations, Materyaller -> Materials klasörüne kaydedildi!");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}