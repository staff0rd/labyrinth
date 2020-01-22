﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Events.Yammer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rest.Yammer;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YammerController : ControllerBase
    {
        private readonly ILogger<YammerController> _logger;

        private readonly RestEventManager _events;

        private readonly Store _store;

        public YammerController(ILogger<YammerController> logger, RestEventManager events, Store store)
        {
            _logger = logger;
            _events = events;
            _store = store;
        }

        [HttpGet]
        [Route("messages")]
        public PagedResult<Events.Message> GetMessages(int pageNumber = 0, int pageSize = 20, string search = "")
        {
            var messages = _store.Messages
                .OrderByDescending(p => p.Value.CreatedAt)
                .Select(p => p.Value);
                
            var result = messages.GetPagedResult(pageNumber, pageSize, (m) => {
                return m;
            });
            return result;
        }

        [HttpGet]
        [Route("")]
        public Overview GetOverview()
        {
            var overview = _store.GetOverview();
            return overview;
        }

        [HttpGet]
        [Route("users/{id}")]
        public UserCard GetUser(string id) {
            var user = _store.Users[id];
            return UserCard.FromUser(user);
        }

        [HttpGet]
        [Route("users")]
        public PagedResult<UserCard> GetUsers(int pageNumber = 0, int pageSize = 20, string search = "")
        {
            var users = _store.Users.Select(x => x.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                Func<string, string> ifNull = (string a) => a == null ? "" : a;
                users = users.Where(u => ifNull(u.Name).ToLower().Contains(search.ToLower()) || ifNull(u.Description).ToLower().Contains(search.ToLower())).ToArray();
            }

            return users.GetPagedResult(pageNumber, pageSize, (u) => UserCard.FromUser(u));
        }

        private static PagedResult<TResult> PagedResult<TCollection, TResult>(TCollection[] items, int pageNumber, int pageSize, Func<TCollection, TResult> selector)
        {
            return new PagedResult<TResult>
            {
                Page = pageNumber,
                PageSize = pageSize,
                Rows = items
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(selector)
                    .ToArray(),
                TotalRows = items.Count()
            };
        }
    }
}
