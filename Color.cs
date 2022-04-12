namespace ConsoleColorConverter;

internal class Color
{
    internal readonly int Id;
    internal readonly string Name;
    internal readonly int Rgb;

    internal Color(int id, string name, int rgb)
    {
        Id = id;
        Name = name;
        Rgb = rgb;
    }
}
