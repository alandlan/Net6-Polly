using Microsoft.AspNetCore.Mvc;
using Polly;
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
        // var retryPolicy = Policy
        //     .Handle<Exception>()
        //     .RetryAsync(5, (exception, retryCount) =>
        //     {
        //         Console.WriteLine($"Retry {retryCount} of {exception.Message}");
        //     });

        // await retryPolicy.ExecuteAsync(async () => await ConnectToApi());

        // var retryWaitPolicy = Policy
        //     .Handle<Exception>()
        //     .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
        //     {
        //         Console.WriteLine($"Retry {retryCount} of {exception.Message} at {timeSpan}");
        //     });

        // await retryWaitPolicy.ExecuteAsync(async () => await ConnectToApi());

        // await ConnectToApi();

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Retry {retryCount} of {exception.Message} at {timeSpan}");
            });

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), (exception, timeSpan) =>
            {
                Console.WriteLine($"Circuit breaker opened at {timeSpan} of {exception.Message}");
            }, () =>
            {
                Console.WriteLine($"Circuit breaker closed");
            });
        
        var finalPolicy = retryPolicy.WrapAsync(circuitBreakerPolicy);

        await finalPolicy.ExecuteAsync(async () => {
            Console.WriteLine("Connecting to API...");
            await ConnectToApi();
        });

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
            throw new Exception("Erro to connect to API");
        }

    }
}
