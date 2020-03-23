using System;
using MediatR;

namespace Events
{
    public class GetMessagesQuery : GetCollectionPagedByPageNumber<Message> { }
}