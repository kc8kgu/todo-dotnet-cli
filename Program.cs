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
            if (args.Length == 1 && args[0] == "list")
            {
                ListTodos();
            }
            else if (args.Length == 2 && args[0] == "add")
            {
                AddTodos();
            }
            else if (args.Length == 3 && args[0] == "edit")
            {
                EditTodos();
            }
            else if (args.Length == 2 && args[0] == "done")
            {
                DoneTodos();
            }
            else if (args.Length == 2 && args[0] == "delete")
            {
                DeleteTodos();
            }
            else if (args.Length == 1 && args[0] == "clear")
            {
                ClearTodos();
            }
            else 
            {
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
        Console.WriteLine("usage: td <list|add|edit|done|delete|clear> <id> <title>");
    }

    static int GetNextId()
    {
        int MaxId = 0;
        return 1 + MaxId;
    }

    static void LoadTodos()
    {
        if (File.Exists(PATH))
        {
            var JsonText = File.ReadAllText(PATH);
            Todos = JsonSerializer.Deserialize<List<Todo>>(JsonText) ?? Todos;
        }
        else
        {
            Todos.Add(new Todo() { Id = 1, Done = false, Title = "get init" });
            Todos.Add(new Todo() { Id = 2, Done = false, Title = "get add ." });
            Todos.Add(new Todo() { Id = 3, Done = false, Title = "get commit -m \"initial commit\"" });
        }
    }

    static void SaveTodos()
    {
        var DeserialzerOptions = new JsonSerializerOptions() {WriteIndented = true, IndentSize = 4};
        var JsonText = JsonSerializer.Serialize(Todos, DeserialzerOptions);
        File.WriteAllText(PATH, JsonText);
    }

    static void ListTodos()
    {
        LoadTodos();

        foreach (var Todo in Todos)
        {
            Console.WriteLine($"{Todo.Id}, {Todo.Done}, {Todo.Title}");
        }
    }

    static void AddTodos()
    {
        LoadTodos();

        var NextId = GetNextId();
        var Title = Environment.GetCommandLineArgs()[1];

        var NewTodo = new Todo() {Id = NextId, Done = false, Title = Title};
        Todos.Add(NewTodo);

        SaveTodos();
    }

    static void EditTodos()
    {
        bool Found = false;

        LoadTodos();

        string IdStr = Environment.GetCommandLineArgs()[2];
        string Title = Environment.GetCommandLineArgs()[3];

        if (!int.TryParse(IdStr, out int Id))
        {
            Console.WriteLine($"Id param not an integer: {IdStr}");
            return;
        }

        foreach (var Todo in Todos)
        {
            if (Todo.Id == Id)
            {
                Todo.Title = Title;
                Found = true;
                break;
            }
        }

        if (Found)
        { 
            SaveTodos();
        } else
        {
            Console.WriteLine($"Id not found: {Id}");          
        }
    }

    static void DoneTodos()
    {
        bool Found = false;

        LoadTodos();

        string IdStr = Environment.GetCommandLineArgs()[2];

        if (!int.TryParse(IdStr, out int Id))
        {
            Console.WriteLine($"Id param not an integer: {IdStr}");
            return;
        }

        foreach (var Todo in Todos)
        {
            if (Todo.Id == Id)
            {
                Todo.Done = !Todo.Done;
                Found = true;
                break;
            }
        }

        if (Found) 
        {
            SaveTodos();
        }
        else
        {
            Console.WriteLine($"Id not found: {Id}");          
        }
    }

    static void DeleteTodos()
    {
        bool Found = false;

        string IdStr = Environment.GetCommandLineArgs()[2];

        if (!int.TryParse(IdStr, out int Id))
        {
            Console.WriteLine($"Id param not an integer: {IdStr}");
            return;
        }

        LoadTodos();

        for (int i = 0; i < Todos.Count; i++)
        {
            if (Todos[i].Id == Id)
            {
                Todos.RemoveAt(i);
                Found = true;
                break;
            }
        }

        if (Found)
        {
            SaveTodos();
        } else
        {
            Console.WriteLine($"Id not found: {Id}");
        }
        
    }

    static void ClearTodos()
    {
        if (File.Exists(PATH)) 
        {
            File.Delete(PATH);
        } else
        {
            Console.WriteLine($"File not found: {PATH}")
        }
    }

}