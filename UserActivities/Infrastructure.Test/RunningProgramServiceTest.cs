using Autofac.Extras.Moq;
using Infrastructure.Services;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CoreActivities.RunningPrograms;
using CoreActivities.FileManager;

namespace Infrastructure.Test
{
    [TestFixture]
    public class RunningProgramServiceTest
    {
        private AutoMock _mock;
        private Mock<IFile> _fileAdapterMoc;
        private Mock<IRunningPrograms> _runningProgramAdapterMoc;
        private RunningProgramService _runningProgramService;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _mock = AutoMock.GetLoose();
        }

        [SetUp]
        public void SetUp()
        {
            _fileAdapterMoc = _mock.Mock<IFile>();
            _runningProgramAdapterMoc = _mock.Mock<IRunningPrograms>();
            _runningProgramService = _mock.Create<RunningProgramService>();
        }

        [TearDown]
        public void Clean()
        {
            _fileAdapterMoc?.Reset();
            _runningProgramAdapterMoc?.Reset();
        }

        [Test, Category("Unit Test")]
        public async Task CaptureProcessNameAsync_ForValidTextFilePath_StoreCaptureProcessesInFile()
        {
            //Arrange
            var filePath = "C:\\Test\\file.txt";
            var processes = new List<string>
            {
                "Process1",
                "Process2"
            };
            var processed = processes.Select((x, index)=> {
                return $"{index + 1}. {x}";
            }).ToList();

            _runningProgramAdapterMoc.Setup(x => x.GetRunningProcessList()).Returns(processes).Verifiable();
            _fileAdapterMoc.Setup(x => x.AppendAllLineAsync(processed, filePath)).Returns(Task.CompletedTask).Verifiable();

            //Act
            await _runningProgramService.CaptureProcessNameAsync(filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileAdapterMoc.Verify(),
                () => _runningProgramAdapterMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("C:\\file.txt")]
        public void CaptureProcessNameAsync_ForInvalidFilePath_ThrowException(string filePath)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _runningProgramService.CaptureProcessNameAsync(filePath));
        }

        [Test, Category("Unit Test")]
        public async Task CaptureProgramTitleAsync_ForValidTextFilePath_StoreCaptureProcessesInFile()
        {
            //Arrange
            var filePath = "C:\\Test\\file.txt";
            var processes = new List<string>
            {
                "Title1",
                "Title2"
            };
            var processed = processes.Select((x, index) => {
                return $"{index + 1}. {x}";
            }).ToList();

            _runningProgramAdapterMoc.Setup(x => x.GetRunningProcessList()).Returns(processes).Verifiable();
            _fileAdapterMoc.Setup(x => x.AppendAllLineAsync(processed, filePath)).Returns(Task.CompletedTask).Verifiable();

            //Act
            await _runningProgramService.CaptureProcessNameAsync(filePath);

            //Assert
            this.ShouldSatisfyAllConditions(
                () => _fileAdapterMoc.Verify(),
                () => _runningProgramAdapterMoc.Verify()
            );
        }

        [Test, Category("Unit Test")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("C:\\file.txt")]
        public void CaptureProgramTitleAsync_ForInvalidFilePath_ThrowException(string filePath)
        {
            //Act & Assert
            Should.Throw<Exception>(() => _runningProgramService.CaptureProcessNameAsync(filePath));
        }
    }
}
