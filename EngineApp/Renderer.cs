using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using System.Linq;
using Silk.NET.Vulkan;

namespace FishEngine
{
    public class Renderer
    {
        private static IWindow? _window;
        private static IInputContext? _inputContext;
        private static GL? _gl;
        private static uint program;

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

		private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
		
		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        public Renderer()
        {
            _window = Window.Create(WindowOptions.Default with
            {
                Title = "FishEngine",
                //Size = new Vector2D<int>(1000, 1000)
            });

            _window.Render += Window_Render;
            _window.Load += Window_Load;

            _window.Run();
        }

        private void Keyboard_KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            Console.WriteLine(arg2);
        }

        private static unsafe void Window_Render(double obj)
        {
            if (_gl is null) return;
            _gl.ClearColor(0f, 0f, 1f, 1f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            uint vao = _gl.GenVertexArray();
            _gl.BindVertexArray(vao);

            uint vertices = _gl.GenBuffer();
            uint colors = _gl.GenBuffer();
            uint indices = _gl.GenBuffer();

            float[] vertexArray = new float[]
            {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f, 0.5f, 0.5f
            };

            float[] colorArray = Enumerable.Repeat(0.5f, 12).ToArray();

            uint[] indexArray = new uint[] { 0, 1, 2 };

            _gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            _gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 0, null);
            _gl.EnableVertexAttribArray(0);

            _gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            _gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            _gl.VertexAttribPointer(1, 4, GLEnum.Float, false, 0, null);
            _gl.EnableVertexAttribArray(1);

            _gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            _gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>) indexArray.AsSpan(), GLEnum.StaticDraw);

            _gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            _gl.DrawElements(GLEnum.Triangles, 3, GLEnum.UnsignedInt, null);

            _gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            _gl.BindVertexArray(vao);

            _gl.DeleteBuffer(vertices);
            _gl.DeleteBuffer(colors);
            _gl.DeleteBuffer(indices);
            _gl.DeleteBuffer(vao);
        }
        
        private void Window_Load()
        {
            if (_window is null) return;

            _gl = GL.GetApi(_window);

            _inputContext = _window.CreateInput();
            foreach (var keyboard in _inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            uint vShader = _gl.CreateShader(ShaderType.VertexShader);
            uint fShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(vShader, VertexShaderSource);
            _gl.ShaderSource(fShader, FragmentShaderSource);
            _gl.CompileShader(vShader);
            _gl.CompileShader(fShader);

            program = _gl.CreateProgram();
            _gl.AttachShader(program, vShader);
            _gl.AttachShader(program, fShader);
            _gl.LinkProgram(program);
            _gl.DetachShader(program, vShader);
            _gl.DetachShader(program, fShader);
            _gl.DeleteShader(vShader);
            _gl.DeleteShader(fShader);

            _gl.GetProgram(program, GLEnum.LinkStatus, out var status);
        }
    }
}
