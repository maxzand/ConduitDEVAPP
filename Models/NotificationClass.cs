using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConduitDEVAPP.Models
{
    public sealed class Notification
    {
        public int CategoryID;
        public UInt32 NotificationUID;
        public string Title { get; set; }
        public string Message { get; set; }
        public string PositiveAction;
        public string NegativeAction;


        public Notification(string title, string message, string positiveAction, string negativeAction, UInt32 notificationUID)
        {
            //this.CategoryID = categoryID;
            //this.NotificationUID = notificationUID;
            this.Title = title;
            this.Message = message;
            this.PositiveAction = positiveAction;
            this.NegativeAction = negativeAction;
            this.NotificationUID = notificationUID;
        }


        

    }
 }       




    
