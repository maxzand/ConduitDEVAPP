using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace ConduitDEVAPP.Models
{
    public sealed class BluetoothManagerSingleton
    {
        private BluetoothDevice device; // Bluetooth Device

        #region Singleton Declaration
        private static readonly BluetoothManagerSingleton _instance = new BluetoothManagerSingleton();
        // Private constructor as this is a singleton
        private BluetoothManagerSingleton()
        {
            device = null;
        }
        public static BluetoothManagerSingleton Instance
        {
            get { return _instance; }
        }
        #endregion

        private string savedname = "Max"; // Saved device name from settings.

        #region Connection Establishment
        private DeviceWatcher deviceWatcher;

        private void StartBleDeviceWatcher()
        {
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

            // BT_Code: Example showing paired and non-paired in a single query.
            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
            string aqsPairedBluetoothLEDevices = BluetoothLEDevice.GetDeviceSelectorFromPairingState(true);

            deviceWatcher =
                    DeviceInformation.CreateWatcher(
                        aqsAllBluetoothLEDevices,
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;


            deviceWatcher.Start();


        }

        private void StopBleDeviceWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            // TO-DO
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // TO-DO
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // TO-DO
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // TO-DO
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (sender == deviceWatcher)
            {
                if (args.Name == savedname && args.Pairing.IsPaired == true)
                {
                    InitiateConnection();
                    Debug.WriteLine("Connecting to device!");
                }
            }
        }

        private void InitiateConnection()
        {
            // TO-DO
        }
        #endregion
    }
}


