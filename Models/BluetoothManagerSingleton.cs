using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConduitDEVAPP.Models
{
    public sealed class BluetoothManagerSingleton
    {

        #region Singleton Declaration
        private static readonly BluetoothManagerSingleton _instance = new BluetoothManagerSingleton();
        // Private constructor as this is a singleton
        private BluetoothManagerSingleton()
        {

        }
        public static BluetoothManagerSingleton Instance
        {
            get { return _instance; }
        }
        #endregion

        private string savedname = "Max"; // Saved device name from settings.

    }
}
