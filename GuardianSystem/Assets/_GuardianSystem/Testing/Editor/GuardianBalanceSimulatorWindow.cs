#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Simulador de balanceamento offline para Guardian System.
/// Nao instancia scenes, nao altera nenhum asset durante simulacoes e nao depende do Play Mode.
/// Menu: Tools > Guardian System > Simulador de Balanceamento
/// </summary>
public sealed class GuardianBalanceSimulatorWindow : EditorWindow
{
    [SerializeField] private DayConfigData day1;
    [SerializeField] private DayConfigData day2;
    [SerializeField] private DayConfigData day3;
    [SerializeField] private EventDatabase eventDatabase;
    [SerializeField] private EventDatabase waterEventDatabase;

    [SerializeField] private int runsPerProfile = 100;
    [SerializeField] private int randomSeed = 20261113;
    [SerializeField] private float averageTravelSeconds = 25f;
    [SerializeField] private bool reproduceCurrentSpawnCondition = true;
    [SerializeField] private float timeStep = 0.25f;

    private string report = "Configure os tres dias e clique em Simular.";
    private string csv = string.Empty;
    private Vector2 scroll;

    [MenuItem("Tools/Guardian System/Simulador de Balanceamento")]
    public static void ShowWindow()
    {
        GetWindow<GuardianBalanceSimulatorWindow>("GS: Balanceamento").minSize = new Vector2(630, 540);
    }

    private void OnEnable()
    {
        AutoDiscover(false);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("GUARDIAN SYSTEM — SIMULADOR", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Simulacao estatistica em tempo virtual; nao clica na interface e nao executa a cena. " +
            "Acoes de jogador e deslocamento das unidades sao aproximados, mas os impactos " +
            "sao lidos dos ScriptableObjects do projeto.", MessageType.Info);

        day1 = (DayConfigData)EditorGUILayout.ObjectField("Dia 1", day1, typeof(DayConfigData), false);
        day2 = (DayConfigData)EditorGUILayout.ObjectField("Dia 2", day2, typeof(DayConfigData), false);
        day3 = (DayConfigData)EditorGUILayout.ObjectField("Dia 3", day3, typeof(DayConfigData), false);
        eventDatabase = (EventDatabase)EditorGUILayout.ObjectField("Banco de chamados", eventDatabase, typeof(EventDatabase), false);
        waterEventDatabase = (EventDatabase)EditorGUILayout.ObjectField("Banco de agua", waterEventDatabase, typeof(EventDatabase), false);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Parametros dos testes", EditorStyles.boldLabel);
        runsPerProfile = EditorGUILayout.IntSlider("Partidas por perfil", runsPerProfile, 10, 1000);
        randomSeed = EditorGUILayout.IntField("Seed", randomSeed);
        averageTravelSeconds = EditorGUILayout.Slider("Viagem media das unidades (s)", averageTravelSeconds, 1f, 90f);
        timeStep = EditorGUILayout.Slider("Passo da simulacao (s)", timeStep, 0.1f, 1f);
        reproduceCurrentSpawnCondition = EditorGUILayout.ToggleLeft(
            "Reproduzir condicao atual de spawn (pode ultrapassar o limite de chamados)",
            reproduceCurrentSpawnCondition);

        EditorGUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Procurar assets", GUILayout.Height(28))) AutoDiscover(true);
        if (GUILayout.Button("Criar 3 dias sugeridos", GUILayout.Height(28))) CreateSuggestedDays();
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(day1 == null || day2 == null || day3 == null || eventDatabase == null);
        if (GUILayout.Button("SIMULAR 3 DIAS / 3 PERFIS", GUILayout.Height(36))) RunSimulation();
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(csv));
        if (GUILayout.Button("Exportar CSV")) Export("csv", csv);
        if (GUILayout.Button("Exportar relatorio TXT")) Export("txt", report);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Relatorio", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MinHeight(190));
        EditorGUILayout.TextArea(report, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    private void RunSimulation()
    {
        try
        {
            var config = new GuardianBalanceSimulation.Parameters
            {
                Days = new[] { day1, day2, day3 },
                RegularDatabase = eventDatabase,
                WaterDatabase = waterEventDatabase,
                RunsPerProfile = runsPerProfile,
                Seed = randomSeed,
                AverageTravelSeconds = averageTravelSeconds,
                Step = timeStep,
                ReproduceSpawnCondition = reproduceCurrentSpawnCondition
            };
            GuardianBalanceSimulation.Report result = GuardianBalanceSimulation.Run(config);
            report = result.Text;
            csv = result.Csv;
            scroll = Vector2.zero;
            Debug.Log("[GuardianBalanceSimulator] Simulacao concluida. Veja Tools > Guardian System.");
        }
        catch (Exception exception)
        {
            report = "Falha na simulacao:\n" + exception;
            Debug.LogException(exception);
        }
    }

    private void AutoDiscover(bool notify)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:DayConfigData"))
        {
            DayConfigData day = AssetDatabase.LoadAssetAtPath<DayConfigData>(AssetDatabase.GUIDToAssetPath(guid));
            if (day == null) continue;
            string name = day.name.ToLowerInvariant();
            if (name.Contains("ending") || name.Contains("final")) continue;
            if (day.DayNumber == 1 && day1 == null) day1 = day;
            if (day.DayNumber == 2 && day2 == null) day2 = day;
            if (day.DayNumber == 3 && day3 == null) day3 = day;
        }

        foreach (string guid in AssetDatabase.FindAssets("t:EventDatabase"))
        {
            EventDatabase db = AssetDatabase.LoadAssetAtPath<EventDatabase>(AssetDatabase.GUIDToAssetPath(guid));
            if (db == null) continue;
            if (db.name.ToLowerInvariant().Contains("water") || db.name.ToLowerInvariant().Contains("agua"))
            {
                if (waterEventDatabase == null) waterEventDatabase = db;
            }
            else if (eventDatabase == null) eventDatabase = db;
        }
        if (notify) ShowNotification(new GUIContent("Busca concluida. Confira os campos."));
    }

