using System;
using System.Configuration;
using System.Security;
using System.Text.Json;
using UserEvidence.BaseClasses;
using UserEvidence.EvidenceClasses;

namespace UserEvidence
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? userFilePath = ConfigurationManager.AppSettings["UserFilePath"];
            Console.WriteLine("Users are loaded from this file: " + userFilePath);

            if (userFilePath != null) 
            {
                UserList? users;

                // Reads the UserList from the file. If the file is not found, a new empty UserList is created.
                if (File.Exists(userFilePath))
                {
                    string usersJson = File.ReadAllText(userFilePath);
                    // Load the UserList of users from the json file
                    users = JsonSerializer.Deserialize<UserList>(usersJson);
                    Console.WriteLine("User file was found and successfully loaded.");
                } else
                {
                    users = new();
                    Console.WriteLine("User file was not found. A new UserList has been created.");
                }

                // Create a new UserList if it wasn't found
                if (users == null)
                {
                    users = new();
                }

                User? user;

                // If there are no users, instantly go for the registration
                if (users.Count == 0)
                {
                    user = PromptRegistration(users);
                    if(user != null)
                    {
                        Login(user);
                    }
                } else // Otherwise ask for login/registration
                {
                    Console.WriteLine("\nWould you like to login or register a new user?");
                    Console.WriteLine("[1] Login");
                    Console.WriteLine("[2] Register");
                    bool actionChosen = false;
                    // Make the user choose an option
                    while(!actionChosen)
                    {
                        string? input = Console.ReadLine();
                        if (input == "1") // Ask the user to log in
                        {
                            actionChosen = true;
                            user = PromptLogin(users);
                            if(user != null)
                            {
                                Login(user);
                            }
                        }
                        else if (input == "2") // Ask the user to register (the user automatically logs in after)
                        {
                            actionChosen = true;
                            user = PromptRegistration(users);
                            if (user != null)
                            {
                                Login(user);
                            }
                        } else
                        {
                            Console.WriteLine("That isn't a valid option.");
                        }
                    }
                }

                SaveUsers(users, userFilePath); // Save the UserList
            } else
            {
                Console.WriteLine("Path to the user save file not found!");
            }
        }

        // Login and greeting message
        static void Login(User user)
        {
            UserLog log = new(user);
            Console.WriteLine("\r\n░██╗░░░░░░░██╗███████╗██╗░░░░░░█████╗░░█████╗░███╗░░░███╗███████╗░░░  ██╗░░░██╗░██████╗███████╗██████╗░██╗\r\n░██║░░██╗░░██║██╔════╝██║░░░░░██╔══██╗██╔══██╗████╗░████║██╔════╝░░░  ██║░░░██║██╔════╝██╔════╝██╔══██╗██║\r\n░╚██╗████╗██╔╝█████╗░░██║░░░░░██║░░╚═╝██║░░██║██╔████╔██║█████╗░░░░░  ██║░░░██║╚█████╗░█████╗░░██████╔╝██║\r\n░░████╔═████║░██╔══╝░░██║░░░░░██║░░██╗██║░░██║██║╚██╔╝██║██╔══╝░░██╗  ██║░░░██║░╚═══██╗██╔══╝░░██╔══██╗╚═╝\r\n░░╚██╔╝░╚██╔╝░███████╗███████╗╚█████╔╝╚█████╔╝██║░╚═╝░██║███████╗╚█║  ╚██████╔╝██████╔╝███████╗██║░░██║██╗\r\n░░░╚═╝░░░╚═╝░░╚══════╝╚══════╝░╚════╝░░╚════╝░╚═╝░░░░░╚═╝╚══════╝░╚╝  ░╚═════╝░╚═════╝░╚══════╝╚═╝░░╚═╝╚═╝");
        }

        static User? PromptLogin(UserList users) 
        {
            bool usernameChosen = false;
            bool passwordValidated = false;

            User? foundUser = null;

            Console.WriteLine("\nPlease log in as an existing user.\n");

            int usernameTries = 3;
            // Make the user choose a username that already exists
            while (!usernameChosen)
            {
                // User can run out of tries (max 3)
                if(usernameTries == 0)
                {
                    Console.WriteLine("\nYou've been timed out for failing to log in too many times.");
                    return null;
                }
                Console.Write("Enter a username: ");
                string? username = Console.ReadLine();

                foundUser = users.FindUser(username);

                // A user with that name wasn't found => take away one try, try again (or get timed out)
                if (foundUser == null)
                {
                    Console.WriteLine("That user doesn't exist!");
                    usernameTries--;
                }
                else
                {
                    usernameChosen = true;
                    int tries = 3; // Same as the username tries
                    // Make the user type in the correct password, time out if they can't do it within 3 tries or less
                    while (!passwordValidated)
                    {
                        if (tries == 0)
                        {
                            Console.WriteLine("\nYou've been timed out for failing to log in too many times.");
                            return null;
                        }

                        Console.Write("Enter the password: ");
                        string? password = Console.ReadLine();

                        // Validate the login
                        if (users.Login(foundUser, password))
                        {
                            passwordValidated = true;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect password!");
                            tries--;
                        }
                    }
                }
            }

            // Returns the user (can be null if the login wasn't successful)
            return foundUser;
        }

        static User? PromptRegistration(UserList users) 
        {
            bool usernameChosen = false;
            bool passwordChosen = false;

            string? username = default;
            string? password = default;

            Console.WriteLine("\nPlease register as a new user.\n");

            // Make the user choose a username that doesn't exist yet
            while (!usernameChosen)
            {
                Console.Write("Enter a username: ");
                username = Console.ReadLine();

                if(users.FindUser(username) != null)
                {
                    Console.WriteLine("That username is already taken!");
                } else
                {
                    if (User.IsUsernameValid(username))
                    {
                        usernameChosen = true;
                    }
                    else
                    {
                        Console.WriteLine("The username given isn't valid!");
                    }
                }
            }

            Console.WriteLine("\nYou will now enter a new password. It must contain at least one lowercase and uppercase letter, one number and one non-alphanumeric character and be at least 8 characters long.\n");

            // Make the user choose a valid password (it must meet the requirements specified in the IsPasswordValid method)
            while(!passwordChosen) 
            {
                Console.Write("Enter a new password: ");
                password = Console.ReadLine();

                if (User.IsPasswordValid(password))
                {
                    passwordChosen = true;
                }
                else
                {
                    Console.WriteLine("The password given isn't valid!");
                }
            }

            // Create the new user
            if(username != null && password != null) 
            {
                User newUser = User.Create(username, password);
                users.Add(newUser);
                Console.WriteLine("Your registration is complete. Thank you for signing up!");
                return newUser;
            } else
            {
                Console.WriteLine("Something went wrong. Registration failed.");
                return null;
            }
        }

        // Saves the UserList in a JSON file
        static void SaveUsers(UserList users, string filePath) 
        {
            string usersJson = JsonSerializer.Serialize<UserList>(users);
            File.WriteAllText(filePath, usersJson);
        }
    }
}