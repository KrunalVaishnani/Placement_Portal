using Microsoft.AspNetCore.Mvc;
using Placement_Portal_APIs.Data;
using Placement_Portal_APIs.Models;
using System.Collections.Generic;

namespace Placement_Portal_APIs.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackRepository _feedbackRepository;

        public FeedbackController(FeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        [HttpGet]
        public IActionResult GetAllFeedback()
        {
            var feedbacks = _feedbackRepository.SelectAll();
            return Ok(feedbacks);
        }

        [HttpGet("{id}")]
        public IActionResult GetFeedbackById(int id)
        {
            var feedback = _feedbackRepository.SelectByPk(id);
            if (feedback == null)
            {
                return NotFound("Feedback not found.");
            }
            return Ok(feedback);
        }

        [HttpPost]
        public IActionResult AddFeedback([FromBody] FeedbackModel feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var isAdded = _feedbackRepository.Add(feedback);
            if (!isAdded)
            {
                return BadRequest("Failed to add feedback.");
            }
            return Ok("Feedback added successfully.");
        }

        [HttpPut]
        public IActionResult UpdateFeedback([FromBody] FeedbackModel feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var isUpdated = _feedbackRepository.Update(feedback);
            if (!isUpdated)
            {
                return BadRequest("Failed to update feedback.");
            }
            return Ok("Feedback updated successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFeedback(int id)
        {
            var isDeleted = _feedbackRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound("Feedback not found.");
            }
            return NoContent();
        }
    }
}
