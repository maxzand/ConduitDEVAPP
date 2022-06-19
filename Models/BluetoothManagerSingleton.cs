using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace ConduitDEVAPP.Models
{
    public sealed class BluetoothManagerSingleton
    {
        private BluetoothLEDevice device; // Bluetooth Devices
        private GattDeviceService ANCS; // ANCS
        private GattCharacteristic NS; // Notification source
        // private List<NotificationClass> NotificationList; // List of notifications

        #region Singleton Declaration
        private static readonly BluetoothManagerSingleton _instance = new BluetoothManagerSingleton();
        // Private constructor as this is a singleton
        private BluetoothManagerSingleton()
        {
            device = null;
            StartBleDeviceWatcher();
        }
        public static BluetoothManagerSingleton Instance
        {
            get { return _instance; }
        }
        #endregion

        private string savedname = "Maxime"; // Saved device name from settings.

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
            Debug.WriteLine("Starting watcher!");

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
                    InitiateConnection(args);
                    Debug.WriteLine("Connecting to device!");
                }
            }
        }

        private async void InitiateConnection(DeviceInformation args)
        {
            StopBleDeviceWatcher();

            try
            {
                // BT_Code: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                device = await BluetoothLEDevice.FromIdAsync(args.Id);

                if (device == null)
                {
                    // e
                }
            }
            catch
            {
                // fuck
            }

            if (device != null)
            {
                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                GattDeviceServicesResult result = await device.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    Debug.WriteLine("Found services!");
                    var services = result.Services;
                    foreach (var service in services)
                    {
                        if (service.Uuid.ToString() == "7905F431-B5CE-4E99-A40F-4B1E122D00D0".ToLower())
                        {
                            ANCS = service;
                            try
                            {
                                // Ensure we have access to the device.
                                var accessStatus = await service.RequestAccessAsync();
                                if (accessStatus == DeviceAccessStatus.Allowed)
                                {
                                    // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                                    // and the new Async functions to get the characteristics of unpaired devices as well. 
                                    var characterresult = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                                    if (result.Status == GattCommunicationStatus.Success)
                                    {
                                        Debug.WriteLine("Found characteristics!");
                                        var characteristics = characterresult.Characteristics;
                                        foreach (var characteristic in characteristics)
                                        {


                                            switch (characteristic.Uuid.ToString().ToUpper())
                                            {
                                                case "9FBF120D-6301-42D9-8C58-25E699A21DBD":
                                                    // Notification Source
                                                    Debug.WriteLine("Found Notification Source!");
                                                    NS = characteristic;
                                                    SubscribeToCharacteristic(NS);
                                                    break;
                                                case "69D1D8F3-45E1-49A8-9821-9BBDFDAAD9D9":
                                                    // Control Point
                                                    break;
                                                case "22EAC6E9-24D6-4BB5-BE44-B36ACE7C7BFB":
                                                    // Data Source
                                                    break;


                                            }
                                        }
                                    }
                                    else
                                    {
                                        // On error, act as if there are no characteristics.
                                        var characteristics = new List<GattCharacteristic>();
                                    }
                                }
                                else
                                {
                                    // Not granted access
                                    // On error, act as if there are no characteristics.
                                    var characteristics = new List<GattCharacteristic>();

                                }
                            }
                            catch (Exception ex)
                            {
                                // On error, act as if there are no characteristics.
                                var characteristics = new List<GattCharacteristic>();
                            }

                        }
                    }

                }
            }
            #endregion
        }

        private async void SubscribeToCharacteristic(GattCharacteristic characteristic)
        {
            // initialize status
            Debug.WriteLine("Attempting to subscribe");
            GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
            var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
            {
                cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            }

            else if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
            }

            try
            {
                // BT_Code: Must write the CCCD in order for server to send indications.
                // We receive them in the ValueChanged event handler.
                status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                if (status == GattCommunicationStatus.Success)
                {
                    AddValueChangedHandler(characteristic);
                }
                else
                {
                    // Could not register for value changes.
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // This usually happens when a device reports that it support indicate, but it actually doesn't.
            }
        }

        private void AddValueChangedHandler(GattCharacteristic characteristic)
        {
            if (characteristic.Uuid == NS.Uuid) 
            { 
                characteristic.ValueChanged += NS_ValueChanged;
                Debug.WriteLine("Registered event handlers");
            }
        }












        #region Event Handlers
        private void NS_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);

            var eventID = reader.ReadByte();
            var eventFlags = reader.ReadByte();
            var categoryID = reader.ReadByte();
            var categoryCount = reader.ReadByte();

            var notificationUID = reader.ReadInt32();

            Debug.WriteLine($"Value at {DateTime.Now:hh:mm:ss.FFF}: EventID:{eventID} EventFlags:{eventFlags} CategoryID:{categoryID} CategoryCount:{categoryCount} NUID:{notificationUID}");

        }
        #endregion
    }
}


