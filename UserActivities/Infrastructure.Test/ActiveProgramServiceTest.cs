using Autofac.Extras.Moq;
using Infrastructure.ActiveProgram;
using Infrastructure.FileManager;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Test
{
    [TestFixture]
    public class ActiveProgramServiceTest
    {
        private AutoMock _mock;
        private Mock<IFileAdapter> _fileAdapterMoc;
        private Mock<IActiveProgramAdapter> _activeProgramAdapterMoc;
        private ActiveProgramService _activeProgramService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _fileAdapterMoc = _mock.Mock<IFileAdapter>();
            _activeProgramAdapterMoc = _mock.Mock<IActiveProgramAdapter>();
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

            _activeProgramAdapterMoc.Setup(x => x.GetActiveWindowTitle()).Returns(activeWindow).Verifiable();
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
