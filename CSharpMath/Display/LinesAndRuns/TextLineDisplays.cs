﻿using CSharpMath.Atoms;
using CSharpMath.Display.Text;
using CSharpMath.FrontEnd;
using CSharpMath.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpMath.Display.LinesAndRuns {
  public static class TextLineDisplays {
    public static TextLineDisplay Create(
      AttributedString text,
      Range range,
      FontMathTable table,
      IGlyphBoundsProvider boundsProvider,
      IEnumerable<IMathAtom> atoms
      ) {
      int index = range.Location;
      List<TextRunDisplay> textRuns = new List<TextRunDisplay>();
      foreach (var run in text.Runs) {
        var innerRange = new Range(index, run.Length);
        var textRun = new TextRunDisplay(
          run,
          innerRange,
          table,
          boundsProvider
          );
        textRuns.Add(textRun);
      }
      return new TextLineDisplay {
        Runs = textRuns,
      }
    }
  }
}