using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using ToDoList.Migrations;
using Xunit;

namespace ToDoList.Tests
{
    [Collection("NonParallelCollection")]
    public class ToDoApiTests: IClassFixture<WebApplicationFactory<TestProgram>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<TestProgram> _factory;

        public ToDoApiTests(WebApplicationFactory<TestProgram> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

        }

        private async Task<Task> CreateTestTodoItem()
        {
            var todoItem = new Task
            {
                Title = "Test Task",
                Description = "Test Description",
                Priority = Priority.Medium,
                Deadline = DateTimeOffset.Now.AddDays(7)
            };
            var json = JsonConvert.SerializeObject(todoItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("task", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdItem = JsonConvert.DeserializeObject<Task>(responseContent);

            Console.WriteLine(todoItem.Title);
            Console.WriteLine(todoItem.Id);

            return createdItem;

        }

        ////Редактирование существующей задачи
        //[Fact]
        //public async void PutTask_ExistingId_ReturnsOk()
        //{
        //    var todoItem = new Task
        //    {
        //        Title = "Updated Task",
        //        Description = "Updated Description",
        //        Priority = Priority.High,
        //        Deadline = DateTimeOffset.Now.AddDays(14)
        //    };
        //    var json = JsonConvert.SerializeObject(todoItem);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");


        //    //Создание задачи
        //    var taskItem = new TaskModel
        //    {
        //        Title = "New Task",
        //        Description = "Test Description",
        //        Priority = Priority.Medium,
        //        Deadline = DateTimeOffset.Now.AddDays(5)
        //    };
        //    var json_ = JsonConvert.SerializeObject(taskItem);
        //    var content_ = new StringContent(json_, Encoding.UTF8, "application/json");

        //    var response_ = await _client.PostAsync("task", content_);
        //    var responseContent_ = await response_.Content.ReadAsStringAsync();
        //    var createdTask = JsonConvert.DeserializeObject<Task>(responseContent_);

        //    var id = createdTask.Id;


        //    var response = await _client.PutAsync($"task/{id}", content);

        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //    var responseContent = await response_.Content.ReadAsStringAsync();
        //    var updatedTask = JsonConvert.DeserializeObject<Task>(responseContent);

        //    Assert.Equal(todoItem.Title, "Updated Task");
        //    Assert.Equal(todoItem.Description, "Updated Description");
        //    Assert.Equal(todoItem.Priority, Priority.High);

        //}


        //Получение всех задач
        [Fact]
        public async void GetToDo_ReturnOkWithListOfTasks()
        {
            var response = await _client.GetAsync("tasks");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

            var content = await response.Content.ReadAsStringAsync();
            var todoItems = JsonConvert.DeserializeObject<List<Task>>(content);

            Assert.True(todoItems.Count >= 0);

        }

        //Получение одной задачи
        [Fact]
        public async void GetToDo_ReturnOkWithTask()
        {
            var response1 = await _client.GetAsync("tasks");
            var content1 = await response1.Content.ReadAsStringAsync();
            var todoItems = JsonConvert.DeserializeObject<List<Task>>(content1);
            var id = todoItems.Last().Id;
            var response = await _client.GetAsync($"task/{id}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

            var content = await response.Content.ReadAsStringAsync();
            var todoItem = JsonConvert.DeserializeObject<Task>(content);

            Assert.NotNull(todoItem);

        }

        //Проверка запроса на получение задачи с несуществующим id 
        [Fact]
        public async void GetToDo_ReturnMotFound()
        {
            var id = Guid.NewGuid();

            var response = await _client.GetAsync($"task/{id}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        //Создание валидной задачи
        [Fact]
        public async void PostTask_ValidTask_ReturnsCreatedAtAction()
        {
            var task = new TaskModel
            {
                Title = "New Task",
                Description = "Test Description",
                Priority = Priority.Medium,
                Deadline = DateTimeOffset.Now.AddDays(5)
            };
            var json = JsonConvert.SerializeObject(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("task", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdTask = JsonConvert.DeserializeObject<Task>(responseContent);

            Assert.Equal(task.Title, createdTask.Title);
            Assert.Equal(task.Description, createdTask.Description);
        }

        //Проверка создания задачи с недопустимым названием
        [Theory]
        [InlineData(null)]
        [InlineData("abc")]
        public async void PostTask_InvalidTask_ReturnsBadRequest(string title)
        {
            var todoItem = new Task
            {
                Title = title,
                Description = "Test Description",
                Priority = Priority.Medium,
                Deadline = DateTimeOffset.Now.AddDays(5)
            };
            var json = JsonConvert.SerializeObject(todoItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("task", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        ////Редактирование задачи
        //[Fact]
        //public async void PutTask_NonExistingId_ReturnsNotFound()
        //{
        //    var nonExistingId = Guid.NewGuid();
        //    var todoItem = new Task
        //    {
        //        Title = "Updated Task",
        //        Description = "Updated Description",
        //        Priority = Priority.High,
        //        Deadline = DateTimeOffset.Now.AddDays(14)
        //    };
        //    var json = JsonConvert.SerializeObject(todoItem);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _client.PutAsync($"task/{nonExistingId}", content);

        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //}


        //Удаление задачи
        [Fact]
        public async void DeleteTask_ExistingId_ReturnsOk()
        {
            var response1 = await _client.GetAsync("tasks");
            var content = await response1.Content.ReadAsStringAsync();
            var todoItems = JsonConvert.DeserializeObject<List<Task>>(content);
            var id = todoItems.First().Id;

            var response = await _client.DeleteAsync($"task/{id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        //Проверка удаления несуществующей задачи
        [Fact]
        public async void DeleteTask_NonExistingId_ReturnsNotFound()
        {
            var nonExistingId = Guid.NewGuid();

            var response = await _client.DeleteAsync($"task/{nonExistingId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        //Проверка выполнения несуществующей задачи
        public async void CompleteTask_NonExistingId_ReturnsBadRequest()
        {
            var nonExistingId = "4b98aca4-f161-4415-a15d-7dee9abc698";

            var response = await _client.GetAsync($"task/complete/{nonExistingId}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        //Проверка выполнения существующей задачи
        public async void CompleteTask_ReturnsOk()
        {
            var response1 = await _client.GetAsync("tasks");
            var content = await response1.Content.ReadAsStringAsync();
            var todoItems = JsonConvert.DeserializeObject<List<Task>>(content);
            var id = todoItems.First().Id;
            var nonExistingId = Guid.NewGuid();

            var response = await _client.GetAsync($"task/complete/{id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