    private void CreateSuggestedDays()
    {
        if (!EditorUtility.DisplayDialog("Criar tres dias?",
                "Serao criados NOVOS assets com valores provisórios. Os assets existentes " +
                "nao serao alterados e a cena nao sera configurada automaticamente.",
                "Criar", "Cancelar")) return;

        const string path = "Assets/_GuardianSystem/ScriptableObjects/Days";
        EnsureFolders(path);
        DayConfigData[] created = new DayConfigData[3];
        for (int i = 0; i < 3; i++)
        {
            int number = i + 1;
            DayConfigData day = ScriptableObject.CreateInstance<DayConfigData>();
            day.DayNumber = number;
            day.dayDurationInSeconds = new[] { 180f, 240f, 300f }[i];
            day.minCallSpawnInterval = new[] { 18f, 12f, 8f }[i];
            day.maxCallSpawnInterval = new[] { 26f, 20f, 15f }[i];
            day.maxSimultaneousCalls = new[] { 2, 3, 4 }[i];
            day.waterVariationPerSecond = new[] { .65f, .9f, 1.2f }[i];
            day.waterDirectionChangeInterval = new[] { 12f, 10f, 8f }[i];
            day.popularityImpactOnWaterCallSpawn = new[] { -1f, -2f, -3f }[i];
            day.emptyWaterTickInterval = new[] { 15f, 12f, 10f }[i];
            day.popularityImpactOnEmptyWaterPerTick = new[] { -.3f, -.5f, -.8f }[i];
            day.minTrashAddInterval = new[] { 17f, 12f, 9f }[i];
            day.maxTrashAddInterval = new[] { 25f, 18f, 14f }[i];
            day.fullTrashTickInterval = new[] { 15f, 12f, 10f }[i];
            day.popularityImpactOnFullTrashPerTick = new[] { -.4f, -.6f, -.9f }[i];
            day.popularityImpactOnCollectedTrash = new[] { .6f, .7f, .8f }[i];
            day.isTutorialActive = false;
            day.activeEventWeights = i == 0 ? new[] { EventWeight.Simple }
                : i == 1 ? new[] { EventWeight.Simple, EventWeight.Medium }
                : new[] { EventWeight.Simple, EventWeight.Medium, EventWeight.Hard };
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "/Day" + number + "_Balance.asset");
            AssetDatabase.CreateAsset(day, assetPath);
            created[i] = day;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        day1 = created[0]; day2 = created[1]; day3 = created[2];
        Selection.activeObject = day1;
        report = "Tres dias provisórios criados. Lembre-se de configurar as cutscenes " +
                 "e arrastar os dias para dayConfigOrder do GameManager. " +
                 "Os valores ainda precisam passar pelos testes.";
    }

