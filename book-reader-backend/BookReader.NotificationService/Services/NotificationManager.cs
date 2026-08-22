using BookReader.NotificationService.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BookReader.NotificationService.Services
{
    public class NotificationManager : INotificationManager
    {
        private readonly ConcurrentDictionary<int, List<HttpResponse>> _connections = new();
        public void Add(int userId, HttpResponse response)
        {
            var connections = _connections.GetOrAdd(
                userId, _  => new List<HttpResponse>());

            lock (connections)
            {
                connections.Add(response);
            }
        }

        public IEnumerable<HttpResponse> GetConnections(int userId)
        {
            if (!_connections.TryGetValue(userId, out var connections))
                return Enumerable.Empty<HttpResponse>();

            lock (connections)
            {
                return connections.ToList();
            }
        }

        public void Remove(int userId, HttpResponse response)
        {
            if (!_connections.TryGetValue(userId, out var connections))
                return;

            lock (connections)
            {
                connections.Remove(response);

                if (connections.Count == 0)
                {
                    _connections.TryRemove(userId, out _);
                }
            }
        }

        public async Task SendAsync<T>(int userId, T message)
        {
            var connections = GetConnections(userId);

            var json = JsonSerializer.Serialize(message);
            foreach (var connection in connections)
            {
                await connection.WriteAsync($"data: {json}\n\n");
                await connection.Body.FlushAsync();
            }
        }
    }
}
