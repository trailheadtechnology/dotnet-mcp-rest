using TodoMcp.Shared;

public class TodoEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");
        api.MapGet("/todos", GetTodosHandler);
        api.MapPost("/todos", AddTodoHandler);
        api.MapPatch("/todos/{id}/complete", MarkTodoCompleteHandler);
        api.MapPatch("/todos/{id}/incomplete", MarkTodoIncompleteHandler);
    }

    private static IResult GetTodosHandler(bool? isComplete)
    {
        var todos = TodoStore.GetAll(isComplete);
        return Results.Ok(todos);
    }

    private static IResult AddTodoHandler(Todo todo)
    {
        var added = TodoStore.Add(todo);
        return Results.Created($"/todos/{added.Id}", added);
    }

    private static IResult MarkTodoCompleteHandler(int id)
    {
        var updated = TodoStore.MarkComplete(id);
        return updated == null ? Results.NotFound() : Results.Ok(updated);
    }

    private static IResult MarkTodoIncompleteHandler(int id)
    {
        var updated = TodoStore.MarkIncomplete(id);
        return updated == null ? Results.NotFound() : Results.Ok(updated);
    }
}