    private static void EnsureFolders(string path)
    {
        string[] parts = path.Split('/');
        string parent = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string current = parent + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(current)) AssetDatabase.CreateFolder(parent, parts[i]);
            parent = current;
        }
    }

    private static void Export(string extension, string content)
    {
        string destination = EditorUtility.SaveFilePanel("Exportar simulacao", "", "GuardianSystem_BalanceReport", extension);
        if (string.IsNullOrEmpty(destination)) return;
        System.IO.File.WriteAllText(destination, content, new UTF8Encoding(true));
        EditorUtility.RevealInFinder(destination);
    }
}

internal static class GuardianBalanceSimulation
{
    internal sealed class Parameters
    {
        public DayConfigData[] Days;
        public EventDatabase RegularDatabase;
        public EventDatabase WaterDatabase;
        public int RunsPerProfile;
        public int Seed;
        public float AverageTravelSeconds;
        public float Step;
        public bool ReproduceSpawnCondition;
    }

    internal sealed class Report
    {
        public string Text;
        public string Csv;
    }

    private sealed class Profile
    {
        public string Name;
        public float ReactionMin, ReactionMax, Accuracy, IgnoreChance;
        public float WaterInterval, WaterSuccess, WaterMin, WaterMax;
        public float TrashInterval, TrashSuccess;
        public int TrashThreshold, TrashActions;

        public static Profile[] Build()
        {
            return new[]
            {
                new Profile { Name = "Iniciante", ReactionMin = 6, ReactionMax = 18,
                    Accuracy = .58f, IgnoreChance = .10f, WaterInterval = 35, WaterSuccess = .7f,
                    WaterMin = 15, WaterMax = 85, TrashInterval = 30, TrashSuccess = .65f,
                    TrashThreshold = 4, TrashActions = 1 },
                new Profile { Name = "Intermediario", ReactionMin = 3, ReactionMax = 9,
                    Accuracy = .80f, IgnoreChance = .03f, WaterInterval = 18, WaterSuccess = .88f,
                    WaterMin = 22, WaterMax = 78, TrashInterval = 18, TrashSuccess = .85f,
                    TrashThreshold = 3, TrashActions = 1 },
                new Profile { Name = "Experiente", ReactionMin = 1, ReactionMax = 5,
                    Accuracy = .95f, IgnoreChance = .01f, WaterInterval = 9, WaterSuccess = .98f,
                    WaterMin = 26, WaterMax = 74, TrashInterval = 10, TrashSuccess = .96f,
                    TrashThreshold = 2, TrashActions = 2 }
            };
        }
    }

    private sealed class ActiveCall
    {
        public EventData Data;
        public float Deadline;
        public float NextAttempt;
        public int WaterZone = -1;
    }

    private struct PendingReward
    {
        public float At;
        public float Amount;
    }

    private sealed class DayResult
    {
        public float Popularity;
        public int Spawned, WaterCalls, Solved, Timeouts, Correct, MaxActive;
        public int OverflowEvents, EmptyTicks, FullTrashTicks, TrashCollected, TrashTrips;
        public int MissingEvents;
    }

    private sealed class Stats
    {
        public double Popularity, Spawned, WaterCalls, Solved, Timeouts, Correct, MaxActive;
        public double OverflowEvents, EmptyTicks, FullTrashTicks, TrashCollected, TrashTrips;
        public int MissingEvents;
        public readonly List<float> Popularities = new List<float>();
        public void Add(DayResult d)
        {
            Popularity += d.Popularity;
            Spawned += d.Spawned;
            WaterCalls += d.WaterCalls;
            Solved += d.Solved;
            Timeouts += d.Timeouts;
            Correct += d.Correct;
            MaxActive += d.MaxActive;
            OverflowEvents += d.OverflowEvents;
            EmptyTicks += d.EmptyTicks;
            FullTrashTicks += d.FullTrashTicks;
            TrashCollected += d.TrashCollected;
            TrashTrips += d.TrashTrips;
            MissingEvents += d.MissingEvents;
            Popularities.Add(d.Popularity);
        }
    }

