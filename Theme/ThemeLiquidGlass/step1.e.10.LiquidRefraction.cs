using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProGlassAutomation.ThemeLiquidGlass;

/// <summary>
/// CPU port of the Aghajari rounded-rect SDF lens used by
/// dragosniamtu/WPF-Liquid-Glass-Effect (MIT, LiquidGlassSample.glsl).
/// Samples a procedural watery field instead of a desktop HWND capture.
/// No XamlTemplates.FX.Wpf, no GDI screenshot, no extra NuGet.
/// </summary>
internal static class LiquidRefraction
{
    internal static double Phase;

    internal readonly struct Request
    {
        public Request(
            double width,
            double height,
            double cornerRadius,
            double frost,
            double strength,
            double displacement,
            double refractionWidth,
            double chromatic,
            double edgeHighlight,
            double worldX,
            double worldY,
            Color canvas,
            Color tint,
            double tintStrength)
        {
            Width = width;
            Height = height;
            CornerRadius = cornerRadius;
            Frost = frost;
            Strength = strength;
            Displacement = displacement;
            RefractionWidth = refractionWidth;
            Chromatic = chromatic;
            EdgeHighlight = edgeHighlight;
            WorldX = worldX;
            WorldY = worldY;
            Canvas = canvas;
            Tint = tint;
            TintStrength = tintStrength;
        }

        public double Width { get; }
        public double Height { get; }
        public double CornerRadius { get; }
        public double Frost { get; }
        public double Strength { get; }
        public double Displacement { get; }
        public double RefractionWidth { get; }
        public double Chromatic { get; }
        public double EdgeHighlight { get; }
        public double WorldX { get; }
        public double WorldY { get; }
        public Color Canvas { get; }
        public Color Tint { get; }
        public double TintStrength { get; }
    }

    internal static void Fill(WriteableBitmap bitmap, in Request request)
    {
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        byte[] pixels = new byte[width * height * 4];

        double outputW = width;
        double outputH = height;
        double halfW = outputW * 0.5;
        double halfH = outputH * 0.5;
        double scaleX = outputW / Math.Max(request.Width, 1);
        double scaleY = outputH / Math.Max(request.Height, 1);
        double scale = Math.Min(scaleX, scaleY);
        double radius = Math.Clamp(request.CornerRadius * scale, 0, Math.Min(halfW, halfH));
        double minSize = Math.Max(Math.Min(outputW, outputH), 1);
        double displacement = Math.Max(request.Displacement, 0) * scale * Math.Max(request.Strength, 0);
        double refractionWidth = Math.Max(request.RefractionWidth, 0.0001);
        double chromatic = Math.Max(request.Chromatic, 0);
        double frost = Math.Clamp(request.Frost, 0, 1);
        double phase = Phase;
        Color canvas = request.Canvas;
        Color tint = request.Tint;
        double tintStrength = Math.Clamp(request.TintStrength, 0, 1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double fragmentX = x + 0.5;
                double fragmentY = y + 0.5;
                double glassX = fragmentX - halfW;
                double glassY = fragmentY - halfH;
                double distancePx = RoundedBoxDistance(glassX, glassY, halfW, halfH, radius);
                double coverage = 1.0 - Smoothstep(-1.2, 1.2, distancePx);
                int index = (y * width + x) * 4;
                if (coverage <= 0.002)
                {
                    pixels[index] = 0;
                    pixels[index + 1] = 0;
                    pixels[index + 2] = 0;
                    pixels[index + 3] = 0;
                    continue;
                }

                double inverseSdf = -distancePx / minSize;
                double distanceFromCenter = 1.0 - Clamp(inverseSdf / refractionWidth, 0, 1);
                double curvatureTerm = Math.Max(1.0 - distanceFromCenter * distanceFromCenter, 0);
                double distortion = 1.0 - Math.Sqrt(curvatureTerm);
                Normalize(glassX, glassY, out double dirX, out double dirY);

                double refractX = fragmentX - dirX * distortion * displacement;
                double refractY = fragmentY - dirY * distortion * displacement;
                double chromaticProfile = Smoothstep(0.0, 0.02, inverseSdf);
                double shift = chromaticProfile * 3.0 * chromatic;
                double worldX = request.WorldX + refractX / scaleX;
                double worldY = request.WorldY + refractY / scaleY;
                double shiftX = dirX * shift / scaleX;
                double shiftY = dirY * shift / scaleY;

                SampleField(worldX - shiftX, worldY - shiftY, phase, canvas, out double redR, out _, out _);
                SampleField(worldX, worldY, phase, canvas, out _, out double greenG, out _);
                SampleField(worldX + shiftX, worldY + shiftY, phase, canvas, out _, out _, out double blueB);
                double r = redR * 0.90;
                double g = greenG * 0.90;
                double b = blueB * 0.90;
                r += (tint.R / 255.0 - r) * tintStrength;
                g += (tint.G / 255.0 - g) * tintStrength;
                b += (tint.B / 255.0 - b) * tintStrength;

                double edgeBand = 1.0 - Smoothstep(0.0, 4.0, Math.Max(-distancePx, 0.0));
                double highlight = edgeBand * Math.Clamp(request.EdgeHighlight, 0, 1) * 0.18;
                r += highlight;
                g += highlight;
                b += highlight;

                double meniscus = Smoothstep(0, 2.5, Math.Max(-distancePx, 0)) * (1.0 - Smoothstep(2.5, 9.0, Math.Max(-distancePx, 0)));
                r *= 1.0 - meniscus * 0.22;
                g *= 1.0 - meniscus * 0.18;
                b *= 1.0 - meniscus * 0.12;

                double rim = distortion;
                double alpha = coverage * (frost * 0.18 + rim * (0.40 + frost * 0.28) + edgeBand * 0.12);
                alpha = Clamp(alpha, 0, 0.92);
                WritePbgra(pixels, index, r, g, b, alpha);
            }
        }

        bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
    }

    private static void SampleField(double x, double y, double phase, Color canvas, out double r, out double g, out double b)
    {
        double u = x * 0.0125;
        double v = y * 0.0125;
        double wave = Math.Sin(u * 2.15 + phase) * Math.Sin(v * 1.72 - phase * 0.73);
        double cross = Math.Sin((u * 1.35 + v * 0.92) * 1.55 + 0.9);
        double radial = Math.Sin(Math.Sqrt(u * u * 0.55 + v * v) * 3.1 - phase * 0.45);
        double field = 0.5 + 0.5 * (0.46 * wave + 0.34 * cross + 0.20 * radial);
        field = Clamp(field, 0, 1);
        double ridge = Math.Pow(field, 2.4);
        double sparkle = Math.Pow(Clamp(0.5 + 0.5 * Math.Sin(u * 11.4 + v * 8.6 + phase * 1.2), 0, 1), 9);

        double cr = canvas.R / 255.0;
        double cg = canvas.G / 255.0;
        double cb = canvas.B / 255.0;
        double dr = cr * 0.48;
        double dg = cg * 0.55;
        double db = cb * 0.62;
        double hr = Math.Min(1, cr + 0.38);
        double hg = Math.Min(1, cg + 0.32);
        double hb = Math.Min(1, cb + 0.22);

        r = dr + (cr - dr) * field;
        g = dg + (cg - dg) * field;
        b = db + (cb - db) * field;
        r += (hr - r) * ridge * 0.85;
        g += (hg - g) * ridge * 0.85;
        b += (hb - b) * ridge * 0.85;
        r = Clamp(r + sparkle * 0.55, 0, 1);
        g = Clamp(g + sparkle * 0.62, 0, 1);
        b = Clamp(b + sparkle * 0.70, 0, 1);
    }

    private static double RoundedBoxDistance(double px, double py, double halfW, double halfH, double radius)
    {
        double qx = Math.Abs(px) - Math.Max(halfW - radius, 0);
        double qy = Math.Abs(py) - Math.Max(halfH - radius, 0);
        double outside = Math.Sqrt(Math.Max(qx, 0) * Math.Max(qx, 0) + Math.Max(qy, 0) * Math.Max(qy, 0));
        double inside = Math.Min(Math.Max(qx, qy), 0);
        return outside + inside - radius;
    }

    private static void Normalize(double x, double y, out double nx, out double ny)
    {
        double mag2 = x * x + y * y;
        if (mag2 <= 0.000001)
        {
            nx = 0;
            ny = 0;
            return;
        }
        double inv = 1.0 / Math.Sqrt(mag2);
        nx = x * inv;
        ny = y * inv;
    }

    private static double Smoothstep(double edge0, double edge1, double x)
    {
        double t = Clamp((x - edge0) / (edge1 - edge0), 0, 1);
        return t * t * (3 - 2 * t);
    }

    private static double Clamp(double value, double min, double max) => value < min ? min : value > max ? max : value;

    private static void WritePbgra(byte[] buffer, int index, double r, double g, double b, double a)
    {
        a = Clamp(a, 0, 1);
        r = Clamp(r, 0, 1) * a;
        g = Clamp(g, 0, 1) * a;
        b = Clamp(b, 0, 1) * a;
        buffer[index] = (byte)Math.Round(b * 255);
        buffer[index + 1] = (byte)Math.Round(g * 255);
        buffer[index + 2] = (byte)Math.Round(r * 255);
        buffer[index + 3] = (byte)Math.Round(a * 255);
    }
}
