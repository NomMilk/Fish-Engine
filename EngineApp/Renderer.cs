using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace FishEngine
{
    public class Renderer
    {
        IWindow _window;
        GL? _gl;

        public Renderer()
        {
            _window = Window.Create(WindowOptions.Default with
            {
                Title = "FishEngine"
            });

            _window.Render += Window_Render;
            _window.Load += Window_Load;

            _window.Initialize(); 

            var _inputContext = _window.CreateInput();
            foreach (var keyboard in _inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            Initial();
        }

        private void Keyboard_KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            Console.WriteLine(arg2);
        }

        private void Window_Render(double obj)
        {
            _gl?.ClearColor(0.5f, 0.5f, 0.5f, 1f);
            _gl?.Clear(ClearBufferMask.ColorBufferBit);
        }
        
        private void Window_Load()
        {
            _gl = GL.GetApi(_window);
        }

        public void Initial()
        {
            _window.Run();
        }

    }
}
