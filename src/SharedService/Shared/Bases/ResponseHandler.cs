﻿using Microsoft.Extensions.Localization;
using Shared.Resources;
using System.Net;

namespace Shared.Bases
{
    public class ResponseHandler
    {
        public readonly IStringLocalizer<SharedResource> _localizer;
        public ResponseHandler(IStringLocalizer<SharedResource> localizer)
        {

            _localizer = localizer;
        }

        public Response<T> Deleted<T>()
        {
            return new Response<T>()
            {
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = _localizer[SharedResourceKeys.DeletedSudccessfully]
            };
        }
        public Response<T> Success<T>(T entity, object? Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = HttpStatusCode.OK,
                Succeeded = true,
                Message = _localizer[SharedResourceKeys.MessageSuccess],
                Meta = Meta
            };
        }

        public Response<T> Unauthorized<T>(string message)
        {
            return new Response<T>()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Succeeded = true,
                Message = _localizer[SharedResourceKeys.UnAuthorized]
            };
        }
        public Response<T> BadRequest<T>(string? Message = null)
        {
            return new Response<T>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Succeeded = false,
                Message = Message == null ? _localizer[SharedResourceKeys.NotExist] : Message
            };
        }

        public Response<T> NotFound<T>(string? message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Succeeded = false,
                Message = message == null ? _localizer[SharedResourceKeys.ThisItemNotFound] : message
            };
        }

        public Response<T> Created<T>(T entity, object? Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.Created,
                Succeeded = true,
                Message = _localizer[SharedResourceKeys.MessageSuccess],
                Meta = Meta
            };
        }
    }
}
