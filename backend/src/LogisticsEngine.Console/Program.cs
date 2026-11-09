using LogisticsEngine.Application;
using LogisticsEngine.Console.Controllers;
using LogisticsEngine.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Composition Root: è il punto di ingresso in cui assembliamo tutte le parti dell'applicazione.
/// Qui decidiamo quali classi concrete utilizzare per svolgere i diversi compiti e come collegarle tra loro.
/// L'obiettivo è evitare che i controller debbano creare autonomamente gli oggetti da cui dipendono.
/// Sarà il contenitore DI a occuparsi della loro creazione.
/// </summary>
public class Program
{
    public static async Task Main()
    {
        // ---------------------------------------------------------
        // DEFINIZIONE DELLA CARTELLA DEI DATI
        // ---------------------------------------------------------
        // AppContext.BaseDirectory restituisce il percorso della cartella in cui viene eseguita l'applicazione compilata.
        //
        // Path.Combine costruisce un percorso valido per il sistema operativo utilizzato, aggiungendo la sottocartella "data".
        // In un'applicazione reale potremmo invece collegarci a un database MongoDB, PostgreSQL o a un altro servizio generico (che faremo).
        string dataDirectory = ResolveDataDirectory();

        // ---------------------------------------------------------
        // CREAZIONE DEL CONTENITORE DI DEPENDENCY INJECTION
        // ---------------------------------------------------------
        // ServiceCollection è una raccolta di registrazioni.
        //
        // ATTENZIONE: in questo momento NON stiamo creando i controller, i repository o i servizi applicativi.
        // Stiamo semplicemente comunicando al contenitore che quando qualcuno avrà bisogno di questa interfaccia,
        // oppure di questa classe, saprai quale oggetto creare e quali dipendenze fornirgli.
        // È una sorta di "elenco delle istruzioni" necessarie per costruire gli oggetti dell'applicazione.
        ServiceCollection services = new();

        // ---------------------------------------------------------
        // REGISTRAZIONE DEL SISTEMA DI LOGGING
        // ---------------------------------------------------------
        // Registriamo i servizi necessari per produrre messaggi di log durante l'esecuzione dell'applicazione.
        // Il logging permette, ad esempio, di registrare:
        // - l'inizio e la fine di un'operazione;
        // - eventuali errori;
        // - informazioni utili per diagnosticare problemi.
        //
        // Anche MediatR utilizza il sistema di logging e si aspetta di trovare ILoggerFactory nel contenitore DI.
        // Il pacchetto necessario è:
        // dotnet add LogisticsEngine.Console package Microsoft.Extensions.Logging
        // dotnet add LogisticsEngine.Console package Microsoft.Extensions.DependencyInjection --version 10.0.14
        services.AddLogging();

        // ---------------------------------------------------------
        // REGISTRAZIONE DEL LIVELLO APPLICATION
        // ---------------------------------------------------------
        // AddApplication è un metodo di estensione (extension method) che abbiamo definito nel progetto LogisticsEngine.Application.
        // Al suo interno registriamo:
        // - gli handler MediatR, cioè le classi che eseguono concretamente i casi d'uso;
        // - i validator FluentValidation, che controllano la correttezza dei dati ricevuti;
        // - i servizi che contengono regole di dominio.
        //
        // Ad esempio, quando un controller invierà un comando "RegisterHubCommand" tramite MediatR, il contenitore DI
        // saprà quale handler utilizzare per gestirlo.
        //
        // Stiamo registrando le dipendenze, non eseguendo direttamente i casi d'uso.
        services.AddApplication();

        // ---------------------------------------------------------
        // REGISTRAZIONE DEL LIVELLO INFRASTRUCTURE
        // ---------------------------------------------------------
        // AddInfrastructure è un altro metodo di estensione, definito nel progetto LogisticsEngine.Infrastructure.
        //
        // Qui colleghiamo le interfacce utilizzate dall'applicazione alle loro implementazioni concrete.
        //
        // Ad esempio, se Application richiede un repository attraverso un'interfaccia, Infrastructure fornisce
        // la classe concreta che sa leggere e salvare i dati.
        //
        // Passiamo dataDirectory perché le implementazioni che gestiscono i file JSON devono sapere dove trovarli.
        // In questo modo Application non deve conoscere:
        // - dove si trovano i file;
        // - come vengono aperti;
        // - come vengono serializzati/deserializzati i dati dalla persistenza.
        services.AddInfrastructure(dataDirectory);

        // ---------------------------------------------------------
        // REGISTRAZIONE DEI CONTROLLER
        // ---------------------------------------------------------
        // Registriamo i controller che gestiscono l'interazione tra il menu della console e i casi d'uso applicativi.
        //
        // AddTransient significa che ogni volta che il contenitore deve fornire una di queste classi, ne crea una nuova istanza.
        //
        // ATTENZIONE: qui NON stiamo ancora creando i controller. Stiamo solamente indicando al contenitore come comportarsi
        // quando qualcuno ne richiederà uno.
        //
        // Ad esempio, HubController potrebbe avere nel costruttore una dipendenza da IMediator.
        //
        // Quando chiederemo HubController al contenitore, sarà quest'ultimo a:
        // 1. creare HubController;
        // 2. individuare che serve IMediator;
        // 3. recuperare o creare l'implementazione di IMediator;
        // 4. fornire IMediator al costruttore di HubController.
        services.AddTransient<HubController>();
        services.AddTransient<ShipmentController>();
        services.AddTransient<ConsoleController>();

        // ---------------------------------------------------------
        // COSTRUZIONE DEL SERVICE PROVIDER
        // ---------------------------------------------------------
        // Fino a questo momento abbiamo soltanto raccolto le registrazioni delle dipendenze.
        // BuildServiceProvider costruisce il contenitore effettivo, che da questo momento è in grado di creare oggetti
        // e risolvere le loro dipendenze.
        // Il ServiceProvider è quindi il componente a cui possiamo chiedere concretamente un oggetto.
        //
        // using garantisce che, al termine del Main, il contenitore venga rilasciato e che le risorse degli oggetti gestiti
        // da DI vengano correttamente liberate.
        using ServiceProvider provider = services.BuildServiceProvider();

        // ---------------------------------------------------------
        // AVVIO DELL'APPLICAZIONE
        // ---------------------------------------------------------
        // Chiediamo al contenitore DI di fornirci ConsoleController.
        //
        // GetRequiredService:
        // - cerca la registrazione di ConsoleController;
        // - crea il controller;
        // - individua le sue dipendenze;
        // - crea o recupera gli oggetti necessari;
        // - li passa al costruttore.
        //
        // A questo punto il controller è pronto per essere utilizzato.
        // RunAsync avvia il menu della console.
        // Da qui l'utente potrà interagire con l'applicazione e inviare richieste ai casi d'uso attraverso MediatR.
        await provider.GetRequiredService<ConsoleController>().RunAsync();
    }

    private static string ResolveDataDirectory()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            bool hasSourceAndTests = Directory.Exists(Path.Combine(directory.FullName, "src"))
                && Directory.Exists(Path.Combine(directory.FullName, "tests"));
            if (hasSourceAndTests)
            {
                return Path.Combine(directory.FullName, "data");
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, "data");
    }
}
