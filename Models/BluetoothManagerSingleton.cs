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
using ConduitDEVAPP.Models;
using System.Collections;
using ConduitDEVAPP.ViewModels;
using System.Collections.ObjectModel;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Media.Audio;
using Windows.ApplicationModel.Calls;

namespace ConduitDEVAPP.Models
{
    public sealed class BluetoothManagerSingleton
    {
        private BluetoothLEDevice device; // Bluetooth Devices
        private GattDeviceService ANCS; // ANCS
        private GattCharacteristic NS; // Notification source
        private GattCharacteristic ControlPoint; // Control point.
        private GattCharacteristic DataSource; // Data source.
        Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        // private List<NotificationClass> NotificationList; // List of notifications

        #region Singleton Declaration
        private static readonly BluetoothManagerSingleton _instance = new BluetoothManagerSingleton();
        // Private constructor as this is a singleton
        private BluetoothManagerSingleton()
        {
            device = null;
            //StartBleDeviceWatcher();
            StartRFCOMMAsync();
        }

        public static BluetoothManagerSingleton Instance
        {
            get { return _instance; }
        }
        #endregion

        public ObservableCollection<Notification> notificationCollection = new ObservableCollection<Notification>();
        private string savedname = "Maxime"; // Saved device name from settings.
        private string foundID;

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
            Debug.WriteLine($"Device removed: {args.Id}");
        }
            
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (sender == deviceWatcher)
            {
                try
                {
                    if ((bool?)args.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"] == true && args.Id == foundID)
                    {
                        InitiateConnection(args.Id);
                        Debug.WriteLine($"Connecting to {args.Id}");
                    }
                }
                catch
                {
                    // Index does not exist (IsConnectable did not update)
                }
            }
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (sender == deviceWatcher)
            {
                if (args.Name == savedname && args.Pairing.IsPaired == true && (bool?)args.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"] == true)
                {
                    foundID = args.Id;
                    InitiateConnection(args.Id);
                    Debug.WriteLine($"Connecting to device! {args.Id}");
                    Debug.WriteLine((bool?)args.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"] == true);
                }
            }
        }

        private async void InitiateConnection(string ID)
        {
            StopBleDeviceWatcher();

            try
            {
                // BT_Code: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                device = await BluetoothLEDevice.FromIdAsync(ID);

                if (device == null)
                {
                    // e
                }
            }
            catch
            {
                // 
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
                                                    ControlPoint = characteristic;
                                                    Debug.WriteLine("Found control point.");
                                                    break;
                                                    
                                                case "22EAC6E9-24D6-4BB5-BE44-B36ACE7C7BFB":
                                                    // Data Source
                                                    DataSource = characteristic;
                                                    SubscribeToCharacteristic(DataSource);
                                                    Debug.WriteLine("Found data source.");
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

        #region ANCS Interactions

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
            if (characteristic.Uuid == DataSource.Uuid)
            {
                Debug.WriteLine("Registered data source");
                characteristic.ValueChanged += DataSource_ValueChanged;
            }
        }


        public async void getNotificationAttributes(sbyte CommandID, UInt32 NotificationUID)
        {

            var writer = new Windows.Storage.Streams.DataWriter();
            writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
            writer.WriteByte((byte)CommandID);
            writer.WriteUInt32(NotificationUID);
            writer.WriteByte((byte)0);
            writer.WriteByte((byte)1);
            writer.WriteUInt16(99);
            writer.WriteByte((byte)3);
            writer.WriteUInt16(99);
            writer.WriteByte((byte)6);
            writer.WriteByte((byte)7);

            /*
            for (sbyte i = 1; i < 2; i++)
            {
                writer.WriteByte();
            }
            */
            var message = writer.DetachBuffer();

            //var reader = DataReader.FromBuffer(message);
            //var output = reader.ReadString(message.Length);

            var writeSuccess = await WriteBufferToControl(message);


        }

        private async void PerformNotificationAction(sbyte CommandID, uint NotificationUID, sbyte ActionID)
        {
            var writer = new Windows.Storage.Streams.DataWriter();
            writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
            writer.WriteByte((byte)CommandID);
            writer.WriteUInt32(NotificationUID);
            writer.WriteByte((byte)ActionID);

            var message = writer.DetachBuffer();

            var writeSuccess = await WriteBufferToControl(message);
        }

        private async Task<bool> WriteBufferToControl(IBuffer message)
        {
            var result = await ControlPoint.WriteValueWithResultAsync(message);

            if (result.Status == GattCommunicationStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        #endregion

        #region Event Handlers
        // Event run when a notification is receieved.
        private void NS_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

            var output = reader.ReadString(args.CharacteristicValue.Length);

            reader = DataReader.FromBuffer(args.CharacteristicValue);
            reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
            var eventID = reader.ReadByte();
            var eventFlags = reader.ReadByte();
            var categoryID = reader.ReadByte();
            var categoryCount = reader.ReadByte();

            var notificationUID = reader.ReadUInt32();

            

            Debug.WriteLine($"Value at {DateTime.Now:hh:mm:ss.FFF}: EventID:{eventID} EventFlags:{eventFlags} CategoryID:{categoryID} CategoryCount:{categoryCount} NUID:{notificationUID}");

            var VMinstance = new NotificationViewModel();
            if (eventID == 0)
            {
                getNotificationAttributes(0, notificationUID);
            }
            if (eventID == 2)
            {
                dispatcherQueue.TryEnqueue(() =>
                {
                    notificationCollection.Remove(notificationCollection.Where(i => (UInt32)i.NotificationUID == (UInt32)notificationUID).FirstOrDefault());
                }
);
            }

            

        }

        private void DataSource_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var ibufferlength = args.CharacteristicValue.Length;
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            var readerlength = reader.UnconsumedBufferLength;

            var output = reader.ReadString(ibufferlength);

            reader = DataReader.FromBuffer(args.CharacteristicValue);
            reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

            var CommandID = reader.ReadByte();
            var notificationUID = reader.ReadUInt32(); 
            var attributeID1 = reader.ReadByte();
            var attribute1length = reader.ReadUInt16();
            var attribute1 = reader.ReadString(attribute1length);
            var attributeID2 = reader.ReadByte();
            var attribute2length = reader.ReadUInt16();
            var attribute2 = reader.ReadString(attribute2length);
            var attributeID3 = reader.ReadByte();
            var attribute3length = reader.ReadUInt16();
            var attribute3 = reader.ReadString(attribute3length);
            var attributeID4 = reader.ReadByte();
            var attribute4length = reader.ReadUInt16();
            var attribute4 = reader.ReadString(attribute4length);
            var attributeID5 = reader.ReadByte();
            var attribute5length = reader.ReadUInt16();
            var attribute5 = reader.ReadString(attribute5length);
            
            if (attribute3 == "Incoming Call")
            {
                PerformNotificationAction((sbyte)2, notificationUID, (sbyte)0);
            }
            

            //Debug.WriteLine($"Value at {DateTime.Now:hh:mm:ss.FFF}: CommandID:{CommandID} notificationUID:{notificationUID} attributeID1:{attributeID1} attribute1length:{attribute1length} attribute1:{attribute1} Title:{attribute2} message:{attribute3}");

            dispatcherQueue.TryEnqueue(() =>
            {
                notificationCollection.Insert(0,new Notification((string)attribute2, (string)attribute3, (string)attribute4, (string)attribute5, (UInt32)notificationUID));
            }
            );

        }


        #endregion

        #region RFCOMM
        private async Task StartRFCOMMAsync()
        {
            // Enumerate devices with the object push service
            var rfcommDevice =
    await DeviceInformation.FindAllAsync(PhoneLineTransportDevice.GetDeviceSelector());
            var connection = PhoneLineTransportDevice.FromId(rfcommDevice[0].Id);
            
            if (connection != null)
            {
                var result = await connection.RequestAccessAsync();
                Debug.WriteLine(result);
                if (result == DeviceAccessStatus.Allowed)
                {
                    Debug.WriteLine("Allowed!");
                    bool connected = await connection.ConnectAsync();
                    Debug.WriteLine(connected);
                    Debug.WriteLine("Connection!");
                    connection.RegisterApp();
                    
                }

            }
            else
            {
                Debug.WriteLine("WRONG");
            }






            /*
            var rfcommDevice =
                await DeviceInformation.FindAllAsync(AudioPlaybackConnection.GetDeviceSelector());
            var hedevice = await BluetoothDevice.FromIdAsync(rfcommDevice[0].Id);
            var connection = AudioPlaybackConnection.TryCreateFromId(rfcommDevice[0].Id);
            if (connection != null)
            {
                await connection.StartAsync();
                var result = connection.OpenAsync();
                Debug.WriteLine(result.Status);
            }
            */






            /*
            var hedevice = await BluetoothDevice.FromIdAsync(rfcommDevice[0].Id);
            var result = await hedevice.GetRfcommServicesAsync();

            if (result.Services.Count > 0)
            {
                var services = result.Services;

                var service = services.Where(i => i.ServiceId.Uuid.ToString().ToLower() == "0000111f-0000-1000-8000-00805f9b34fb".ToLower()).Single();
                var _socket = new StreamSocket();
                await _socket.ConnectAsync(
                    service.ConnectionHostName,
                    service.ConnectionServiceName,
                    SocketProtectionLevel
                        .BluetoothEncryptionWithAuthentication);

                
                _ = await _socket.InputStream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.None);
                
                DataReader reader = DataReader.FromBuffer(buffer);
                var text = reader.ReadString(buffer.Length);

                Debug.WriteLine(_socket.OutputStream);
                Debug.WriteLine(_socket.InputStream);
            */
        }


            
        }




        #endregion
    }


