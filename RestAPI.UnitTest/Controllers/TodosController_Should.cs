using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Contracts.Enums;
using Contracts.Models.RequestModels;
using Contracts.Models.ResponseModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Persistence.Models.ReadModels;
using Persistence.Repositories;
using RestAPI.Controllers;
using RestAPI.UnitTest.Attributes;
using Xunit;

namespace RestAPI.UnitTest.Controllers
{
    public class TodosController_Should
    {
        private readonly Mock<HttpContext> _httpContextMock = new Mock<HttpContext>();
        private readonly Mock<ITodosRepository> _todosRepositoryMock = new Mock<ITodosRepository>();
        private readonly Mock<IUserRepository> _usersRepositoryMock = new Mock<IUserRepository>();

        private readonly TodosController _sut;

        public TodosController_Should()
        {
            _sut = new TodosController(_todosRepositoryMock.Object, _usersRepositoryMock.Object)
            {
                ControllerContext =
                {
                    HttpContext = _httpContextMock.Object
                }
            };
        }
        // UnitOfWork_StateUnderTest_ExpectedBehavior
        [Theory, AutoMoqData]
        public async Task GetAllTodoItems_When_GetAllIsCalled(
            List<TodoItemReadModel> todos
            )
        {
            // Arrange

            var userId = SetupHttpContext();

            _todosRepositoryMock.Setup(x => x.GetAllAsync(userId)).ReturnsAsync(todos);

            // Act
            var result = await _sut.GetAll();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<IEnumerable<TodosItemResponse>>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(todos);

            _todosRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task GetTodoItem_When_GetIsCalled_With_TodoIsNull(
            Guid id
            )
        {
            var userId = SetupHttpContext();
            _todosRepositoryMock.Setup(x => x.GetAsync(userId,id)).ReturnsAsync((TodoItemReadModel) null);

            var result = await _sut.Get(id);
            
            result.Should().BeOfType<ActionResult<TodosItemResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"Todo item with id: '{id}' does not exist");
            
            _todosRepositoryMock.Verify(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }
        [Theory, AutoData]
        public async Task Get_ReturnsTodoItem_When_IdParameterIsPassed(
            Guid id,
            TodoItemReadModel todoItemReadModel)
        {
            // Arrange
            var userId = SetupHttpContext();

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.GetAsync(id, userId))
                .ReturnsAsync(todoItemReadModel);

            // Act
            var result = await _sut.Get(id);

            //Assert
            result.Should().BeOfType<ActionResult<TodosItemResponse>>()
                .Which.Value.Should().BeOfType<TodosItemResponse>()
                .Which.Should().BeEquivalentTo(todoItemReadModel);

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }


        [Theory, AutoData]
        public async Task Create_TodoItem__When_CreateTodoItemRequest_Recivede(
            TodoItemReadModel todoItemReadModel,
            CreateTodoItemRequest request)
        {
            // Arrange
            var userId = SetupHttpContext();


            todoItemReadModel.UserId = userId;
            todoItemReadModel.Title = request.Title;
            todoItemReadModel.Description = request.Description;
            todoItemReadModel.Difficulty = request.Difficulty;
            todoItemReadModel.IsDone = false;

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.SaveOrUpdateAsync(It.IsAny<TodoItemReadModel>()));
                

            // Act
            var result = await _sut.Create(request);

