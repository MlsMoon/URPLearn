using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private const string BufferName = "Render Camera";
    private CommandBuffer _buffer = new CommandBuffer
    {
        name = BufferName
    };
    
    private ScriptableRenderContext _context;   
    private Camera _camera;
    private CullingResults _cullingResults;
    private static readonly ShaderTagId UnlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    

    /// <summary>
    /// 渲染入口
    /// </summary>
    /// <param name="context"></param>
    /// <param name="camera"></param>
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        //initial
        this._context = context;
        this._camera = camera;

        PrepareBuffer();
        PrepareForSceneWin();
        if (!Cull())
            return;
        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShader();
        DrawGizmos();
        Submit();
    }

    
    
    #region Render
    private void DrawVisibleGeometry()
    {
        var sortingSettings = new SortingSettings(_camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(
            UnlitShaderTagId,sortingSettings
        );
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        //绘制不透明物体
        _context.DrawRenderers(
            _cullingResults ,
            ref drawingSettings ,
            ref filteringSettings
            );
        //绘制天空盒子
        _context.DrawSkybox(_camera);
        //绘制透明物体
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        _context.DrawRenderers(
            _cullingResults,
            ref drawingSettings,
            ref filteringSettings
        );
    }
    private void Setup()
    {
        _context.SetupCameraProperties(_camera);
        var flags = _camera.clearFlags;
        _buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth,
            flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ?
                _camera.backgroundColor.linear : Color.clear
        );
        _buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }
    private void Submit()
    {
        _buffer.EndSample(SampleName);
        ExecuteBuffer();
        _context.Submit();
    }
    private void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
    private bool Cull()
    {
        if (_camera.TryGetCullingParameters(out var p))
        {
            _cullingResults = _context.Cull(ref p);
            return true;
        }
        return false;
    }
    #endregion


}
