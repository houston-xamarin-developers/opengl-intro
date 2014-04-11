# Intro to OpenGL on iOS w/ C# 

OpenGL is primarily a C API for sending commands to a SIMD super computer (with a graphics bias)

OpenGL is necessary, but not sufficient for GPU-accelerated graphics on iPhone (and most hardware). 

OpenTK is a .NET wrapper for OpenGL. It is 1:1 in most cases. However it does fill in some missing gaps making it sufficient for graphics use.

## Prerequisites

- MonoTouch stuff (e.g. Have to use Monotouch flavored builds of all libraries, like OpenTK)
- Vector arithmetic
- Transformations via Matrix Multiplication
- GPU Programming with Shaders

## Tips

- Start simple: one triangle, one color
- Know the default view / projection (-1 ... 1)
- Use a higher level API if you can (e.g. MonoGame)
    - No window / device creation
    - No input (e.g. hit detection)
    - No file I/O
    - No "camera" concept
    - No vector arithmetic help
    - Virtually no complex types

## Benefits

- Portability
    - Unsupported in Windows 8 Store Apps
- Performance
    - Like a separate super computer in your phone
- SIMD
- Interpolation

## History

- SGI "The GL" or "IRIS GL" pre 1992
- Was managed by ARB
- Now managed by Khronos Group
- Subset OpenGL ES (Embedded)
    - Although modern phones are more capable than original SGI workstations
- Fixed Function (non-programmable) Pipeline
- Shaders : Programmable Pipeline
- Direct Mode -> Retained Mode

## Nuances
- One-way
- State based
- No objects; just uints...

## Intro
- EAGL
- ES 2.0
- Map CPU values to GPU values
- Pipeline & Stages

## Intro Code

    using OpenTK.Graphics.ES20

### Vertex Program

    attribute vec4 position;
    
    void main()
    {
        gl_Position = position;
    }
  
### Fragment Program

    void main()
    {
        gl_FragColor = vec4(1.0, 1.0, 1.0, 1.0);
    }
    
### Setup

    void CompileAndAttach(ShaderType type, string src)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, 1, new string[] { src }, (int[])null);
        GL.CompileShader(shader);
        GL.AttachShader(_program, shader);
    }

----
    
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
    
### Draw

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


## Pitfalls & Gotchas

- OpenTK-1.0 vs OpenTK reference
- OpenTK is FOSS
    - You might need to build from source
    - For a while it was unclear whether it was still being maintained and which fork was the one to follow...
    - It is now on github
- monotouch build
- Works in simulator != Works on device
- Simulator perf != Device perf
- Resize / device rotation
- Thread affinity
- Error checking
    - Looks like latest OpenTK version will throw exception in DEBUG mode. Haven't tried it yet.
- Bit reinterpretation
- Shader compilation typically at run-time
- Vertex winding & back-face culling
    - `GL.Disable(EnableCap.CullFace);`
- Clean-up / Disposal
- vsync (CADisplayLink)
- Transformation
    - Order matters
    - Rotation accumulation is non-trivial
- Resource bug (change resource, rebuild)
- Can send values to shader that aren't connected
- No free int to float cast in shaders (can't do vec4(1,1,1,1))

## My favorite references

- History of the teapot
- TurboSquid
- StackOverflow
- NeHe
- The Orange Book

## New iOS Helpers

- Multithreading support
- GLKit

