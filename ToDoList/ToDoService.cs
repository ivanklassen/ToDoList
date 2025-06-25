using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ToDoList
{
    public class ToDoService
    {
        private readonly ApplicationDbContext _context;

        public ToDoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ToDoList.Task Create(TaskModel model)
        {
            (Priority? taskPriority, string cleanedTitleWithDeadline) = ParsePriorityFromTitle(model.Title);
            model.Title = cleanedTitleWithDeadline;

            if (taskPriority == null && model.Priority == null)
            {
                model.Priority = Priority.Medium;
            }

            if (taskPriority != null && model.Priority == null)
            {
                model.Priority = taskPriority;
            }

            (DateTimeOffset? deadline, string cleanedTitle) = ParseDeadlineFromTitle(model.Title);
            model.Title = cleanedTitle;
            if (cleanedTitle.Length < 4)
            {
                return null;
            }

            if (deadline != null && model.Deadline == null)
            {
                model.Deadline = deadline;
            }


            var task = new Task
            {
                Title = model.Title,
                Description = model.Description,
                Deadline = model.Deadline,
                Status = Status.Active,
                Priority = (Priority)model.Priority
            };

            if (task.Deadline < DateTime.Today)
            {
                task.Status = Status.Overdue;
            }

            _context.AddAsync(task);
            _context.SaveChangesAsync();

            return task;
        }

        public async Task<Task> Get(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            return task;
        }

        public Task Edit(TaskModel model, Guid id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return null;
            }

            (Priority? taskPriority, string cleanedTitleWithDeadline) = ParsePriorityFromTitle(model.Title);
            model.Title = cleanedTitleWithDeadline;

            if (taskPriority == null && model.Priority == null)
            {
                model.Priority = Priority.Medium;
            }

            if (taskPriority != null && model.Priority == null)
            {
                model.Priority = taskPriority;
            }

            (DateTimeOffset? deadline, string cleanedTitle) = ParseDeadlineFromTitle(model.Title);
            model.Title = cleanedTitle;
            if (cleanedTitle.Length < 4)
            {
                return null;
            }

            if (deadline != null && model.Deadline == null)
            {
                model.Deadline = deadline;
            }

            task.Title = model.Title;
            task.Description = model.Description;
            task.Deadline = model.Deadline;
            task.Priority = (Priority)model.Priority;
            task.EditingDate = DateTime.UtcNow;

            if (task.Deadline <= DateTime.UtcNow)
            {
                task.Status = Status.Overdue;
            }

            if (task.Deadline > DateTime.UtcNow && task.Status == Status.Late)
            {
                task.Status = Status.Completed;
            }

           _context.SaveChanges();

            return task;
        }

        public bool Delete(Guid id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }

        public async Task<List<Task>> GetAll()
        {
            var tasks = await _context.Tasks.AsNoTracking().ToListAsync();

            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Deadline < DateTime.UtcNow && tasks[i].Status == Status.Active)
                {
                    tasks[i].Status = Status.Overdue;
                }
            }

            return tasks;
        }

        public async Task<StatusModel> Complete(Guid id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return null;
            }

            if (task.Deadline == null)
            {
                if (task.Status == Status.Active)
                {
                    task.Status = Status.Completed;
                    _context.SaveChanges();
                    return new StatusModel { Status = task.Status };
                }
                else
                {
                    task.Status = Status.Active;
                    _context.SaveChanges();
                    return new StatusModel { Status = task.Status };
                }
            }

            else
            {


                if (task.Deadline >= DateTime.UtcNow && task.Status == Status.Active)
                {
                    task.Status = Status.Completed;
                    _context.SaveChanges();
                    return new StatusModel { Status = task.Status };
                }

                if (task.Deadline < DateTime.UtcNow && task.Status == Status.Active)
                {
                    task.Status = Status.Late;
                    _context.SaveChanges();
                    return new StatusModel { Status = task.Status };

                }

                if (task.Deadline >= DateTime.UtcNow && task.Status == Status.Completed)
                {
                    task.Status = Status.Active;
                    _context.SaveChanges();
                    return new StatusModel { Status = task.Status };

                }

                if (task.Deadline < DateTime.UtcNow && (task.Status == Status.Completed || task.Status == Status.Late))
                {
                    task.Status = Status.Overdue;
                    _context.SaveChanges();
                    return new StatusModel { Status = task.Status };

                }

                if (task.Status == Status.Overdue && task.Deadline < DateTime.UtcNow)
                {
                    task.Status = Status.Late;
                    _context.SaveChanges();
                    return new StatusModel { Status = task.Status };

                }

            }


            return new StatusModel { Status = task.Status };
        }




        private (Priority?, string) ParsePriorityFromTitle(string title)
        {
            Match match = Regex.Match(title, @"!(?:1|2|3|4)");

            if (match.Success)
            {
                string macros = match.Value;
                string cleanedTitle = title.Replace(macros, "").Trim();

                Priority priority = Priority.Medium;

                switch (macros)
                {
                    case "!1":
                        priority = Priority.Critical;
                        break;
                    case "!2":
                        priority = Priority.High;
                        break;
                    case "!3":
                        priority = Priority.Medium;
                        break;
                    case "!4":
                        priority = Priority.Low;
                        break;
                }

                return (priority, cleanedTitle);
            }

            return (null, title);
        }

        private (DateTimeOffset?, string) ParseDeadlineFromTitle(string title)
        {
            Match match = Regex.Match(title, @"!before\s(\d{2})[.-](\d{2})[.-](\d{4})");

            if (match.Success)
            {
                try
                {
                    int day = int.Parse(match.Groups[1].Value);
                    int month = int.Parse(match.Groups[2].Value);
                    int year = int.Parse(match.Groups[3].Value);

                    DateTimeOffset deadlineInput = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
                    DateTimeOffset deadline = deadlineInput.ToUniversalTime();

                    string macros = match.Value;
                    string cleanedTitle = title.Replace(macros, "").Trim();

                    return (deadline, cleanedTitle);
                }

                catch (FormatException)
                {
                    return (null, title);
                }

                catch (ArgumentOutOfRangeException)
                {
                    return (null, title);
                }

            }

            return (null, title);
        }


    }
}
