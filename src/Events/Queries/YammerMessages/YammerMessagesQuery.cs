using System;
using MediatR;

namespace Events
{
    public class YammerMessagesQuery : GetCollectionPagedByPageNumber<Message> { }
}