using System.Numerics;

namespace HolyScreenSaver.Data;

public class HudElement
{
    public string ElementId { get; set; } = string.Empty;
    public Vector2 CurrentPosition { get; set; }
    public Vector2 FirstPosition { get; set; }
    public Vector2 Velocity { get; set; }
}
