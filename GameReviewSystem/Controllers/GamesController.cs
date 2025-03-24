using AutoMapper;
using GameReviewSystem.Services;
using GameReviewSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using GameReviewSystem.Models;

namespace GameReviewSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GamesController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
        {
            var games = await _gameService.GetAllGamesAsync();
            var gameDtos = _mapper.Map<IEnumerable<GameDto>>(games);
            return Ok(gameDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGameById(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null) return NotFound();

            var gameDto = _mapper.Map<GameDto>(game);
            return Ok(gameDto);
        }

        [HttpPost]
        public async Task<ActionResult<GameDto>> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            var game = _mapper.Map<Game>(createGameDto);
            var createdGame = await _gameService.CreateGameAsync(game);

            var gameDto = _mapper.Map<GameDto>(createdGame);
            return CreatedAtAction(nameof(GetGameById), new { id = gameDto.GameId }, gameDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] CreateGameDto updateGameDto)
        {
            var existingGame = await _gameService.GetGameByIdAsync(id);
            if (existingGame == null) return NotFound();

            // Map updated fields to existing entity
            _mapper.Map(updateGameDto, existingGame);
            await _gameService.UpdateGameAsync(existingGame);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            await _gameService.DeleteGameAsync(id);
            return NoContent();
        }

        // Additional endpoints: filter by genre, status
        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByGenre(string genre)
        {
            var games = await _gameService.GetGamesByGenreAsync(genre);
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByStatus(string status)
        {
            var games = await _gameService.GetGamesByStatusAsync(status);
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        [HttpGet("random")]
        public async Task<ActionResult<GameDto>> GetRandomGame()
        {
            var allBacklogGames = await _gameService.GetGamesByStatusAsync("Backlog");
            if (!allBacklogGames.Any()) return NotFound("No games in backlog.");

            var randomGame = allBacklogGames.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            return _mapper.Map<GameDto>(randomGame);
        }

    }
}