    public static Report Run(Parameters p)
    {
        Validate(p);
        var profiles = Profile.Build();
        var perProfile = new Stats[profiles.Length, 3];
        var badEndings = new int[profiles.Length];
        var sb = new StringBuilder();
        var csv = new StringBuilder();
        int runs = p.RunsPerProfile;
        var culture = CultureInfo.InvariantCulture;

        for (int profileIndex = 0; profileIndex < profiles.Length; profileIndex++)
        {
            for (int day = 0; day < 3; day++) perProfile[profileIndex, day] = new Stats();

            for (int run = 0; run < runs; run++)
            {
                int seed = unchecked(p.Seed + profileIndex * 1000003 + run * 101);
                var rng = new System.Random(seed);
                float popularity = 50f;
                for (int dayIndex = 0; dayIndex < 3; dayIndex++)
                {
                    DayResult d = SimulateDay(p, p.Days[dayIndex], profiles[profileIndex], rng, popularity);
                    popularity = d.Popularity;
                    perProfile[profileIndex, dayIndex].Add(d);
                }
                if (popularity <= 45f) badEndings[profileIndex]++;
            }
        }

        sb.AppendLine("GUARDIAN SYSTEM — RELATORIO DE BALANCEAMENTO");
        sb.AppendLine("Partidas por perfil: " + runs + " | Seed: " + p.Seed +
                      " | Passo: " + p.Step.ToString("0.##", culture) + "s" +
                      " | Viagem media ESTIMADA: " + p.AverageTravelSeconds.ToString("0.#", culture) + "s");
        sb.AppendLine("Chamados: " + (p.ReproduceSpawnCondition ? "condicao atual do GameManager" : "limite corrigido"));
        sb.AppendLine("Final ruim: popularidade <= 45; caso contrario: final bom (inclui Neutral).");
        sb.AppendLine("Pop inicial 50, persistente entre os dias. Agua/lixo resetam a cada dia.");
        sb.AppendLine();

        csv.AppendLine("Perfil;Dia;Partidas;PopularidadeMedia;P10Popularidade;P90Popularidade;ChamadosMedia;ChamadosAguaMedia;ResolvidosMedia;TimeoutsMedia;AcertosMedia;PicoChamadosMedia;TransbordamentosMedia;TicksSemAguaMedia;TicksLixoCheioMedia;ColetasMedia;LixoRemovidoMedia;FinaisRuinsPercent");
        for (int pi = 0; pi < profiles.Length; pi++)
        {
            sb.AppendLine("== " + profiles[pi].Name.ToUpperInvariant() + " ==");
            for (int di = 0; di < 3; di++)
            {
                Stats s = perProfile[pi, di];
                var ordered = s.Popularities.OrderBy(x => x).ToList();
                float p10 = Percentile(ordered, .10f);
                float p90 = Percentile(ordered, .90f);
                string dayLabel = p.Days[di].name;
                sb.AppendLine("Dia " + (di + 1) + " (" + dayLabel + ") | popularidade " +
                    F(s.Popularity / runs) + " [P10 " + F(p10) + ", P90 " + F(p90) + "]" +
                    " | chamados " + F(s.Spawned / runs) +
                    " (agua " + F(s.WaterCalls / runs) + ")" +
                    " | resolvidos " + F(s.Solved / runs) +
                    " | timeouts " + F(s.Timeouts / runs) +
                    " | pico simultaneo " + F(s.MaxActive / runs));
                sb.AppendLine("    transbordamentos " + F(s.OverflowEvents / runs) +
                    " | ticks de agua vazia " + F(s.EmptyTicks / runs) +
                    " | ticks de lixo cheio " + F(s.FullTrashTicks / runs) +
                    " | viagens de coleta " + F(s.TrashTrips / runs) +
                    " | lixo recolhido " + F(s.TrashCollected / runs));
                if (s.MissingEvents > 0)
                    sb.AppendLine("    AVISO: " + s.MissingEvents + " tentativas de spawn sem eventos validos!");

                csv.AppendLine(string.Join(";", new[] {
                    profiles[pi].Name, (di + 1).ToString(), runs.ToString(),
                    F(s.Popularity / runs), F(p10), F(p90), F(s.Spawned / runs),
                    F(s.WaterCalls / runs), F(s.Solved / runs), F(s.Timeouts / runs),
                    F(s.Correct / runs), F(s.MaxActive / runs), F(s.OverflowEvents / runs),
                    F(s.EmptyTicks / runs), F(s.FullTrashTicks / runs),
                    F(s.TrashTrips / runs), F(s.TrashCollected / runs),
                    di == 2 ? F(100d * badEndings[pi] / runs) : "" }));
            }
            sb.AppendLine("Finais RUINS: " + badEndings[pi] + "/" + runs +
                          " (" + F(100d * badEndings[pi] / runs) + "%)");
            sb.AppendLine();
        }

        sb.AppendLine("AVISOS DE CONTEUDO E LIMITACOES");
        HashSet<EventWeight> availableWeights = new HashSet<EventWeight>();
        if (p.RegularDatabase.Events != null)
            foreach (EventData e in p.RegularDatabase.Events)
                if (e != null) availableWeights.Add(e.eventWeight);
        for (int i = 0; i < 3; i++)
        {
            var unavailable = (p.Days[i].activeEventWeights ?? new EventWeight[0])
                .Where(w => !availableWeights.Contains(w)).Distinct().ToArray();
            if (unavailable.Length > 0)
                sb.AppendLine("- Dia " + (i + 1) + ": nao existem eventos para " + string.Join(", ", unavailable.Select(x => x.ToString()).ToArray()));
        }
        sb.AppendLine("- Escolha de respostas: maior impacto positivo representa a resposta ideal; erros sorteiam outras respostas.");
        sb.AppendLine("- Interacao simulada: tempos de reacao, atencao a agua/lixo e taxa de acerto sao HIPOTESES editaveis no codigo.");
        sb.AppendLine("- Viagem das unidades: estimada por segundos medios; a distancia real entre RectTransforms NAO e lida da cena.");
        sb.AppendLine("- Limite de pontos livres no mapa e animacoes/pooling nao sao simulados.");
        sb.AppendLine("- Resultados de despacho sao creditados no retorno da unidade, ou no fim do dia se ainda pendentes.");
        sb.AppendLine("- Nao equivale a teste de usabilidade, nem garante ausencia de bugs na build.");
        return new Report { Text = sb.ToString(), Csv = csv.ToString() };
    }

