// References
// https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database
// https://stackoverflow.com/a/52782777

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Newtonsoft.Json;
using ST10445832_PROG6212_POE.Areas.Approver.Controllers;
using ST10445832_PROG6212_POE.Areas.HR.Models;
using ST10445832_PROG6212_POE.Areas.Lecturer.Controllers;
using ST10445832_PROG6212_POE.Areas.Lecturer.Models;
using ST10445832_PROG6212_POE.Data;
using ST10445832_PROG6212_POE.Hubs;
using ST10445832_PROG6212_POE.Models;
using System.Net.Mime;
using System.Security.Claims;

namespace ST10445832_PROG6212_POE.UnitTests
{
    [TestClass]
    public sealed class ClaimSystemTest
    {
        //==================================================================//
        // Tests that the correct data is loaded into the create claim view
        [TestMethod]
        public async Task CreateView_CorrectModules_ReturnsTrue()
        {
            // Arrange
            // Copilot generated test data
            var modules = new List<CourseModuleWithCodes>
            {
                new CourseModuleWithCodes
                {
                    CourseModuleId = 1,
                    ModuleId = 1,
                    ModuleCode = "CS101",
                },
                new CourseModuleWithCodes
                {
                    CourseModuleId = 2,
                    ModuleId = 2,
                    ModuleCode = "CS102",
                },
                new CourseModuleWithCodes
                {
                    CourseModuleId = 3,
                    ModuleId = 3,
                    ModuleCode = "CS103",
                },
            };
            // End Copilot

            var mockRepository = new Mock<ILecturerRepository>();

            mockRepository.Setup(repo => repo.GetLecturerModulesAsync("lecturer123"))
                .Returns(Task.FromResult(new List<CourseModuleWithCodes>
                {
                    modules[0],
                    modules[1]
                }));
            // Copilot - create mock user
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "lecturer123") }, "mock"));
            // End Copilot
            var controller = new ClaimController(mockRepository.Object);
            // Copilot - set controller context with mock user
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = mockUser } };
            // End Copilot

            // Act
            var result = await controller.Create() as ViewResult;
            var model = result.Model as CreateClaimViewModel;

            var expectedModules = new List<CourseModuleWithCodes> { modules[0], modules[1] };
            var resultModules = model.Modules;

            // Assert
            // check that the getlecturermodules method was called
            mockRepository.Verify(repo => repo.GetLecturerModulesAsync("lecturer123"));
            Console.WriteLine("GetLecturerModules Called");

            Assert.IsNotNull(result);
            Console.WriteLine("Create view not null");

            CollectionAssert.AreEquivalent(expectedModules, resultModules);
            Console.WriteLine("Correct modules listed");
        }

        //====================================================================//
        // Tests that a new claim is created successfully with valid data
        [TestMethod]
        public async Task CreateNewClaim_Valid_ReturnsTrue()
        {
            // Arrange
            IFormFile testFile;
            using (var stream = new MemoryStream())
            {
                testFile = new FormFile(new MemoryStream(), 0, 1024, "testfile", "testfile.pdf")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                };
            }


            // copilot generated test data
            CreateClaimViewModel viewModel = new CreateClaimViewModel
            {
                LecturerID = "lecturer123",
                CourseModuleID = 1,
                HoursWorked = 10,
                HourlyRate = 50,
                AdditionalNotes = "Test claim",
                SupportingDocuments = new List<IFormFile>
                {
                    testFile
                },
                TempFiles = new List<TempFile>()
            };

            Models.Claim claim = new Models.Claim
            {
                ClaimID = 1,
                LecturerID = viewModel.LecturerID,
                LecturerModuleID = 1,
                HoursWorked = viewModel.HoursWorked,
                HourlyRate = viewModel.HourlyRate,
                AdditionalNotes = viewModel.AdditionalNotes,
                StatusID = 1
            };

            var mockRepository = new Mock<ILecturerRepository>();
            mockRepository.Setup(repo => repo.CreateClaimObjectAsync(viewModel, viewModel.LecturerID))
                .Returns(Task.FromResult(claim));
            mockRepository.Setup(repo => repo.AddClaimAsync(claim))
                .Returns(Task.FromResult(claim));
            mockRepository.Setup(repo => repo.AddDocumentAsync(viewModel.SupportingDocuments[0], 1));
            mockRepository.Setup(repo => repo.SaveChangesAsync());

            // Copilot - create mock user
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "lecturer123") }, "mock"));
            // End Copilot
            var controller = new ClaimController(mockRepository.Object);
            // Copilot - set controller context with mock user
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = mockUser } };
            // End Copilot

            // Act
            var result = await controller.Create(viewModel) as RedirectToActionResult;
            var expectedAction = "ClaimCreated";
            var resultAction = result.ActionName;

            // Assert
            mockRepository.Verify(repo => repo.CreateClaimObjectAsync(viewModel, viewModel.LecturerID));
            Console.WriteLine("CreateClaimObject Called");

            mockRepository.Verify(repo => repo.AddClaimAsync(claim));
            Console.WriteLine("AddClaim Called");

            mockRepository.Verify(repo => repo.AddDocumentAsync(viewModel.SupportingDocuments[0], 1));
            Console.WriteLine("AddDocument Called");

            mockRepository.Verify(repo => repo.SaveChangesAsync());
            Console.WriteLine("SaveChanges Called");

            Assert.AreEqual(expectedAction, resultAction);
        }

        //====================================================================//
        // tests that a new claim is not created with invalid data
        [TestMethod]
        public async Task CreateNewClaim_InvalidFormData_ReturnsTrue()
        {
            // Arrange
            IFormFile testFile;
            using (var stream = new MemoryStream())
            {
                testFile = new FormFile(new MemoryStream(), 0, 1024, "testfile", "testfile.pdf")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = MediaTypeNames.Text.Csv
                };
            }


            // copilot generated test data
            CreateClaimViewModel viewModel = new CreateClaimViewModel
            {
                LecturerID = "lecturer123",
                CourseModuleID = 0,
                HoursWorked = -1,
                HourlyRate = -1,
                AdditionalNotes = "Test claim",
                SupportingDocuments = new List<IFormFile>
                {
                    testFile
                },
                TempFiles = new List<TempFile>()
            };

            Models.Claim claim = new Models.Claim
            {
                ClaimID = 1,
                LecturerID = viewModel.LecturerID,
                LecturerModuleID = 1,
                HoursWorked = viewModel.HoursWorked,
                HourlyRate = viewModel.HourlyRate,
                AdditionalNotes = viewModel.AdditionalNotes,
                StatusID = 1
            };

            var mockRepository = new Mock<ILecturerRepository>();

            // Copilot - create mock user
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "lecturer123") }, "mock"));
            // End Copilot
            var controller = new ClaimController(mockRepository.Object);
            // Copilot - set controller context with mock user
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = mockUser } };
            // End Copilot

            // Act
            var result = await controller.Create(viewModel) as ViewResult;

            var expectedDocumentValidationState = "Invalid";
            var resultDocumentValidationState = controller.ModelState.GetFieldValidationState("SupportingDocuments");


            var expectedModuleValidationState = "Invalid";
            var resultModuleValidationState = controller.ModelState.GetFieldValidationState("ModuleID");

            var expectedHoursWorkedValidationState = "Invalid";
            var resultHoursWorkedtValidationState = controller.ModelState.GetFieldValidationState("HoursWorked");

            var expectedHourlyRateValidationState = "Invalid";
            var resultHourlyRateValidationState = controller.ModelState.GetFieldValidationState("HourlyRate");

            // Assert
            // document detected as invalid
            Console.WriteLine("Document Validation State");
            Console.WriteLine($"Expected: {expectedDocumentValidationState}, Actual: {resultDocumentValidationState.ToString()}");
            Assert.AreEqual(expectedDocumentValidationState, resultDocumentValidationState.ToString());
            // module id detected as invalid
            Console.WriteLine("ModuleID Validation State");
            Console.WriteLine($"Expected: {expectedModuleValidationState}, Actual: {resultModuleValidationState.ToString()}");
            Assert.AreEqual(expectedModuleValidationState, resultModuleValidationState.ToString());
            // hours worked detected as invalid
            Console.WriteLine("HoursWorked Validation State");
            Console.WriteLine($"Expected: {expectedHoursWorkedValidationState}, Actual: {resultHoursWorkedtValidationState.ToString()}");
            Assert.AreEqual(expectedHoursWorkedValidationState, resultHoursWorkedtValidationState.ToString());
            // hourly rate detected as invalid
            Console.WriteLine("HourlyRate Validation State");
            Console.WriteLine($"Expected: {expectedHourlyRateValidationState}, Actual: {resultHourlyRateValidationState.ToString()}");
            Assert.AreEqual(expectedHourlyRateValidationState, resultHourlyRateValidationState.ToString());
        }

        //====================================================================//
        // tests that a claim can be verified successfully
        // teast that error handling works when attempting to verify a
        // non-existent claim
        [TestMethod]
        public async Task VerifyClaim_Verify_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<IApproverRepository>();
            // Copilot
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);
            var mockHubContext = new Mock<IHubContext<StatusHub>>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            var statusListener = new StatusListener(mockHubContext.Object);
            // End Copilot

            mockRepository.Setup(repo => repo.GetClaimAsync(1))
                .ReturnsAsync(new Models.Claim
                {
                    ClaimID = 1,
                    AdditionalNotes = "Test Claim",
                    HoursWorked = 10,
                    HourlyRate = 50,
                    StatusID = 1
                });
            mockRepository.Setup(repo => repo.GetStatusIdAsync("Verified"))
                .ReturnsAsync(2);
            mockRepository.Setup(repo => repo.AddClaimApprovalAsync(It.IsAny<ClaimApproval>()))
                .Returns(Task.CompletedTask);
            mockRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Copilot - create mock user
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "lecturer123") }, "mock"));
            // End Copilot
            var controller = new VerifyClaimsController(mockRepository.Object, statusListener);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = mockUser } };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>()); // Copilot autocomplete

            // Act

            await controller.VerifyClaim(1);
            var toastStringOne = controller.TempData["ToastData"] as string;
            ToastPartialData toastDataOne = JsonConvert.DeserializeObject<ToastPartialData>(toastStringOne);
            var resultOne = toastDataOne.ToastType;
            var expectedOne = true;

            await controller.VerifyClaim(2);
            var toastStringTwo = controller.TempData["ToastData"] as string;
            ToastPartialData toastDataTwo = JsonConvert.DeserializeObject<ToastPartialData>(toastStringTwo);
            var resultTwo = toastDataTwo.ToastType;
            var expectedTwo = false;

            // Assert
            mockRepository.Verify(repo => repo.GetClaimAsync(1), Times.Once);
            mockRepository.Verify(repo => repo.GetStatusIdAsync("Verified"), Times.Once);
            mockRepository.Verify(repo => repo.AddClaimApprovalAsync(It.IsAny<ClaimApproval>()), Times.Once);
            mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            Console.WriteLine("VerifyClaim: ClaimID = 1");
            Console.WriteLine($"Expected: {expectedOne}, Actual: {resultOne}");
            Assert.AreEqual(expectedOne, resultOne);
            Console.WriteLine("VerifyClaim: ClaimID = 2");
            Console.WriteLine($"Expected: {expectedTwo}, Actual: {resultTwo}");
            Assert.AreEqual(expectedTwo, resultTwo);
        }

        //====================================================================//
        // tests that a claim can be approved successfully
        // teast that error handling works when attempting to approve a
        // non-existent claim
        [TestMethod]
        public async Task ApproveClaim_Approve_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<IApproverRepository>();
            // Copilot
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);
            var mockHubContext = new Mock<IHubContext<StatusHub>>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            var statusListener = new StatusListener(mockHubContext.Object);
            // End Copilot

            mockRepository.Setup(repo => repo.GetClaimAsync(1))
                .ReturnsAsync(new Models.Claim
                {
                    ClaimID = 1,
                    AdditionalNotes = "Test Claim",
                    HoursWorked = 10,
                    HourlyRate = 50,
                    StatusID = 2
                });
            mockRepository.Setup(repo => repo.GetStatusIdAsync("Approved"))
                .ReturnsAsync(3);
            mockRepository.Setup(repo => repo.AddClaimApprovalAsync(It.IsAny<ClaimApproval>()))
                .Returns(Task.CompletedTask);
            mockRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Copilot - create mock user
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "lecturer123") }, "mock"));
            // End Copilot
            var controller = new ApproveClaimsController(mockRepository.Object, statusListener);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = mockUser } };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>()); // Copilot autocomplete

            // Act
            await controller.ApproveClaim(1);
            var toastStringOne = controller.TempData["ToastData"] as string;
            ToastPartialData toastDataOne = JsonConvert.DeserializeObject<ToastPartialData>(toastStringOne);
            var resultOne = toastDataOne.ToastType;
            var expectedOne = true;

            await controller.ApproveClaim(2);
            var toastStringTwo = controller.TempData["ToastData"] as string;
            ToastPartialData toastDataTwo = JsonConvert.DeserializeObject<ToastPartialData>(toastStringTwo);
            var resultTwo = toastDataTwo.ToastType;
            var expectedTwo = false;

            // Assert
            mockRepository.Verify(repo => repo.GetClaimAsync(1), Times.Once);
            mockRepository.Verify(repo => repo.GetStatusIdAsync("Approved"), Times.Once);
            mockRepository.Verify(repo => repo.AddClaimApprovalAsync(It.IsAny<ClaimApproval>()), Times.Once);
            mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            Console.WriteLine("ApproveClaim: ClaimID = 1");
            Console.WriteLine($"Expected: {expectedOne}, Actual: {resultOne}");
            Assert.AreEqual(expectedOne, resultOne);
            Console.WriteLine("ApproveClaim: ClaimID = 2");
            Console.WriteLine($"Expected: {expectedTwo}, Actual: {resultTwo}");
            Assert.AreEqual(expectedTwo, resultTwo);
        }

        //====================================================================//
        // tests that a claim can be rejected successfully
        // teast that error handling works when attempting to reject a
        // non-existent claim
        [TestMethod]
        public async Task Approver_RejectClaim_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<IApproverRepository>();
            // Copilot
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);
            var mockHubContext = new Mock<IHubContext<StatusHub>>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
            var statusListener = new StatusListener(mockHubContext.Object);
            // End Copilot

            mockRepository.Setup(repo => repo.GetClaimAsync(1))
                .ReturnsAsync(new Models.Claim
                {
                    ClaimID = 1,
                    AdditionalNotes = "Test Claim",
                    HoursWorked = 10,
                    HourlyRate = 50,
                    StatusID = 1
                });
            mockRepository.Setup(repo => repo.GetStatusIdAsync("Rejected"))
                .ReturnsAsync(4);
            mockRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Copilot - create mock user
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "lecturer123") }, "mock"));
            // End Copilot
            var controller = new ApproveClaimsController(mockRepository.Object, statusListener);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = mockUser } };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>()); // Copilot autocomplete

            // Act

            await controller.RejectClaim(1);
            var toastStringOne = controller.TempData["ToastData"] as string;
            ToastPartialData toastDataOne = JsonConvert.DeserializeObject<ToastPartialData>(toastStringOne);
            var resultOne = toastDataOne.ToastType;
            var expectedOne = true;

            await controller.RejectClaim(2);
            var toastStringTwo = controller.TempData["ToastData"] as string;
            ToastPartialData toastDataTwo = JsonConvert.DeserializeObject<ToastPartialData>(toastStringTwo);
            var resultTwo = toastDataTwo.ToastType;
            var expectedTwo = false;

            // Assert
            mockRepository.Verify(repo => repo.GetClaimAsync(1), Times.Once);
            mockRepository.Verify(repo => repo.GetStatusIdAsync("Rejected"), Times.Once);
            mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Exactly(2));

            Console.WriteLine("RejectClaim: ClaimID = 1");
            Console.WriteLine($"Expected: {expectedOne}, Actual: {resultOne}");
            Assert.AreEqual(expectedOne, resultOne);
            Console.WriteLine("RejectClaim: ClaimID = 2");
            Console.WriteLine($"Expected: {expectedTwo}, Actual: {resultTwo}");
            Assert.AreEqual(expectedTwo, resultTwo);
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\