using System.Text.Json;

class Program
{
    public class Todo
    {
        public int Id { get; set; }
        public bool Done { get; set; }
        public string Title { get; set; } = string.Empty;
        // can't make title required for deserialization
        // so lets give default value so we don't have to make it optional
    }

    static List<Todo> Todos = new List<Todo>();

    const string PATH = "./todo.json";

    static void Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                Usage();
            }
            else if (args.Length == 1 && args[0] == "list")
            {
                ListTodos();
            }
            else if (args.Length == 2 && args[0] == "add")
            {
                AddTodos(args[1]);
            }
            else if (args.Length == 3 && args[0] == "edit")
            {
                EditTodos(args[1], args[2]);
            }
            else if (args.Length == 2 && args[0] == "done")
            {
                DoneTodos(args[1]);
            }
            else if (args.Length == 2 && args[0] == "delete")
            {
                DeleteTodos(args[1]);
            }
            else if (args.Length == 1 && args[0] == "clear")
            {
                ClearTodos();
            }
            else if (args.Length == 1)
            {
                Console.WriteLine($"Unknown command: {args[0]}.");
                Usage();
            } else
            {
                Console.WriteLine($"Wrong number of parameters for command: {args[0]}.");
                Usage();
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.ToString()}");
        }

    }

    static void Usage()
    {
        Console.WriteLine();
        Console.WriteLine("usage: td <command> [<args>]");
        Console.WriteLine();
        Console.WriteLine("commands:");
        Console.WriteLine("  list                  list all todos");
        Console.WriteLine("  add          <title>  add a new todo");
        Console.WriteLine("  edit    <id> <title>  edit the title of a todo");
        Console.WriteLine("  done    <id>          toggle the done status of a todo");
        Console.WriteLine("  delete  <id>          delete a todo");
        Console.WriteLine("  clear                 delete all todos");
        Console.WriteLine();
    }

    static int GetNextId()
    {
        int maxId = 0;

        foreach (var todo in Todos)
        {
            if (maxId < todo.Id)
            {
                maxId = todo.Id;
            }
        }

        return 1 + maxId;
    }

    static void LoadTodos()
    {
        if (File.Exists(PATH))
        {
            var jsonText = File.ReadAllText(PATH);
            Todos = JsonSerializer.Deserialize<List<Todo>>(jsonText) ?? Todos;
        }
    }

    static void SaveTodos()
    {
        var serializerOptions = new JsonSerializerOptions() {WriteIndented = true, IndentSize = 4};
        var jsonText = JsonSerializer.Serialize(Todos, serializerOptions);
        File.WriteAllText(PATH, jsonText);
    }

    static void ListTodos()
    {
        LoadTodos();

        foreach (var todo in Todos)
        {
            Console.WriteLine($"{todo.Id}, {todo.Done}, {todo.Title}");
        }
    }

    static void AddTodos(string title)
    {
        LoadTodos();

        var nextId = GetNextId();

        var newTodo = new Todo() {Id = nextId, Done = false, Title = title};
        Todos.Add(newTodo);

        SaveTodos();
    }

    static void EditTodos(string idStr, string title)
    {
        bool found = false;

        LoadTodos();

        if (!int.TryParse(idStr, out int id))
        {
            Console.WriteLine($"Id param not an integer: {idStr}");
            return;
        }

        foreach (var todo in Todos)
        {
            if (todo.Id == id)
            {
                todo.Title = title;
                found = true;
                break;
            }
        }

        if (found)
        {
            SaveTodos();
        } else
        {
            Console.WriteLine($"Id not found: {id}");
        }
    }

    static void DoneTodos(string idStr)
    {
        bool found = false;

        LoadTodos();

        if (!int.TryParse(idStr, out int id))
        {
            Console.WriteLine($"Id param is not an integer: {idStr}");
            return;
        }

        foreach (var todo in Todos)
        {
            if (todo.Id == id)
            {
                todo.Done = !todo.Done;
                found = true;
                break;
            }
        }

        if (found)
        {
            SaveTodos();
        }
        else
        {
            Console.WriteLine($"Id not found: {id}");
        }
    }

    static void DeleteTodos(string idStr)
    {
        bool found = false;

        if (!int.TryParse(idStr, out int id))
        {
            Console.WriteLine($"Id param not an integer: {idStr}");
            return;
        }

        LoadTodos();

        for (int i = 0; i < Todos.Count; i++)
        {
            if (Todos[i].Id == id)
            {
                Todos.RemoveAt(i);
                found = true;
                break;
            }
        }

        if (found)
        {
            SaveTodos();
        } else
        {
            Console.WriteLine($"Id not found: {id}");
        }

    }

    static void ClearTodos()
    {
        if (File.Exists(PATH)) 
        {
            File.Delete(PATH);
        } else
        {
            Console.WriteLine($"File not found: {PATH}");
        }
    }

}