using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;
using OpenTK;
using OpenTK.Graphics.ES20;
using OpenTK.Platform.iPhoneOS;

namespace TryOpenGL
{
    [Register("EAGLView")]
    public class EAGLView : iPhoneOSGameView
    {
        float[,] _vertices = {
            { -0.5f, -0.33f, -0.5f},
            {  0.5f, -0.33f, -0.5f},
            { -0.5f,  0.33f, -0.5f},
        };
        int _vertexAttribute = -1;
        int _program = -1;

        void CompileAndAttach(ShaderType type, string src)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, 1, new string[] { src }, (int[])null);
            GL.CompileShader(shader);
            GL.AttachShader(_program, shader);
        }

        void Setup()
        {
            _program = GL.CreateProgram();
            var vertShaderSource = @"
                attribute vec4 position;
                void main()
                {
                    gl_Position = position;
                }
            ";
            CompileAndAttach(ShaderType.VertexShader, vertShaderSource);
            var fragShaderSource = @"
                void main()
                {
                    gl_FragColor = vec4(1.0, 1.0, 1.0, 1.0);
                }
            ";
            CompileAndAttach(ShaderType.FragmentShader, fragShaderSource);
            GL.LinkProgram(_program);
            _vertexAttribute = GL.GetAttribLocation(_program, "position");
        }

        void Draw()
        {
            MakeCurrent();
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(_program);
            var vertexComponents = _vertices.GetLength(1);
            GL.VertexAttribPointer(_vertexAttribute, size: vertexComponents, 
                type: VertexAttribPointerType.Float, normalized: false, stride: 0, ptr: _vertices);
            GL.EnableVertexAttribArray(_vertexAttribute);
            GL.DrawArrays(BeginMode.Triangles, first: 0, count: 3);
            SwapBuffers();
        }

        #region OpenGL Boilerplate
        [Export("initWithCoder:")]
        public EAGLView(NSCoder coder) : base(coder)
        {
            LayerRetainsBacking = true;
            LayerColorFormat = EAGLColorFormat.RGBA8;
        }

        [Export("layerClass")]
        public static new Class GetLayerClass()
        {
            return iPhoneOSGameView.GetLayerClass();
        }

        protected override void ConfigureLayer(CAEAGLLayer eaglLayer)
        {
            eaglLayer.Opaque = true;
        }

        protected override void CreateFrameBuffer()
        {
            ContextRenderingApi = EAGLRenderingAPI.OpenGLES2;
            base.CreateFrameBuffer();
            Setup();
        }

        #region DisplayLink support

        int frameInterval;
        CADisplayLink displayLink;

        public bool IsAnimating { get; private set; }
        // How many display frames must pass between each time the display link fires.
        public int FrameInterval {
            get {
                return frameInterval;
            }
            set {
                if (value <= 0)
                    throw new System.ArgumentException();
                frameInterval = value;
                if (IsAnimating)
                {
                    StopAnimating();
                    StartAnimating();
                }
            }
        }

        public void StartAnimating()
        {
            if (IsAnimating)
                return;

            CreateFrameBuffer();
            displayLink = UIScreen.MainScreen.CreateDisplayLink(this, new Selector("drawFrame"));
            displayLink.FrameInterval = frameInterval;
            displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoop.NSDefaultRunLoopMode);

            IsAnimating = true;
        }

        public void StopAnimating()
        {
            if (!IsAnimating)
                return;

            displayLink.Invalidate();
            displayLink = null;
            DestroyFrameBuffer();
            IsAnimating = false;
        }

        [Export("drawFrame")]
        void DrawFrame()
        {
            OnRenderFrame(new FrameEventArgs());
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Draw();
        }
        #endregion
        #endregion
    }
}
