using System;
using System.Collections.Generic;
using System.Linq;
using AstronautScheduler.Patterns;
using AstronautScheduler.Utils;

namespace AstronautScheduler.Utils
{
    public sealed class Logger
    {
        private static readonly Logger instance = new Logger();
        private Logger() { }
        public static Logger Instance => instance;

        public void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
            Console.ResetColor();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
            Console.ResetColor();
        }
    }
}

namespace AstronautScheduler.Patterns
{
    public interface IObserver
    {
        void Update(string message);
    }

    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify(string message);
    }
}

namespace AstronautScheduler.Core
{
    public class Task
    {
        public string Description { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Priority { get; set; }

        public override string ToString()
        {
            return $"{Start:hh\\:mm} - {End:hh\\:mm}: {Description} [{Priority}]";
        }
    }

    public static class TaskFactory
    {
        public static Task Create(string description, string start, string end, string priority)
        {
            if (!TimeSpan.TryParse(start, out TimeSpan s) ||
                !TimeSpan.TryParse(end, out TimeSpan e))
            {
                throw new ArgumentException("Invalid time format. Use HH:mm");
            }

            if (s >= e)
                throw new ArgumentException("Start time must be before End time");

            return new Task
            {
                Description = description,
                Start = s,
                End = e,
                Priority = priority
            };
        }
    }

    public sealed class ScheduleManager : ISubject
    {
        private static readonly ScheduleManager instance = new ScheduleManager();
        private List<Task> tasks = new List<Task>();
        private List<IObserver> observers = new List<IObserver>();

        private ScheduleManager() { }
        public static ScheduleManager Instance => instance;

        public void AddTask(Task task)
        {
            var conflict = tasks.FirstOrDefault(t =>
                (task.Start < t.End && task.End > t.Start));

            if (conflict != null)
            {
                Notify($"Conflict with task \"{conflict.Description}\"");
                throw new InvalidOperationException($"Task conflicts with existing task \"{conflict.Description}\"");
            }

            tasks.Add(task);
            tasks = tasks.OrderBy(t => t.Start).ToList();
            Logger.Instance.Log($"Task \"{task.Description}\" added successfully.");
        }

        public void RemoveTask(string description)
        {
            var task = tasks.FirstOrDefault(t => t.Description.Equals(description, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                throw new InvalidOperationException("Task not found.");
            }
            tasks.Remove(task);
            Logger.Instance.Log($"Task \"{description}\" removed successfully.");
        }

        public void ViewTasks()
        {
            if (!tasks.Any())
            {
                Console.WriteLine("No tasks scheduled for the day.");
                return;
            }

            foreach (var task in tasks)
                Console.WriteLine(task);
        }

        public void Attach(IObserver observer) => observers.Add(observer);
        public void Detach(IObserver observer) => observers.Remove(observer);
        public void Notify(string message)
        {
            foreach (var obs in observers)
                obs.Update(message);
        }
    }

    public class Astronaut : IObserver
    {
        private string name;
        public Astronaut(string name) => this.name = name;
        public void Update(string message) =>
            Console.WriteLine($"[NOTIFY {name}]: {message}");
    }
}

namespace AstronautScheduler
{
    using AstronautScheduler.Core;
    using AstronautScheduler.Utils;

    class Program
    {
        static void Main(string[] args)
        {
            var manager = ScheduleManager.Instance;

            var astronaut = new Astronaut("Neil");
            manager.Attach(astronaut);

            try
            {
                var t1 = TaskFactory.Create("Morning Exercise", "07:00", "08:00", "High");
                manager.AddTask(t1);

                var t2 = TaskFactory.Create("Team Meeting", "09:00", "10:00", "Medium");
                manager.AddTask(t2);

                Console.WriteLine("\n-- Current Tasks --");
                manager.ViewTasks();

                var t3 = TaskFactory.Create("Training Session", "09:30", "10:30", "High");
                manager.AddTask(t3); // will throw
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }

            Console.WriteLine("\n-- Remove Task Example --");
            try
            {
                manager.RemoveTask("Morning Exercise");
                manager.ViewTasks();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }

            Console.WriteLine("\n-- Invalid Removal --");
            try
            {
                manager.RemoveTask("Non-existent Task");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }
        }
    }
}
