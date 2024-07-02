using UnityEngine;

public class DebugColor
{
    Material _material;

    public DebugColor(Material mat)
    {
        _material = mat;
    }

    public void ChangeColor(Color color)
    {
        _material.color = color;
    }

}
