using Microsoft.AspNetCore.Mvc;
using GameReviewSystem.Services;

namespace GameReviewSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatGPTService _chatService;

        public ChatController(ChatGPTService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskChatGPT([FromBody] ChatRequest request)
        {
            // request.Prompt is the user input
            var response = await _chatService.SendMessageAsync(request.Prompt);
            return Ok(new { reply = response });
        }
    }

    public class ChatRequest
    {
        public string Prompt { get; set; } = string.Empty;
    }
}
