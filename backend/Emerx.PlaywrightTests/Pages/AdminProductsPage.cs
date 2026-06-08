using Emerx.PlaywrightTests.Constants;
using Emerx.PlaywrightTests.Helpers;
using Emerx.PlaywrightTests.Services;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Emerx.PlaywrightTests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AdminProductsPage : PageTest
{
    private BackendApiService _api;

    private ILocator SearchBar => Page.GetByTestId("admin-search-bar");
    private ILocator OpenCreateDrawerButton => Page.GetByTestId("open-create-drawer-button");
    private ILocator CreateProductDrawer => Page.GetByTestId("create-product-drawer");
    private ILocator CreateNameField => CreateProductDrawer.GetByLabel("Name");
    private ILocator CreateDescriptionField => CreateProductDrawer.GetByLabel("Description");
    private ILocator CreateCategoryField => CreateProductDrawer.GetByLabel("Category");
    private ILocator CreatePriceField => CreateProductDrawer.GetByLabel("Price");
    private ILocator CreateStockField => CreateProductDrawer.GetByLabel("Stock");
    private ILocator CreateSubmitButton => CreateProductDrawer.GetByTestId("submit-create-product");

    private ILocator UpdateProductDrawer => Page.GetByTestId("update-product-drawer");
    private ILocator UpdateNameField => UpdateProductDrawer.GetByLabel("Name");
    private ILocator UpdateDescriptionField => UpdateProductDrawer.GetByLabel("Description");
    private ILocator UpdateCategoryField => UpdateProductDrawer.GetByLabel("Category");
    private ILocator UpdatePriceField => UpdateProductDrawer.GetByLabel("Price");
    private ILocator UpdateStockField => UpdateProductDrawer.GetByLabel("Stock");
    private ILocator UpdateSubmitButton => UpdateProductDrawer.GetByTestId("submit-update-product");


    [SetUp]
    public async Task SetupAsAdmin()
    {
        _api = new BackendApiService(Playwright);
        await _api.ConnectAsync(GlobalSetup.AdminToken);

        await AuthHelper.LoginAsAdmin(Page);

        await Page.GotoAsync(PageUrls.AdminProductPage);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15000 });
    }

    [Test]
    public async Task ClickCreate_OpensCreateDrawer()
    {
        await OpenCreateDrawerButton.ClickAsync();

        await Expect(CreateProductDrawer).ToBeVisibleAsync();
    }

    [Test]
    public async Task CreateProduct_ValidData_CreatesProduct()
    {
        await OpenCreateDrawerButton.ClickAsync();

        var uniqueName = GenerateUniqueName();
        await FillOutCreateForm(uniqueName);

        try
        {
            await CreateSubmitButton.ClickAsync();
            await Expect(Page.GetByText("Successfully created product")).ToBeVisibleAsync();
        }
        finally
        {
            var previouslyCreatedProduct = await _api.GetProductByName(uniqueName);
            if (previouslyCreatedProduct is not null)
                await _api.DeleteProduct(previouslyCreatedProduct.Id);
        }
    }

    [Test]
    public async Task CreateProduct_EmptyFields_DoesNotSubmit()
    {
        await OpenCreateDrawerButton.ClickAsync();

        await CreateSubmitButton.ClickAsync();

        var isNameValid = await CreateNameField
            .CheckFieldValidity();
        var isDescriptionValid = await CreateDescriptionField
            .CheckFieldValidity();

        Assert.Multiple(() =>
        {
            Assert.That(isNameValid, Is.False);
            Assert.That(isDescriptionValid, Is.False);
        });
    }

    [Test]
    public async Task CreateProduct_NumbersNegative_DoesNotSubmit()
    {
        await OpenCreateDrawerButton.ClickAsync();

        await FillOutCreateForm("-1", "-1");

        await CreateSubmitButton.ClickAsync();

        await Expect(
            CreateProductDrawer.Locator("#price-helper-text")
        ).ToBeVisibleAsync();

        await Expect(
            CreateProductDrawer.Locator("#stock-helper-text")
        ).ToBeVisibleAsync();
    }

    [Test]
    public async Task ClickEdit_OpensEditDrawer()
    {
        // Arrange
        var uniqueName = GenerateUniqueName();
        var createdProduct = await _api.CreateProduct(uniqueName);

        try
        {
            await SearchBar.FillAsync(uniqueName);
            await Expect(
                Page.GetByText(uniqueName)
            ).ToBeVisibleAsync();

            var editButton = GetEditButton(createdProduct.Id);

            // Act
            await editButton.ClickAsync();

            // Assert
            await Expect(UpdateProductDrawer).ToBeVisibleAsync();
            await Expect(UpdateNameField).ToHaveValueAsync(createdProduct.Name);
            await Expect(UpdateDescriptionField).ToHaveValueAsync(createdProduct.Description);
            await Expect(UpdateCategoryField).ToHaveValueAsync(createdProduct.Category);
            await Expect(UpdatePriceField).ToHaveValueAsync(createdProduct.Price.ToString());
            await Expect(UpdateStockField).ToHaveValueAsync(createdProduct.Stock.ToString());
        }
        finally
        {
            await _api.DeleteProduct(createdProduct.Id);
        }
    }

    [Test]
    public async Task EditProduct_ValidData_EditsProduct()
    {
        // Arrange
        var uniqueName = GenerateUniqueName();
        var createdProduct = await _api.CreateProduct(uniqueName);

        try
        {
            await SearchBar.FillAsync(uniqueName);
            await Expect(
                Page.GetByText(uniqueName)
            ).ToBeVisibleAsync();

            await GetEditButton(createdProduct.Id).ClickAsync();

            const string newName = "Edited";
            const string newStock = "99";
            await UpdateNameField.FillAsync(newName);
            await UpdateStockField.FillAsync(newStock);

            // Act
            await UpdateSubmitButton.ClickAsync();

            // Assert
            await Expect(Page.GetByText("Successfully updated product")).ToBeVisibleAsync();
            await Expect(UpdateProductDrawer).ToBeHiddenAsync();
            await SearchBar.FillAsync(newName);
            await Expect(
                Page.GetByText(newName)
            ).ToBeVisibleAsync();
            await Expect(Page.GetByText($"{newStock} units")).ToBeVisibleAsync();
        }
        finally
        {
            await _api.DeleteProduct(createdProduct.Id);
        }
    }

    [Test]
    public async Task UpdateProduct_ChangeNothing_ShowsNothingToUpdateSnackbar()
    {
        var uniqueName = GenerateUniqueName();
        var createdProduct = await _api.CreateProduct(uniqueName);

        try
        {
            await SearchBar.FillAsync(uniqueName);
            await Expect(
                Page.GetByText(uniqueName)
            ).ToBeVisibleAsync();

            await GetEditButton(createdProduct.Id).ClickAsync();

            await UpdateSubmitButton.ClickAsync();

            await Expect(
                Page.GetByText("All set - there is noting to update.")).ToBeVisibleAsync();
        }
        finally
        {
            await _api.DeleteProduct(createdProduct.Id);
        }
    }

    // [Test]
    // public async Task CreateProduct_NumbersNegative_DoesNotSubmit()
    // {
    //     await OpenCreateDrawerButton.ClickAsync();
    //
    //     await FillOutCreateForm("-1", "-1");
    //
    //     await CreateSubmitButton.ClickAsync();
    //
    //     await Expect(
    //         CreateProductDrawer.Locator("#price-helper-text")
    //     ).ToBeVisibleAsync();
    //
    //     await Expect(
    //         CreateProductDrawer.Locator("#stock-helper-text")
    //     ).ToBeVisibleAsync();
    // }

    [TearDown]
    public async Task TearDown()
    {
        await _api.DisposeAsync();
    }

    private async Task FillOutCreateForm(string name = "Playwright", string price = "10", string stock = "100")
    {
        await CreateNameField.FillAsync(name);
        await CreateDescriptionField.FillAsync("Playwright");
        await CreateCategoryField.FillAsync("Playwright");
        await CreatePriceField.FillAsync(price);
        await CreateStockField.FillAsync(stock);
    }

    private string GenerateUniqueName()
    {
        return $"Playwright-{Guid.NewGuid()}";
    }

    private ILocator GetEditButton(string productId) => Page.GetByTestId($"edit-product-{productId}");
    private ILocator GetDeleteButton(string productId) => Page.GetByTestId($"delete-product-{productId}");
}