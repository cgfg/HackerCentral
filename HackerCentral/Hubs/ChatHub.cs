using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using HackerCentral.Models;
using WebMatrix.WebData;
using System.Data.Entity;

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

                using (var context = new HackerCentralContext(null))
                {
                    foreach (Delivery delivery in context.Deliveries.Include(d => d.Message).Include(d => d.Message.Sender).Where(d => d.TimeDelivered == null).OrderBy(d => d.Message.TimeStamp))
                    {
                        delivery.TimeDelivered = DateTime.UtcNow;
                        foreach (string connection in connections)
                        {
                            Clients.Client(connection).addNewMessageToPage(delivery.Message.Sender.UserName, delivery.Message.Text);
                        }
                    }
                    context.SaveChanges();
                }
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

        public void Send(string userListJson, string text)
        {
            lock (ActiveUsers)
            {
                HashSet<string> userSet = new HashSet<string>(JsonConvert.DeserializeObject<string[]>(userListJson));
                userSet.Add(Context.User.Identity.Name);
                using (var context = new HackerCentralContext(null))
                {
                    if (userSet.Any(u => u.ToLower() == "*"))
                    {
                        userSet.Remove("*");
                        foreach (UserProfile user in context.UserProfiles)
                        {
                            userSet.Add(user.UserName);
                        }
                    }
                    else
                    {
                        if (userSet.Any(u => u.ToLower() == "!"))
                        {
                            userSet.Remove("!");
                            foreach (string userName in ActiveUsers.Keys)
                            {
                                userSet.Add(userName);
                            }
                        }
                        if (userSet.Any(u => u.ToLower() == "@"))
                        {
                            userSet.Remove("@");
                            foreach (string userName in context.UserProfiles.AsEnumerable().Where(u => ActiveUsers.ContainsKey(u.UserName)).Select(u => u.UserName))
                            {
                                userSet.Add(userName);
                            }
                        }
                    }

                    Message message = new Message
                    {
                        TimeStamp = DateTime.UtcNow,
                        Sender = context.UserProfiles.Find(WebSecurity.CurrentUserId),
                        Text = text,
                        Deliveries = new List<Delivery>()

                    };

                    foreach (UserProfile user in context.UserProfiles.Where(u => userSet.Contains(u.UserName)))
                    {
                        message.Deliveries.Add(new Delivery
                            {
                                Message = message,
                                TimeDelivered = ActiveUsers.ContainsKey(user.UserName) ? new Nullable<DateTime>(DateTime.UtcNow) : null,
                                Reciever = user
                            });
                    }

                    context.Messages.Add(message);
                    context.SaveChanges();

                    HashSet<string> connections;
                    foreach (string username in userSet)
                    {
                        connections = null;
                        ActiveUsers.TryGetValue(username, out connections);
                        if (connections != null)
                        {
                            foreach (string connection in connections)
                            {
                                Clients.Client(connection).addNewMessageToPage(Context.User.Identity.Name, text);
                            }
                        }
                    }
                }
            }
        }

        public void GetUserList()
        {
            using (var context = new SimpleContext())
            {
                Clients.Client(Context.ConnectionId).listUsers(JsonConvert.SerializeObject(context.UserProfiles.Where(u => u.UserName != Context.User.Identity.Name).Select(u => new { userName = u.UserName, isOnline = ActiveUsers.Keys.Any(v => v == u.UserName) }).ToArray()));
            }
        }
    }
}
