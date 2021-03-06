﻿using CSharpMath.Enumerations;
using CSharpMath.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpMath.Atoms {
  public class Table : MathAtom, IMathTable {
    public List<ColumnAlignment> Alignments { get; } = new List<ColumnAlignment>();
    public List<List<IMathList>> Cells { get; set; } = new List<List<IMathList>>();

    public float InterColumnSpacing { get; set; }
    public float InterRowAdditionalSpacing { get; set; }

    private string _environment;

    public Table(string environment): base(MathAtomType.Table, string.Empty) {
      _environment = environment;
    }

    public Table():this(null) { }

    /// <summary>
    /// Deep copy, finalized or not.
    /// </summary>
    public Table(Table cloneMe, bool finalize) : base(cloneMe, finalize) {
      InterColumnSpacing = cloneMe.InterColumnSpacing;
      InterRowAdditionalSpacing = cloneMe.InterRowAdditionalSpacing;
      Environment = cloneMe.Environment;
      Alignments = cloneMe.Alignments.ToList();
      Cells = new List<List<IMathList>>(cloneMe.Cells.Select(list =>
      new List<IMathList>(list.Select(sublist =>
      AtomCloner.Clone(sublist, finalize)))));
    }

    public string Environment {
      get => _environment;
      set => _environment = value;
    }


    public int NRows => Cells.Count;
    public int NColumns => (NRows == 0) ? 0 : Cells.Max(row => row.Count);

    public void SetCell(IMathList list, int iRow, int iColumn) {
      while (Cells.Count <= iRow) {
        Cells.Add(new List<IMathList>());
      }
      while (Cells[iRow].Count <= iColumn) {
        Cells[iRow].Add(new MathList());
      }
      Cells[iRow][iColumn] = list;
    }

    public void SetAlignment(ColumnAlignment alignment, int columnIndex) {
      while (Alignments.Count <= columnIndex) {
        Alignments.Add(ColumnAlignment.Center);
      }
      Alignments[columnIndex] = alignment;
    }

    public ColumnAlignment GetAlignment(int columnIndex) {
      if (Alignments.Count <= columnIndex) {
        return ColumnAlignment.Center;
      }
      return Alignments[columnIndex];
    }


    public bool EqualsTable(Table otherTable) =>
        EqualsAtom(otherTable) &&
        NRows == otherTable.NRows &&
        Cells.EqualsEnumerable(otherTable.Cells, (c1, c2) => c1.EqualsEnumerable(c2)) &&
        Alignments.EqualsEnumerable(otherTable.Alignments);

    public override bool Equals(object obj) => EqualsTable(obj as Table);

    public override int GetHashCode() {
      unchecked {
        return base.GetHashCode()
          + 109 * Cells.GetHashCode()
          + 113 * Alignments.GetHashCode();
      }
    }

    public override T Accept<T, THelper>(IMathAtomVisitor<T, THelper> visitor, THelper helper)
=> visitor.Visit(this, helper);
  }
}
