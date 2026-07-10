using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.Windows;
using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;
using StrategyGame.ConsoleGame.Game.Resources;
using StrategyGame.ConsoleGame.Game.Buildings;
using StrategyGame.ConsoleGame.Game.Saves;

namespace StrategyGame.ConsoleGame.Game.Save;

public static partial class SaveGameManager
{
    public static SaveModel BuildSaveModel(Map map, List<Player> players, int currentPlayerIndex, int day, int week)
    {
        var m = new SaveModel
        {
            SavedAt = DateTime.Now,
            Day = day,
            Week = week,
            CurrentPlayerIndex = currentPlayerIndex,
            Players = BuildPlayerModels(players),
            Buildings = BuildBuildingModels(map, players)
        };

        m.PlayerExplored = BuildPlayerExplored(map, players);
        m.Monsters = BuildMonsterModels(map);

        return m;
    }

    // --- helper methods to keep BuildSaveModel small ---

    private static List<PlayerModel> BuildPlayerModels(List<Player> players)
    {
        return players.Select(p => new PlayerModel
        {
            Type = p.Type.ToString(),
            Color = p.Color.ToString(),
            X = p.position.X,
            Y = p.position.Y,
            MaxMoves = p.MaxMoves,
            MovesRemaining = p.MovesRemaining,
            MaxMagic = p.MaxMagic,
            MagicRemaining = p.MagicRemaining,
            TempMoveBonusPercent = p.TempMoveBonusPercent,
            UnitSlots = p.UnitSlots,
            Units = Enumerable.Range(0, p.UnitSlots).Select(i =>
            {
                var u = p.GetUnitSlot(i);
                if (u == null) return null;
                return new UnitModel { TypeName = u.TypeName, Count = u.Count, CurrentHp = u.CurrentHp, MaxHp = u.MaxHp };
            }).Where(x => x != null).ToList()!,
            // save player's resources explicitly
            Resources = p.Resources.Select(r => new ResourceModel { Type = r.GetType().Name, Amount = r.Amount }).ToList()
        }).ToList();
    }

    private static List<BuildingModel> BuildBuildingModels(Map map, List<Player> players)
    {
        var list = new List<BuildingModel>();
        foreach (var b in map.Buildings)
        {
            var bm = new BuildingModel { Type = b.Type, X = b.Position.X, Y = b.Position.Y, OwnerIndex = b.Owner != null ? players.IndexOf(b.Owner) : (int?)null };
            if (b is Castle castle)
            {
                bm.IsCastle = true;
                bm.Garrison = new List<UnitModel>();
                foreach (var g in castle.Garrison)
                {
                    if (g == null) { bm.Garrison.Add(null); continue; }
                    bm.Garrison.Add(new UnitModel { TypeName = g.TypeName, Count = g.Count, CurrentHp = g.CurrentHp, MaxHp = g.MaxHp });
                }

                bm.CastleBuildings = new List<CastleBuildingState>();
                foreach (var cb in castle.Buildings)
                {
                    var cbs = new CastleBuildingState { Name = cb.Name, IsBuilt = cb.IsBuilt };
                    if (cb.ProducedUnits != null && cb.ProducedUnits.Count > 0)
                    {
                        // use full type names for produced units keys to make deserialization explicit
                        cbs.ProducedUnits = cb.ProducedUnits.ToDictionary(kv => (kv.Key.FullName ?? kv.Key.Name), kv => kv.Value);
                    }
                    bm.CastleBuildings.Add(cbs);
                }
            }

            list.Add(bm);
        }
        return list;
    }

    private static List<bool[][]> BuildPlayerExplored(Map map, List<Player> players)
    {
        var result = new List<bool[][]>();
        foreach (var p in players)
        {
            var arr = map.GetPlayerExplored(p);
            if (arr == null)
            {
                result.Add(null);
                continue;
            }

            int r = arr.GetLength(0);
            int c = arr.GetLength(1);
            var jagged = new bool[r][];
            for (int i = 0; i < r; i++)
            {
                jagged[i] = new bool[c];
                for (int j = 0; j < c; j++)
                    jagged[i][j] = arr[i, j];
            }
            result.Add(jagged);
        }
        return result;
    }

    private static List<MonsterModel> BuildMonsterModels(Map map)
    {
        var monsters = new List<MonsterModel>();
        var monstersField = typeof(Map).GetField("monsters", BindingFlags.Instance | BindingFlags.NonPublic);
        if (monstersField != null)
        {
            var dict = monstersField.GetValue(map) as IDictionary<(int, int), List<UnitBase>>;
            if (dict != null)
            {
                foreach (var kv in dict)
                {
                    var mm = new MonsterModel { X = kv.Key.Item1, Y = kv.Key.Item2 };
                    mm.Units = kv.Value.Select(u => new UnitModel { TypeName = u.TypeName, Count = u.Count, CurrentHp = u.CurrentHp, MaxHp = u.MaxHp }).ToList();
                    monsters.Add(mm);
                }
            }
        }
        return monsters;
    }