    private static float Percentile(List<float> sorted, float fraction)
    {
        if (sorted.Count == 0) return 0;
        int index = Mathf.Clamp(Mathf.RoundToInt((sorted.Count - 1) * fraction), 0, sorted.Count - 1);
        return sorted[index];
    }

    private static string F(double value) { return value.ToString("0.##", CultureInfo.InvariantCulture); }

    private static void Validate(Parameters p)
    {
        if (p == null || p.Days == null || p.Days.Length != 3 || p.Days.Any(d => d == null))
            throw new ArgumentException("Selecione tres DayConfigData validos.");
        if (p.RegularDatabase == null) throw new ArgumentException("Selecione o banco de chamados.");
        if (p.RunsPerProfile < 1 || p.Step <= 0) throw new ArgumentException("Parametros de simulacao invalidos.");
        for (int i = 0; i < 3; i++)
        {
            DayConfigData d = p.Days[i];
            if (d.dayDurationInSeconds <= 0 || d.maxSimultaneousCalls <= 0)
                throw new ArgumentException("Dia " + (i + 1) + ": duracao e maximo simultaneo precisam ser > 0.");
            if (d.minCallSpawnInterval <= 0 || d.maxCallSpawnInterval < d.minCallSpawnInterval)
                throw new ArgumentException("Dia " + (i + 1) + ": intervalos de chamado invalidos.");
            if (d.minTrashAddInterval <= 0 || d.maxTrashAddInterval < d.minTrashAddInterval)
                throw new ArgumentException("Dia " + (i + 1) + ": intervalos de lixo invalidos.");
        }
    }

