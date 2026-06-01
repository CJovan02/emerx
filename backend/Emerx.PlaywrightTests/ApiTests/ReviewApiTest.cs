using System.Text.Json;
using EMerx.Common.Filters;
using EMerx.DTOs.Reviews.Response;
using Emerx.PlaywrightTests.ApiTests.Helpers;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MongoDB.Bson;

namespace Emerx.PlaywrightTests.ApiTests;

[TestFixture]
public class ReviewApiTest : PlaywrightTest
{
    private IAPIRequestContext _request;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    [SetUp]
    public async Task SetUp()
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
        };

        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = ServerUrls.BackendTest,
            ExtraHTTPHeaders = headers,
            IgnoreHTTPSErrors = true,
        });
    }

    [Test]
    public async Task GetAll_ValidRequest_ReturnsPagedOrders()
    {
        // Arrange
        const int page = 1;
        const int pageSize = 3;

        // Act
        await using var response =
            await _request.GetAsync($"{ReviewUrls.Base}?PageSize={pageSize}&Page={page}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        var pageResponse =
            await response.JsonAsync<PageOfResponse<ReviewResponse>>(_jsonOptions);

        Assert.Multiple(() =>
        {
            Assert.That(pageResponse, Is.Not.Null);
            Assert.That(pageResponse.Page, Is.EqualTo(page));
            Assert.That(pageResponse.PageSize, Is.EqualTo(pageSize));
        });
    }

    [Test]
    public async Task GetPaged_InvalidPage_ReturnsBadRequest()
    {
        // Arrange
        const int page = 0;
        const int pageSize = -5;

        // Act
        await using var response =
            await _request.GetAsync($"{ReviewUrls.Base}?PageSize={pageSize}&Page={page}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(400));
    }

    [Test]
    public async Task GetById_ExistingReview_ReturnsReview()
    {
        // Arrange
        var createdReview =
            await ReviewApiHelpers.PostReview(_request);

        try
        {
            // Act
            await using var response =
                await _request.GetAsync($"{ReviewUrls.Base}/{createdReview.Id}");

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            var review =
                await response.JsonAsync<ReviewResponse>(_jsonOptions);

            Assert.Multiple(() =>
            {
                Assert.That(review, Is.Not.Null);
                Assert.That(review.Id, Is.EqualTo(createdReview.Id));
                Assert.That(review.ProductId, Is.EqualTo(ReviewApiHelpers.TestProductId));
            });
        }
        finally
        {
            await ReviewApiHelpers.DeleteReview(_request, createdReview.Id);
        }
    }

    [Test]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var response =
            await _request.GetAsync($"{ReviewUrls.Base}/{nonExistingId}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task GetProductReviews_ExistingProduct_ReturnsReviews()
    {
        // Arrange
        var existingProductId = ReviewApiHelpers.TestProductId;
        var createdReview = await ReviewApiHelpers.PostReview(_request);

        // Act
        try
        {
            await using var response = await _request.GetAsync($"{ReviewUrls.GetProductReviews}/{existingProductId}");

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));
            var reviews =
                await response.JsonAsync<List<ReviewResponse>>(_jsonOptions);

            Assert.Multiple(() =>
            {
                Assert.That(reviews, Is.Not.Null);
                Assert.That(reviews, Is.Not.Empty);
                Assert.That(reviews[0].ProductId, Is.EqualTo(ReviewApiHelpers.TestProductId));
            });
        }
        finally
        {
            await ReviewApiHelpers.DeleteReview(_request, createdReview.Id);
        }
    }

    [Test]
    public async Task GetProductReviews_NonExistingProduct_ReturnsReviews()
    {
        // Arrange
        var nonExistingProductId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var response =
            await _request.GetAsync($"{ReviewUrls.GetProductReviews}/{nonExistingProductId}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        var reviews =
            await response.JsonAsync<List<ReviewResponse>>(_jsonOptions);

        Assert.That(reviews, Is.Empty);
    }

    [Test]
    public async Task Create_ValidRequest_ReturnsCreatedReview()
    {
        // Arrange
        var request =
            ReviewApiHelpers.CreateReviewRequest();

        ReviewResponse? createdReview = null;

        try
        {
            // Act
            await using var response =
                await _request.PostAsync(
                    ReviewUrls.Base,
                    new()
                    {
                        DataObject = request
                    });

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            createdReview =
                await response.JsonAsync<ReviewResponse>(_jsonOptions);

            Assert.Multiple(() =>
            {
                Assert.That(createdReview, Is.Not.Null);
                Assert.That(createdReview.ProductId, Is.EqualTo(ReviewApiHelpers.TestProductId));
            });
        }
        finally
        {
            if (createdReview is not null)
                await ReviewApiHelpers.DeleteReview(
                    _request,
                    createdReview.Id);
        }
    }

    [Test]
    public async Task Create_ReviewProductTwoTimes_ReturnsForbidden()
    {
        // Arrange
        var request =
            ReviewApiHelpers.CreateReviewRequest();

        ReviewResponse? createdReview = null;

        try
        {
            // Act
            await using var response =
                await _request.PostAsync(
                    ReviewUrls.Base,
                    new()
                    {
                        DataObject = request
                    });
            await using var response2 =
                await _request.PostAsync(
                    ReviewUrls.Base,
                    new()
                    {
                        DataObject = request
                    });

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            createdReview =
                await response.JsonAsync<ReviewResponse>(_jsonOptions);

            Assert.That(response2.Status, Is.EqualTo(409));
        }
        finally
        {
            if (createdReview is not null)
                await ReviewApiHelpers.DeleteReview(
                    _request,
                    createdReview.Id);
        }
    }

    [Test]
    public async Task PatchReview_ValidParameters_ReturnsReview()
    {
        // Arrange
        var createdReview = await ReviewApiHelpers.PostReview(_request);

        var newDescription = "New Description";
        var data = ReviewApiHelpers.CreatePatchReviewRequest(_request, newDescription);

        try
        {
            // Act
            await using var response =
                await _request.PatchAsync($"{ReviewUrls.Base}/{createdReview.Id}", new APIRequestContextOptions
                {
                    DataObject = data
                });

            // Assert
            Assert.That(response.Status, Is.EqualTo(200));

            var newReview =
                await response.JsonAsync<ReviewResponse>(_jsonOptions);

            Assert.Multiple(() =>
            {
                Assert.That(newReview, Is.Not.Null);
                Assert.That(newReview.Description, Is.EqualTo(newDescription));
            });
        }
        finally
        {
            await ReviewApiHelpers.DeleteReview(_request, createdReview.Id);
        }
    }

    [Test]
    public async Task PatchReview_InvalidParameters_ReturnsBadRequest()
    {
        // Arrange
        var createdReview = await ReviewApiHelpers.PostReview(_request);

        var invalidRating = 15;
        var data = ReviewApiHelpers.CreatePatchReviewRequest(_request, null, invalidRating);

        try
        {
            // Act
            await using var response =
                await _request.PatchAsync($"{ReviewUrls.Base}/{createdReview.Id}", new APIRequestContextOptions
                {
                    DataObject = data
                });

            // Assert
            Assert.That(response.Status, Is.EqualTo(400));
        }
        finally
        {
            await ReviewApiHelpers.DeleteReview(_request, createdReview.Id);
        }
    }

    [Test]
    public async Task DeleteReview_ValidId_ReturnsOk()
    {
        // Arrange
        var createdReview = await ReviewApiHelpers.PostReview(_request);

        // Act
        await using var response = await _request.DeleteAsync($"{ReviewUrls.Base}/{createdReview.Id}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(200));

        await using var getResponse =
            await _request.GetAsync($"{ReviewUrls.Base}/{createdReview.Id}");

        Assert.That(getResponse.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task DeleteReview_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // Act
        await using var response = await _request.DeleteAsync($"{ReviewUrls.Base}/{nonExistingId}");

        // Assert
        Assert.That(response.Status, Is.EqualTo(404));
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        await _request.DisposeAsync();
    }
}