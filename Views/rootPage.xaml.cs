using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ConduitDEVAPP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class rootPage : Page
    {
        public rootPage()
        {
            this.InitializeComponent();

            // Navigate to corresponding frames.
            topleftFrame.Navigate(typeof(ConnectionView));
            toprightFrame.Navigate(typeof(ConnectionView));
            bottomrightFrame.Navigate(typeof(ConnectionView));
            bottomleftFrame.Navigate(typeof(ConnectionView));

            // Initialize ViewModel
            //ViewModel = new ViewModels.rootPageViewModel();
            //this.DataContext = ViewModel;
            

        }
        //public ViewModels.rootPageViewModel ViewModel { get; set; }
    }
}