            // Assert
            result.Should().BeOfType<ActionResult<TodosItemResponse>>()
                .Which.Result.Should().BeOfType<CreatedAtActionResult>()
                .Which.Value.Should().BeOfType<TodosItemResponse>()
                .Which.Should().BeEquivalentTo(todoItemReadModel, option => option
                .Excluding(model => model.Id)
                .Excluding(model => model.DateCreated));

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository
                .SaveOrUpdateAsync(It.Is<TodoItemReadModel>(model => model.UserId.Equals(todoItemReadModel.UserId)
                && model.Title.Equals(todoItemReadModel.Title)
                && model.Description.Equals(todoItemReadModel.Description)
                && model.Difficulty.Equals(todoItemReadModel.Difficulty)
                && model.IsDone.Equals(todoItemReadModel.IsDone))), Times.Once);
        }

        [Theory, AutoData]
        public async Task Update_ReturnsNotFoundObjectResult_When_TodoItemIsNull(
            Guid id,
            UpdateTodoItemRequest request)
        {
            // Arrange
            var userId = SetupHttpContext();

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((TodoItemReadModel)null);

            // Act
            var result = await _sut.Update(id, request);

            //Assert
            result.Should().BeOfType<ActionResult<TodosItemResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"Todo item with id: '{id}' does not exist");

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.GetAsync(id, userId), Times.Once);
        }

        [Theory, AutoData]
        public async Task Update_ReturnsTodoItem_When_IdParameterIsPassed(
            Guid id,
            UpdateTodoItemRequest request,
            TodoItemReadModel todoItemReadModel)
        {
            // Arrange
            var userId = SetupHttpContext();
            todoItemReadModel.Id = id;
            todoItemReadModel.UserId = userId;

            

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.GetAsync(id, userId))
                .ReturnsAsync(todoItemReadModel);

            // Act
            var result = await _sut.Update(id, request);

            //Assert
            result.Should().BeOfType<ActionResult<TodosItemResponse>>()
                .Which.Value.Should().BeOfType<TodosItemResponse>()
                .Which.Should().BeEquivalentTo(todoItemReadModel);

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.SaveOrUpdateAsync(todoItemReadModel), Times.Once);
        }

        [Theory, AutoData]
        public async Task UpdateStatus_ReturnsNotFoundObjectResult_When_TodoItemIsNull(
            Guid id)
        {
            // Arrange
            var userId = SetupHttpContext();

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((TodoItemReadModel)null);

            // Act
            var result = await _sut.UpdateStatus(id);

            //Assert
            result.Should().BeOfType<ActionResult<TodosItemResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"Todo item with id: '{id}' does not exist");

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.GetAsync(id, userId), Times.Once);
        }

        [Theory, AutoData]
        public async Task UpdateStatus_ReturnsTodoItem_When_IdParameterIsPassed(
            Guid id,
            TodoItemReadModel todoItemReadModel)
        {
            // Arrange
            var userId = SetupHttpContext();
            todoItemReadModel.Id = id;
            todoItemReadModel.UserId = userId;

            

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.GetAsync(id, userId))
                .ReturnsAsync(todoItemReadModel);

            // Act
            var result = await _sut.UpdateStatus(id);

            //Assert
            result.Should().BeOfType<ActionResult<TodosItemResponse>>()
                .Which.Value.Should().BeOfType<TodosItemResponse>()
                .Which.Should().BeEquivalentTo(todoItemReadModel);

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.SaveOrUpdateAsync(todoItemReadModel), Times.Once);
        }

        [Theory, AutoData]
        public async Task Delete_ReturnsNotFoundObjectResult_When_TodoItemIsNull(
            Guid id)
        {
            // Arrange
            var userId = SetupHttpContext();

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((TodoItemReadModel)null);

            // Act
            var result = await _sut.Delete(id);

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"Todo item with id: '{id}' does not exist");

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.GetAsync(id, userId), Times.Once);
        }

        [Theory, AutoData]
        public async Task Delete_ReturnsNoContent_When_IdParameterIsPassed(
            Guid id,
           TodoItemReadModel todoItemReadModel)
        {
            // Arrange
            var userId = SetupHttpContext();

            _todosRepositoryMock
                .Setup(todosRepository => todosRepository.GetAsync(id, userId))
                .ReturnsAsync(todoItemReadModel);

            // Act
            var result = await _sut.Delete(id);

            //Assert
            result.Should().BeOfType<NoContentResult>();

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);

            _todosRepositoryMock
                .Verify(todosRepository => todosRepository.DeleteAsync(id), Times.Once);
        }

        private Guid SetupHttpContext()
        {
            var userId = Guid.NewGuid();
            
            _httpContextMock.SetupGet(httpContext => httpContext.Items["userId"]).Returns(userId);
            
            return userId;
        }
    }
}