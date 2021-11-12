using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Persistence.Models.ReadModels;
using Persistence.Repositories;
using RestAPI.Models;
using RestAPI.Options;
using RestAPI.Services;
using RestAPI.UnitTest.Attributes;
using Xunit;

namespace RestAPI.UnitTest.Services
{
    public class ApikeyService_Should
    {
     [Theory, AutoMoqData]
        public async Task CreateApiKey_ReturnsBadHttpException_When_UserIsNull(
            string username,
            string password,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            ApikeyService sut)
        {
            userRepositoryMock.Setup(mock => mock.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((UserReadModel) null);
            
            // Act &  // Assert
            /*var result = await sut.CreateApiKey(username, password);*/
            var result = await sut.Invoking(sut => sut.CreateApiKey(username, password))
                .Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage($"User with Username: {username} does not exists!");

            result.Which.StatusCode.Should().Be(404);
            userRepositoryMock.Verify(userRepositoryMock =>
                userRepositoryMock.GetAsync(username), Times.Once);
        }
        [Theory, AutoMoqData]
        public async Task CreateApiKey_ReturnsBadHttpException_When_PasswordIsWrong(
            string username,
            string password,
            UserReadModel userReadModel,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            ApikeyService sut)
        {
            userRepositoryMock
                .Setup(mock => mock.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(userReadModel);
            
            // Act &  // Assert
            /*var result = await sut.CreateApiKey(username, password);*/
            var result = await sut
                .Invoking(sut => sut.CreateApiKey(username, password))
                .Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage($"Wrong password for user: {userReadModel.UserName}");

            result.Which.StatusCode.Should().Be(400);
            
            userRepositoryMock.Verify(userRepository =>
                userRepository.GetAsync(username), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task CreateApiKey_ReturnsBadHttpException_When_ApiKeyLimit_Is_Reached(
            string username,
            string password,
            UserReadModel userReadModel,
            ApiKeySettings apiKeySettings,
            IEnumerable<ApiKeyReadModel> apiKeys,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            [Frozen] Mock<IApiKeyRepository> apiKeuRepositoryMock,
            [Frozen] Mock<IOptions<ApiKeySettings>> apiKeySettingMock,
            ApikeyService sut)
        {
            //  Arrange
            userRepositoryMock
                .Setup(mock => mock.GetAsync(userReadModel.UserName))
                .ReturnsAsync(userReadModel);
            apiKeuRepositoryMock
                .Setup(mock => mock.GetApiKeysAsync(userReadModel.Id))
                .ReturnsAsync(apiKeys);
            apiKeySettings.ApiKeyLimit = apiKeys.Count();

            apiKeySettingMock
                .Setup(mock => mock.Value)
                .Returns(apiKeySettings);
            //  Act &&  Assert
            
            var result = await sut
                .Invoking(sut => sut.CreateApiKey(userReadModel.UserName, userReadModel.Password))
                .Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage($"Api key limit is reached");
           
            result.Which.StatusCode.Should().Be(400);
            
            userRepositoryMock.Verify(userRepository => userRepository.GetAsync(It.IsAny<string>()), Times.Once);

            apiKeuRepositoryMock.Verify(mock => mock.GetApiKeysAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task CreateApiKey_When_AllChecksPass(
            UserReadModel userReadModel,
            ApiKeySettings apiKeySettings,
            IEnumerable<ApiKeyReadModel> apiKeys,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            [Frozen] Mock<IApiKeyRepository> apiKeyRepositoryMock,
            [Frozen] Mock<IOptions<ApiKeySettings>> apiKeySettingsMock,
            ApikeyService sut)
        {
            // Arrange
            userRepositoryMock
                .Setup(mock => mock.GetAsync(userReadModel.UserName))
                .ReturnsAsync(userReadModel);

            apiKeyRepositoryMock
                .Setup(mock => mock.GetApiKeysAsync(userReadModel.Id))
                .ReturnsAsync(apiKeys);

            apiKeySettings.ApiKeyLimit = apiKeys.Count() + 1;
            
            apiKeySettingsMock
                .SetupGet(mock => mock.Value)
                .Returns(apiKeySettings);
            
            // Act
            var result = await sut.CreateApiKey(userReadModel.UserName, userReadModel.Password);
            
            // Assert
            userRepositoryMock.Verify(userRepository => userRepository.GetAsync(It.IsAny<string>()), Times.Once);
            
            apiKeyRepositoryMock.Verify(mock => mock.GetApiKeysAsync(It.IsAny<Guid>()), Times.Once);
            
            apiKeyRepositoryMock
                .Verify(mock => mock.SaveAsync(It.Is<ApiKeyReadModel>(model => 
                    model.UserId.Equals(userReadModel.Id) &&
                    model.IsActive)));

            result.UserId.Should().Be(userReadModel.Id);
            result.IsActive.Should().BeTrue();
        }
        [Theory, AutoMoqData]
        public async Task GetAllApiKey_ReturnsBadHttpException_When_UserIsNull(string username,
            string password,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            ApikeyService sut)
        {
            userRepositoryMock.Setup(mock => mock.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((UserReadModel) null);
            
            // Act &  // Assert
            /*var result = await sut.CreateApiKey(username, password);*/
            var result = await sut.Invoking(sut => sut.GetAllApiKey(username, password))
                .Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage($"User with Username: {username} does not exists!");

            result.Which.StatusCode.Should().Be(404);
            userRepositoryMock.Verify(userRepositoryMock =>
                userRepositoryMock.GetAsync(username), Times.Once);
        }
        [Theory, AutoMoqData]
        public async Task GetAllApiKey_ReturnsBadHttpException_When_PasswordIsWrong(
            string username,
            string password,
            UserReadModel userReadModel,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            ApikeyService sut)
        {
            userRepositoryMock
                .Setup(mock => mock.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(userReadModel);
            
            // Act &  // Assert
            /*var result = await sut.CreateApiKey(username, password);*/
            var result = await sut
                .Invoking(sut => sut.GetAllApiKey(username, password))
                .Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage($"Wrong password for user: {userReadModel.UserName}");

            result.Which.StatusCode.Should().Be(400);
            
            userRepositoryMock.Verify(userRepository =>
                userRepository.GetAsync(username), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task GetAllApiKey_When_AllChecksPass(
            string username,
            string password,
            UserReadModel userReadModel,
            List<ApiKeyReadModel> apiKeys,
            [Frozen] Mock<IUserRepository> userRepositoryMock,
            [Frozen] Mock<IApiKeyRepository> apiKeyRepositoryMock,
            ApikeyService sut)
        {
            // Arrange
            userReadModel.Password = password;
            userRepositoryMock
                .Setup(mock => mock.GetAsync(username))
                .ReturnsAsync(userReadModel);

            apiKeyRepositoryMock
                .Setup(mock => mock.GetApiKeysAsync(userReadModel.Id))
                .ReturnsAsync(apiKeys);

            // Act
            var result = (await sut.GetAllApiKey(username, password)).ToList();

            // Assert
            userRepositoryMock.Verify(userRepository => userRepository.GetAsync(It.IsAny<string>()), Times.Once);

            apiKeyRepositoryMock.Verify(mock => mock.GetApiKeysAsync(It.IsAny<Guid>()), Times.Once);

            result.Should().BeEquivalentTo(apiKeys, options => options.ComparingByMembers<ApiKeyModel>());
        }
    
        [Theory, AutoMoqData]
        public async Task UpdateApiKey_ReturnsBadHttpException_When_ApiKeyIsNull(Guid id, 
            bool state,
            [Frozen] Mock<IApiKeyRepository> apiKeyRepositoryMock,
            ApikeyService sut)
        {
            apiKeyRepositoryMock.Setup(mock => mock.GetByApiKeyIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ApiKeyReadModel) null);

            // Act &  // Assert
            /*var result = await sut.CreateApiKey(username, password);*/
            var result = await sut.Invoking(sut => sut.UpdateApiKey(id, state))
                .Should()
                .ThrowAsync<BadHttpRequestException>()
                .WithMessage($"Api key with Id: {id} does not exists");

            result.Which.StatusCode.Should().Be(404);
            apiKeyRepositoryMock.Verify(apikeyRepository => apikeyRepository.GetByApiKeyIdAsync(id), Times.Once);
        }
        
        [Theory, AutoMoqData]
        public async Task UpdateApiKey_ReturnsBadHttpException_When_AllChecksPass(Guid id, 
            bool state,
            ApiKeyReadModel apiKey,
            [Frozen] Mock<IApiKeyRepository> apiKeyRepositoryMock,
            ApikeyService sut)
        {
            apiKey.Id = id;
            apiKey.IsActive = state;
            apiKeyRepositoryMock.Setup(mock => mock.UpdateIsActive(It.IsAny<Guid>(), It.IsAny<bool>()));
            apiKeyRepositoryMock.Setup(mock => mock.GetByApiKeyIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(apiKey);

            // Act &  // Assert
            /*var result = await sut.CreateApiKey(username, password);*/
            var result = await sut.UpdateApiKey(id, state);
            apiKeyRepositoryMock.Verify(apikeyRepository => apikeyRepository.UpdateIsActive(id,state), Times.Once);
            apiKeyRepositoryMock.Verify(api => api.GetByApiKeyIdAsync(id), Times.Exactly(2));


            result.Id.Should().Be(id);
            result.IsActive.Should().Be(state);
        }
    }
}