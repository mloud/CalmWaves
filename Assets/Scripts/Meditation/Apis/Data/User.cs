using System;
using OneDay.Core.Modules.Data;

namespace Meditation.Apis.Data
{
    public class User : BaseDataObject
    {
        // initial user
        public User()
        {
            NickName = "";
            
            //streak;
            Streak = 0;
            LastFinishedDay = DateTime.MinValue;
        }
        
        public string NickName;
        public int Streak;
        public DateTime LastFinishedDay;
    }
}