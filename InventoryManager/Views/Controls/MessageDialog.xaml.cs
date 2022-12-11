using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InventoryManager.Views.Controls
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : ContentControl
    {
        public MessageDialog()
        {
            InitializeComponent();
        }


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MessageDialog), new PropertyMetadata(new CornerRadius(5)));


        public double DialogHeight
        {
            get { return (double)GetValue(DialogHeightProperty); }
            set { SetValue(DialogHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogHeightProperty =
            DependencyProperty.Register("DialogHeight", typeof(double), typeof(MessageDialog), new PropertyMetadata(100.0));


        public double DialogWidth
        {
            get { return (double)GetValue(DialogWidthProperty); }
            set { SetValue(DialogWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register("DialogWidth", typeof(double), typeof(MessageDialog), new PropertyMetadata(200.0));



        public Visibility IsTitleVisible
        {
            get { return (Visibility)GetValue(IsTitleVisibleProperty); }
            set { SetValue(IsTitleVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTitleVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTitleVisibleProperty =
            DependencyProperty.Register("IsTitleVisible", typeof(Visibility), typeof(MessageDialog), new PropertyMetadata(Visibility.Visible));


        public Thickness BorderStrokeThickness
        {
            get { return (Thickness)GetValue(BorderStrokeThicknessProperty); }
            set { SetValue(BorderStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderStrokeThicknessProperty =
            DependencyProperty.Register("BorderStrokeThickness", typeof(Thickness), typeof(MessageDialog), new PropertyMetadata(new Thickness(1)));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MessageDialog), new PropertyMetadata("Dialog"));


    }
}
