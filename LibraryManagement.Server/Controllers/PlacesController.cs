using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using Common.Models;

[ApiController]
[Route("api/places")]
public class PlacesController : ControllerBase
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public PlacesController(IHttpClientFactory factory, IOptions<GoogleOptions> opts)
    {
        _http = factory.CreateClient();
        _apiKey = opts.Value.PlacesApiKey!;
    }

    [HttpGet("autocomplete")]
    public async Task<ActionResult<IEnumerable<PlaceSuggestion>>> Autocomplete([FromQuery] string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Ok(Array.Empty<PlaceSuggestion>());

        var url = "https://places.googleapis.com/v1/places:autocomplete";

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Add("X-Goog-Api-Key", _apiKey);
        req.Headers.Add("X-Goog-FieldMask",
            "suggestions.placePrediction.placeId," +
            "suggestions.placePrediction.text"); // what we need back

        // ✅ input must be a string, not {text: ...}
        var payload = new
        {
            input = input,
            includedPrimaryTypes = new[] { "street_address", "premise", "subpremise" },
            regionCode = "US",
            languageCode = "en"
            // sessionToken = Guid.NewGuid().ToString()  // optional but recommended per session
        };
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var res = await _http.SendAsync(req);
        var json = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            return StatusCode((int)res.StatusCode, json);  // ← show Google’s error in Swagger

        using var doc = JsonDocument.Parse(json);
        var list = new List<PlaceSuggestion>();
        if (doc.RootElement.TryGetProperty("suggestions", out var arr))
        {
            foreach (var s in arr.EnumerateArray())
            {
                var p = s.GetProperty("placePrediction");
                var placeId = p.GetProperty("placeId").GetString() ?? "";
                var fullText = p.GetProperty("text").GetProperty("text").GetString() ?? "";
                list.Add(new PlaceSuggestion { PlaceId = placeId, Description = fullText });
            }
        }
        return Ok(list);
    }


    [HttpGet("details")]
    public async Task<ActionResult<PlaceDetailsAddress>> Details([FromQuery] string placeId)
    {
        if (string.IsNullOrWhiteSpace(placeId)) return BadRequest("Missing placeId");

        var url =
            $"https://places.googleapis.com/v1/places/{placeId}" +
            $"?fields=addressComponents,formattedAddress" +
            $"&key={_apiKey}";

        var res = await _http.GetAsync(url);
        var json = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode)
            return StatusCode((int)res.StatusCode, json);

        using var doc = JsonDocument.Parse(json);

        string streetNo = "", route = "", city = "", state = "", zip = "";
        if (doc.RootElement.TryGetProperty("addressComponents", out var comps))
        {
            foreach (var c in comps.EnumerateArray())
            {
                var types = c.GetProperty("types").EnumerateArray().Select(x => x!.GetString()).ToArray();
                var longText = c.TryGetProperty("longText", out var lt) ? lt.GetString() ?? "" : "";
                var shortText = c.TryGetProperty("shortText", out var st) ? st.GetString() ?? "" : "";

                if (types.Contains("street_number")) streetNo = longText;
                else if (types.Contains("route")) route = longText;
                else if (types.Contains("locality")) city = longText;
                else if (types.Contains("administrative_area_level_1")) state = shortText;
                else if (types.Contains("postal_code")) zip = longText;
            }
        }

        return Ok(new PlaceDetailsAddress
        {
            Street1 = $"{streetNo} {route}".Trim(),
            City = city,
            State = state,
            Zip = zip
        });
    }


    public class GoogleOptions { public string? PlacesApiKey { get; set; } }
}
