using System.Text.Json;
using EMerx.DTOs.Reviews.Request;
using EMerx.DTOs.Reviews.Response;
using Emerx.PlaywrightTests.Constants;
using Microsoft.Playwright;

namespace Emerx.PlaywrightTests.ApiTests.Helpers;

public static class ReviewApiHelpers
{
    public static string TestProductId = "6a1d7853560f5e66666adb13";

    public static ReviewRequest CreateReviewRequest()
    {
        return new ReviewRequest
        {
            ProductId = TestProductId,
            Description = "Test Review Description",
            Rating = 5
        };
    }

    public static async Task<ReviewResponse> PostReview(IAPIRequestContext request)
    {
        var data = CreateReviewRequest();

        await using var response = await request.PostAsync(ReviewUrls.Base, new APIRequestContextOptions
        {
            DataObject = data
        });

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return await response.JsonAsync<ReviewResponse>(options);
    }

    public static PatchReviewRequest CreatePatchReviewRequest
    (IAPIRequestContext request, string? description = null, double? rating = null)
    {
        return new PatchReviewRequest
        {
            Description = description,
            Rating = rating
        };
    }

    public static async Task DeleteReview(IAPIRequestContext request, string reviewId)
    {
        await using var response = await request.DeleteAsync($"{ReviewUrls.Base}/{reviewId}");
    }
}