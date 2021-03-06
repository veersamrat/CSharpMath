using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CSharpMath.Forms.Example {
  using Display;
  using Rendering;
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class EditorPage : ContentPage {
    public EditorPage() {
      InitializeComponent();
      Content = new EditorView();
    }
  }

  public class EditorView : ContentView {
    static void M() {
      var view = new global::SkiaSharp.Views.Forms.SKCanvasView { HeightRequest = 225 };
      var keyboard = new CSharpMath.Rendering.MathKeyboard();
      keyboard.BindDisplay(view, new CSharpMath.SkiaSharp.MathPainter {
        TextColor = SKColors.Black
      }, new SKColor(0, 0, 0, 153)); // BindDisplay automatically enables touch!

      keyboard.KeyPress(CSharpMath.Editor.MathKeyboardInput.X); // Wire input like this
    }
    public EditorView() {
      // Basic functionality
      var view = new SKCanvasView { HeightRequest = 225 };
      var keyboard = new MathKeyboard();
      var viewModel = keyboard.ViewModel;
      viewModel.BindDisplay(view, new SkiaSharp.MathPainter {
        TextColor = SKColors.Black
      }, new SKColor(0, 0, 0, 153));

      // Input from physical keyboard
      var entry = new Entry { Placeholder = "Enter keystrokes..." };
      entry.TextChanged += (sender, e) => {
          entry.Text = "";
          foreach (var c in e.NewTextValue)
            // The (int) extra conversion seems to be required by Android or a crash occurs
            viewModel.KeyPress((Editor.MathKeyboardInput)(int)c);
      };

      // Debug labels
      var latex = new Label { Text = "LaTeX = " };
      var ranges = new Label { Text = "Ranges = " };
      var index = new Label { Text = "Index = " };
      viewModel.RedrawRequested += (sender, e) => {
        latex.Text = "LaTeX = " + viewModel.LaTeX;
        ranges.Text = "Ranges = " + string.Join(", ", ((ListDisplay<Fonts, Glyph>)viewModel.Display).Displays.Select(x => x.Range));
        index.Text = "Index = " + viewModel.InsertionIndex;
      };

      // Assemble
      Content = new StackLayout { Children = { latex, ranges, index, view, keyboard, entry } };
    }
  }
}