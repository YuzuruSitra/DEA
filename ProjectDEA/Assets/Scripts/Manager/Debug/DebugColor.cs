using UnityEngine;

namespace Manager.Debug
{
    public class DebugColor
    {
        private readonly Material _material;

        public DebugColor(Material mat)
        {
            _material = mat;
        }

        public void ChangeColor(Color color)
        {
            _material.color = color;
        }

    }
}
