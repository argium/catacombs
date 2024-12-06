using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => 
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("lightsout", LogLevel.Information)
        .AddConsole());

ILogger logger = factory.CreateLogger("lightsout");

if (args.Length != 2) {
    logger.LogInformation("dotnet run \"<starting positions>\" \"<grouped statues>\"");
    logger.LogError("Please enter two the starting positions (comma-separated) and grouped statues (pos:grouped, semicolon-separated)");
    logger.LogError("Example: dotnet run \"1 2 3 4\" \"1:6 7 24 25;2:4 6 11 15\"");
    return 1;
}

// Parse starting positions
List<uint> startingPositions = new();
try {
    startingPositions = args[0].Split(" ").Select(uint.Parse).ToList();
}
catch (FormatException e) {
    logger.LogError("Starting positions must be numbers");
    return 1;
}

if (startingPositions.Count == 0) {
    logger.LogError("Please enter at least one starting position");
    return 1;
}

if (startingPositions.Any(n => n < 1 || n > 25)) {
    logger.LogError("Starting positions must be between 1-25");
    return 1;
}

// Parse grouped statues
Dictionary<uint, HashSet<uint>> groups = new();
// Treat as a file
string path = args[1];
if (!File.Exists(path)) {
    logger.LogError("File not found: {Path}", path);
    return 1;
}

string[] text = File.ReadAllLines(path);
for (uint i = 1; i <= text.Length; i++) {
    string[] parts = text[i-1].Split(":");

    if (parts.Length != 2) {
        logger.LogError("Invalid grouping for statue {I}: must be in the format 'pos:space-separated stsatues'", i);
        return 1;
    }

    // uint pos = uint.Parse(parts[0]);
    HashSet<uint> statues = new();
    try {
        statues = [.. parts[1].Split(" ").Select(uint.Parse)];
    }
    catch (FormatException e) {
        logger.LogError("Invalid grouping for statue {I}: must be numbers", i);
        logger.LogError(e.ToString());
        return 1;
    }

    if (statues.Count != 4) {
        logger.LogError("Invalid grouping for statue {I}: must have 4 links", i);
        return 1;
    }

    if (statues.Contains(i)) {
        logger.LogError("Invalid grouping for statue {I}: cannot link to itself", i);
        return 1;
    }

    if (statues.Any(n => n < 1 || n > 25)) {
        logger.LogError("Invalid grouping for statue {I}: all numbers must be between 1-25", i);
        return 1;
    }

    if (groups.ContainsKey(i)) {
        logger.LogError("Invalid grouping for statue {I}: duplicate", i);
        return 1;
    }        

    groups[i] = statues;

}

if (groups.Count != 25) {
    logger.LogError("Please enter 25 statue groupings");
    // return 1;
}

// Constants
int MaxDepth = 25;
int WorkerCount = 1;  // Probably won't work without a concurrent priority queue or locks

// State and action queue
var state = new State { Up = new(startingPositions), Groups = groups };
var visited = new HashSet<State>();
// var queue = new ConcurrentQueue<ActionContext>();
var queue = new PriorityQueue<ActionContext, int>();
var cts = new CancellationTokenSource();
int workerId = 0;

// Worker loop
async Task SolveAsyncImpl(CancellationToken cancellationToken)
{
    int id = Interlocked.Increment(ref workerId);
    logger.LogInformation("Worker {Id} started", id);
    while (queue.TryDequeue(out ActionContext? context, out int _)) {
        logger.LogTrace(context.ToString());

        if (cancellationToken.IsCancellationRequested) {
            logger.LogDebug("Cancellation requested");
            return;
        }

        // Keep track of permutations that we've seen before
        visited.Add(context.State);

        // Click the owl. If the puzzle is solved, log the solution and stop.
        var actions = new List<uint>(context.Actions) { context.Next };
        var newState = context.State.ClickOwl(context.Next);
        if (newState.IsSolved()) {
            cts.Cancel();
            logger.LogInformation("Solved!");
            logger.LogInformation("Actions: {Actions}", string.Join(",", actions));
            return;
        }
        else {
            if (context.Actions.Count+1 == MaxDepth) {
                logger.LogDebug("Can't recurse further");
                continue;
            }

            // If it's not solved then queue up the next possible actions
            foreach (uint next in newState.Up) {
                if (!visited.Contains(newState)) {
                    queue.Enqueue(new ActionContext { State = newState, Actions = actions, Next = next }, newState.Up.Count);
                }
            }
        }
    }

    logger.LogInformation("Worker {Id} stopped", id);
}

// Add initial actions to the queue
foreach (uint next in state.Up) {
    queue.Enqueue(new ActionContext { State = state, Actions = [], Next = next }, state.Up.Count);
}

logger.LogInformation("queue.Count={Count}", queue.Count);

// Start workers
logger.LogInformation("Starting {WorkerCount} workers", WorkerCount);
var workers = new List<Task>();
for (int i = 0; i < WorkerCount; i++) {
    workers.Add(SolveAsyncImpl(cts.Token));
}

await Task.WhenAll(workers);
cts.Cancel();
return 0;


public class ActionContext
{
    public required State State { get; init; }
    public required List<uint> Actions { get; init; }
    public required uint Next { get; init; }

    public override string ToString() => $"State={State} Actions={string.Join(",", Actions)} Next={Next}";
}

public class State : IEquatable<State>
{
    public required HashSet<uint> Up { get; init; } = new();

    public required IReadOnlyDictionary<uint, HashSet<uint>> Groups { get; init; }

    public bool IsSolved() => Up.Count == 0;

    public State ClickOwl(uint index) 
    {
        // Create a new state. Clone the Up list.
        State newState = new() { Up = new(Up), Groups = Groups };

        // Toggle each owl in the linked group
        foreach (uint i in Groups[index]) {
            if (newState.Up.Contains(i)) {
                newState.Up.Remove(i);
            }
            else {
                newState.Up.Add(i);
            }
        }
        return newState;
    }

    public override string ToString() => string.Join(",", Up);

    public bool Equals(State? other)
    {
        if (other is null) return false;
        return Up.SetEquals(other.Up);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as State);
    }

    public override int GetHashCode()
    {
        return Up.GetHashCode();
    }
}
