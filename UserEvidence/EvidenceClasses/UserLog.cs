using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserEvidence.BaseClasses;

namespace UserEvidence.EvidenceClasses
{
    public class UserLog
    {
        private string username;
        private DateTime date;

        // Creates a new log for a user at a given time (DateTime.Now)
        public UserLog(User user) 
        {
            username = user.Username;
            date = DateTime.Now;

            LogToFile();
        }

        // Write the log to the log file
        private void LogToFile() 
        {
            string? destination = ConfigurationManager.AppSettings["LogPath"];

            if (destination == null)
            {
                throw new Exception("Log file path not found.");
            }

            using(StreamWriter writer = new StreamWriter(destination, true))
            {
                writer.WriteLine(username + ": " + date.ToString());
            }
        }
    }
}
