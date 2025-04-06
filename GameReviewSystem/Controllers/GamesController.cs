using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;  // If you want to protect with JWT
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameReviewSystem.Services;  // For IGameService
using GameReviewSystem.DTOs;     // For GameDto, CreateGameDto
using GameReviewSystem.Models;   // For the Game entity

namespace YourApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // => /api/games
    // [Authorize] // uncomment if you want these endpoints to require a valid JWT
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GamesController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        // GET /api/games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
        {
            var games = await _gameService.GetAllGamesAsync();
            var gameDtos = _mapper.Map<IEnumerable<GameDto>>(games);
            return Ok(gameDtos);
        }

        // GET /api/games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGameById(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null) return NotFound();
            return Ok(_mapper.Map<GameDto>(game));
        }

        // POST /api/games
        [HttpPost]
        public async Task<ActionResult<GameDto>> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            var gameEntity = _mapper.Map<Game>(createGameDto);
            var createdGame = await _gameService.CreateGameAsync(gameEntity);

            var gameDto = _mapper.Map<GameDto>(createdGame);
            return CreatedAtAction(nameof(GetGameById), new { id = gameDto.GameId }, gameDto);
        }

        // PUT /api/games/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] CreateGameDto updateGameDto)
        {
            var existingGame = await _gameService.GetGameByIdAsync(id);
            if (existingGame == null) return NotFound();

            _mapper.Map(updateGameDto, existingGame);
            await _gameService.UpdateGameAsync(existingGame);

            return NoContent();
        }

        // DELETE /api/games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            await _gameService.DeleteGameAsync(id);
            return NoContent();
        }

        // GET /api/games/genre/RPG
        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByGenre(string genre)
        {
            var games = await _gameService.GetGamesByGenreAsync(genre);
            var dtos = _mapper.Map<IEnumerable<GameDto>>(games);
            return Ok(dtos);
        }

        // GET /api/games/status/Backlog
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByStatus(string status)
        {
            var games = await _gameService.GetGamesByStatusAsync(status);
            var dtos = _mapper.Map<IEnumerable<GameDto>>(games);
            return Ok(dtos);
        }

        // GET /api/games/random
        [HttpGet("random")]
        public async Task<ActionResult<GameDto>> GetRandomGame()
        {
            var backlogGames = await _gameService.GetGamesByStatusAsync("Backlog");
            if (!backlogGames.Any()) return NotFound("No games in backlog.");

            var randomGame = backlogGames.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
            var dto = _mapper.Map<GameDto>(randomGame);
            return Ok(dto);
        }
       
        [HttpGet("search")]
        public async Task<IActionResult> SearchGames(
            [FromQuery] string? filter,
            [FromQuery] string? sort = "asc"
        )
        {
            // filter might be a genre or partial title
            // sort might be "asc" or "desc" by Title

            var games = await _gameService.GetAllGamesAsync();

            if (!string.IsNullOrEmpty(filter))
                games = games.Where(g => g.Title.Contains(filter));

            if (sort == "desc")
                games = games.OrderByDescending(g => g.Title);
            else
                games = games.OrderBy(g => g.Title);

            return Ok(games);
        }

    }
}
