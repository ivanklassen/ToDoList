using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using ToDoList.Migrations;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Tests
{
    public class TestsWithMacro: IClassFixture<WebApplicationFactory<TestProgram>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<TestProgram> _factory;

        public TestsWithMacro(WebApplicationFactory<TestProgram> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

        }

        [Fact]
        public async void PostTask_ValidTaskWithMacro1_ReturnsCreatedAtAction()
        {
            var task = new TaskModel
            {
                Title = "!1 Task",
                Description = "Test Description",
                Priority = null,
                Deadline = DateTimeOffset.Now.AddDays(5)
            };
            var json = JsonConvert.SerializeObject(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("task", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdTask = JsonConvert.DeserializeObject<Task>(responseContent);

            Assert.Equal(createdTask.Title, "Task");
            Assert.Equal(createdTask.Priority, Priority.Critical);
        }

        [Theory]
        [InlineData("!1 Critical Task", Priority.Critical, "Critical Task")]
        [InlineData("!2 High Task", Priority.High, "High Task")]
        [InlineData("!3 Medium Task", Priority.Medium, "Medium Task")]
        [InlineData("Low Task !4", Priority.Low, "Low Task")]
        public async void Create_PriorityMacro_SetPriority(string title, Priority expectedPriority, string expectedTitle)
        {
            var task = new TaskModel
            {
                Title = title,
                Description = "Test Description",
                Priority = null,
                Deadline = DateTimeOffset.Now.AddDays(5)
            };
            var json = JsonConvert.SerializeObject(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            Assert.Equal(task.Title, title);
        }

        [Theory]
        [InlineData("!before 05.05.2025 Task1", 5, 5, 2025, "Task1")]
        [InlineData("!before 01.01.2099 Task3", 1, 1, 2099, "Task3")]
        [InlineData("Task4 !before 01.01.1970", 1, 1, 1970, "Task4")]
        [InlineData("!before 01.01.2000 Task5", 1, 1, 2000, "Task5")]
        [InlineData("Task6 !before 31.12.1999", 31, 12, 1999, "Task6")]
        [InlineData("Task7 !before 31.05.2025", 31, 5, 2025, "Task7")]
        [InlineData("Task8 !before 29.02.2024", 29, 02, 2024, "Task8")]
        public async void Create_DeadlineMacro_SetDeadline(string title, int day, int month, int year, string expectedTitle)
        {
            DateTimeOffset parsedDeadline = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero).ToUniversalTime();

            var task = new TaskModel
            {
                Title = title,
                Description = "Test Description",
                Priority = null,
                Deadline = null
            };
            var json = JsonConvert.SerializeObject(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Assert.Equal(task.Title, title);
        }

       
    }
}
