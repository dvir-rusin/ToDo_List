using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;

namespace ToDo_List2
{
    public partial class MainPage : ContentPage
    {
        private int counter = 0;
        public ObservableCollection<TaskItem> Tasks { get; set; }
        public ObservableCollection<TaskItem> FinishedTasks { get; set; }

        CollectionView collectionView = new CollectionView();
        
        public MainPage()

        {
            
            
            InitializeComponent();

            Tasks = new ObservableCollection<TaskItem>();
            FinishedTasks = new ObservableCollection<TaskItem>();

            TasksCollectionView.ItemsSource = Tasks;
            FinishedTasksCollectionView.ItemsSource = FinishedTasks;


            BindingContext= this;
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
                ID = counter++
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

        private void OnFinishTaskClicked(object sender, EventArgs e)//task finished
        {//remove task from tasks, add into finishedtasks
            var button = (Button)sender;
            var task = (TaskItem)button.CommandParameter;
            Tasks.Remove(task);
            task.ISDONE = true;//change done state to true
            FinishedTasks.Add(task);
        }

        private void OnUnfinishTaskClicked(object sender, EventArgs e)//remove task from finshedtasks, add into tasks
        {
            var button = (Button)sender;
            var task = (TaskItem)button?.CommandParameter;  
           FinishedTasks.Remove(task);
           task.ISDONE = false;//change the done state back
            Tasks.Add(task);
                

            
        }


        private void OnEditTaskClicked(object sender, EventArgs e)//, ValueChangedEventArgs args)
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
            TasksCollectionView.ItemsSource = Tasks;
            //clear title and priority from display
            TitleEntry.Text = string.Empty;
            PriorityPicker.SelectedItem = null;
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
        
    }
}