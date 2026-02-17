using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Game/Color Palette")]
public class ColorPalette : ScriptableObject
{
    public Color background;
    public Color timeDilationNormalSpeed;
    public Color timeDilationFastSpeed;
    public Color disabled;
    public Color alphaLaserTransparent;
    public Color alphaLaserSolidify;
}
