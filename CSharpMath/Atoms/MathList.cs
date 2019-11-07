using CSharpMath.Enumerations;
using CSharpMath.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CSharpMath.Atoms {
  public class MathList : IMathList {

    public List<IMathAtom> Atoms { get; set; } = new List<IMathAtom>();
    public MathList() { }

    public bool IsAtomAllowed(IMathAtom atom) => atom != null && atom.AtomType != MathAtomType.Boundary;

    public MathList(IMathList cloneMe, bool finalize) : this() {
      if (!finalize) {
        foreach (var atom in cloneMe.Atoms) {
          var cloneAtom = AtomCloner.Clone(atom, finalize);
          Add(cloneAtom);

        }
      } else {
        IMathAtom prevNode = null;
        foreach (var atom in cloneMe.Atoms) {
          var newNode = AtomCloner.Clone(atom, finalize);
          if (atom.IndexRange == Range.Zero) {
            int prevIndex = prevNode is null ? 0 : prevNode.IndexRange.Location + prevNode.IndexRange.Length;
            newNode.IndexRange = new Range(prevIndex, 1);
          }
          switch (newNode.AtomType) {
            case MathAtomType.BinaryOperator:
              switch (prevNode?.AtomType) {
                case null:
                case MathAtomType.BinaryOperator:
                case MathAtomType.Relation:
                case MathAtomType.Open:
                case MathAtomType.Punctuation:
                case MathAtomType.Operator:
                case MathAtomType.LargeOperator:
                  newNode.AtomType = MathAtomType.UnaryOperator;
                  break;
              }
              break;
            case MathAtomType.Relation:
            case MathAtomType.Punctuation:
            case MathAtomType.Close:
              if (prevNode != null && prevNode.AtomType == MathAtomType.BinaryOperator) {
                prevNode.AtomType = MathAtomType.UnaryOperator;
              }
              break;
            case MathAtomType.Number:
              if (prevNode != null && prevNode.AtomType == MathAtomType.Number && prevNode.Subscript == null && prevNode.Superscript == null) {
                prevNode.Fuse(newNode);
                continue; // do not add the new node; we fused it instead.
              }
              break;

          }
          Add(newNode);
          prevNode = newNode;

        }
      }

    }
    
    ///<summary>Iteratively expands all groups in this list.</summary>
    public void ExpandGroups() {
      for (int i = 0; i < Count; i++) {
        if (Atoms[i].AtomType == MathAtomType.Group) {
          var group = (Group)Atoms[i];
          var initial = group.InnerList.Count - 1;
          var scriptsExpanded = false;
          for (int j = initial; j >= 0; j--) {
            Atoms.Insert(i + 1, group.InnerList[j]);
            if (!scriptsExpanded) {
              Atoms[i + 1].Superscript = group.Superscript;
              Atoms[i + 1].Subscript = group.Subscript;
              scriptsExpanded = true;
            }
          }
          if (!scriptsExpanded)
            Atoms.Insert(i + 1, new MathAtom(MathAtomType.Ordinary, "") {
              Superscript = group.Superscript,
              Subscript = group.Subscript
            });
          Atoms.RemoveAt(i);
        }
      }
    }

    public int Count => Atoms.Count;

    public string StringValue =>
      string.Concat(System.Linq.Enumerable.Select(Atoms, a => a.StringValue));

    public bool IsReadOnly => false;

    public IMathAtom this[int index] { get => Atoms[index]; set => Atoms[index] = value; }

    public void Append(IMathList list) => Atoms.AddRange(list.Atoms);
    public IMathList DeepCopy() => AtomCloner.Clone(this, false);
    public IMathList FinalizedList() => AtomCloner.Clone(this, true);
    public void RemoveAtoms(Range inRange) => Atoms.RemoveRange(inRange.Location, inRange.Length);
    public bool EqualsList(MathList otherList) {
      if (otherList == null) {
        return false;
      } 
      if (otherList.Count!=this.Count) {
        return false;
      }
      for (int i=0; i<this.Count; i++) {
        if (!this[i].NullCheckingEquals(otherList[i])) {
          return false;
        }
      }
      return true;
    }
    public override bool Equals(object obj) {
      if (obj is MathList) {
        return EqualsList((MathList)obj);
      }
      return false;
    }
    public override int GetHashCode() => Atoms.GetHashCode();
    public IEnumerator<IMathAtom> GetEnumerator() => Atoms.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Atoms.GetEnumerator();
    public int IndexOf(IMathAtom item) => Atoms.IndexOf(item);
    private void ThrowInvalid(IMathAtom item) {
      if (item == null) {
        throw new InvalidOperationException("MathList cannot contain null.");
      }
      if (item.AtomType == MathAtomType.Boundary) {
        throw new InvalidOperationException("MathList cannot contain items of type " + item.AtomType);
      }
    }
    public void Insert(int index, IMathAtom item) {
      if (IsAtomAllowed(item)) {
        Atoms.Insert(index, item);
      } else {
        ThrowInvalid(item);
      }
    }
    public void RemoveAt(int index) => Atoms.RemoveAt(index);
    public void Add(IMathAtom item) {
      if (IsAtomAllowed(item)) {
        Atoms.Add(item);
      } else {
        ThrowInvalid(item);
      }
    }
    public void Clear() => Atoms.Clear();
    public bool Contains(IMathAtom item) => Atoms.Contains(item);
    public void CopyTo(IMathAtom[] array, int arrayIndex) => Atoms.CopyTo(array, arrayIndex);
    public bool Remove(IMathAtom item) => Atoms.Remove(item);
  }
}
