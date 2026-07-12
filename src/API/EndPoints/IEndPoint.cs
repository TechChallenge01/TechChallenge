using API.Extensions;
﻿namespace API.EndPoints
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
