using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SignalRChat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> ActiveUsers = new ConcurrentDictionary<string, HashSet<string>>();

        public override Task OnConnected()
        {
            bool isNew = !ActiveUsers.ContainsKey(Context.User.Identity.Name);

            HashSet<string> connections = ActiveUsers.GetOrAdd(Context.User.Identity.Name, new HashSet<string>());
            lock (connections)
            {
                connections.Add(Context.ConnectionId);
            }

            if (isNew)
            {
                Clients.All.addNewActiveUser(Context.User.Identity.Name);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            HashSet<string> connections;
            ActiveUsers.TryGetValue(Context.User.Identity.Name, out connections);

            bool hasLeft = false;
            if (connections != null)
            {
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                    hasLeft = !connections.Any();
                    if (hasLeft)
                    {
                        HashSet<string> removed;
                        ActiveUsers.TryRemove(Context.User.Identity.Name, out removed);
                    }
                }
            }
            else
            {
                hasLeft = true;
            }

            if (hasLeft)
            {
                Clients.All.removeInactiveUser(Context.User.Identity.Name);
            }

            return base.OnDisconnected();
        }

        private HashSet<string> GetConnections(string username)
        {
            HashSet<string> connections;
            ActiveUsers.TryGetValue(username, out connections);

            return connections;
        }

        // example of how groups work
        //public Task Join()
        //{
        //    return Groups.Add(Context.ConnectionId, "foo");
        //}

        //public Task Send(string message)
        //{
        //    return Clients.Group("foo").addMessage(message);
        //}

        public void Send(string userListJson, string message)
        {
            HashSet<string> userSet = new HashSet<string>(JsonConvert.DeserializeObject<string[]>(userListJson));
            if (userSet.Any(u => u.ToLower() == "all" || u.ToLower() == "*"))
            {
                Clients.All.addNewMessageToPage(Context.User.Identity.Name, message);
            }
            else
            {
                userSet.Add(Context.User.Identity.Name);

                lock (ActiveUsers)
                {
                    HashSet<string> connections;
                    foreach (string username in userSet)
                    {
                        connections = null;
                        ActiveUsers.TryGetValue(username, out connections);
                        if (connections != null)
                        {
                            foreach (string connection in connections)
                            {
                                Clients.Client(connection).addNewMessageToPage(Context.User.Identity.Name, message);
                            }
                        }
                    }
                }
            }
        }

        public void GetActiveUsers()
        {
            Clients.Client(Context.ConnectionId).listActiveUsers(JsonConvert.SerializeObject(ActiveUsers.Keys.Where(u => u != Context.User.Identity.Name).ToArray()));
        }
    }
}
