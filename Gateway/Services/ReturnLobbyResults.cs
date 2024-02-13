using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway.Services;
public class ReturnLobbyResults
{
    private readonly WebSocket _webSocket;

    public ReturnLobbyResults(WebSocket webSocket)
    {
        _webSocket = webSocket;
    }

    public async Task ReturnResults()
    {
        var buffer = new byte[1024 * 4]; 
        WebSocketReceiveResult result;

        do
        {
            result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text && result.EndOfMessage)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await SendMessageAsync(message);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

        } while (!result.CloseStatus.HasValue);
    }

    private async Task SendMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
