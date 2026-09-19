using Windsmoon.ReadOnlyCollections;

// Run from the repository root:
// dotnet run --project CSharpProject/CSharpProject/CSharpProject.csproj
int passed = 0;
int failed = 0;

Run("Constructors reject null", () =>
{
    Throws<ArgumentNullException>(() => new ReadOnlyList<int>(null!));
    Throws<ArgumentNullException>(() => new ReadOnlyHashSet<int>(null!));
    Throws<ArgumentNullException>(() => new ReadOnlyDictionary<string, int>(null!));
});

Run("List: queries", () =>
{
    var list = new ReadOnlyList<int>(new List<int> { 1, 2, 3, 2, 4 });
    Equal(5, list.Count);
    Check(!list.IsEmpty);
    Equal(3, list[2]);
    Check(list.Contains(2));
    Check(!list.Contains(9));
    Equal(1, list.IndexOf(2));
    Equal(-1, list.IndexOf(9));
    Check(list.Exists(x => x % 2 == 0));
    Check(!list.Exists(x => x > 10));
    Equal(2, list.FindFirst(x => x % 2 == 0));
    Equal(4, list.FindLast(x => x % 2 == 0));
    Equal(1, list.FindIndex(x => x == 2));
    Equal(3, list.FindLastIndex(x => x == 2));
    Equal(0, list.FindFirst(x => x > 10));
    Equal(0, list.FindLast(x => x > 10));
    Equal(-1, list.FindIndex(x => x > 10));
    Equal(-1, list.FindLastIndex(x => x > 10));
});

Run("List: empty collection and invalid arguments", () =>
{
    var list = new ReadOnlyList<string>(new List<string>());
    Check(list.IsEmpty);
    Equal(0, list.Count);
    Check(!list.Exists(_ => true));
    Equal<string?>(null, list.FindFirst(_ => true));
    Equal<string?>(null, list.FindLast(_ => true));
    Equal(-1, list.FindIndex(_ => true));
    Equal(-1, list.FindLastIndex(_ => true));
    Throws<ArgumentNullException>(() => list.Exists(null!));
    Throws<ArgumentNullException>(() => list.FindFirst(null!));
    Throws<ArgumentNullException>(() => list.FindLast(null!));
    Throws<ArgumentNullException>(() => list.FindIndex(null!));
    Throws<ArgumentNullException>(() => list.FindLastIndex(null!));
    Throws<ArgumentOutOfRangeException>(() => _ = list[0]);
});

Run("List: all CopyTo overloads", () =>
{
    var list = new ReadOnlyList<int>(new List<int> { 1, 2, 3 });
    var all = new int[3];
    list.CopyTo(all);
    Sequence(new[] { 1, 2, 3 }, all);
    var offset = new[] { -1, -1, -1, -1, -1 };
    list.CopyTo(offset, 1);
    Sequence(new[] { -1, 1, 2, 3, -1 }, offset);
    var range = new[] { -1, -1, -1, -1 };
    list.CopyTo(1, range, 1, 2);
    Sequence(new[] { -1, 2, 3, -1 }, range);
    Throws<ArgumentNullException>(() => list.CopyTo(null!));
    Throws<ArgumentException>(() => list.CopyTo(new int[2]));
    Throws<ArgumentOutOfRangeException>(() => list.CopyTo(all, -1));
});

Run("HashSet: lookup and set relationships", () =>
{
    var set = new ReadOnlyHashSet<string>(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Alpha", "Beta"
    });
    Equal(2, set.Count);
    Check(!set.IsEmpty);
    Check(set.Contains("ALPHA"));
    Check(!set.Contains("Gamma"));
    Check(set.TryGetValue("alpha", out var stored));
    Equal("Alpha", stored);
    Check(!set.TryGetValue("Gamma", out var missing));
    Equal<string?>(null, missing);
    Check(set.IsSubsetOf(new[] { "alpha", "beta", "gamma" }));
    Check(!set.IsSubsetOf(new[] { "alpha" }));
    Check(set.IsProperSubsetOf(new[] { "alpha", "beta", "gamma" }));
    Check(!set.IsProperSubsetOf(new[] { "alpha", "beta" }));
    Check(set.IsSupersetOf(new[] { "alpha" }));
    Check(!set.IsSupersetOf(new[] { "gamma" }));
    Check(set.IsProperSupersetOf(new[] { "alpha" }));
    Check(!set.IsProperSupersetOf(new[] { "alpha", "beta" }));
    Check(set.Overlaps(new[] { "BETA", "gamma" }));
    Check(!set.Overlaps(new[] { "gamma" }));
    Check(set.SetEquals(new[] { "BETA", "ALPHA" }));
    Check(!set.SetEquals(new[] { "alpha" }));
});