    private static DayResult SimulateDay(Parameters p, DayConfigData day, Profile profile,
                                         System.Random rng, float initialPopularity)
    {
        const int zoneCount = 5;
        float popularity = initialPopularity;
        float[] water = new float[zoneCount];
        int[] waterDirection = new int[zoneCount];
        int[] trash = new int[zoneCount];
        float[] trashPenaltyTimer = new float[zoneCount];
        bool[] waterCallActive = new bool[zoneCount];
        bool[] collecting = new bool[zoneCount];
        float[] collectorArrival = new float[zoneCount];
        float[,] unitBusyUntil = new float[4, 2];  // 2 unidades de cada tipo
        var pendingRewards = new List<PendingReward>();
        var calls = new List<ActiveCall>();
        var metrics = new DayResult();
        var normalEvents = GetValidEvents(p.RegularDatabase, day.activeEventWeights);
        var waterEvents = GetValidEvents(p.WaterDatabase, null);
        for (int z = 0; z < zoneCount; z++)
        {
            water[z] = 50;
            waterDirection[z] = rng.NextDouble() > .5 ? -1 : 1;
        }

        float waterDirectionTimer = 0, waterPenaltyTimer = 0;
        float nextCallAt = RandomRange(rng, day.minCallSpawnInterval, day.maxCallSpawnInterval);
        float nextTrashAt = RandomRange(rng, day.minTrashAddInterval, day.maxTrashAddInterval);
        float nextWaterCheck = profile.WaterInterval;
        float nextTrashCheck = profile.TrashInterval;
        bool waitingForCapacity = false;
        float duration = day.dayDurationInSeconds;
        float maxTime = duration + 180f; // limite de seguranca para Overtime
        int stepCount = Mathf.CeilToInt(maxTime / p.Step);

        for (int stepIndex = 0; stepIndex < stepCount; stepIndex++)
        {
            float time = (stepIndex + 1) * p.Step;
            bool running = time <= duration + .00001f;

            for (int r = pendingRewards.Count - 1; r >= 0; r--)
            {
                if (pendingRewards[r].At > time) continue;
                popularity = ClampPopularity(popularity + pendingRewards[r].Amount);
                pendingRewards.RemoveAt(r);
            }

            for (int z = 0; z < zoneCount; z++)
            {
                if (!collecting[z] || collectorArrival[z] > time) continue;
                popularity = ClampPopularity(popularity + trash[z] * day.popularityImpactOnCollectedTrash);
                metrics.TrashCollected += trash[z];
                trash[z] = 0;
                collecting[z] = false;
            }

            // Chamados continuam expirando no Overtime; resposta encerra o chamado imediatamente.
            for (int i = calls.Count - 1; i >= 0; i--)
            {
                ActiveCall call = calls[i];
                if (time >= call.Deadline)
                {
                    popularity = ClampPopularity(popularity + call.Data.popularityImpactOnTimeout);
                    if (call.WaterZone >= 0) waterCallActive[call.WaterZone] = false;
                    calls.RemoveAt(i);
                    metrics.Timeouts++;
                    continue;
                }
                if (time < call.NextAttempt) continue;
                if (TryRespondToCall(call, profile, rng, time, p.AverageTravelSeconds,
                                     unitBusyUntil, pendingRewards, ref popularity, metrics))
                {
                    if (call.WaterZone >= 0) waterCallActive[call.WaterZone] = false;
                    calls.RemoveAt(i);
                }
                else call.NextAttempt = time + 1.5f;
            }

            if (!running)
            {
                if (calls.Count == 0) break;
                continue;
            }

            // WaterLevelModel.UpdateValues: 5 zonas, timer de direcao e de penalidade compartilhados.
            waterDirectionTimer += p.Step;
            waterPenaltyTimer += p.Step;
            if (day.waterDirectionChangeInterval > 0 && waterDirectionTimer >= day.waterDirectionChangeInterval)
            {
                waterDirectionTimer = 0;
                for (int z = 0; z < zoneCount; z++) waterDirection[z] = rng.NextDouble() < .5 ? -1 : 1;
            }
            if (day.emptyWaterTickInterval > 0 && waterPenaltyTimer >= day.emptyWaterTickInterval)
            {
                waterPenaltyTimer = 0;
                for (int z = 0; z < zoneCount; z++)
                    if (water[z] <= 0)
                    {
                        popularity = ClampPopularity(popularity + day.popularityImpactOnEmptyWaterPerTick);
                        metrics.EmptyTicks++;
                    }
            }
            for (int z = 0; z < zoneCount; z++)
            {
                if (water[z] <= 0 || water[z] >= 100) continue;
                water[z] = Mathf.Clamp(water[z] + waterDirection[z] * day.waterVariationPerSecond * p.Step, 0, 100);
                if (water[z] >= 100)
                {
                    metrics.OverflowEvents++;
                    if (waterCallActive[z] || waterEvents.Count == 0) continue;
                    CreateCall(waterEvents[rng.Next(waterEvents.Count)], z, time, profile, rng, calls, metrics);
                    waterCallActive[z] = true;
                    popularity = ClampPopularity(popularity + day.popularityImpactOnWaterCallSpawn);
                }
            }

            for (int z = 0; z < zoneCount; z++)
            {
                if (trash[z] < 4) { trashPenaltyTimer[z] = 0; continue; }
                trashPenaltyTimer[z] += p.Step;
                if (day.fullTrashTickInterval > 0 && trashPenaltyTimer[z] >= day.fullTrashTickInterval)
                {
                    trashPenaltyTimer[z] = 0;
                    popularity = ClampPopularity(popularity + day.popularityImpactOnFullTrashPerTick);
                    metrics.FullTrashTicks++;
                }
            }

            // Jogador hipotetico inspeciona agua em intervalos, mexendo manualmente no slider.
            if (time >= nextWaterCheck)
            {
                nextWaterCheck += profile.WaterInterval;
                for (int z = 0; z < zoneCount; z++)
                    if ((water[z] <= profile.WaterMin || water[z] >= profile.WaterMax) &&
                        rng.NextDouble() < profile.WaterSuccess)
                        water[z] = 50f;
            }

            // O jogador pode enviar coletores para ate 1 ou 2 zonas por checagem.
            if (time >= nextTrashCheck)
            {
                nextTrashCheck += profile.TrashInterval;
                int actions = profile.TrashActions;
                for (int count = 4; count >= profile.TrashThreshold && actions > 0; count--)
                for (int z = 0; z < zoneCount && actions > 0; z++)
                {
                    if (collecting[z] || trash[z] != count || rng.NextDouble() >= profile.TrashSuccess)
                        continue;
                    int unitIndex = AvailableUnit(unitBusyUntil, UnitType.TrashCollector, time);
                    if (unitIndex < 0) continue;
                    float travel = TravelTime(rng, p.AverageTravelSeconds);
                    unitBusyUntil[(int)UnitType.TrashCollector, unitIndex] = time + travel;
                    collectorArrival[z] = time + travel;
                    collecting[z] = true;
                    metrics.TrashTrips++;
                    actions--;
                }
            }

            // GameManager.SpawnCallCoroutine: sorteio do intervalo e verificação de capacidade.
            if (time >= nextCallAt)
            {
                if (waitingForCapacity)
                {
                    if (calls.Count >= day.maxSimultaneousCalls)
                        nextCallAt = time + .5f;
                    else
                    {
                        waitingForCapacity = false;
                        nextCallAt = time + RandomRange(rng, day.minCallSpawnInterval, day.maxCallSpawnInterval);
                    }
                }
                else
                {
                    if (p.ReproduceSpawnCondition || calls.Count < day.maxSimultaneousCalls)
                    {
                        if (normalEvents.Count == 0) metrics.MissingEvents++;
                        else CreateCall(normalEvents[rng.Next(normalEvents.Count)], -1, time,
                                        profile, rng, calls, metrics);
                    }
                    if (calls.Count >= day.maxSimultaneousCalls)
                    {
                        waitingForCapacity = true;
                        nextCallAt = time + .5f;
                    }
                    else nextCallAt = time + RandomRange(rng, day.minCallSpawnInterval, day.maxCallSpawnInterval);
                }
            }

            if (time >= nextTrashAt)
            {
                int zone = rng.Next(zoneCount);
                if (trash[zone] < 4) trash[zone]++;
                nextTrashAt = time + RandomRange(rng, day.minTrashAddInterval, day.maxTrashAddInterval);
            }
            metrics.MaxActive = Mathf.Max(metrics.MaxActive, calls.Count);
        }

        // No OnDayEnded o UnitItemView conclui os despachos restantes.
        foreach (PendingReward reward in pendingRewards)
            popularity = ClampPopularity(popularity + reward.Amount);
        for (int z = 0; z < zoneCount; z++)
            if (collecting[z])
            {
                popularity = ClampPopularity(popularity + trash[z] * day.popularityImpactOnCollectedTrash);
                metrics.TrashCollected += trash[z];
            }
        metrics.Popularity = popularity;
        return metrics;
    }

