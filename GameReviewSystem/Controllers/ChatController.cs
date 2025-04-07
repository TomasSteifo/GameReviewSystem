using Microsoft.AspNetCore.Mvc;
using GameReviewSystem.Services;

namespace GameReviewSystem.Controllers
{
    // Mark this class as an API controller which automatically handles model binding, validation, etc.
    [ApiController]
    // Define the base route for this controller. The [controller] token will be replaced with the class name without the "Controller" suffix.
    // For ChatController, the route becomes "api/chat".
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        // Private field to hold the instance of ChatGPTService
        private readonly ChatGPTService _chatService;

        // Constructor: ChatGPTService is injected via dependency injection.
        // This service handles sending prompts to the ChatGPT API and returning responses.
        public ChatController(ChatGPTService chatService)
        {
            _chatService = chatService;
        }

        // Define a POST endpoint at "api/chat/ask".
        // This endpoint will accept a JSON request body that includes a chat prompt from the user.
        [HttpPost("ask")]
        public async Task<IActionResult> AskChatGPT([FromBody] ChatRequest request)
        {
            // The [FromBody] attribute tells ASP.NET Core to bind the incoming JSON request to a ChatRequest object.
            // 'request.Prompt' contains the user's prompt (text input).

            // Call the SendMessageAsync method on the ChatGPTService to send the user's prompt to the ChatGPT API.
            // This call is asynchronous, so we use 'await' to asynchronously wait for the response.
            var response = await _chatService.SendMessageAsync(request.Prompt);

            // Return an HTTP 200 OK response with a JSON object that includes the reply from ChatGPT.
            // The reply is contained in the "reply" property of the JSON object.
            return Ok(new { reply = response });
        }
    }

    // Define a Data Transfer Object (DTO) for the chat request.
    // This class represents the structure of the JSON payload that the client must send.
    public class ChatRequest
    {
        // The Prompt property is where the client sends their text input.
        // It is initialized to an empty string to ensure it is never null.
        public string Prompt { get; set; } = string.Empty;
    }
}
