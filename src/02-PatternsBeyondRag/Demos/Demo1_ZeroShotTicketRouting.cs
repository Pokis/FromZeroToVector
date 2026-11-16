using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace PatternsBeyondRag.Demos;

public static class Demo1_ZeroShotTicketRouting
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "DEMO 1 OF 5: Zero-Shot Ticket Routing (< 1 Millisecond)",
            "Problem: How do we route incoming support webhooks to 5 teams without training a custom ML model?");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Demo1_ZeroShotTicketRouting.cs",
            "var teamVectors = await embedder.GenerateAsync(teamDescriptions); // 1. Embed team descriptions once at startup",
            "var ticketVec   = await embedder.GenerateVectorAsync(incomingTicket);",
            "var bestTeam    = teams.Zip(teamVectors)",
            "                       .MaxBy(t => TensorPrimitives.Dot(ticketVec.Span, t.Second.Vector.Span));");

        // --------------------------------------------------------------------
        // SUB-STEP 1A: Define 1 description sentence per team & embed once
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 1A", "At startup, we write 1 description sentence for each team and turn it into a float[384] coordinate:");

        var teams = new (string TeamName, string Description)[]
        {
            ("BILLING-TEAM",  "Credit card declined, payment gateway 502 checkout failure, invoice VAT tax rate, refund"),
            ("SECURITY-IAM",  "Two-factor authentication MFA 2FA reset, locked account, SSO OAuth token, password login"),
            ("CLIENT-MOBILE", "Mobile app crashes when uploading profile picture avatar photo, screen flickering, UI freeze"),
            ("INFRA-SRE",     "Production database connection pool exhausted, SQL query timeout, deadlock, memory outage"),
            ("PRODUCT-IDEAS", "Feature request, add dark mode theme support, export CSV webhook integration enhancement")
        };

        var teamVectors = await embedder.GenerateAsync(teams.Select(t => t.Description));

        for (int i = 0; i < teams.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"    • {teams[i].TeamName,-14} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{ConsoleUi.FormatVectorPreview(teamVectors[i].Vector.Span, 2)} ");
            Console.ResetColor();
            Console.WriteLine($"<- \"{teams[i].Description[..Math.Min(54, teams[i].Description.Length)]}...\"");
        }

        ConsoleUi.PauseForSubStep("test 1 incoming ticket against all 5 team coordinates", autoRunAll);

        // --------------------------------------------------------------------
        // SUB-STEP 1B: Inspect 1 single incoming ticket against ALL 5 teams
        // --------------------------------------------------------------------
        string firstTicket = "Customer says their Visa card keeps getting rejected on the checkout page";
        ConsoleUi.WriteSubStepHeader("Step 1B", $"Incoming Ticket: \"{firstTicket}\"");

        var firstTicketVec = await embedder.GenerateVectorAsync(firstTicket);

        // 🔴 BREAKPOINT TIP: Put a breakpoint here to inspect `scoredTeams` in your IDE debugger!
        var scoredTeams = teams
            .Zip(teamVectors)
            .Select(t => (
                Team: t.First.TeamName,
                Score: TensorPrimitives.Dot(firstTicketVec.Span, t.Second.Vector.Span)
            ))
            .OrderByDescending(x => x.Score)
            .ToList();

        Console.WriteLine("    Comparing ticket coordinate against all 5 teams:");
        for (int i = 0; i < scoredTeams.Count; i++)
        {
            var item = scoredTeams[i];
            Console.Write("      ");
            ConsoleUi.WriteScoreBar(item.Score, 14);
            Console.ForegroundColor = i == 0 ? ConsoleColor.Green : ConsoleColor.Gray;
            Console.WriteLine($"  {item.Team}{(i == 0 ? "  ★ WINNER (Routed here!)" : "")}");
            Console.ResetColor();
        }

        ConsoleUi.PauseForSubStep("route a batch of 4 more incoming webhooks", autoRunAll);

        // --------------------------------------------------------------------
        // SUB-STEP 1C: Route a batch of incoming webhooks
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 1C", "Routing remaining incoming webhooks in < 1 millisecond:");

        string[] moreWebhooks =
        [
            "Lost my phone and cannot get past the 6-digit authenticator code screen",
            "Every time I pick a PNG for my user avatar the iOS app hangs",
            "Would love a way to automatically push weekly reports to a Slack webhook",
            "Primary Postgres replica is rejecting connections with pool timeout"
        ];

        var batchVectors = await embedder.GenerateAsync(moreWebhooks);

        foreach (var (text, vec) in moreWebhooks.Zip(batchVectors))
        {
            var winner = teams
                .Zip(teamVectors)
                .Select(t => (Team: t.First.TeamName, Score: TensorPrimitives.Dot(vec.Vector.Span, t.Second.Vector.Span)))
                .OrderByDescending(x => x.Score)
                .First();

            Console.Write("    ");
            ConsoleUi.WriteScoreBar(winner.Score, 14);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($" -> [{winner.Team,-13}] ");
            Console.ResetColor();
            Console.WriteLine($"\"{text}\"");
        }
    }
}
