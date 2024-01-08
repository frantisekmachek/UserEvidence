using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UserEvidence.BaseClasses;

namespace UserEvidence.EvidenceClasses
{
    // 'sealed' means that no classes can inherit this class
    public sealed class UserList : HashSet<User>
    {
        [JsonConstructorAttribute]
        public UserList()
        {
        }
        // Returns true if successful, false if not.
        public bool Login(User user, string? password)
        {
            if(password == null)
            {
                return false;
            }

            if (!Contains(user))
            {
                throw new ArgumentException("User not found.");
            }

            return user.ValidatePassword(password);
        }

        // Finds a user with a given username
        public User? FindUser(string? username) 
        {

            if(Count == 0 || username == null)
            {
                return null;
            }

            User? user = this.FirstOrDefault(user => user.Username == username);
            return user;
        }
    }
}
