using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TodoMcp.Shared;

[McpServerToolType]
public static class TodoMcpTools
{
    [McpServerTool]
    [Description("Get todos from the todo list. Can optionally filter by completion status.")]
    public static string GetTodos(
        [Description("Filter by completion status: true for completed items, false for incomplete items, or omit to get all items")] 
        bool? isComplete = null)
    {
        var todos = TodoStore.GetAll(isComplete);
        return JsonSerializer.Serialize(todos);
    }

    [McpServerTool]
    [Description("Add a single todo to the todo list.")]
    public static string AddTodo([Description("The title of the todo to add")] string title)
    {
        var added = TodoStore.Add(title);
        return JsonSerializer.Serialize(added);
    }

    [McpServerTool]
    [Description("Add multiple todos at once. Use this when the user wants to add several items. Provide a JSON array of title strings.")]
    public static string AddMultipleTodos([Description("JSON array of todo titles, e.g. [\"Buy milk\", \"Walk dog\", \"Send email\"]")] string titlesJson)
    {
        var titles = JsonSerializer.Deserialize<string[]>(titlesJson) ?? Array.Empty<string>();
        var addedTodos = titles.Select(title => TodoStore.Add(title)).ToList();
        return JsonSerializer.Serialize(addedTodos);
    }

    [McpServerTool, Description("Mark a todo item as complete.")]
    public static string MarkTodoComplete([Description("The ID of the todo to mark as complete")] int id)
    {
        var updated = TodoStore.MarkComplete(id);
        if (updated == null)
        {
            return JsonSerializer.Serialize(new { success = false, error = $"Todo with ID {id} not found" });
        }
        return JsonSerializer.Serialize(new { success = true, todo = updated });
    }

    [McpServerTool]
    [Description("Mark a todo item as incomplete (not done).")]
    public static string MarkTodoIncomplete([Description("The ID of the todo to mark as incomplete")] int id)
    {
        var updated = TodoStore.MarkIncomplete(id);
        if (updated == null)
        {
            return JsonSerializer.Serialize(new { success = false, error = $"Todo with ID {id} not found" });
        }
        return JsonSerializer.Serialize(new { success = true, todo = updated });
    }
}