Run("HashSet: Exists and FindFirst", () =>
{
    var source = new HashSet<int> { 4, 1, 6, 3, 2 };
    source.Remove(1);
    source.Add(8);
    var set = new ReadOnlyHashSet<int>(source);
    // Use the source's current enumeration order, not an assumed insertion order.
    Equal(source.First(x => x % 2 == 0), set.FindFirst(x => x % 2 == 0));
    Check(set.Exists(x => x == 3));
    Check(!set.Exists(x => x > 10));
    Equal(0, set.FindFirst(x => x > 10));
    int calls = 0;
    Check(set.Exists(_ => { calls++; return true; }));
    Equal(1, calls);
    calls = 0;
    Equal(source.First(), set.FindFirst(_ => { calls++; return true; }));
    Equal(1, calls);

    var empty = new ReadOnlyHashSet<string>(new HashSet<string>());
    Check(empty.IsEmpty);
    Equal(0, empty.Count);
    Check(!empty.Exists(_ => throw new Exception("Empty set must not invoke the predicate.")));
    Equal<string?>(null, empty.FindFirst(_ => throw new Exception("Empty set must not invoke the predicate.")));
    Throws<ArgumentNullException>(() => empty.Exists(null!));
    Throws<ArgumentNullException>(() => empty.FindFirst(null!));
    var strings = new ReadOnlyHashSet<string>(new HashSet<string> { "Alpha" });
    Equal<string?>(null, strings.FindFirst(_ => false));
});

Run("HashSet: all CopyTo overloads", () =>
{
    var source = new HashSet<int> { 7, 3, 9 };
    var set = new ReadOnlyHashSet<int>(source);
    var expected = source.ToArray();
    var all = new int[3];
    set.CopyTo(all);
    Sequence(expected, all);
    var offset = new int[5];
    set.CopyTo(offset, 1);
    Sequence(new[] { 0 }.Concat(expected).Append(0), offset);
    var range = new int[4];
    set.CopyTo(range, 1, 2);
    Sequence(new[] { 0, expected[0], expected[1], 0 }, range);
    Throws<ArgumentNullException>(() => set.CopyTo(null!));
    Throws<ArgumentException>(() => set.CopyTo(new int[2]));
    Throws<ArgumentOutOfRangeException>(() => set.CopyTo(all, -1));
});

Run("Dictionary: lookup, keys and values", () =>
{
    var dictionary = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["Alpha"] = 10, ["Beta"] = 20
    });
    Equal(2, dictionary.Count);
    Check(!dictionary.IsEmpty);
    Equal(10, dictionary["ALPHA"]);
    Check(dictionary.ContainsKey("alpha"));
    Check(!dictionary.ContainsKey("Gamma"));
    Check(dictionary.ContainsValue(20));
    Check(!dictionary.ContainsValue(30));
    Check(dictionary.TryGetValue("BETA", out var value));
    Equal(20, value);
    Check(!dictionary.TryGetValue("Gamma", out value));
    Equal(0, value);
    Check(new HashSet<string>(dictionary.Keys).SetEquals(new[] { "Alpha", "Beta" }));
    Check(new HashSet<int>(dictionary.Values).SetEquals(new[] { 10, 20 }));
    Throws<KeyNotFoundException>(() => _ = dictionary["Gamma"]);
    Throws<ArgumentNullException>(() => dictionary.ContainsKey(null!));
});

Run("Dictionary: Exists and FindFirst", () =>
{
    var source = new Dictionary<string, int> { ["A"] = 4, ["B"] = 1, ["C"] = 6 };
    source.Remove("B");
    source.Add("D", 8);
    var dictionary = new ReadOnlyDictionary<string, int>(source);
    Check(dictionary.Exists(pair => pair.Key == "C" && pair.Value == 6));
    Equal(source.First(pair => pair.Value % 2 == 0),
        dictionary.FindFirst(pair => pair.Value % 2 == 0));
    Check(!dictionary.Exists(pair => pair.Value > 10));
    Equal(default(KeyValuePair<string, int>), dictionary.FindFirst(pair => pair.Value > 10));
    int calls = 0;
    Check(dictionary.Exists(_ => { calls++; return true; }));
    Equal(1, calls);
    calls = 0;
    Equal(source.First(), dictionary.FindFirst(_ => { calls++; return true; }));
    Equal(1, calls);
});

