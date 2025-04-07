using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameReviewSystem.Services;  
using GameReviewSystem.DTOs;
using GameReviewSystem.Models;

namespace YourApp.Controllers
{
    // This attribute designates the class as an API controller, enabling features like automatic model validation.
    [ApiController]

    // Define the base route for all actions in this controller.
    // The [controller] token is replaced by the controller name without the "Controller" suffix.
    // In this case, "GamesController" maps to "/api/games".
    [Route("api/[controller]")]
    // Uncomment the [Authorize] attribute if you want all endpoints in this controller to require JWT authentication.
    // [Authorize]
    public class GamesController : ControllerBase
    {
        // Private field for accessing game-related business logic.
        private readonly IGameService _gameService;

        // Private field for mapping entities to DTOs and vice versa.
        private readonly IMapper _mapper;

        // The constructor receives dependencies (IGameService and IMapper) via dependency injection.
        public GamesController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        // GET /api/games
        // Retrieves all games from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
        {
            // Call the service to retrieve all game entities asynchronously.
            var games = await _gameService.GetAllGamesAsync();

            // Use AutoMapper to convert the list of game entities to a list of GameDto objects.
            var gameDtos = _mapper.Map<IEnumerable<GameDto>>(games);

            // Return a 200 OK response with the list of GameDto objects.
            return Ok(gameDtos);
        }

        // GET /api/games/{id}
        // Retrieves a specific game by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGameById(int id)
        {
            // Retrieve the game entity with the specified ID.
            var game = await _gameService.GetGameByIdAsync(id);

            // If no game is found, return a 404 Not Found response.
            if (game == null) return NotFound();

            // Map the game entity to a GameDto and return a 200 OK response.
            return Ok(_mapper.Map<GameDto>(game));
        }

        // POST /api/games
        // Creates a new game.
        [HttpPost]
        public async Task<ActionResult<GameDto>> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            // Map the incoming CreateGameDto to a Game entity.
            var gameEntity = _mapper.Map<Game>(createGameDto);

            // Call the service to create the game in the database.
            var createdGame = await _gameService.CreateGameAsync(gameEntity);

            // Map the newly created game entity back to a GameDto.
            var gameDto = _mapper.Map<GameDto>(createdGame);

            // Return a 201 Created response.
            // The CreatedAtAction method automatically sets the Location header to the URL for the GetGameById action.
            return CreatedAtAction(nameof(GetGameById), new { id = gameDto.GameId }, gameDto);
        }

        // PUT /api/games/{id}
        // Updates an existing game identified by its ID.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] CreateGameDto updateGameDto)
        {
            // Retrieve the existing game from the database.
            var existingGame = await _gameService.GetGameByIdAsync(id);

            // If the game doesn't exist, return 404 Not Found.
            if (existingGame == null) return NotFound();

            // Map the fields from the updateGameDto to the existing game entity.
            _mapper.Map(updateGameDto, existingGame);

            // Save the changes to the database.
            await _gameService.UpdateGameAsync(existingGame);

            // Return a 204 No Content response to indicate the update was successful.
            return NoContent();
        }

        // DELETE /api/games/{id}
        // Deletes a game identified by its ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            // Call the service to delete the game.
            await _gameService.DeleteGameAsync(id);

            // Return a 204 No Content response to indicate successful deletion.
            return NoContent();
        }

        // GET /api/games/genre/{genre}
        // Retrieves games filtered by a specific genre.
        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByGenre(string genre)
        {
            // Retrieve games with the specified genre.
            var games = await _gameService.GetGamesByGenreAsync(genre);

            // Map the game entities to DTOs.
            var dtos = _mapper.Map<IEnumerable<GameDto>>(games);

            // Return the list of games as a 200 OK response.
            return Ok(dtos);
        }

        // GET /api/games/status/{status}
        // Retrieves games filtered by a specific status.
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByStatus(string status)
        {
            // Retrieve games with the specified status.
            var games = await _gameService.GetGamesByStatusAsync(status);

            // Map them to DTOs.
            var dtos = _mapper.Map<IEnumerable<GameDto>>(games);

            // Return a 200 OK response with the list of DTOs.
            return Ok(dtos);
        }

        // GET /api/games/random
        // Retrieves a random game from those with the status "Backlog".
        [HttpGet("random")]
        public async Task<ActionResult<GameDto>> GetRandomGame()
        {
            // Retrieve all games that have a status of "Backlog".
            var backlogGames = await _gameService.GetGamesByStatusAsync("Backlog");

            // If there are no games with the "Backlog" status, return 404 Not Found.
            if (!backlogGames.Any()) return NotFound("No games in backlog.");

            // Randomly order the games and select the first one.
            var randomGame = backlogGames.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();

            // Map the selected game to a GameDto.
            var dto = _mapper.Map<GameDto>(randomGame);

            // Return the DTO in a 200 OK response.
            return Ok(dto);
        }

        // GET /api/games/search?filter=...&sort=asc
        // Allows filtering and sorting of games based on query parameters.
        [HttpGet("search")]
        public async Task<IActionResult> SearchGames(
            [FromQuery] string? filter,   // Optional query parameter for filtering (e.g., part of the title)
            [FromQuery] string? sort = "asc" // Optional query parameter for sorting order (defaults to "asc")
        )
        {
            // Retrieve all games from the service.
            var games = await _gameService.GetAllGamesAsync();

            // If a filter value is provided, filter the games based on whether the Title contains the filter string.
            if (!string.IsNullOrEmpty(filter))
                games = games.Where(g => g.Title.Contains(filter));

            // Sort the games by Title; sort order is based on the 'sort' query parameter.
            if (sort == "desc")
                games = games.OrderByDescending(g => g.Title);
            else
                games = games.OrderBy(g => g.Title);

            // Return the filtered and sorted list of games.
            return Ok(games);
        }
    }
}
