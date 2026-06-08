# Running Api Tests IMPORTANT

You need to run the server in `Api-Testing` mode, either by selecting that mode in **rider/visual-studio** or by running:
```bash
dotnet run --launch-profile Api-Testing
```

In this mode Firebase Authentication and Authorization is bypassed, allowing you to freely  test the API.