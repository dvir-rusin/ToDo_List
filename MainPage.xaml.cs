using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.IO;


namespace ToDo_List2
{
    public partial class MainPage : ContentPage
    {
        //RELATED TO USER SAVING
        private UserProfile currentUserProfile;
        private string currentUsername;

        private int counter = 0;// COUNTS TASKS AND ACTS AS ID
        public ObservableCollection<TaskItem> Tasks { get; set; }//UNFINISHED TASKS LIST
        public ObservableCollection<TaskItem> FinishedTasks { get; set; }//FINISHED TASKS LIST

        CollectionView collectionView = new CollectionView();
        
        public MainPage()

        {
            
        public MainPage()
            
        {
            var button = new Button();
            var task = new TaskItem();
            InitializeComponent();

            Tasks = new ObservableCollection<TaskItem>();
            FinishedTasks = new ObservableCollection<TaskItem>();

            TasksCollectionView.ItemsSource = Tasks;
            FinishedTasksCollectionView.ItemsSource = FinishedTasks;


            BindingContext= this;
            LoadLastUser();
        }

        private void LoadLastUser()
        {
            currentUsername = UserProfileManager.LoadLastUser();
            if (!string.IsNullOrEmpty(currentUsername))
            {
                currentUserProfile = UserProfileManager.LoadUserProfile(currentUsername) ?? new UserProfile { Username = currentUsername };
                Tasks = currentUserProfile.Tasks;
                FinishedTasks = currentUserProfile.FinishedTasks;
            }
            else
            {
                currentUserProfile = new UserProfile();
                Tasks = new ObservableCollection<TaskItem>();
                FinishedTasks = new ObservableCollection<TaskItem>();
            }

            TasksCollectionView.ItemsSource = Tasks;
            FinishedTasksCollectionView.ItemsSource = FinishedTasks;
            BindingContext = this;
        }


        private void SaveCurrentUserProfile()
        {
            if (currentUserProfile != null)
            {
                UserProfileManager.SaveUserProfile(currentUserProfile);
                UserProfileManager.SaveLastUser(currentUsername);
            }
        }

        private void OnLoadUserTasksClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoadTaskUsernameEntry.Text))
            {
                DisplayAlert("Error", "Please enter a username.", "OK");
                return;
            }

            currentUsername = LoadTaskUsernameEntry.Text;
            currentUserProfile = UserProfileManager.LoadUserProfile(currentUsername) ?? new UserProfile { Username = currentUsername };

            Tasks = currentUserProfile.Tasks;
            FinishedTasks = currentUserProfile.FinishedTasks;

