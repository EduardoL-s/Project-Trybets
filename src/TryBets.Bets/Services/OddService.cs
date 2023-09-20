using System.Net.Http;
namespace TryBets.Bets.Services;

public class OddService : IOddService
{
    private readonly HttpClient _client;
    public OddService(HttpClient client)
    {
        _client = client;
    }

    public async Task<object> UpdateOdd(int MatchId, int TeamId, decimal BetValue)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/odd/{MatchId}/{TeamId}/{BetValue}");
        requestMessage.Headers.Add("Accept", "application/json");

        var response = await _client.SendAsync(requestMessage);

        var result = await response.Content.ReadFromJsonAsync<object>();

        return result;


    }
}