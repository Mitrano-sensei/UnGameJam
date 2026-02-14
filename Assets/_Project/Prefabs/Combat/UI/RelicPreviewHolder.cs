using System;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class RelicPreviewHolder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform parentTransform;
    
    [FormerlySerializedAs("previewSclaeFactor")]
    [Header("Settings")]
    [SerializeField] private float previewScaleFactor = 0.5f;

    private readonly List<APreview> _relicPreviewRef = new(); 
    
    private void OnValidate()
    {
        _relicPreviewRef.ForEach(p => p.transform.localScale = Vector3.one * previewScaleFactor);
    }

    [Button]
    public void GeneratePreviews()
    {
        var relicSystem = Registry<RelicSystem>.GetFirst();

        if (relicSystem == null)
        {
            Debug.LogError("No relic system in scene");
            return;
        }
        List<RelicData> relics = relicSystem.ActiveRelics;

        foreach (var relic in relics)
        {
            var preview = (RelicPreview)relic.GeneratePreview(false, false);
            preview.SetScale(previewScaleFactor, true);
            preview.transform.SetParent(parentTransform);
            _relicPreviewRef.Add(preview);
        }
    }
}
