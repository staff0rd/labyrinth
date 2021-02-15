using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Events
{
    ///public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    public class HydrateEntityCommandHandler<T> : IRequestHandler<HydrateEntityCommand<T>>
        where T : IExternalEntity
    {
        private readonly ILogger<HydrateEntityCommandHandler<T>> _logger;
        private readonly EventRepository _events;
        private readonly IProgress _progress;
        private readonly Store _store;
        private readonly IMediator _mediator;

        public HydrateEntityCommandHandler(ILogger<HydrateEntityCommandHandler<T>> logger, Store store, IMediator mediator,
            EventRepository events, IProgress progress)
        {
            _logger = logger;
            _store = store;
            _events = events;
            _progress = progress;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(HydrateEntityCommand<T> request, CancellationToken cancellationToken)
        {
            await Hydrate(
                new Credential(request.LabyrinthUsername, request.LabyrinthPassword), 
                request.EntityType,
                request.SourceId,
                request.EventType,
                request.Dictionary
            );

            return Unit.Value;
        }

        public Func<Event[], int, Task<int>> FillFromEvents(Dictionary<string, T> dictionary)
        {
            var count = 0;
            return (events, totalCount) => {
                var eventName = $"{typeof(T).Name}Created";
                var deserialized = events
                    .Where(p => p.EventName == eventName)
                    .Select(p => JsonConvert.DeserializeObject<T>(p.Body))
                    .ToList();
                foreach (var item in deserialized) {
                    count++;
                    _progress.Set(count, totalCount);
                    if (!dictionary.ContainsKey(item.Id))
                        dictionary.Add(item.Id, item);
                }
                return Task.FromResult(events.Last().Id);
            };
        }

        private async Task Hydrate(Credential credential, string entityType, Guid sourceId, string eventType, Dictionary<string, T> dictionary)
        {
            var count = await _events.GetCount(credential.Username, sourceId, eventType);
            if (count == 0)
                return;
            _logger.LogInformation($"Hydrating {entityType}...");
            await _progress.New();
            Func<Event[], int, Task<int>> eventProcessor = FillFromEvents(dictionary);
            await _events.ReadForward(credential, sourceId, count, eventProcessor);
        }
    }
}