    private static List<EventData> GetValidEvents(EventDatabase db, EventWeight[] weights)
    {
        var result = new List<EventData>();
        if (db == null || db.Events == null) return result;
        foreach (EventData e in db.Events)
        {
            if (e == null) continue;
            if (weights == null || Array.Exists(weights, w => w == e.eventWeight)) result.Add(e);
        }
        return result;
    }

    private static void CreateCall(EventData data, int waterZone, float time, Profile player,
                                   System.Random rng, List<ActiveCall> calls, DayResult result)
    {
        if (data == null) return;
        calls.Add(new ActiveCall
        {
            Data = data,
            WaterZone = waterZone,
            Deadline = time + Mathf.Max(.01f, data.eventDuration),
            NextAttempt = rng.NextDouble() < player.IgnoreChance ? float.MaxValue :
                          time + RandomRange(rng, player.ReactionMin, player.ReactionMax)
        });
        result.Spawned++;
        if (waterZone >= 0) result.WaterCalls++;
        result.MaxActive = Mathf.Max(result.MaxActive, calls.Count);
    }

    private static bool TryRespondToCall(ActiveCall call, Profile profile, System.Random rng,
                                         float now, float averageTravel, float[,] busy,
                                         List<PendingReward> pending, ref float popularity,
                                         DayResult metrics)
    {
        EventData data = call.Data;
        if (data.eventType == EventType.Question)
        {
            EventAnswer[] answers = data.possibleAnswers;
            if (answers == null || answers.Length < 2) return false;
            int best = 0;
            for (int i = 1; i < answers.Length; i++)
                if (answers[i].popularityImpact > answers[best].popularityImpact) best = i;
            bool correct = rng.NextDouble() < profile.Accuracy;
            int choice = best;
            if (!correct)
            {
                int offset = rng.Next(answers.Length - 1);
                choice = offset >= best ? offset + 1 : offset;
            }
            popularity = ClampPopularity(popularity + answers[choice].popularityImpact);
            metrics.Solved++;
            if (correct) metrics.Correct++;
            return true;
        }

        UnitType[] required = data.requiredUnits;
        if (required == null || required.Length < 1 || required.Length > 2) return false;
        if (required.Length == 2 && required[0] == required[1]) return false;

        bool correctDispatch = rng.NextDouble() < profile.Accuracy;
        UnitType[] chosen = (UnitType[])required.Clone();
        if (!correctDispatch)
        {
            var choices = new List<UnitType>();
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
                if (Array.IndexOf(required, type) < 0) choices.Add(type);
            if (choices.Count > 0) chosen[0] = choices[rng.Next(choices.Count)];
            else correctDispatch = true;
        }
        if (chosen.Length == 2 && chosen[0] == chosen[1]) return false;

        int[] allocated = new int[chosen.Length];
        for (int u = 0; u < chosen.Length; u++)
        {
            allocated[u] = AvailableUnit(busy, chosen[u], now);
            if (allocated[u] < 0) return false;
        }
        for (int u = 0; u < chosen.Length; u++)
        {
            float travel = TravelTime(rng, averageTravel);
            busy[(int)chosen[u], allocated[u]] = now + travel;
            if (u == 0)
                pending.Add(new PendingReward
                {
                    At = now + travel,
                    Amount = correctDispatch ? data.popularityImpactOnSuccess : data.popularityImpactOnWrongUnits
                });
        }
        metrics.Solved++;
        if (correctDispatch) metrics.Correct++;
        return true;
    }

    private static int AvailableUnit(float[,] busy, UnitType type, float time)
    {
        int t = (int)type;
        if (t < 0 || t >= busy.GetLength(0)) return -1;
        for (int i = 0; i < busy.GetLength(1); i++)
            if (busy[t, i] <= time) return i;
        return -1;
    }

    private static float TravelTime(System.Random rng, float mean)
    {
        return Mathf.Max(.01f, mean * RandomRange(rng, .6f, 1.4f));
    }

    private static float RandomRange(System.Random rng, float min, float max)
    {
        return min + (float)rng.NextDouble() * Mathf.Max(0f, max - min);
    }

    private static float ClampPopularity(float value) { return Mathf.Clamp(value, 0f, 100f); }
}
#endif
