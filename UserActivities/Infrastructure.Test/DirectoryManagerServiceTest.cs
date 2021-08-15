using Autofac.Extras.Moq;
using Infrastructure.DirectoryManager;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace Infrastructure.Test
{
    [TestFixture]
    public class DirectoryManagerServiceTest
    {
        private AutoMock _mock;
        private Mock<IDirectoryManagerAdapter> _directoryManagerAdapterMock;
        private IDirectoryManagerService _directoryManagerService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _directoryManagerAdapterMock = _mock.Mock<IDirectoryManagerAdapter>();
            _directoryManagerService = _mock.Create<DirectoryManagerService>();
        }

        [TearDown]
        public void Clean()
        {
            _directoryManagerAdapterMock?.Reset();
        }

        [OneTimeTearDown]
        public void ClassClean()
        {
            _mock?.Dispose();
        }

        [Test, Category("Unit Test")]
        public void ChecknCreateDirectory_ForValidPath_ReturnsTrue()
        {
            //Arrange
            var path = "C:\\Test";
            _directoryManagerAdapterMock.Setup(x => x.Exists(path)).Returns(false).Verifiable();
            _directoryManagerAdapterMock.Setup(x => x.CreateDirectory(path)).Verifiable();

            //Act
            var result = _directoryManagerService.ChecknCreateDirectory(path);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _directoryManagerAdapterMock.Verify(),
                () => result.ShouldBe(true)
            );
        }

        [Test, Category("Unit Test")]
        public void ChecknCreateDirectory_ForEmptyString_ReturnsFalse()
        {
            //Act
            var result = _directoryManagerService.ChecknCreateDirectory(string.Empty);

            //Assert
            this.ShouldSatisfyAllConditions(() => _directoryManagerAdapterMock.Verify(), () => result.ShouldBe(false));
        }

        [Test, Category("Unit Test")]
        public void ChecknCreateDirectory_ForExistingPath_ReturnsFalse()
        {
            //Arrange
            var path = "C:\\Test";
            _directoryManagerAdapterMock.Setup(x => x.Exists(path)).Returns(true).Verifiable();

            //Act
            var result = _directoryManagerService.ChecknCreateDirectory(path);

            //Assert
            this.ShouldSatisfyAllConditions(() => _directoryManagerAdapterMock.Verify(), () => result.ShouldBe(false));
        }

        [Test, Category("Unit Test")]
        public void GetProgramDataDirectoryPath_ForValidFolderName_ReturnsProgramDataDirectorOfFolder()
        {
            //Arrange
            var folder = "TestFolder";
            var programData = "C:\\ProgramData";
            _directoryManagerAdapterMock.Setup(x => x.CommonApplicationPath).Returns(programData).Verifiable();

            //Act
            var result = _directoryManagerService.GetProgramDataDirectoryPath(folder);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _directoryManagerAdapterMock.Verify(),
                () => result.ShouldBe($"{programData}\\{folder}")
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetProgramDataDirectoryPath_ForEmptyString_ThrowException(string folder)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _directoryManagerService.GetProgramDataDirectoryPath(folder));
        }

        [Test, Category("Unit Test")]
        public void CreateProgramDataFilePath_ForValidFolderAndFileName_ReturnsProgramDataDirectorOfFile()
        {
            //Arrange
            var folder = "TestFolder";
            var programData = "C:\\ProgramData";
            var file = "img.jpg";
            _directoryManagerAdapterMock.Setup(x => x.CommonApplicationPath).Returns(programData).Verifiable();
            _directoryManagerAdapterMock.Setup(x => x.Exists($"{programData}\\{folder}")).Returns(false).Verifiable();
            _directoryManagerAdapterMock.Setup(x => x.CreateDirectory($"{programData}\\{folder}")).Verifiable();

            //Act
            var result = _directoryManagerService.CreateProgramDataFilePath(folder, file);

            //Assert
            this.ShouldSatisfyAllConditions(() => _directoryManagerAdapterMock.Verify(), () => result.ShouldBe($"{programData}\\{folder}\\{file}"));
        }

        [Test, Category("Unit Test")]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(" ", null)]
        [TestCase(null, "")]
        [TestCase(null, " ")]
        public void CreateProgramDataFilePath_ForInvalidFolderOrFileName_ThrowException(string folderName, string fileName)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _directoryManagerService.CreateProgramDataFilePath(folderName, fileName));
        }
    }
}