Run("Dictionary: empty collection, null values and invalid predicates", () =>
{
    var source = new Dictionary<string, string?>();
    var dictionary = new ReadOnlyDictionary<string, string?>(source);
    Check(dictionary.IsEmpty);
    Equal(0, dictionary.Count);
    Check(!dictionary.Exists(_ => throw new Exception("Empty dictionary must not invoke the predicate.")));
    Equal(default(KeyValuePair<string, string?>),
        dictionary.FindFirst(_ => throw new Exception("Empty dictionary must not invoke the predicate.")));
    Throws<ArgumentNullException>(() => dictionary.Exists(null!));
    Throws<ArgumentNullException>(() => dictionary.FindFirst(null!));
    source.Add("NullValue", null);
    Check(dictionary.TryGetValue("NullValue", out var value));
    Equal<string?>(null, value);
    Check(!dictionary.TryGetValue("Missing", out value));
    Equal<string?>(null, value);
});

Run("Dictionary: all CopyTo overloads", () =>
{
    var source = new Dictionary<string, int> { ["A"] = 1, ["B"] = 2 };
    var dictionary = new ReadOnlyDictionary<string, int>(source);
    var all = new KeyValuePair<string, int>[2];
    dictionary.CopyTo(all);
    Sequence(source, all);
    var offset = new KeyValuePair<string, int>[4];
    dictionary.CopyTo(offset, 1);
    Sequence(new[] { default(KeyValuePair<string, int>) }.Concat(source).Append(default), offset);
    Throws<ArgumentNullException>(() => dictionary.CopyTo(null!));
    Throws<ArgumentException>(() => dictionary.CopyTo(new KeyValuePair<string, int>[1]));
    Throws<ArgumentOutOfRangeException>(() => dictionary.CopyTo(all, -1));
});

Run("Native enumerators, foreach and live views", () =>
{
    var listSource = new List<int> { 1, 2 };
    var setSource = new HashSet<int> { 3, 4 };
    var dictionarySource = new Dictionary<string, int> { ["A"] = 5 };
    var list = new ReadOnlyList<int>(listSource);
    var set = new ReadOnlyHashSet<int>(setSource);
    var dictionary = new ReadOnlyDictionary<string, int>(dictionarySource);

    // These explicit types also verify the native struct enumerator return types.
    using (List<int>.Enumerator enumerator = list.GetEnumerator())
    {
        var items = new List<int>();
        while (enumerator.MoveNext()) items.Add(enumerator.Current);
        Sequence(listSource, items);
    }
    using (HashSet<int>.Enumerator enumerator = set.GetEnumerator())
    {
        var items = new List<int>();
        while (enumerator.MoveNext()) items.Add(enumerator.Current);
        Sequence(setSource, items);
    }
    using (Dictionary<string, int>.Enumerator enumerator = dictionary.GetEnumerator())
    {
        var items = new List<KeyValuePair<string, int>>();
        while (enumerator.MoveNext()) items.Add(enumerator.Current);
        Sequence(dictionarySource, items);
    }

    var listItems = new List<int>();
    foreach (int item in list) listItems.Add(item);
    Sequence(listSource, listItems);
    var setItems = new List<int>();
    foreach (int item in set) setItems.Add(item);
    Sequence(setSource, setItems);
    var dictionaryItems = new List<KeyValuePair<string, int>>();
    foreach (var item in dictionary) dictionaryItems.Add(item);
    Sequence(dictionarySource, dictionaryItems);

    listSource.Add(6);
    setSource.Add(7);
    dictionarySource.Add("B", 8);
    Equal(3, list.Count);
    Equal(6, list[2]);
    Equal(3, set.Count);
    Check(set.Contains(7));
    Equal(2, dictionary.Count);
    Equal(8, dictionary["B"]);
});

Console.WriteLine($"\nResults: {passed} passed, {failed} failed.");
Environment.ExitCode = failed == 0 ? 0 : 1;

void Run(string name, Action test)
{
    try
    {
        test();
        passed++;
        Console.WriteLine($"[PASS] {name}");
    }
    catch (Exception exception)
    {
        failed++;
        Console.WriteLine($"[FAIL] {name}\n{exception}");
    }
}

static void Check(bool condition)
{
    if (!condition) throw new InvalidOperationException("Expected condition to be true.");
}

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"Expected <{expected}>, got <{actual}>.");
}

static void Sequence<T>(IEnumerable<T> expected, IEnumerable<T> actual)
{
    if (!expected.SequenceEqual(actual))
        throw new InvalidOperationException($"Expected [{string.Join(", ", expected)}], got [{string.Join(", ", actual)}].");
}

static void Throws<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}
