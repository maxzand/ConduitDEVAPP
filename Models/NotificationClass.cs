using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConduitDEVAPP.Models
{
    internal class Notification
    {
        private int eventID;
        private int categoryID;
        private int notificationUID;
        private string notificationText;

        Notification(int eventID, int categoryID, int notificationUID)
        {
            this.eventID = eventID;
            this.categoryID = categoryID;
            this.notificationUID = notificationUID
            notificationText = "";

            

        }





    }
}