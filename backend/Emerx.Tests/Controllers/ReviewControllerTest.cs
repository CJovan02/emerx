using EMerx.Common.Filters;
using EMerx.Controllers;
using EMerx.DTOs.Reviews.Response;
using EMerx.Services.Reviews;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Emerx.Tests;

public class ReviewControllerTest
{
    private ReviewController _reviewController;
    private Mock<IReviewService> _reviewService;

    [SetUp]
    public void SetUp()
    {
        _reviewService = new Mock<IReviewService>();
        _reviewController = new ReviewController(_reviewService.Object);
    }

   
}