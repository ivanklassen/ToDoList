using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ToDoList.Tests
{
    public class ToDoServiceUnitTests
    {
        private readonly ApplicationDbContext _context;

        public ToDoServiceUnitTests()
        {

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ToDoUnitTestsDb")
                .Options;

            _context = new ApplicationDbContext(options);

            _context.Database.EnsureCreated();

        }


        //Проверка создание задачи с длиной названия больше 4 символов
        [Theory]
        [InlineData("abcd")]
        [InlineData("abcde")]

        public void Create_TitleLength(string title)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = title };

            var task = service.Create(model);

            Assert.NotNull(task);
        }

        //Проверка того, что задача с менее 4 символами в названии не создается
        [Fact]
        public void Create_NoValidationTitle()
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "abc" };

            var task = service.Create(model);

            Assert.Null(task);
        }

        //Корректное обработка названия задачи с указанием макроса приоритета
        [Theory]
        [InlineData("!1 Critical Task", Priority.Critical, "Critical Task")]
        [InlineData("!2 High Task", Priority.High, "High Task")]
        [InlineData("!3 Medium Task", Priority.Medium, "Medium Task")]
        [InlineData("Low Task !4", Priority.Low, "Low Task")]
        public void Create_PriorityMacro_SetPriority(string title, Priority expectedPriority, string expectedTitle)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = title};

            service.Create(model);

            var addedTask = _context.Tasks.FirstOrDefault(t => t.Title == expectedTitle);
            Assert.NotNull(addedTask);
            Assert.Equal(expectedPriority, addedTask.Priority);
            Assert.Equal(expectedTitle, addedTask.Title);
        }

        //Корректное обработка названия задачи с указанием не существующего макроса приоритета
        [Theory]
        [InlineData("!0 Task")]
        [InlineData("!5 Task")]
        public void Create_PriorityNoValidMacro_SetPriority(string title)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = title };

            service.Create(model);

            var addedTask = _context.Tasks.FirstOrDefault(t => t.Title == title);
            Assert.NotNull(addedTask);
            Assert.Equal(title, addedTask.Title);
        }

        //Корректная обработка названия задачи с указанием макроса дедлайна
        [Theory]
        [InlineData("!before 05.05.2025 Task1", 5, 5, 2025, "Task1")]
        [InlineData("!before 01.01.2099 Task3", 1, 1, 2099, "Task3")]
        [InlineData("Task4 !before 01.01.1970", 1, 1, 1970, "Task4")]
        [InlineData("!before 01.01.2000 Task5", 1, 1, 2000, "Task5")]
        [InlineData("Task6 !before 31.12.1999", 31, 12, 1999, "Task6")]
        [InlineData("Task7 !before 31.05.2025", 31, 5, 2025, "Task7")]
        [InlineData("Task8 !before 29.02.2024", 29, 02, 2024, "Task8")]
        public void Create_DeadlineMacro_SetDeadline(string title, int day, int month, int year, string expectedTitle)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = title };
            DateTimeOffset parsedDeadline = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero).ToUniversalTime();

            service.Create(model);

            var addedTask = _context.Tasks.FirstOrDefault(t => t.Title == expectedTitle);
            Assert.NotNull(addedTask);
            Assert.Equal(parsedDeadline, addedTask.Deadline);
            Assert.Equal(expectedTitle, addedTask.Title);
        }

        //Корректная обработка названия задачи с указанием не валидного макроса дедлайна
        [Theory]
        [InlineData("!before 05.05.25 Task")]
        [InlineData("!before 13-13-2025 Task")]
        [InlineData("!before Task")]
        [InlineData("!before 05,05,2025 Task")]
        [InlineData("!before 30.02.2024 Task")]
        public void Create_DeadlineNoValidMacro_SetDeadline(string title)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = title };

            service.Create(model);

            var addedTask = _context.Tasks.FirstOrDefault(t => t.Title == title);
            Assert.NotNull(addedTask);
            Assert.Equal(title, addedTask.Title);
        }

        //Корректная обработка названия задачи с указанием обоих макросов
        [Theory]
        [InlineData("!before 05.05.2025 !1 Task1", 5, 5, 2025, "Task1", Priority.Critical)]
        [InlineData("!2 !before 25-05-2025 Task2", 25, 5, 2025, "Task2", Priority.High)]
        [InlineData("!before 01.01.2099 Task3 !3", 1, 1, 2099, "Task3", Priority.Medium)]
        [InlineData("Task4 !before 01.01.1970 !4", 1, 1, 1970, "Task4", Priority.Low)]
        public void Create_DeadlineBothMacro_SetDeadlineAndPriority(string title, int day, int month, int year, string expectedTitle, Priority priority)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = title };
            DateTimeOffset parsedDeadline = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero).ToUniversalTime();

            service.Create(model);

            var addedTask = _context.Tasks.FirstOrDefault(t => t.Title == expectedTitle);
            Assert.NotNull(addedTask);
            Assert.Equal(parsedDeadline, addedTask.Deadline);
            Assert.Equal(expectedTitle, addedTask.Title);
            Assert.Equal(priority, addedTask.Priority);
        }

        //Проверка удаления задачи
        [Fact]
        public void Delete_ExistingId_DeleteTaskFromDatabase()
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task to Delete" };
            var task = service.Create(model);
            var id = task.Id;

            service.Delete(id);

            var deletedTask = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Null(deletedTask);
        }

        //Проверка редактирования задачи
        [Fact]
        public void Update_TaskExists_UpdateTaskInDatabase()
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "old title", Description = "old description", Deadline = DateTimeOffset.ParseExact("15.05.2025", "dd.MM.yyyy", null), Priority = Priority.Low };
            service.Create(model);
            var task = _context.Tasks.FirstOrDefault(t => t.Title == "old title");
            var id = task.Id;


            model.Title = "new title";
            model.Description = "new description";
            model.Priority = Priority.High;
            model.Deadline = DateTimeOffset.ParseExact("30.05.2025", "dd.MM.yyyy", null);

            service.Edit(model, id);

            var updatedTask = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.NotNull(updatedTask);
            Assert.Equal("new title", updatedTask.Title);
            Assert.Equal("new description", updatedTask.Description);
            Assert.Equal(Priority.High, updatedTask.Priority);
            Assert.Equal(DateTimeOffset.ParseExact("30.05.2025", "dd.MM.yyyy", null), updatedTask.Deadline);
        }

        //Проверка корректного установления статуса при создании задачи с истекшим дедлайном
        [Fact]
        public void Update_DeadlinePassed_ChangeStatusToOverdue()
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task", Deadline = DateTimeOffset.UtcNow.AddDays(-1) };

            var task = service.Create(model);

            var id = task.Id;
            var Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Equal(Status.Overdue, Task.Status);
        }

        //Проверка корректного установления статуса при создании задачи с еще не наступившим дедлайном
        [Fact]
        public void Update_Deadline_ChangeStatusToActive()
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task", Deadline = DateTimeOffset.UtcNow.AddDays(5) };

            var task = service.Create(model);

            var id = task.Id;
            var Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Equal(Status.Active, Task.Status);
        }

        //Проверка корректного установления статуса при создании задачи с дедлайном в этот же день
        [Fact]
        public void Update_DeadlineToday_ChangeStatusToActive()
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task", Deadline = DateTime.Today };

            var task = service.Create(model);

            var id = task.Id;
            var Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Equal(Status.Active, Task.Status);
        }

        //Проверка статуса выполненной задачи с учетом дедлайна
        [Theory]
        [InlineData("05.05.2025", Status.Late)]
        [InlineData("09.09.2027", Status.Completed)]

        public void Complete_Deadline_ChangeStatus(string deadline, Status status)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task", Deadline = DateTime.Parse(deadline) };
            var task = service.Create(model);
            var id = task.Id;

            service.Complete(id);

            var Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Equal(status, Task.Status);
        }

        //Проверка статуса задачи при маркировании ее как невыполненной
        [Theory]
        [InlineData("05.05.2025", Status.Overdue)]
        [InlineData("09.09.2027", Status.Active)]

        public void Complete_Deadline_ChangeCompletedOrLateStatus(string deadline, Status status)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task", Deadline = DateTime.Parse(deadline) };
            var task = service.Create(model);
            var id = task.Id;

            service.Complete(id);

            service.Complete(id);

            var Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Equal(status, Task.Status);
        }

        //Проверка корректного отображения статуса выполненной задачи после изменения дедлайна
        [Theory]
        [InlineData("05.05.2025", Status.Late)]
        [InlineData("09.09.2027", Status.Completed)]

        public void Update_Deadline_ChangeStatus(string deadline, Status status)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task", Deadline = DateTime.UtcNow.AddDays(1) };
            var task = service.Create(model);
            var id = task.Id;
            model.Deadline = DateTime.Parse(deadline);

            service.Edit(model, id);

            service.Complete(id);

            var Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Equal(status, Task.Status);
        }

        //Проверка корректного отображения статуса невыполненной задачи после изменения дедлайна
        [Theory]
        [InlineData("05.05.2025", Status.Overdue)]
        [InlineData("09.09.2027", Status.Active)]

        public void Update_Deadline_ChangeStatusOverdueOrActive(string deadline, Status status)
        {
            var service = new ToDoService(_context);
            var model = new TaskModel { Title = "Task", Deadline = DateTime.UtcNow.AddDays(1) };
            var task = service.Create(model);
            var id = task.Id;
            model.Deadline = DateTime.Parse(deadline);

            service.Edit(model, id);

            var Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Assert.Equal(status, Task.Status);
        }



    }
}
