using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MergeMeshesTool : EditorWindow
{
    [MenuItem("GameObject/Fondi Foresta Selezionata", false, 0)]
    static void Combine()
    {
        // Prende l'oggetto contenitore che hai selezionato
        GameObject parent = Selection.activeGameObject;
        if (parent == null) return;

        // Trova tutte le mesh e i renderer nei figli
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] meshRenderers = parent.GetComponentsInChildren<MeshRenderer>();

        if (meshFilters.Length == 0) return;

        // Raccoglie tutti i materiali unici usati
        List<Material> uniqueMaterials = new List<Material>();
        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (!uniqueMaterials.Contains(mat))
                    uniqueMaterials.Add(mat);
            }
        }

        // Prepara le combine instance raggruppate per materiale
        List<CombineInstance> finalCombines = new List<CombineInstance>();

        foreach (Material mat in uniqueMaterials)
        {
            List<CombineInstance> subCombines = new List<CombineInstance>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                if (meshRenderers[i].sharedMaterials.Length <= 0) continue;

                // Cerca a quale submesh corrisponde il materiale
                for (int sub = 0; sub < meshRenderers[i].sharedMaterials.Length; sub++)
                {
                    if (meshRenderers[i].sharedMaterials[sub] == mat)
                    {
                        CombineInstance ci = new CombineInstance();
                        ci.mesh = meshFilters[i].sharedMesh;
                        ci.subMeshIndex = sub;
                        ci.transform = parent.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
                        subCombines.Add(ci);
                    }
                }
            }

            // Fonde tutte le parti che usano questo materiale
            Mesh tempMesh = new Mesh();
            tempMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Fondamentale per mesh enormi
            tempMesh.CombineMeshes(subCombines.ToArray(), true, true);

            CombineInstance finalCi = new CombineInstance();
            finalCi.mesh = tempMesh;
            finalCi.subMeshIndex = 0;
            finalCi.transform = Matrix4x4.identity;
            finalCombines.Add(finalCi);
        }

        // Crea la Mesh finale con i vari "SubMeshes" per i materiali
        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        finalMesh.CombineMeshes(finalCombines.ToArray(), false, false);

        // Salva la mesh nella cartella Assets in modo che il tuo amico possa vederla
        AssetDatabase.CreateAsset(finalMesh, "Assets/ForestaUnita_" + parent.name + ".asset");
        AssetDatabase.SaveAssets();

        // Crea il nuovo GameObject pulito
        GameObject combinedObject = new GameObject(parent.name + "_Fuso");
        combinedObject.AddComponent<MeshFilter>().sharedMesh = finalMesh;
        combinedObject.AddComponent<MeshRenderer>().sharedMaterials = uniqueMaterials.ToArray();

        // Disattiva il vecchio gruppo pieno di alberi
        parent.SetActive(false);
        Selection.activeGameObject = combinedObject;
    }
}