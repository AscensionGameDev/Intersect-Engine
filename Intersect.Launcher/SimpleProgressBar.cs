namespace Intersect.Launcher;

/// <summary>
/// Barre de progression dessinée à la main pour ne pas dépendre du style natif
/// (bleu Windows) du ProgressBar standard.
/// </summary>
public sealed class SimpleProgressBar : Control
{
    private int _value;

    public Color TrackColor { get; set; } = Color.FromArgb(40, 38, 50);

    public Color FillColor { get; set; } = Color.FromArgb(235, 196, 110);

    public int Value
    {
        get => _value;
        set
        {
            var clamped = Math.Clamp(value, 0, 100);
            if (clamped == _value)
            {
                return;
            }

            _value = clamped;
            Invalidate();
        }
    }

    public SimpleProgressBar()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer,
            true
        );
        Height = 8;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var trackRect = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var trackBrush = new SolidBrush(TrackColor))
        {
            FillRoundedRect(g, trackBrush, trackRect, Height / 2);
        }

        if (_value > 0)
        {
            var fillWidth = (int)(Width * (_value / 100.0));
            var fillRect = new Rectangle(0, 0, Math.Max(fillWidth - 1, Height), Height - 1);
            using var fillBrush = new SolidBrush(FillColor);
            FillRoundedRect(g, fillBrush, fillRect, Height / 2);
        }
    }

    private static void FillRoundedRect(Graphics g, Brush brush, Rectangle rect, int radius)
    {
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }

        var diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
        using var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        g.FillPath(brush, path);
    }
}
