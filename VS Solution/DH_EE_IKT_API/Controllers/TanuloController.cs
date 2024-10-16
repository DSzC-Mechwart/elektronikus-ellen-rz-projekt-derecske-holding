﻿using DH_EE_IKT_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DH_EE_IKT_API.Models;

namespace DH_EE_IKT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TanuloController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TanuloController(AppDbContext context) { _context = context;  }

        [HttpGet]
        public IEnumerable<Tanulo> GetTanulok()
        {
            return _context.Tanulok;
        }

        [HttpGet("{osztalyId}")]
        public IEnumerable<Tanulo> GetOsztalyTanulok(string osztalyId)
        {
            return _context.Tanulok.Where( x => x.Osztaly_ID == osztalyId);
        }

        [HttpPost]
        public async Task<IActionResult> AddTanulo([FromBody] Tanulo tanulo) 
        {
            _context.Tanulok.Add(tanulo);
            await _context.SaveChangesAsync();
            return Created();
        }
    }
}