    public static string SaveToFile(SaveModel model, string savesDir)
    {
        Directory.CreateDirectory(savesDir);
        string fileName = $"save_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string fullPath = Path.Combine(savesDir, fileName);

        var opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        string json = JsonSerializer.Serialize(model, opts);
        File.WriteAllText(fullPath, json);
        return fullPath;
    }

    public static SaveModel? LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return null;
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<SaveModel>(json, opts);
    }

    public static string? FindMatchingMap(SaveModel model, string mapsDir)
    {
        if (!Directory.Exists(mapsDir)) return null;
        var mapFiles = Directory.GetFiles(mapsDir).Where(x => !string.IsNullOrEmpty(x)).ToArray();
        foreach (var mf in mapFiles)
        {
            var tmp = new Map();
            tmp.LoadFromFile(mf);
            var savedSet = new HashSet<(int x, int y, MapCell t)>(model.Buildings.Select(b => (b.X, b.Y, b.Type)));
            var mapSet = new HashSet<(int x, int y, MapCell t)>(tmp.Buildings.Select(b => (b.Position.X, b.Position.Y, b.Type)));
            if (savedSet.SetEquals(mapSet))
            {
                return mf;
            }
        }
        return null;
    }

    /// <summary>
    /// Interactive load: choose save file, find/ask map, and restore full game state. Returns LoadOutcome or null.
    /// </summary>
    public static LoadOutcome? InteractiveLoad(string savesDir, string mapsDir)
    {
        if (!Directory.Exists(savesDir))
        {
            new ConsoleWindow<int>("No saves found.", "Load").Show();
            return null;
        }

        var files = Directory.GetFiles(savesDir).Where(f => !string.IsNullOrEmpty(f)).OrderByDescending(f => f).ToArray();
        if (files.Length == 0)
        {
            new ConsoleWindow<int>("No saves found.", "Load").Show();
            return null;
        }

        var menuItems = files.Select(f => Path.GetFileName(f)).ToArray();
        int sel = new MenuWindow("Select save:", menuItems, title: "Load", buttonPosition: ButtonPosition.CenterVertically).Show();
        var chosen = files[sel];

        var model = LoadFromFile(chosen);
        if (model == null)
        {
            new ConsoleWindow<int>("Invalid save file.", "Load").Show();
            return null;
        }

        string? matchedMap = FindMatchingMap(model, mapsDir);
        if (matchedMap == null)
        {
            string[] allMaps = Directory.Exists(mapsDir) ? Directory.GetFiles(mapsDir).Where(x => !string.IsNullOrEmpty(x)).ToArray() : Array.Empty<string>();
            if (allMaps.Length == 0)
            {
                new ConsoleWindow<int>("No maps available to load.", "Load").Show();
                return null;
            }
            int msel = new MenuWindow("No matching map found automatically. Choose a map to load (should match save):", allMaps.Select(Path.GetFileName).ToArray(), title: "Choose map", buttonPosition: ButtonPosition.CenterVertically).Show();
            matchedMap = allMaps[Math.Clamp(msel, 0, allMaps.Length - 1)];
        }

        // load selected map into game map
        var map = new Map();
        map.LoadFromFile(matchedMap!);

        // Reconstruct players
        var players = new List<Player>();
        foreach (var pm in model.Players)
        {
            PlayerType pt = PlayerType.Knight;
            try { pt = Enum.Parse<PlayerType>(pm.Type, ignoreCase: true); } catch { }
            PlayerColor pc = PlayerColor.Red;
            try { pc = Enum.Parse<PlayerColor>(pm.Color, ignoreCase: true); } catch { }

            var p = new Player(pt, new Coordinate(pm.X, pm.Y), pc);
            p.MovesRemaining = pm.MovesRemaining;
            p.MagicRemaining = pm.MagicRemaining;
            p.TempMoveBonusPercent = pm.TempMoveBonusPercent;

            // restore player resources from model
            if (pm.Resources != null)
            {
                try
                {
                    p.Resources.Clear();
                    foreach (var rm in pm.Resources)
                    {
                        Type? rtype = null;
                        var asm = Assembly.GetExecutingAssembly();
                        foreach (var t in asm.GetTypes())
                        {
                            if (!typeof(Resource).IsAssignableFrom(t) || t.IsAbstract) continue;
                            if (string.Equals(t.Name, rm.Type, StringComparison.OrdinalIgnoreCase) || string.Equals(t.FullName, rm.Type, StringComparison.OrdinalIgnoreCase))
                            {
                                rtype = t; break;
                            }
                        }
                        if (rtype != null)
                        {
                            var created = (Resource?)Activator.CreateInstance(rtype, new object[] { rm.Amount });
                            if (created != null) p.Resources.Add(created);
                        }
                    }
                }
                catch { }
            }

            players.Add(p);
        }

        // initialize fog arrays for these player objects
        map.InitializePlayerFog(players);

        // restore per-player explored
        if (model.PlayerExplored != null)
        {
            for (int pi = 0; pi < players.Count && pi < model.PlayerExplored.Count; pi++)
            {
                var jag = model.PlayerExplored[pi];
                if (jag == null) continue;
                int r = jag.Length;
                int c = jag.Length > 0 ? jag[0].Length : 0;
                if (r == 0 || c == 0) continue;
                var arr = new bool[r, c];
                for (int i = 0; i < r; i++)
                    for (int j = 0; j < c; j++)
                        arr[i, j] = jag[i][j];
                map.SetPlayerExplored(players[pi], arr);
            }
        }

        // restore building ownership using OwnerIndex, garrison and castle buildings
        foreach (var bm in model.Buildings)
        {
            var b = map.Buildings.FirstOrDefault(x => x.Position.X == bm.X && x.Position.Y == bm.Y && x.Type == bm.Type);
            if (b == null) continue;
            if (bm.OwnerIndex != null && bm.OwnerIndex >= 0 && bm.OwnerIndex < players.Count)
            {
                b.Capture(players[bm.OwnerIndex.Value]);
            }

            if (bm.IsCastle && b is Castle castle)
            {
                // restore garrison
                if (bm.Garrison != null)
                {
                    for (int gi = 0; gi < castle.GarrisonSlots && gi < bm.Garrison.Count; gi++)
                    {
                        var um = bm.Garrison[gi];
                        if (um == null) continue;

                        // find unit type by TypeName
                        Type? unitType = null;
                        var asm = Assembly.GetExecutingAssembly();
                        foreach (var t in asm.GetTypes())
                        {
                            if (!t.IsClass || t.IsAbstract) continue;
                            if (!typeof(UnitBase).IsAssignableFrom(t)) continue;
                            try
                            {
                                var inst = (UnitBase?)Activator.CreateInstance(t);
                                if (inst != null && string.Equals(inst.TypeName, um.TypeName, StringComparison.OrdinalIgnoreCase))
                                {
                                    unitType = t;
                                    break;
                                }
                            }
                            catch { }
                        }

                        if (unitType != null)
                        {
                            try
                            {
                                var stackType = typeof(UnitStack<>).MakeGenericType(unitType);
                                var created = (ICombatant?)Activator.CreateInstance(stackType, new object[] { Math.Max(0, um.Count) });
                                if (created != null)
                                {
                                    try { created.CurrentHp = um.CurrentHp; } catch { }
                                    created.Owner = players[bm.OwnerIndex ?? 0];
                                    castle.TrySetGarrisonSlot(gi, created);
                                }
                            }
                            catch { }
                        }
                    }
                }

                // restore castle buildings
                if (bm.CastleBuildings != null)
                {
                    foreach (var cbState in bm.CastleBuildings)
                    {
                        var cb = castle.Buildings.FirstOrDefault(x => x.Name == cbState.Name);
                        if (cb == null) continue;
                        if (cbState.IsBuilt) cb.Build(); else cb.Demolish();

                        if (cbState.ProducedUnits != null)
                        {
                            // Use public import helper to set produced units without reflection.
                            try
                            {
                                cb.ImportProducedUnitsByTypeName(cbState.ProducedUnits);
                            }
                            catch
                            {
                                // ignore failures restoring produced units for this building
                            }
                        }
                    }
                }
            }
        }

        // restore monsters
        var monstersField = typeof(Map).GetField("monsters", BindingFlags.Instance | BindingFlags.NonPublic);
        if (monstersField != null && model.Monsters != null)
        {
            var dict = new Dictionary<(int, int), List<UnitBase>>();
            foreach (var mm in model.Monsters)
            {
                var list = new List<UnitBase>();
                foreach (var um in mm.Units)
                {
                    // find unit type by TypeName
                    Type? unitType = null;
                    var asm = Assembly.GetExecutingAssembly();
                    foreach (var t in asm.GetTypes())
                    {
                        if (!t.IsClass || t.IsAbstract) continue;
                        if (!typeof(UnitBase).IsAssignableFrom(t)) continue;
                        try
                        {
                            var inst = (UnitBase?)Activator.CreateInstance(t);
                            if (inst != null && string.Equals(inst.TypeName, um.TypeName, StringComparison.OrdinalIgnoreCase))
                            {
                                unitType = t;
                                break;
                            }
                        }
                        catch { }
                    }

                    if (unitType != null)
                    {
                        try
                        {
                            var created = (UnitBase?)Activator.CreateInstance(unitType);
                            if (created != null)
                            {
                                try { created.CurrentHp = um.CurrentHp; } catch { }
                                // if it's a stack type, we can't set count easily — monsters are UnitBase, assume Count==1
                                list.Add(created);
                            }
                        }
                        catch { }
                    }
                }

                if (list.Count > 0)
                    dict[(mm.X, mm.Y)] = list;
            }

            try
            {
                // set private field 'monsters' on map instance
                monstersField.SetValue(map, dict);
            }
            catch { }
        }

        // build and return outcome
        var outcome = new LoadOutcome
        {
            Map = map,
            Players = players,
            CurrentPlayerIndex = model.CurrentPlayerIndex,
            Day = model.Day,
            Week = model.Week,
            SaveFileName = Path.GetFileName(chosen)
        };

        return outcome;
    }
}
