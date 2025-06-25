namespace ToDoList
{
    public class GetTaskModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset? Deadline { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }
    }
}
