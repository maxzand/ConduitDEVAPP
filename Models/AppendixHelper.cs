﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConduitDEVAPP.Models
{
    public sealed class AppendixHelper
    {
        public string[] categoryIDHelper = new string[] { "CategoryIDOther", "CategoryIDIncomingCall", "CategoryIDMissedCall", "CategoryIDVoicemail", "CategoryIDSocial", "CategoryIDSchedule", "CategoryIDEmail", "CategoryIDNews", "CategoryIDHealthAndFitness", "CategoryIDBusinessAndFinance", "CategoryIDLocation", "CategoryIDEntertainment", "ReservedCategoryIDValues" };
        public string[] eventIDHelper = new string[] { "EventIDNotificationAdded", "EventIDNotificationModified", "EventIDNotificationRemoved", "ReservedEventIDValues" };

        public string[] Title;
        public string[] message;
        public string[] positiveAction;
        public string[] negativeAction;
        
    }
    

}
