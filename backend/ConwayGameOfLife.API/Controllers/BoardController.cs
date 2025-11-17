// using ConwayGameOfLife.API.DTOs;
// using ConwayGameOfLife.Core.DTOs;
// using ConwayGameOfLife.Core.Services;
// using ConwayGameOfLife.Infrastructure.Models;
// using ConwayGameOfLife.Infrastructure.Repositories;
// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;

// namespace ConwayGameOfLife.API.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class BoardController : ControllerBase
//     {

//         private readonly IGameOfLifeService _gameOfLifeService;
//         private readonly BoardRepository _boardRepository;

//         public BoardController(IGameOfLifeService gameOfLifeService, BoardRepository boardRepository) { 
//             _gameOfLifeService = gameOfLifeService;
//             _boardRepository = boardRepository;
//         }

//         // Save the current board state
//         [HttpPost("upload")]
//         public async Task<ActionResult<UploadBoardResponse>> CreateBoard([FromBody] UploadBoardRequest request)
//         {
//             if (request == null || request.LiveCells == null)
//             {
//                 return BadRequest("Invalid request data.");
//             }

//             string liveCellsJson = JsonConvert.SerializeObject(request.LiveCells);

//             var board = new GameBoard
//             {
//                 LiveCellsJson = liveCellsJson
//             };

//             await _boardRepository.AddAsync(board);

//             return CreatedAtAction(nameof(GetCurrentBoard), new { id = board.Id }, new UploadBoardResponse { BoardId = board.Id });
//         }

//         // Get the saved Board List
//         [HttpGet("all")]
//         public async Task<ActionResult<GetAllBoardResponse>> GetAllBoards()
//         {
//             var boards = await _boardRepository.GetAllAsync();
//             if (boards == null)
//             {
//                 return NotFound();
//             }

//             return Ok(boards);
//         }

//         // Get the final stable state
//         [HttpPost("simulate")]
//         public async Task<ActionResult<SimulateResponse>> Simulate([FromBody] SimulateRequest request)
//         {
//             if (request == null || request.LiveCells == null)
//             {
//                 return BadRequest("Invalid request data.");
//             }
  
//             var liveCellsSet = request.LiveCells.Select(cell => (cell.X, cell.Y)).ToHashSet();

//             var result = await _gameOfLifeService.SimulateAsync(liveCellsSet, request.MaxStepNum, true);

//             var response = new SimulateResponse
//             {
//                 LiveCells = new List<CellDto>()
//             };


//             response.LiveCells = result.LiveCells.Select(cell => new CellDto { X = cell.X, Y = cell.Y }).ToList();
//             response.PeriodicNum = result.Periodic;
//             response.GenerateStepNum = result.NextStepNum;

//             return Ok(response);
//         }

        
//         // Get the Current board State by id
//         [HttpGet("{id}/current")]
//         public async Task<ActionResult<GetBoardResponse>> GetCurrentBoard(int id)
//         {
//             var board = await _boardRepository.GetByIdAsync(id);
//             if (board == null)
//             {
//                 return NotFound();
//             }

//             // Deserialize live cells JSON back to list
//             var liveCellsList = JsonConvert.DeserializeObject<List<Cell>>(board.LiveCellsJson);

//             var response = new GetBoardResponse
//             {
//                 LiveCells = new List<CellDto>()
//             };

//             response.LiveCells = liveCellsList.Select(cell => new CellDto { X = cell.X, Y = cell.Y }).ToList();

//             return Ok(response);
//         }

//         // Get the Next board state by id
//         [HttpGet("{id}/next")]
//         public async Task<ActionResult<GetBoardResponse>> GetNextBoard(int id)
//         {
//             var board = await _boardRepository.GetByIdAsync(id);
//             if (board == null)
//             {
//                 return NotFound();
//             }

//             var liveCellsSet = JsonConvert.DeserializeObject<List<Cell>>(board.LiveCellsJson).Select(cell => (cell.X, cell.Y)).ToHashSet();

//             // get only next step
//             var result = await _gameOfLifeService.SimulateAsync(liveCellsSet, 1, false);

//             var response = new SimulateResponse
//             {
//                 LiveCells = new List<CellDto>()
//             };
//             response.LiveCells = result.LiveCells.Select(cell => new CellDto { X = cell.X, Y = cell.Y }).ToList();
//             response.PeriodicNum = result.Periodic;
//             response.GenerateStepNum = result.NextStepNum;
//             var LiveCellsJson = JsonConvert.SerializeObject(response.LiveCells);
//             board.LiveCellsJson = LiveCellsJson;
//             await _boardRepository.UpdateAsync(board);
//             return Ok(response);
//         }

//         // Get the Next N state
//         [HttpGet("{id}/nextN/{generationNum}")]
//         public async Task<ActionResult<GenerateNAheadResponse>> GenerateNAhead(int id, int generationNum)
//         {
//             var board = await _boardRepository.GetByIdAsync(id);
//             if (board == null)
//             {
//                 return NotFound();
//             }

//             var liveCellsSet = JsonConvert.DeserializeObject<List<Cell>>(board.LiveCellsJson).Select(cell => (cell.X, cell.Y)).ToHashSet();

//             var result = await _gameOfLifeService.SimulateAsync(liveCellsSet, generationNum, false);

//             var response = new GenerateNAheadResponse
//             {
//                 LiveCells = new List<CellDto>()
//             };

//             response.LiveCells = result.LiveCells.Select(cell => new CellDto { X = cell.X, Y = cell.Y }).ToList();
//             var LiveCellsJson = JsonConvert.SerializeObject(response.LiveCells);
//             board.LiveCellsJson = LiveCellsJson;
//             await _boardRepository.UpdateAsync(board);
//             return Ok(response);
//         }

//         [HttpDelete("{id}")]
//         public async Task <ActionResult> DeleteBoardState(int id)
//         {
//             var board = await _boardRepository.GetByIdAsync(id);
//             if (board == null)
//             {
//                 return NotFound();
//             }
//             await _boardRepository.DeleteAsync(id);
//             return Ok();
//         }
//     }
// }

