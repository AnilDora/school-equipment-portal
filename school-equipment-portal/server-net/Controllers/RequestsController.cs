using Microsoft.AspNetCore.Mvc;
using SchoolEquipmentApi.DTOs;
using SchoolEquipmentApi.Modules;

namespace SchoolEquipmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IDataStore _dataStore;

        public RequestsController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        [HttpPost]
        public IActionResult CreateRequest([FromBody] RequestInput input)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            if (string.IsNullOrWhiteSpace(input.EquipmentId) || string.IsNullOrWhiteSpace(input.StartDate) || string.IsNullOrWhiteSpace(input.EndDate))
                return BadRequest(new { message = "equipmentId, startDate and endDate required" });
                
            var eq = data.Equipment.FirstOrDefault(x => x.Id == input.EquipmentId);
            if (eq == null) 
                return NotFound(new { message = "Equipment not found" });
                
            var approved = data.Requests.Where(r => r.EquipmentId == input.EquipmentId && r.Status == "approved").ToList();
            foreach (var a in approved)
            {
                if (Helpers.Overlaps(input.StartDate, input.EndDate, a.StartDate, a.EndDate)) 
                    return Conflict(new { message = "Requested period overlaps with existing approved booking" });
            }
            
            var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x");
            var reqItem = new BorrowRequest 
            { 
                Id = id, 
                EquipmentId = input.EquipmentId, 
                Requester = session.Username, 
                StartDate = input.StartDate, 
                EndDate = input.EndDate, 
                Status = "pending", 
                CreatedAt = DateTimeOffset.UtcNow.ToString("o") 
            };
            
            data.Requests.Add(reqItem);
            _dataStore.Write(data);
            
            return Ok(reqItem);
        }

        [HttpGet]
        public IActionResult GetRequests()
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            if (session.Role == "admin" || session.Role == "staff") 
                return Ok(data.Requests);
                
            var mine = data.Requests.Where(r => r.Requester == session.Username).ToList();
            return Ok(mine);
        }

        [HttpPut("{id}/approve")]
        public IActionResult ApproveRequest(string id)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            if (session.Role != "admin" && session.Role != "staff") 
                return StatusCode(403);
                
            var idx = data.Requests.FindIndex(r => r.Id == id);
            if (idx == -1) 
                return NotFound();
                
            var r = data.Requests[idx];
            var approved = data.Requests.Where(x => x.EquipmentId == r.EquipmentId && x.Status == "approved" && x.Id != r.Id).ToList();
            
            foreach (var a in approved)
            {
                if (Helpers.Overlaps(r.StartDate, r.EndDate, a.StartDate, a.EndDate)) 
                    return Conflict(new { message = "Requested period overlaps with existing approved booking" });
            }
            
            r.Status = "approved";
            r.Approver = session.Username;
            r.ApprovedAt = DateTimeOffset.UtcNow.ToString("o");
            
            // decrement equipment quantity when approved
            var eqIdx = data.Equipment.FindIndex(e => e.Id == r.EquipmentId);
            if (eqIdx == -1) 
                return NotFound(new { message = "Equipment not found" });
                
            var eq = data.Equipment[eqIdx];
            if (eq.Quantity <= 0) 
                return Conflict(new { message = "No items available to approve this request" });
                
            eq.Quantity -= 1;
            if (eq.Quantity == 0) eq.Available = false;
            data.Equipment[eqIdx] = eq;

            data.Requests[idx] = r;
            _dataStore.Write(data);
            
            return Ok(r);
        }

        [HttpPut("{id}/reject")]
        public IActionResult RejectRequest(string id)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            if (session.Role != "admin" && session.Role != "staff") 
                return StatusCode(403);
                
            var idx = data.Requests.FindIndex(r => r.Id == id);
            if (idx == -1) 
                return NotFound();
                
            var r = data.Requests[idx];
            r.Status = "rejected";
            r.Approver = session.Username;
            r.RejectedAt = DateTimeOffset.UtcNow.ToString("o");
            
            data.Requests[idx] = r;
            _dataStore.Write(data);
            
            return Ok(r);
        }

        [HttpPut("{id}/return")]
        public IActionResult ReturnRequest(string id)
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            var idx = data.Requests.FindIndex(r => r.Id == id);
            if (idx == -1) 
                return NotFound();
                
            var r = data.Requests[idx];
            if (session.Role != "admin" && session.Role != "staff" && session.Username != r.Requester) 
                return StatusCode(403);
                
            r.Status = "returned";
            r.ReturnedAt = DateTimeOffset.UtcNow.ToString("o");
            
            // increment equipment quantity when returned
            var eqIdxR = data.Equipment.FindIndex(e => e.Id == r.EquipmentId);
            if (eqIdxR != -1)
            {
                var eqR = data.Equipment[eqIdxR];
                eqR.Quantity += 1;
                if (eqR.Quantity > 0) eqR.Available = true;
                data.Equipment[eqIdxR] = eqR;
            }

            data.Requests[idx] = r;
            _dataStore.Write(data);
            
            return Ok(r);
        }
    }
}