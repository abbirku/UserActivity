using Autofac.Extras.Moq;
using CoreActivities.ActiveProgram;
using CoreActivities.FileManager;
using Infrastructure.ActiveProgram;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Test
{
    [TestFixture]
    public class ActiveProgramServiceTest
    {
        private AutoMock _mock;
        private Mock<IFile> _fileAdapterMoc;
        private Mock<IActiveProgram> _activeProgramAdapterMoc;
        private ActiveProgramService _activeProgramService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _fileAdapterMoc = _mock.Mock<IFile>();
            _activeProgramAdapterMoc = _mock.Mock<IActiveProgram>();
            _activeProgramService = _mock.Create<ActiveProgramService>();
        }

        [TearDown]
        public void Clean()
        {
            _fileAdapterMoc?.Reset();
            _activeProgramAdapterMoc?.Reset();
        }


        [Test, Category("Unit Test")]
        public async Task CaptureActiveProgramTitleAsync_ForValidTextFilePath_StoreActiveWindowTitleInFile()
        {
            //Arrange
            var filePath = "C:\\Test\\file.txt";
            var activeWindow = "vscode.exe";

            _activeProgramAdapterMoc.Setup(x => x.CaptureActiveProgramTitle()).Returns(activeWindow).Verifiable();
            _fileAdapterMoc.Setup(x => x.AppendAllTextAsync(activeWindow, filePath)).Returns(Task.CompletedTask).Verifiable();

            //Act
            await _activeProgramService.CaptureActiveProgramTitleAsync(filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileAdapterMoc.Verify(),
                () => _activeProgramAdapterMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("C:\\file.txt")]
        public void CaptureActiveProgramTitleAsync_ForInvalidFilePath_ThrowException(string filePath)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _activeProgramService.CaptureActiveProgramTitleAsync(filePath));
        }
    }
}
