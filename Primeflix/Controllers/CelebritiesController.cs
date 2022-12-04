using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primeflix.Data;
using Primeflix.DTO;
using Primeflix.Models;
using Primeflix.Services;

namespace Primeflix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CelebritiesController : Controller
    {
        private ICelebrityRepository _celebrityRepository;

        public CelebritiesController(ICelebrityRepository celebrityRepository)
        {
            _celebrityRepository = celebrityRepository;
        }

        //api/celebrities
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetCelebrities()
        {
            var celebrities = _celebrityRepository.GetCelebrities();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebritiesDto = new List<CelebrityDto>();
            foreach (var celebrity in celebrities)
            {
                celebritiesDto.Add(new CelebrityDto
                {
                    Id = celebrity.Id,
                    FirstName = celebrity.FirstName,
                    LastName = celebrity.LastName
                });
            }
            return Ok(celebritiesDto);
        }

        //api/celebrities/celebrityId
        [HttpGet("{celebrityId}", Name = "GetCelebrity")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CelebrityDto>))]
        public IActionResult GetCelebrity(int celebrityId)
        {
            if (!_celebrityRepository.CelebrityExists(celebrityId))
                return NotFound();

            var celebrity = _celebrityRepository.GetCelebrity(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var celebrityDto = new CelebrityDto()
            {
                Id = celebrity.Id,
                FirstName = celebrity.FirstName,
                LastName = celebrity.LastName
            };

            return Ok(celebrityDto);
        }

        //api/celebrities
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Celebrity))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCelebrity([FromBody] Celebrity celebrityToCreate)
        {
            if (celebrityToCreate == null)
                return BadRequest(ModelState);

            var celebrity = _celebrityRepository.GetCelebrities()
                .Where(c => c.FirstName.Trim().ToUpper() == celebrityToCreate.FirstName.Trim().ToUpper() && c.LastName.Trim().ToUpper() == celebrityToCreate.LastName.Trim().ToUpper())
                .FirstOrDefault();

            if (celebrity != null)
            {
                ModelState.AddModelError("", $"Celebrity {celebrityToCreate.FirstName} {celebrityToCreate.LastName} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_celebrityRepository.CreateCelebrity(celebrityToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {celebrityToCreate.FirstName} {celebrityToCreate.LastName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCelebrity", new { celebrityId = celebrityToCreate.Id }, celebrityToCreate);
        }

        //api/celebrities/celebrityId
        [HttpPut("{celebrityId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCelebrity(int celebrityId, [FromBody] Celebrity updatedCelebrity)
        {
            if (updatedCelebrity == null)
                return BadRequest(ModelState);

            if (celebrityId != updatedCelebrity.Id)
                return BadRequest(ModelState);

            if (!_celebrityRepository.CelebrityExists(celebrityId))
                return NotFound();

            bool test = _celebrityRepository.IsDuplicate(celebrityId, updatedCelebrity.FirstName, updatedCelebrity.LastName);

            if (test)
            {
                ModelState.AddModelError("", $"Celebrity {updatedCelebrity.FirstName} {updatedCelebrity.LastName} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_celebrityRepository.UpdateCelebrity(updatedCelebrity))
            {
                ModelState.AddModelError("", $"Something went wrong update {updatedCelebrity.FirstName} {updatedCelebrity.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/celebrities/celebrityId
        [HttpDelete("{celebrityId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCelebrity(int celebrityId)
        {
            if (!_celebrityRepository.CelebrityExists(celebrityId))
                return NotFound();

            var celebrityToDelete = _celebrityRepository.GetCelebrity(celebrityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_celebrityRepository.DeleteCelebrity(celebrityToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {celebrityToDelete.FirstName} {celebrityToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
