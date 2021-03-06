using CSharpMath.Display.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CSharpMath.Display;

namespace CSharpMath.FrontEnd {
  public interface IGlyphBoundsProvider<TFont, TGlyph>
    where TFont: IFont<TGlyph> {
    /// <summary>The width of the glyph run.</summary>
    float GetTypographicWidth(TFont font, AttributedGlyphRun<TFont, TGlyph> run);
    /// <summary>This should treat the glyphs independently. In other words,
    /// we don't assume they are one after the other; likely use case is considering
    /// different options.</summary>
    IEnumerable<RectangleF> GetBoundingRectsForGlyphs(TFont font, ForEach<TGlyph> glyphs, int nGlyphs);
    /// <summary>The advances for the individual glyphs and the overall advance.</summary>
    (IEnumerable<float> Advances, float Total) GetAdvancesForGlyphs(TFont font, ForEach<TGlyph> glyphs, int nGlyphs);
  }
}
