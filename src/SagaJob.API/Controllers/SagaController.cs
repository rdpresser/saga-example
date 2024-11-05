using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SagaJob.Contracts.Enums;
using SagaJob.Contracts.Messages;

namespace SagaJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagaController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        //saga deve publicar evento de BatchSubmitted/SubmitBatch
        public SagaController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentException(nameof(publishEndpoint));
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { result = "hello" });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SubmitBatch batch)
        {
            await _publishEndpoint.Publish<ExportTokensBatchReceived>(new
            {
                BatchId = NewId.NextGuid(),
                BatchType = BatchTypeEnum.Unknown,
                InVar.Timestamp,
                TokenIds = GenerateTokens(batch.TokenCount),
                batch.ActiveThreshold
            });

            return Ok();
        }

        private Guid[] GenerateTokens(int count)
        {
            var tokens = new Guid[count];
            for (int i = 0; i < count; i++)
            {
                tokens[i] = Guid.NewGuid();
            }

            return tokens;
        }
    }
}