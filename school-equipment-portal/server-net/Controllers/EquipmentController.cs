using Microsoft.AspNetCore.Mvc;
using SchoolEquipmentApi.DTOs;
using SchoolEquipmentApi.Modules;

namespace SchoolEquipmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IDataStore _dataStore;

        public EquipmentController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        [HttpGet]
        public IActionResult GetAllEquipment()
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            return Ok(data.Equipment);
        }

        [HttpGet("{id}")]
        public IActionResult GetEquipment(string id)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            var equipment = data.Equipment.FirstOrDefault(e => e.Id == id);
            if (equipment == null)
                return NotFound();
                
            return Ok(equipment);
        }

        [HttpPost]
        public IActionResult CreateEquipment([FromBody] EquipmentInput input)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            if (session.Role != "admin")
                return StatusCode(403);
                
            if (string.IsNullOrWhiteSpace(input.Name))
                return BadRequest(new { message = "Name is required" });
                
            var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x");
            var equipment = new Equipment
            {
                Id = id,
                Name = input.Name,
                Category = input.Category ?? "",
                Condition = input.Condition ?? "Good",
                Quantity = input.Quantity,
                Available = input.Available
            };
            
            data.Equipment.Add(equipment);
            _dataStore.Write(data);
            
            return Ok(equipment);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEquipment(string id, [FromBody] EquipmentInput input)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            if (session.Role != "admin")
                return StatusCode(403);
                
            if (string.IsNullOrWhiteSpace(input.Name))
                return BadRequest(new { message = "Name is required" });
                
            var idx = data.Equipment.FindIndex(e => e.Id == id);
            if (idx == -1)
                return NotFound();
                
            var equipment = data.Equipment[idx];
            equipment.Name = input.Name;
            equipment.Category = input.Category ?? equipment.Category;
            equipment.Condition = input.Condition ?? equipment.Condition;
            equipment.Quantity = input.Quantity;
            equipment.Available = input.Available;
            
            data.Equipment[idx] = equipment;
            _dataStore.Write(data);
            
            return Ok(equipment);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEquipment(string id)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            if (session.Role != "admin")
                return StatusCode(403);
                
            var idx = data.Equipment.FindIndex(e => e.Id == id);
            if (idx == -1)
                return NotFound();
                
            // Check if equipment has pending or approved requests
            var hasActiveRequests = data.Requests.Any(r => r.EquipmentId == id && 
                (r.Status == "pending" || r.Status == "approved"));
                
            if (hasActiveRequests)
                return Conflict(new { message = "Cannot delete equipment with active requests" });
                
            data.Equipment.RemoveAt(idx);
            _dataStore.Write(data);
            
            return Ok(new { message = "Equipment deleted successfully" });
        }
    }
}