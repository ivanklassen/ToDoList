namespace ToDoList
{
    public enum Status { Active, Completed, Overdue, Late}

    public enum Priority { Low, Medium, High, Critical}

    public class Task
    {
        public Guid Id {  get; set; } = Guid.NewGuid();
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset? Deadline { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public DateTime? EditingDate { get; set; }   
    }
}
