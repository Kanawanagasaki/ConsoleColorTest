using System.Text;
using ConsoleColorConverter;

int[] colorSets = new int[] { 0, 1, 2, 2, 3, 3, 4, 4, 4, 4, 4, 4 }; // Just for demonstration

int colorSet = 4;

StringBuilder builder = new();

var start = DateTime.Now;
while (true)
{
    var info = $"Color set: {colorSet switch { 0 => 2, 1 => 8, 2 => 16, 3 => 256, 4 => 0xFFFFFF, _ => 0 }} colors";

    var width = Console.WindowWidth;
    var height = Console.WindowHeight;

    var span = DateTime.Now - start;
    colorSet = colorSets[((int)(span.TotalSeconds / Math.PI)) % colorSets.Length];
    builder.Clear();
    builder.Append(info);
    for (double iy = 0; iy < height; iy++)
    {
        for (double ix = 0; ix < width; ix++)
        {
            if(iy == 0 && ix < info.Length)
                continue;

            double xx = ix / (width - 1) * 2 - 1;
            double yy = iy / (height - 1) * 2 - 1;
            double distance = Math.Sqrt(xx * xx + yy * yy) / 1.5;

            double angle = Math.Atan2(iy - height / 2, ix - width / 2) + (span.TotalSeconds) % (Math.PI * 2);
            while (angle > Math.PI) angle -= Math.PI * 2;
            while (angle < -Math.PI) angle += Math.PI * 2;

            var color = ColorSheet.FromHsl(angle, 1, distance / 2, colorSet switch { 1 => 8, 2 => 16, 3 => 256, _ => 256 });
            switch (colorSet)
            {
                case 0:
                    builder.Append($"\x1b[4{(angle < 0 ? 0 : 7)}m ");
                    break;
                case 1:
                    builder.Append($"\x1b[4{color.Id}m ");
                    break;
                case 2:
                    builder.Append($"\x1b[4{color.Id % 8}{(color.Id < 8 ? "" : "1")}m ");
                    break;
                case 3:
                    builder.Append($"\u001b[48;5;{color.Id}m ");
                    break;
                case 4:
                    var rgb = FromHsl(angle, 1, distance / 2);
                    builder.Append($"\x1b[48;2;{rgb.r};{rgb.g};{rgb.b}m ");
                    break;
            }
        }
    }

    builder.Append("\x1b[0m");

    Console.SetCursorPosition(0, 0);
    Console.Write(builder.ToString());
    Console.CursorVisible = false;
}

(byte r, byte g, byte b) FromHsl(double h, double s, double l)
{
    byte r, g, b;
    if (s == 0)
    {
        r = (byte)Math.Round(l * 255d);
        g = (byte)Math.Round(l * 255d);
        b = (byte)Math.Round(l * 255d);
    }
    else
    {
        double t1, t2;
        double th = h / 6.0d;

        if (l < 0.5d)
        {
            t2 = l * (1d + s);
        }
        else
        {
            t2 = (l + s) - (l * s);
        }
        t1 = 2d * l - t2;

        double tr, tg, tb;
        tr = th + (1.0d / 3.0d);
        tg = th;
        tb = th - (1.0d / 3.0d);

        tr = ColorCalc(tr, t1, t2);
        tg = ColorCalc(tg, t1, t2);
        tb = ColorCalc(tb, t1, t2);
        r = (byte)Math.Round(tr * 255d);
        g = (byte)Math.Round(tg * 255d);
        b = (byte)Math.Round(tb * 255d);
    }
    return (r, g, b);
}

double ColorCalc(double c, double t1, double t2)
{
    if (c < 0) c += 1d;
    if (c > 1) c -= 1d;
    if (6.0d * c < 1.0d) return t1 + (t2 - t1) * 6.0d * c;
    if (2.0d * c < 1.0d) return t2;
    if (3.0d * c < 2.0d) return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
    return t1;
}