            TasksCollectionView.ItemsSource = Tasks;
            FinishedTasksCollectionView.ItemsSource = FinishedTasks;
            BindingContext = this;

            
            SaveCurrentUserProfile();
        }

        private void OnSaveUserTasksClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SaveTaskUsernameEntry.Text))
            {
                DisplayAlert("Error", "Please enter a username.", "OK");
                return;
            }

            currentUsername = SaveTaskUsernameEntry.Text;
            if (currentUserProfile == null || currentUserProfile.Username != currentUsername)
            {
                currentUserProfile = new UserProfile { Username = currentUsername };
            }

            //empty the display
            
            SaveCurrentUserProfile();
        }

        private void OnAddTaskClicked(object sender, EventArgs e)//add task
        {
            if (string.IsNullOrWhiteSpace(TitleEntry.Text) || PriorityPicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Please enter a title and select a priority.", "OK");
                return;
            }


            var task = new TaskItem // create a new task
            {
                TITLE = TitleEntry.Text,
                DEADLINE = DeadlinePicker.Date,
                PRIORITY = PriorityPicker.SelectedItem.ToString(),
                ISDONE = false,
                ID = counter++,
                SUBMISSION_STATUS = "NONE"
            };

            Tasks.Add(task);//add to tasks array

            //empty the display
            TitleEntry.Text = string.Empty;
            PriorityPicker.SelectedItem = null;
        }

        private void OnDeleteTaskClicked(object sender, EventArgs e)//delete a task 
        {
            var button = (Button)sender;
            var task = (TaskItem)button.CommandParameter;
            if (task != null)
            {
                if (task.ISDONE)//delete from finished tasks 
                {
                    FinishedTasks.Remove(task);
                } 
                else // delete from tasks
                {
                    Tasks.Remove(task);
                }
            }
        }

        private void UpdateTaskStatus(TaskItem task, bool isFinished)
        {
            if (task == null) return;


            if (isFinished)
                    {
                Tasks.Remove(task);
                task.ISDONE = true;
                        task.SUBMISSIONDATE = DateTime.Now;
                task.SUBMISSION_STATUS = task.SUBMISSIONDATE > task.DEADLINE ? "OVERDUE" : "ON TIME";
                FinishedTasks.Add(task);
                    }
            else
            {
                FinishedTasks.Remove(task);
                task.ISDONE = false;
                task.SUBMISSIONDATE = default;
                task.SUBMISSION_STATUS = "NONE";
                Tasks.Add(task);
                }

            SaveCurrentUserProfile();
            }
      
        private void OnFinishTaskClicked(object sender, EventArgs e)
                    {
            var button = (Button)sender;
            var task = (TaskItem)button.CommandParameter;
            UpdateTaskStatus(task, true);
        }

        private void OnUnfinishTaskClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskItem)button.CommandParameter;
            UpdateTaskStatus(task, false);
        }


        private void OnEditTaskClicked(object sender, EventArgs e)
        {
            //ERROR check
            if (string.IsNullOrWhiteSpace(TitleEntry.Text) || PriorityPicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Please enter a title and select a priority.", "OK");
                return;
            }

            var button = (Button) sender;
            var task = (TaskItem) button.CommandParameter ;
            var taskId = task.ID;

            //go over the tasks in Tasks
            foreach (TaskItem task2 in Tasks)
            {
                if (task2.ID==task.ID)
                {
                    task2.TITLE = TitleEntry.Text;
                    task2.DEADLINE = DeadlinePicker.Date;
                    task2.PRIORITY = PriorityPicker.SelectedItem.ToString();
                }
            }
            //go over the tasks in FinishedTasks(they are all done)
            if (task.ISDONE)
            {
                foreach (TaskItem task2 in FinishedTasks)
                {
                    if (task2.ID == task.ID)
                    {
                        task2.TITLE = TitleEntry.Text;
                        task2.DEADLINE = DeadlinePicker.Date;
                        task2.PRIORITY = PriorityPicker.SelectedItem.ToString();
                    }
                }
            }
            //updating the display
            TasksCollectionView.ItemsSource = Tasks;
            //clear title and priority from display
            TitleEntry.Text = string.Empty;
            PriorityPicker.SelectedItem = null;
        }



        //USER TASKS SAVING RELATED -------------------------------------------------↓↓↓   




        
        //Description: LoadLastUser() function

        //The LoadLastUser method is responsible for retrieving the last user who used the 
        //application and loading their profile along with their tasks. It first attempts to load the 
        //username of the last user. If a username is found, it loads the corresponding user profile. If 
        //the profile does not exist, it creates a new user profile with the retrieved username. If no 
        //username is found, it initializes a new user profile and empty task collections. The method 
        //then updates the item sources for the task collection views and sets the binding context to 
        //ensure the UI reflects the current data.



        private void LoadLastUser()
        {
            // Load the username of the last user who used the application
            currentUsername = UserProfileManager.LoadLastUser();

            // Check if a username was successfully retrieved
            if (!string.IsNullOrEmpty(currentUsername))
            {
                // Load the user profile for the retrieved username
                // If the profile does not exist, create a new one with the current username
                currentUserProfile = UserProfileManager.LoadUserProfile(currentUsername) ?? new UserProfile { Username = currentUsername };

                // Assign the tasks and finished tasks to the current user's tasks
                Tasks = currentUserProfile.Tasks;
                FinishedTasks = currentUserProfile.FinishedTasks;
    }
            else
            {
                // If no username was retrieved, initialize a new user profile and empty task collections
                currentUserProfile = new UserProfile();
                Tasks = new ObservableCollection<TaskItem>();
                FinishedTasks = new ObservableCollection<TaskItem>();
            }
            // Set the items source for the tasks and finished tasks collection views
            TasksCollectionView.ItemsSource = Tasks;
            FinishedTasksCollectionView.ItemsSource = FinishedTasks;

            // Update the binding context to reflect the current state
            BindingContext = this;
        }




        //Description: SaveCurrentUserProfile() function

        //SaveCurrentUserProfile method responsible for: saving the current user's profile data 
        //and marking the current user as the last user who used the application. 
        //If there is a valid user profile, 
        //it saves the profile data to a file and updates the record of the last user by 
        //saving the current username. This ensures that when the application is reopened, the last 
        //user's data can be retrieved and loaded.
        private void SaveCurrentUserProfile()
        {
            if (currentUserProfile != null)
            {
                // Save the current user's profile
                UserProfileManager.SaveUserProfile(currentUserProfile);

                // Save the username of the current user as the last user
                UserProfileManager.SaveLastUser(currentUsername);
            }
        }








        //Description: OnLoadUserTasksClicked(object sender, EventArgs e) function

        //The OnLoadUserTasksClicked method is triggered when the user clicks the button to load 
        //tasks for a specific username. It first checks if the username entry is empty and displays an 
        //error message if so. If a username is provided, it retrieves and loads the user profile for 
        //that username, creating a new profile if none exists. It updates the task and finished task 
        //collections and sets the item sources for the collection views to display the loaded tasks. 
        //The method also updates the binding context to reflect the current state and saves the 
        //current user profile, marking the current user as the last user.


        private void OnLoadUserTasksClicked(object sender, EventArgs e)
        {   // Check if the username entry is empty
            if (string.IsNullOrWhiteSpace(LoadTaskUsernameEntry.Text))
            {
                // Display an error message if the username is empty
                DisplayAlert("Error", "Please enter a username.", "OK");
                return;
            }
            // Retrieve the entered username
            currentUsername = LoadTaskUsernameEntry.Text;

            // Load the user profile for the entered username
            // If the profile does not exist, create a new one with the entered username
            currentUserProfile = UserProfileManager.LoadUserProfile(currentUsername) ?? new UserProfile { Username = currentUsername };

            // Assign the tasks and finished tasks to the current user's tasks
            Tasks = currentUserProfile.Tasks;
            FinishedTasks = currentUserProfile.FinishedTasks;


            // Set the items source for the tasks and finished tasks collection views
            TasksCollectionView.ItemsSource = Tasks;
            FinishedTasksCollectionView.ItemsSource = FinishedTasks;

            // Update the binding context to reflect the current state
            BindingContext = this;

            // Save the current user profile and mark the current user as the last user
            SaveCurrentUserProfile();
        }








        //Description: OnSaveUserTasksClicked(object sender, EventArgs e) function
        
        //The OnSaveUserTasksClicked method is triggered when the user clicks the button to save 
        //tasks for a specific username. It checks if the username entry is empty and displays an 
        //error message if so. If a username is provided, it retrieves the entered username and 
        //checks if the current user profile is null or if the username has changed. If either condition 
        //is met, it creates a new user profile with the entered username. The method then saves the 
        //current user profile and marks the current user as the last user.



        private void OnSaveUserTasksClicked(object sender, EventArgs e)
        {
            // Check if the username entry is empty
            if (string.IsNullOrWhiteSpace(SaveTaskUsernameEntry.Text))
            {
                // Display an error message if the username is empty
                DisplayAlert("Error", "Please enter a username.", "OK");
                return;
            }
            // Retrieve the entered username
            currentUsername = SaveTaskUsernameEntry.Text;

            // Check if the current user profile is null or if the username has changed
            if (currentUserProfile == null || currentUserProfile.Username != currentUsername)
            {
                // Create a new user profile with the entered username
                currentUserProfile = new UserProfile { Username = currentUsername };
            }

            //empty the display

            // Save the current user profile and mark the current user as the last user
            SaveCurrentUserProfile();
        }

    }

    public class UserProfile
    {
        public string Username { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; }
        public ObservableCollection<TaskItem> FinishedTasks { get; set; }

        public UserProfile()
        {
            Tasks = new ObservableCollection<TaskItem>();
            FinishedTasks = new ObservableCollection<TaskItem>();
        }
    }

    public static class UserProfileManager
    {
        private static string GetUserProfilePath(string username)
            => Path.Combine(FileSystem.Current.AppDataDirectory, $"{username}.json");

        public static void SaveUserProfile(UserProfile profile)
        {
            string json = JsonSerializer.Serialize(profile);
            File.WriteAllText(GetUserProfilePath(profile.Username), json);
        }

        public static UserProfile LoadUserProfile(string username)
        {
            string path = GetUserProfilePath(username);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<UserProfile>(json);
            }
            return null;
    }

        public static void SaveLastUser(string username)
        {
            File.WriteAllText(Path.Combine(FileSystem.Current.AppDataDirectory, "lastUser.txt"), username);
        }

        public static string LoadLastUser()
        {
            string path = Path.Combine(FileSystem.Current.AppDataDirectory, "lastUser.txt");
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }
    }

    //created TaskItem(ID ,TITLE,DEADLINE,PRIORITY,ISDONE)

    public class TaskItem : ObservableObject
    {
        private int id;
        private string Title;
        private DateTime Deadline;
        private string Priority;
        private bool IsDone;
        private DateTime SubmissionDate;
        private string Submission_Status;
        private bool IsOverdue;


        public int ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string TITLE
        {
            get { return Title; }
            set { SetProperty(ref Title, value); }
        }

        public DateTime DEADLINE
        {
            get { return Deadline; }
            set { SetProperty(ref Deadline, value); }
        }

        public string PRIORITY
        {
            get { return Priority; }
            set { SetProperty(ref Priority, value); }
        }

        public bool ISDONE
        {
            get { return IsDone; }
            set { SetProperty(ref IsDone, value); }
        }

        public DateTime SUBMISSIONDATE
        {
            get { return SubmissionDate; }
            set { SetProperty(ref SubmissionDate, value); }
        }
        
        public string SUBMISSION_STATUS
        {
            get { return Submission_Status; }
            set { SetProperty(ref Submission_Status, value); }
        }

        public bool ISOVERDUE
        {
            get { return IsOverdue; }
            set { SetProperty(ref IsOverdue, value); }
        }



    }
}