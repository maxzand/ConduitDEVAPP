using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConduitDEVAPP.Models;

namespace ConduitDEVAPP.ViewModels
{
    public sealed partial class NotificationViewModel : ObservableObject
    {
        readonly BluetoothManagerSingleton Instance = BluetoothManagerSingleton.Instance;
        public ObservableCollection<Notification> NotificationCollection { get { return Instance.notificationCollection; } }

        public NotificationViewModel() {

        }

        public void AddNotificationToList(Notification NewNotification)
        {
            NotificationCollection.Add(NewNotification);
        }

        public void RemoveNotificationFromList(UInt32 notificationUID)   //
        {
            NotificationCollection.Remove(NotificationCollection.Where(i => i.NotificationUID == notificationUID).FirstOrDefault());
        }

    }
}
