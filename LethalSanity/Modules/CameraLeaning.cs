using LethalSex_Core;

namespace LethalSanity.Modules
{
    internal class CameraLeaning : LethalClass
    {
        public CameraLeaning Module { get; private set; }

        protected override void Awake()
        {
            // Additional initialization logic specific to SubClass
            base.Awake();
        }

        protected override void Destroy()
        {
            base.Destroy();
        }
    }
}