using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UserEvidence.BaseClasses
{
    public class User
    {
        private string username;
        private string password;

        // A private constructor which sets the username and password
        private User(string username, string password)
        { 
            this.username = username;
            this.password = password;
        }

        [JsonConstructorAttribute]
        public User()
        {

        }

        // A factory method for creating a new User
        public static User Create(string username, string password) 
        {
            if(!IsUsernameValid(username)) 
            {
                throw new ArgumentException("The username is invalid.", username);
            }
            if (!IsPasswordValid(password))
            {
                throw new ArgumentException("The password is invalid.", password);
            }
            return new User(username, password);
        }

        public string Password 
        { 
            set 
            { 
                // Updates the password if the provided string is valid, otherwise an exception is thrown.
                if(IsPasswordValid(value)) 
                { 
                    password = value;
                } else
                {
                    throw new ArgumentException("The new password provided is not valid.", value);
                }
            }
            get => password;
        }

        // Checks if the password is equal to another string
        public bool ValidatePassword(string password) 
        {
            return this.password == password;
        }

        public string Username 
        { 
            get => username;
            set
            {
                // Updates the username if the provided string is valid, otherwise an exception is thrown.
                if (IsUsernameValid(value))
                {
                    username = value;
                } else
                {
                    throw new ArgumentException("The new username provided is not valid.", value);
                }
            }
        }

        public static bool IsPasswordValid(string? password)
        {
            if (password == null)
            {
                return false;
            }
            // Required pattern for the password (at least one lowercase and uppercase letter, one number and one non-alphanumeric character and at least 8+ characters)
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[a-zA-Z\d\W]{8,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(password);
        }

        public static bool IsUsernameValid(string? username) 
        {
            if (username == null)
            {
                return false;
            }
            // Required pattern for the username
            string pattern = @"^[a-zA-Z0-9]{5,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(username);
        }

        // The Equals bool is calculated based on the user's username
        public override bool Equals(object? obj)
        {
            return obj is User user && username == user.username;
        }

        // The hash code is created from the user's username
        public override int GetHashCode()
        {
            return HashCode.Combine(username);
        }
    }
}
