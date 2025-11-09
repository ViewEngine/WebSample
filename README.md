# ViewEngine Web Sample

A standalone web application that demonstrates how to use the ViewEngine REST API. Users can test page retrieval and see the results including content, thumbnails, and metadata.

## Features

- 🔑 **API Key Management** - Securely stored in browser localStorage
- 🌐 **URL Retrieval** - Enter any URL to retrieve its content
- 📊 **Real-time Status** - Live polling shows retrieval progress
- 📸 **Thumbnail Display** - View captured page thumbnails
- 📄 **Content Preview** - See the full page data JSON
- 🎨 **Modern UI** - Beautiful, responsive interface

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- A ViewEngine API key (get one from https://www.viewengine.io)

### Running the Application

1. **Navigate to the project directory:**
   ```bash
   cd ViewEngine.WebSample
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Open your browser:**
   - Navigate to `http://localhost:5000` or `https://localhost:5001`

4. **Enter your API key:**
   - Paste your ViewEngine API key in the API Key field
   - The key is stored locally in your browser for convenience

5. **Test a retrieval:**
   - Enter a URL (e.g., `https://example.com`)
   - Optionally check "Force fresh retrieval" to bypass cache
   - Click "Retrieve Page"
   - Watch the real-time progress as the page is processed
   - View the results including thumbnail and content

## How It Works

### Architecture

```
Browser <-> WebSample Server <-> ViewEngine API
```

The WebSample server acts as a proxy to:
- Keep your API key secure (not exposed in browser JavaScript)
- Handle CORS issues
- Simplify the API interaction

### API Endpoints

The application creates three proxy endpoints:

1. **POST /api/retrieve** - Submit a retrieval request
2. **GET /api/status/{requestId}** - Poll for retrieval status
3. **GET /api/content/{requestId}** - Download the page content

### Workflow

1. User enters API key and URL
2. WebSample sends request to ViewEngine API
3. Polls for status updates every 2 seconds
4. When complete, downloads and displays:
   - Page metadata
   - Thumbnail image
   - Full JSON content

## Configuration

To use a different ViewEngine API endpoint, edit `Program.cs`:

```csharp
const string API_BASE_URL = "https://your-custom-endpoint.com";
```

For local development with the ViewEngine API running locally:

```csharp
const string API_BASE_URL = "http://localhost:5072";
```

## Security Notes

- API keys are stored in browser localStorage (client-side only)
- The proxy server never logs or stores API keys
- All requests to ViewEngine API include proper authentication headers
- CORS is enabled to allow browser-based requests

## Troubleshooting

### "Invalid API key" error
- Verify your API key is correct
- Check that your API key has the `mcp:retrieve` scope

### "Connection error"
- Ensure the ViewEngine API is accessible
- Check your internet connection
- Verify the API_BASE_URL is correct

### Results not appearing
- Check browser console for errors
- Verify the job completed successfully
- Ensure the response includes content data

## Example URLs to Try

- `https://example.com` - Simple page
- `https://news.google.com` - News site
- `https://github.com` - GitHub homepage
- `https://www.viewengine.io` - ViewEngine homepage

## Building for Production

```bash
dotnet publish -c Release -o ./publish
```

Then deploy the contents of `./publish` to your web server.

## License

This sample application is provided as-is for demonstration purposes.
