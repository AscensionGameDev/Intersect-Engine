using System.Numerics;
using System.Runtime.InteropServices;

using ImGuiNET;

using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Input;
using Intersect.Client.Framework.Windows;
using Intersect.Numerics;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface;

/// <summary>
/// ImGui renderer for use with XNA-likes (FNA & MonoGame)
/// </summary>
public abstract class ImGuiRenderer
{
    private static readonly VertexDeclaration _vertexDeclaration;

    static unsafe ImGuiRenderer()
    {
        _vertexDeclaration = new(
            sizeof(ImDrawVert),
            new VertexElement[]
            {
                new(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new(16, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            }
        );
    }

    private readonly GraphicsDevice _graphicsDevice;
    private readonly InputManager _inputManager;
    private readonly SystemWindow _window;

    private Effect _effect;
    private readonly Rasterizer _rasterizerState;

    private byte[] _vertexData;
    private VertexBuffer _vertexBuffer;
    private int _vertexBufferSize;

    private byte[] _indexData;
    private IndexBuffer _indexBuffer;
    private int _indexBufferSize;

    // Textures
    private readonly Dictionary<IntPtr, Texture> _loadedTextures;

    private int _textureId;
    private IntPtr? _fontTextureId;

    private ImDrawDataPtr _drawDataPtr;

    // Input
    private int _scrollWheelValue;

    private readonly List<int> _keys;

    protected ImGuiRenderer(GraphicsDevice graphicsDevice, SystemWindow window, InputManager inputManager)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
        _window = window ?? throw new ArgumentNullException(nameof(window));

        var context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        _keys = new();
        _loadedTextures = new();

        _rasterizerState = _graphicsDevice.CreateRasterizer(
            cullMode: CullMode.None,
            depthBias: 0,
            fillMode: FillMode.Solid,
            multiSampleAntiAlias: false,
            scissorTestEnable: true,
            slopeScaleDepthBias: 0
        );

        SetupInput();
    }

    #region ImGuiRenderer

    /// <summary>
    /// Creates a texture and loads the font data from ImGui. Should be called when the <see cref="GraphicsDevice" /> is initialized but before any rendering is done
    /// </summary>
    public virtual unsafe void RebuildFontAtlas()
    {
        // Get font texture from ImGui
        var io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out var width, out var height, out var bytesPerPixel);

        // Copy the data to a managed array
        var pixels = new byte[width * height * bytesPerPixel];
        unsafe { Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length); }

        // Create and register the texture as an XNA texture
        var tex2d = _graphicsDevice.CreateTexture(width, height, false);
        tex2d.SetData(pixels);

        // Should a texture already have been build previously, unbind it first so it can be deallocated
        if (_fontTextureId.HasValue) UnbindTexture(_fontTextureId.Value);

        // Bind the new texture to an ImGui-friendly id
        _fontTextureId = BindTexture(tex2d);

