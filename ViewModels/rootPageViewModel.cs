using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConduitDEVAPP.ViewModels
{
    public partial class RootPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private int fullScreenValue;

        public RootPageViewModel()
        {
            FullScreenValue = 1;
        }

        [ObservableProperty]
        private string topLeftVisibility;

        [ObservableProperty]
        private string topRightVisibility;

        [ObservableProperty]
        private string bottomLeftVisibility;

        [ObservableProperty]
        private string bottomRightVisibility;

        [ObservableProperty]
        private string topLeftButtonVisibility;

        [ObservableProperty]
        private string topRightButtonVisibility;

        [ObservableProperty]
        private string bottomLeftButtonVisibility;

        [ObservableProperty]
        private string bottomRightButtonVisibility;
















        [ICommand]
        void TopLeftFullScreen()
        {
            TopRightVisibility = "Collapsed";
            BottomRightVisibility = "Collapsed";
            BottomLeftVisibility = "Collapsed";
            TopRightButtonVisibility = "Collapsed";
            BottomRightButtonVisibility = "Collapsed";
            BottomLeftButtonVisibility = "Collapsed";
            TopLeftButtonVisibility = "Collapsed";
            FullScreenValue = 2;
        }

        [ICommand]
        void ExitFullScreen()
        {

        }








    }

}
