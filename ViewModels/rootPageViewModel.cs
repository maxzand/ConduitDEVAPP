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


        public RootPageViewModel()
        {
            ExitFullScreenButtonVisibility = "Collapsed";
            TopRightRow = TopRightOriginal[0]; TopRightColumn = TopRightOriginal[1];
            BottomRightRow = BottomRightOriginal[0]; BottomRightColumn = BottomRightOriginal[1];
            BottomLeftRow = BottomLeftOriginal[0]; BottomLeftColumn = BottomLeftOriginal[1];

        }

        /// <summary>
        /// All the observable properties which will be used to update the UI
        /// </summary>
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

        [ObservableProperty]
        private string exitFullScreenButtonVisibility;

        [ObservableProperty]
        private int topLeftFullScreenValue;

        [ObservableProperty]
        private int topRightFullScreenValue;

        [ObservableProperty]
        private int bottomLeftFullScreenValue;

        [ObservableProperty]
        private int bottomRightFullScreenValue;

        /// <summary>
        /// Constants declared to represent each cell's original position
        /// </summary>
        private static readonly int[] TopRightOriginal = { 0, 1 };
        private static readonly int[] BottomLeftOriginal = { 1, 0 };
        private static readonly int[] BottomRightOriginal = { 1, 1 };

        [ObservableProperty]
        private int topRightRow;

        [ObservableProperty]
        private int topRightColumn;

        [ObservableProperty]
        private int bottomRightRow;

        [ObservableProperty]
        private int bottomRightColumn;

        [ObservableProperty]
        private int bottomLeftRow;

        [ObservableProperty]
        private int bottomLeftColumn;

        [ICommand]
        void TopLeftFullScreen()
        {
            // Disable other Views
            TopRightVisibility = "Collapsed";
            BottomRightVisibility = "Collapsed";
            BottomLeftVisibility = "Collapsed";

            // Disable their buttons
            TopRightButtonVisibility = "Collapsed";
            BottomRightButtonVisibility = "Collapsed";
            BottomLeftButtonVisibility = "Collapsed";
            TopLeftButtonVisibility = "Collapsed";

            // Make selected View fullscreen
            TopLeftFullScreenValue = 2;
            ExitFullScreenButtonVisibility = "Visible";
            Debug.WriteLine(TopRightOriginal[1]);
        }

        [ICommand]
        void TopRightFullScreen()
        {
            // Disable other Views
            TopLeftVisibility = "Collapsed";
            BottomRightVisibility = "Collapsed";
            BottomLeftVisibility = "Collapsed";

            // Disable their buttons
            TopLeftButtonVisibility = "Collapsed";
            BottomRightButtonVisibility = "Collapsed";
            BottomLeftButtonVisibility = "Collapsed";
            TopRightButtonVisibility = "Collapsed";

            // Make selected View fullscreen
            TopRightFullScreenValue = 2;
            ExitFullScreenButtonVisibility = "Visible";

            // Move the cell to the top left
            TopRightRow = 0; TopRightColumn = 0;
        }

        [ICommand]
        void BottomLeftFullScreen()
        {
            // Disable other Views
            TopLeftVisibility = "Collapsed";
            BottomRightVisibility = "Collapsed";
            TopRightVisibility = "Collapsed";

            // Disable their buttons
            TopLeftButtonVisibility = "Collapsed";
            BottomRightButtonVisibility = "Collapsed";
            BottomLeftButtonVisibility = "Collapsed";
            TopRightButtonVisibility = "Collapsed";

            // Make selected View fullscreen
            BottomLeftFullScreenValue = 2;
            ExitFullScreenButtonVisibility = "Visible";

            // Move the cell to the top left
            BottomLeftRow = 0; BottomLeftColumn = 0;
        }

        [ICommand]
        void BottomRightFullScreen()
        {
            // Disable other Views
            TopLeftVisibility = "Collapsed";
            BottomLeftVisibility = "Collapsed";
            TopRightVisibility = "Collapsed";

            // Disable their buttons
            TopLeftButtonVisibility = "Collapsed";
            BottomRightButtonVisibility = "Collapsed";
            BottomLeftButtonVisibility = "Collapsed";
            TopRightButtonVisibility = "Collapsed";

            // Make selected View fullscreen
            BottomRightFullScreenValue = 2;
            ExitFullScreenButtonVisibility = "Visible";

            // Move the cell to the top left
            BottomRightRow = 0; BottomRightColumn = 0;
        }



        [ICommand]
        void ExitFullScreen()
        {

            // Disable other Views
            TopLeftVisibility = "Visible";
            TopRightVisibility = "Visible";
            BottomRightVisibility = "Visible";
            BottomLeftVisibility = "Visible";

            // Disable their buttons
            TopLeftButtonVisibility = "Visible";
            TopRightButtonVisibility = "Visible";
            BottomRightButtonVisibility = "Visible";
            BottomLeftButtonVisibility = "Visible";
            TopLeftButtonVisibility = "Visible";

            // Make selected View fullscreen
            TopLeftFullScreenValue = 1;
            TopRightFullScreenValue = 1;
            BottomLeftFullScreenValue = 1;
            BottomRightFullScreenValue = 1;
            ExitFullScreenButtonVisibility = "Collapsed";

            // Set cells back to their initial position
            TopRightRow = TopRightOriginal[0]; TopRightColumn = TopRightOriginal[1];
            BottomRightRow = BottomRightOriginal[0]; BottomRightColumn = BottomRightOriginal[1];
            BottomLeftRow = BottomLeftOriginal[0]; BottomLeftColumn = BottomLeftOriginal[1];

        }








    }

}
