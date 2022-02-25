using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

partial class CameraRenderer
{
    partial void DrawUnsupportedShader();
    partial void DrawGizmos();
    partial void PrepareForSceneWin();
    
    static ShaderTagId[] _legacyShaderID =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    static Material _errorMat;
    
#if UNITY_EDITOR
    partial void DrawGizmos()
    {
        if (UnityEditor.Handles.ShouldRenderGizmos())
        {
            _context.DrawGizmos(_camera , GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera , GizmoSubset.PostImageEffects);
        }
    }
    partial void DrawUnsupportedShader()
    {
        if (_errorMat == null)
        {
            _errorMat = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        var d = new DrawingSettings(
            _legacyShaderID[0],new SortingSettings(_camera)
        )
        {
            overrideMaterial = _errorMat
        };
        for (int i = 1; i < _legacyShaderID.Length; i++)
        {
            d.SetShaderPassName(i,_legacyShaderID[i]);
        }
        var f = new FilteringSettings();
        _context.DrawRenderers(_cullingResults, ref d, ref f);        
    }
    partial void PrepareForSceneWin()
    {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }
#endif
}
