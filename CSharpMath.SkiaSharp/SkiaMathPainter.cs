﻿using System.Drawing;
using CSharpMath.Rendering;
using CSharpMath.Structures;
using SkiaSharp;

namespace CSharpMath.SkiaSharp {
  public class SkiaMathPainter : MathPainter<SKCanvas, SKColor>, ICanvasPainter<SKCanvas, MathSource, SKColor> {
    public SkiaMathPainter(float fontSize = DefaultFontSize, bool antiAlias = true) : base(fontSize) =>
      AntiAlias = antiAlias;
    
    public SKStrokeCap StrokeCap { get; set; }
    public bool AntiAlias { get; set; }

    public void Draw(SKCanvas canvas, SKPoint point) => Draw(canvas, point.X, point.Y);

    protected override bool CoordinatesFromBottomLeftInsteadOfTopLeft => false;

    public override SKColor UnwrapColor(Color color) => color.ToNative();

    public override Color WrapColor(SKColor color) => color.FromNative();

    public override ICanvas WrapCanvas(SKCanvas canvas) =>
      new SkiaCanvas(canvas, StrokeCap, AntiAlias);

  }
}