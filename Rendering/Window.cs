using Silk.NET.Core;
using Silk.NET.Vulkan;

namespace Fish.Renderer
{
    class Window
    {
        private Vk _vk;
        private Instance _instance;

        public void Init()
        {
            _vk = Vk.GetApi();
            var instanceCreateInfo = new InstanceCreateInfo();
            _instance = _vk.CreateInstance(instanceCreateInfo);
        }

        public void Cleanup()
        {
            _vk.DestroyInstance(_instance);
        }
    }
}
