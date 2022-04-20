using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Dispatch.WPF.Controls;
/// <summary>
/// 
/// </summary>
/// <remarks>
/// Credit goes to this StackOverflow answer: https://stackoverflow.com/a/6782715/13385161
/// </remarks>
public class ZoomBorder : Border
{
    private UIElement? _child = null;
    private Point _origin;
    private Point _start;

    public double CurrentZoomX { get; set; }
    public double CurrentZoomY { get; set; }

    private TranslateTransform GetTranslateTransform(UIElement element)
    {
        return (TranslateTransform)((TransformGroup)element.RenderTransform)
          .Children.First(tr => tr is TranslateTransform);
    }

    private ScaleTransform GetScaleTransform(UIElement element)
    {
        return (ScaleTransform)((TransformGroup)element.RenderTransform)
          .Children.First(tr => tr is ScaleTransform);
    }

    public override UIElement? Child
    {
        get => base.Child;
        set
        {
            if (value != null && value != Child)
                Initialize(value);
            base.Child = value;
        }
    }

    public void Initialize(UIElement element)
    {
        _child = element;
        if (_child == null) return;
        var group = new TransformGroup();
        var st = new ScaleTransform();
        @group.Children.Add(st);
        var tt = new TranslateTransform();
        @group.Children.Add(tt);
        _child.RenderTransform = @group;
        _child.RenderTransformOrigin = new Point(0.0, 0.0);
        MouseWheel += Child_MouseWheel;
        MouseLeftButtonDown += Child_MouseLeftButtonDown;
        MouseLeftButtonUp += Child_MouseLeftButtonUp;
        MouseMove += Child_MouseMove;
        PreviewMouseRightButtonDown += Child_PreviewMouseRightButtonDown;
    }

    public void Reset()
    {
        if (_child == null) return;
        // reset zoom
        var st = GetScaleTransform(_child);
        CurrentZoomX = st.ScaleX = 1.0;
        CurrentZoomY = st.ScaleY = 1.0;

        // reset pan
        var tt = GetTranslateTransform(_child);
        tt.X = 0.0;
        tt.Y = 0.0;
    }

    #region Child Events

    private void Child_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (_child == null) return;
        var st = GetScaleTransform(_child);
        var tt = GetTranslateTransform(_child);

        var zoom = e.Delta > 0 ? .2 : -.2;
        if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
            return;

        var relative = e.GetPosition(_child);

        var absoluteX = relative.X * st.ScaleX + tt.X;
        var absoluteY = relative.Y * st.ScaleY + tt.Y;

        st.ScaleX += zoom;
        st.ScaleY += zoom;

        tt.X = absoluteX - relative.X * st.ScaleX;
        tt.Y = absoluteY - relative.Y * st.ScaleY;

        CurrentZoomX = st.ScaleX;
        CurrentZoomY = st.ScaleY;
    }

    private void Child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_child == null) return;
        var tt = GetTranslateTransform(_child);
        _start = e.GetPosition(this);
        _origin = new Point(tt.X, tt.Y);
        Cursor = Cursors.Hand;
        _child.CaptureMouse();
    }

    private void Child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_child == null) return;
        _child.ReleaseMouseCapture();
        Cursor = Cursors.Arrow;
    }

    void Child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        // I do not want the right click to reset the pan/zoom in this project
        //this.Reset();
    }

    private void Child_MouseMove(object sender, MouseEventArgs e)
    {
        if (_child is not { IsMouseCaptured: true }) return;
        var tt = GetTranslateTransform(_child);
        var v = _start - e.GetPosition(this);
        tt.X = _origin.X - v.X;
        tt.Y = _origin.Y - v.Y;
    }

    #endregion
}
