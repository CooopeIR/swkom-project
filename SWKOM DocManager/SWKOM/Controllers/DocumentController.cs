﻿using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SWKOM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "GetDocument")]
        public string Post()
        {
            var documentInformation = new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1."
            };

            return JsonSerializer.Serialize(documentInformation);
        }

        [HttpGet(Name = "GetDocument")]
        public string Get()
        {
            var documentInformation = new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1."
            };

            return JsonSerializer.Serialize(documentInformation);
        }

        [HttpPut(Name = "GetDocument")]
        public string Put()
        {
            var documentInformation = new DocumentInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Title = "Title 1",
                Author = "Me",
                Content = "Line 1."
            };

            return JsonSerializer.Serialize(documentInformation);
        }
    }
}