        // Let ImGui know where to find the texture
        io.Fonts.SetTexID(_fontTextureId.Value);
        io.Fonts.ClearTexData(); // Clears CPU side texture data
    }

    /// <summary>
    /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="ImGui.Image" />. That pointer is then used by ImGui to let us know what texture to draw
    /// </summary>
    public virtual IntPtr BindTexture(Texture texture)
    {
        var id = new IntPtr(_textureId++);

        _loadedTextures.Add(id, texture);

        return id;
    }

    /// <summary>
    /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
    /// </summary>
    public virtual void UnbindTexture(IntPtr textureId)
    {
        _loadedTextures.Remove(textureId);
    }

    /// <summary>
    /// Sets up ImGui for a new frame, should be called at frame start
    /// </summary>
    public virtual void BeforeLayout(FrameTime frameTime)
    {
        ImGui.GetIO().DeltaTime = (float)frameTime.Delta.TotalSeconds;

        UpdateInput(frameTime);

        ImGui.NewFrame();
    }

    /// <summary>
    /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls
    /// </summary>
    public virtual void AfterLayout()
    {
        ImGui.Render();

        _drawDataPtr = ImGui.GetDrawData();
    }

    public unsafe void Draw()
    {
        if (_drawDataPtr.NativePtr != default)
        {
            RenderDrawData(_drawDataPtr);
        }
    }

    #endregion ImGuiRenderer

    #region Setup & Update

    /// <summary>
    /// Maps ImGui keys to XNA keys. We use this later on to tell ImGui what keys were pressed
    /// </summary>
    protected virtual void SetupInput()
    {
        var io = ImGui.GetIO();

        _keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab);
        _keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left);
        _keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right);
        _keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up);
        _keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down);
        _keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp);
        _keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home);
        _keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Key.End);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.Back);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Space] = (int)Key.Space);
        _keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Key.A);
        _keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Key.C);
        _keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Key.V);
        _keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Key.X);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y);
        _keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z);

        _window.TextInput += (s, a) =>
        {
            if (a.Character == '\t') return;

            io.AddInputCharacter(a.Character);
        };

        _ = io.Fonts.AddFontDefault();
    }

    /// <summary>
    /// Updates the <see cref="Effect" /> to the current matrices and texture
    /// </summary>
    protected virtual Effect UpdateEffect(Texture texture)
    {
        var io = ImGui.GetIO();

        _effect ??= _graphicsDevice.CreateEffect();
        _effect.World = Matrix4x4.Identity;
        _effect.View = Matrix4x4.Identity;
        _effect.Projection = Matrix4x4.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f);
        _effect.TextureEnabled = true;
        _effect.Texture = texture;
        _effect.VertexColorEnabled = true;

        return _effect;
    }

    /// <summary>
    /// Sends XNA input state to ImGui
    /// </summary>
    protected virtual void UpdateInput(FrameTime frameTime)
    {
        var io = ImGui.GetIO();

        _inputManager.Update(frameTime);
        var keyboardState = _inputManager.Keyboard.State;
        var mouseState = _inputManager.Mouse.State;

        for (var i = 0; i < _keys.Count; i++)
        {
            io.KeysDown[_keys[i]] = keyboardState[(Key)_keys[i]] == ButtonState.Pressed;
        }

        io.KeyShift = keyboardState.HasModifier(KeyModifiers.Shift);
        io.KeyCtrl = keyboardState.HasModifier(KeyModifiers.Ctrl);
        io.KeyAlt = keyboardState.HasModifier(KeyModifiers.Alt);
        io.KeySuper = keyboardState[Key.LWin] == ButtonState.Pressed || keyboardState[Key.RWin] == ButtonState.Pressed;

        io.DisplaySize = _graphicsDevice.BackBufferSize;
        io.DisplayFramebufferScale = Vector2.One;

        io.MousePos = new Vector2(mouseState.X, mouseState.Y);

        io.MouseDown[0] = mouseState.Primary == ButtonState.Pressed;
        io.MouseDown[1] = mouseState.Secondary == ButtonState.Pressed;
        io.MouseDown[2] = mouseState.Middle == ButtonState.Pressed;

        var scrollDelta = mouseState.ScrollX - _scrollWheelValue;
        io.MouseWheel = Math.Sign(scrollDelta);
        _scrollWheelValue = mouseState.ScrollX;
    }

    #endregion Setup & Update

    #region Internals

    /// <summary>
    /// Gets the geometry as set up by ImGui and sends it to the graphics device
    /// </summary>
    private void RenderDrawData(ImDrawDataPtr drawData)
    {
        // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
        var viewport = _graphicsDevice.Viewport;
        var clippingBounds = _graphicsDevice.ClippingBounds;

        _graphicsDevice.BlendFactor = Intersect.Graphics.Color.White;
        _graphicsDevice.Blending = Blending.NonPremultiplied;
        _graphicsDevice.DepthStencil = DepthStencil.DepthRead;
        _graphicsDevice.Rasterizer = _rasterizerState;

        // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
        drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

        // Setup projection
        _graphicsDevice.Viewport = new Viewport(0, 0, _graphicsDevice.BackBufferSize);

        UpdateBuffers(drawData);

        RenderCommandLists(drawData);

        // Restore modified state
        _graphicsDevice.Viewport = viewport;
        _graphicsDevice.ClippingBounds = clippingBounds;
    }

    private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
    {
        if (drawData.TotalVtxCount == 0)
        {
            return;
        }

        // Expand buffers if we need more room
        if (drawData.TotalVtxCount > _vertexBufferSize)
        {
            _vertexBuffer?.Dispose();

            _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
            _vertexBuffer = _graphicsDevice.CreateVertexBuffer(_vertexDeclaration, _vertexBufferSize, BufferUsage.None);
            _vertexData = new byte[_vertexBufferSize * _vertexDeclaration.Stride];
        }

        if (drawData.TotalIdxCount > _indexBufferSize)
        {
            _indexBuffer?.Dispose();

            _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
            _indexBuffer = _graphicsDevice.CreateIndexBuffer(IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
            _indexData = new byte[_indexBufferSize * sizeof(ushort)];
        }

        // Copy ImGui's vertices and indices to a set of managed byte arrays
        var vtxOffset = 0;
        var idxOffset = 0;

        for (var n = 0; n < drawData.CmdListsCount; n++)
        {
            var cmdList = drawData.CmdListsRange[n];

            fixed (void* vtxDstPtr = &_vertexData[vtxOffset * _vertexDeclaration.Stride])
            fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
            {
                Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, _vertexData.Length, cmdList.VtxBuffer.Size * _vertexDeclaration.Stride);
                Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, _indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
            }

            vtxOffset += cmdList.VtxBuffer.Size;
            idxOffset += cmdList.IdxBuffer.Size;
        }

        // Copy the managed byte arrays to the gpu vertex- and index buffers
        _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * _vertexDeclaration.Stride);
        _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
    }

    private unsafe void RenderCommandLists(ImDrawDataPtr drawData)
    {
        if (drawData.TotalVtxCount == 0)
        {
            return;
        }

        _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        _graphicsDevice.SetIndexBuffer(_indexBuffer);

        var vtxOffset = 0;
        var idxOffset = 0;

        for (var n = 0; n < drawData.CmdListsCount; n++)
        {
            var cmdList = drawData.CmdListsRange[n];

            for (var cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
            {
                var drawCmd = cmdList.CmdBuffer[cmdi];

                if (!_loadedTextures.ContainsKey(drawCmd.TextureId))
                {
                    throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                }

                _graphicsDevice.ClippingBounds = new Rectangle(
                    (int)drawCmd.ClipRect.X,
                    (int)drawCmd.ClipRect.Y,
                    (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                    (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                );

                var effect = UpdateEffect(_loadedTextures[drawCmd.TextureId]);

                effect.OnEachPass(() =>
                {
                    _graphicsDevice.DrawIndexedPrimitives(
                        primitiveType: PrimitiveType.TriangleList,
                        baseVertex: vtxOffset,
                        startIndex: idxOffset,
                        primitiveCount: (int)drawCmd.ElemCount / 3
                    );
                });

                idxOffset += (int)drawCmd.ElemCount;
            }

            vtxOffset += cmdList.VtxBuffer.Size;
        }
    }

    #endregion Internals
}
