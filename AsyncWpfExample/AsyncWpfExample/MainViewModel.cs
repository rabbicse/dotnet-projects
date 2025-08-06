using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;

namespace AsyncWpfExample;

internal class MainViewModel : ObservableObject
{
    #region Declaration(s)
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(2, 2);
    private readonly HttpClient httpClient = new HttpClient();
    #endregion

    #region Notification Properties
    private string? _downloadMessage;
    public string? DownloadMessage
    {
        get => _downloadMessage; set
        {
            _downloadMessage = value;
            OnPropertyChanged(() => DownloadMessage);
        }
    }

    private string? _downloadedMessage;
    public string? DownloadedMessage
    {
        get => _downloadedMessage; set
        {
            _downloadedMessage = value;
            OnPropertyChanged(() => DownloadedMessage);
        }
    }

    public string? ParallelForMessage
    {
        get => _parallelForMessage; set
        {
            _parallelForMessage = value;
            OnPropertyChanged(() => ParallelForMessage);
        }
    }

    private string? _parallelForMessage;
    #endregion


    public async Task DownloadDataAsync(uint counter)
    {
        DownloadMessage = $"Downloading Data [{counter}]";

        //System.Threading.Thread.Sleep(1000);
        //Test();

        // await Task.Delay(1000); // Simulate some delay

        if (await DownloadHttpDataAsync())
        {
            DownloadedMessage = $"Download Data done! [{counter}]";
        }
        else
        {
            DownloadedMessage = $"Download Failed! [{counter}]";
        }
    }

    public async Task<uint> DownloadDataWithCounterAsync(uint counter)
    {
        DownloadMessage = $"Downloading Data [{counter}]";

        //System.Threading.Thread.Sleep(1000);
        //Test();

        // await Task.Delay(1000); // Simulate some delay

        if (await DownloadHttpDataAsync())
        {
            DownloadedMessage = $"Download Data done! [{counter}]";
        }
        else
        {
            DownloadedMessage = $"Download Failed! [{counter}]";
        }

        return counter;
    }

    private async void Test()
    {
        await Task.Delay(2000);
    }

    public async Task<bool> DownloadHttpDataAsync()
    {
        try
        {
            await _lock.WaitAsync();

            //await Task.Delay(1000);

            var data = await httpClient.GetAsync("https://learn.microsoft.com/en-us/dotnet/csharp/async");

            return data != null && data.IsSuccessStatusCode;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void ParallelForExample()
    {
        // Parallel For
        Parallel.For(0, 100, index =>
        {
            Thread.Sleep(1000);
            ParallelForMessage = $"Result: {index}";

            Debug.WriteLine($"Result: {index}");
        });
    }

    public async Task ParallelForExampleAsync()
    {
        await Task.Run(() =>
        {
            // Parallel For
            Parallel.For(0, 100, index =>
            {
                Thread.Sleep(1000);
                ParallelForMessage = $"Result: {index}";
            });
        });
    }

    /// <summary>
    /// GetPrimeListWithParallel returns Prime numbers by using Parallel.ForEach
    /// </summary>
    /// <param name="numbers"></param>
    /// <returns></returns>
    private static IList<int> GetPrimeListWithParallel(IList<int> numbers)
    {
        var primeNumbers = new ConcurrentBag<int>();

        Parallel.ForEach(numbers, number =>
        {
            if (IsPrime(number))
            {
                primeNumbers.Add(number);
            }
        });

        return primeNumbers.ToList();
    }

    /// <summary>
    /// IsPrime returns true if number is Prime, else false.(https://en.wikipedia.org/wiki/Prime_number)
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private static bool IsPrime(int number)
    {
        if (number < 2)
        {
            return false;
        }

        for (var divisor = 2; divisor <= Math.Sqrt(number); divisor++)
        {
            if (number % divisor == 0)
            {
                return false;
            }
        }
        return true;
    }
}
