using System.Threading;

namespace TodoMcp.Shared;

public static class TodoStore
{
    private static readonly List<Todo> _todos = new();
    private static int _nextId = 0;

    public static IEnumerable<Todo> GetAll(bool? isComplete = null)
    {
        if (isComplete == null)
            return _todos;
        
        return _todos.Where(t => t.IsComplete == isComplete.Value);
    }

    public static Todo Add(string title)
    {
        var id = Interlocked.Increment(ref _nextId);
        var todo = new Todo(id, title, false);
        _todos.Add(todo);
        return todo;
    }

    public static Todo Add(Todo todo)
    {
        var id = Interlocked.Increment(ref _nextId);
        var t = new Todo(id, todo.Title, todo.IsComplete);
        _todos.Add(t);
        return t;
    }

    public static Todo? MarkComplete(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null) return null;
        
        var updated = new Todo(todo.Id, todo.Title, true);
        var index = _todos.IndexOf(todo);
        _todos[index] = updated;
        return updated;
    }

    public static Todo? MarkIncomplete(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null) return null;
        
        var updated = new Todo(todo.Id, todo.Title, false);
        var index = _todos.IndexOf(todo);
        _todos[index] = updated;
        return updated;
    }
}
