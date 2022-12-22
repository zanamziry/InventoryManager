using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace InventoryManager.Views.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:InventoryManager.Views"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:InventoryManager.Views;assembly=InventoryManager.Views"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    [ContentProperty("Content")]

    public class ZMDialog : Control
    {

        static ZMDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZMDialog), new FrameworkPropertyMetadata(typeof(ZMDialog)));
        }


        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ZMDialog), new PropertyMetadata(null));


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ZMDialog), new PropertyMetadata(new CornerRadius(5)));


        public double DialogHeight
        {
            get { return (double)GetValue(DialogHeightProperty); }
            set { SetValue(DialogHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogHeightProperty =
            DependencyProperty.Register("DialogHeight", typeof(double), typeof(ZMDialog), new PropertyMetadata(100.0));


        public double DialogWidth
        {
            get { return (double)GetValue(DialogWidthProperty); }
            set { SetValue(DialogWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register("DialogWidth", typeof(double), typeof(ZMDialog), new PropertyMetadata(200.0));



        public Visibility IsTitleVisible
        {
            get { return (Visibility)GetValue(IsTitleVisibleProperty); }
            set { SetValue(IsTitleVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTitleVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTitleVisibleProperty =
            DependencyProperty.Register("IsTitleVisible", typeof(Visibility), typeof(ZMDialog), new PropertyMetadata(Visibility.Visible));


        public Thickness BorderStrokeThickness
        {
            get { return (Thickness)GetValue(BorderStrokeThicknessProperty); }
            set { SetValue(BorderStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderStrokeThicknessProperty =
            DependencyProperty.Register("BorderStrokeThickness", typeof(Thickness), typeof(ZMDialog), new PropertyMetadata(new Thickness(1)));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ZMDialog), new PropertyMetadata("Dialog"));


    }
}
