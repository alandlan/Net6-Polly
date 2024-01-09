using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace ApiRetry.ServicesRequestsController;

[ApiController]
[Route("[controller]")]
public class ServicesRequestsController : ControllerBase
{
    private readonly ILogger<ServicesRequestsController> _logger;

    public ServicesRequestsController(ILogger<ServicesRequestsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetData")]
    public async Task<IActionResult> Get()
    {
        await ConnectToApi();
        return  Ok();
    }

    private async Task ConnectToApi()
    {
        var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";

        var client = new RestClient();
        
        var request  = new RestRequest(url, Method.Get);

        request.AddHeader("accept", "application/json");
        request.AddHeader("X-RapidAPI-Key", "794831b28fmshfcae1f75d4d6997p1e581fjsn7262a5eae8da");
        request.AddHeader("X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");

        var response = await client.ExecuteAsync(request);

        if(response.IsSuccessful){
            Console.WriteLine(response.Content);
        }else{
            Console.WriteLine(response.ErrorMessage);
        }

    }
}
