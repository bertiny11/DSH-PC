using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Listens for Heisenberg Quest discovery broadcasts and answers with a ready signal.
/// Attach this component to the PC-side Unity scene.
/// </summary>
public sealed class PCNetworkDiscovery : MonoBehaviour
{
    private const string DiscoveryMessage = "HEISENBERG_LOOKING";
    private const string PcReadyMessage = "PC_READY";

    [Header("Discovery")]
    [SerializeField] private int discoveryPort = 47777;

    private CancellationTokenSource cancellationSource;
    private UdpClient udpListener;
    private Task listenerTask;

    private void OnEnable()
    {
        StartListening();
    }

    private void OnDisable()
    {
        StopListening();
    }

    private void OnDestroy()
    {
        StopListening();
    }

    public void StartListening()
    {
        if (listenerTask != null && !listenerTask.IsCompleted)
        {
            return;
        }

        UdpClient listener = CreateListener(discoveryPort);

        cancellationSource = new CancellationTokenSource();
        udpListener = listener;
        listenerTask = ListenForQuestAsync(listener, cancellationSource.Token);
    }

    public void StopListening()
    {
        CancellationTokenSource source = cancellationSource;
        UdpClient listener = udpListener;
        Task task = listenerTask;

        source?.Cancel();

        // Closing the socket interrupts any pending ReceiveAsync call immediately.
        listener?.Close();
        listener?.Dispose();
        udpListener = null;
        listenerTask = null;

        DisposeCancellationSourceWhenTaskFinishes(source, task);
        cancellationSource = null;
    }

    private static UdpClient CreateListener(int port)
    {
        UdpClient listener = new UdpClient();
        listener.EnableBroadcast = true;
        listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        listener.Client.Bind(new IPEndPoint(IPAddress.Any, port));

        return listener;
    }

    private async Task ListenForQuestAsync(UdpClient listener, CancellationToken cancellationToken)
    {
        byte[] readyPayload = Encoding.UTF8.GetBytes(PcReadyMessage);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await listener.ReceiveAsync().ConfigureAwait(false);
                string message = Encoding.UTF8.GetString(result.Buffer);

                if (!string.Equals(message, DiscoveryMessage, StringComparison.Ordinal))
                {
                    continue;
                }

                await listener.SendAsync(readyPayload, readyPayload.Length, result.RemoteEndPoint)
                    .ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (SocketException)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }

    private static void DisposeCancellationSourceWhenTaskFinishes(CancellationTokenSource source, Task task)
    {
        if (source == null)
        {
            return;
        }

        if (task == null || task.IsCompleted)
        {
            source.Dispose();
            return;
        }

        task.ContinueWith(
            completedTask =>
            {
                _ = completedTask.Exception;
                source.Dispose();
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }
}
