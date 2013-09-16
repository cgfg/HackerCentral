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
            //bool isNewActiveUser = !ActiveUsers.ContainsKey(Context.User.Identity.Name);
            bool isNewConnection = false;
            HashSet<string> connections = ActiveUsers.GetOrAdd(Context.User.Identity.Name, new HashSet<string>());
            lock (connections)
            {
                isNewConnection = connections.Add(Context.ConnectionId);
            }
            System.Diagnostics.Debug.WriteLine("OnConnected() - {0}, isNewCon: {1}", Context.User.Identity.Name, isNewConnection);
            if (isNewConnection)
            {
                Clients.All.addNewActiveUser(Context.User.Identity.Name);

                using (var context = new HackerCentralContext(null))
                {
                    var allDeliveries = context.Deliveries
                        .Include(d => d.Message)
                        .Include(d => d.Message.Sender)
                        .Include(d => d.Reciever)
                        .Where(d => d.Reciever.UserName.Equals(Context.User.Identity.Name))
                        .OrderByDescending(d => d.Message.TimeStamp)
                        .ToArray();

                    long tickStamp = DateTime.UtcNow.Ticks;
                    long initialTickSpan = TimeSpan.FromMinutes(15).Ticks;

                    var recentDeliveries = allDeliveries
                        .TakeWhile(d =>
                        {
                            if (d.TimeDelivered == null)
                            {
                                return true;
                            }
                            if (tickStamp - d.Message.TimeStamp.Ticks < initialTickSpan)
                            {
                                tickStamp = d.Message.TimeStamp.Ticks;
                                return true;
                            }
                            else return false;
                        });

                    //var undelievered = allDeliveries.Where(d => d.TimeDelivered == null);
                    //foreach (Delivery delivery in recentDeliveries.Concat(undelievered).OrderBy(d => d.Message.TimeStamp))
                    foreach (Delivery delivery in recentDeliveries.OrderBy(d => d.Message.TimeStamp))
                    {
                        string[] usernameList = context.Deliveries.Include(d => d.Message).Include(d => d.Reciever).Where(d => d.Message.Id == delivery.Message.Id).OrderBy(d => d.Reciever.UserName).Select(d => d.Reciever.UserName).ToArray();
                        //string[] usernameList = delivery.Message.Deliveries.Where(d => d.Reciever.UserId != delivery.Reciever.UserId).OrderBy(d => d.Reciever.UserName).Select(d => d.Reciever.UserName).ToArray();
                        System.Diagnostics.Debug.WriteLine(string.Join(", ", usernameList));
                        //string usernameListJson = JsonConvert.SerializeObject(usernameList);
                        delivery.TimeDelivered = DateTime.UtcNow;
                        //foreach (string connection in connections)
                        //{
                        Clients.Client(Context.ConnectionId).addNewMessageToPage(delivery.Message.Sender.UserName, usernameList, delivery.Message.Text,delivery.Message.TimeStamp);
                        //}
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
                HashSet<string> userSet = new HashSet<string>(JsonConvert.DeserializeObject<string[]>(userListJson).Select(u => u.ToLowerInvariant()));
                userSet.Add(Context.User.Identity.Name);
                using (var context = new HackerCentralContext(null))
                { 
                    if (userSet.Any(u => u.Equals("*")))
                    {
                        userSet.Remove("*");
                        foreach (UserProfile user in context.UserProfiles)
                        {
                            userSet.Add(user.UserName);
                        }
                    }
                    else
                    {
                        if (userSet.Any(u => u.Equals("+")))
                        {
                            userSet.Remove("+");
                            Team role = Team.Pro;
                            foreach (UserProfile user in context.UserProfileDiscussions.Where(u => u.RegisteredDiscussion.ConversationId == AthenaBridgeAPISettings.CONVERSATION_ID).Where(u => u.BelongTo == role).Select(u => u.User).ToArray())
                            {
                                userSet.Add(user.UserName);
                            }
                        }
                        if (userSet.Any(u => u.Equals("-")))
                        {
                            Team role = Team.Con;
                            foreach (UserProfile user in context.UserProfileDiscussions.Where(u => u.RegisteredDiscussion.ConversationId == AthenaBridgeAPISettings.CONVERSATION_ID).Where(u => u.BelongTo == role).Select(u => u.User).ToArray())
                            {
                                userSet.Add(user.UserName);
                            }
                        }
                        if (userSet.Any(u => u.Equals("!")))
                        {
                            userSet.Remove("!");
                            foreach (string userName in ActiveUsers.Keys)
                            {
                                userSet.Add(userName);
                            }
                        }
                        if (userSet.Any(u => u.Equals("@")))
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

                    System.Diagnostics.Debug.WriteLine("sender: {0}, rec: [{1}], mes: {2}", Context.User.Identity.Name, string.Join(",", userSet), text);

                    HashSet<string> connections;
                    var userList = context.UserProfiles.Where(u => userSet.Contains(u.UserName));
                    string[] usernameList = userList.OrderBy(u => u.UserName).Select(u => u.UserName).ToArray();
                    //string usernameListJson = JsonConvert.SerializeObject(usernameList);
                    foreach (UserProfile user in userList)
                    {
                        message.Deliveries.Add(new Delivery
                            {
                                Message = message,
                                TimeDelivered = ActiveUsers.ContainsKey(user.UserName) ? new Nullable<DateTime>(DateTime.UtcNow) : null,
                                Reciever = user
                            });

                        connections = null;
                        ActiveUsers.TryGetValue(user.UserName, out connections);

                        System.Diagnostics.Debug.WriteLine("sender: {0}, rec: {1}, con: [{2}]", Context.User.Identity.Name, user.UserName, connections == null ? "null" : string.Join(",", connections));

                        if (connections != null)
                        {
                            foreach (string connection in connections)
                            {
                                Clients.Client(connection).addNewMessageToPage(Context.User.Identity.Name, usernameList, text);
                                //Clients.Client(connection).addNewMessageToPage(Context.User.Identity.Name, text);
                            }
                        }
                    }

                    context.Messages.Add(message);
                    context.SaveChanges();
                }
            }
        }

        public void GetUserList()
        {
            using (var context = new SimpleContext())
            {
                Clients.Client(Context.ConnectionId).listUsers(JsonConvert.SerializeObject(context.UserProfiles.Where(u => !u.UserName.Equals(Context.User.Identity.Name)).Select(u => new { userName = u.UserName, isOnline = ActiveUsers.Keys.Any(v => v == u.UserName) }).ToArray()));
            }
        }

        //public String GetUserName()
        //{
        //    return Context.User.Identity.Name;
        //}

        //[TODO]
        //public void getOldMessage(String userListJson, String time)
        //{
        //    HashSet<string> userSet = new HashSet<string>(JsonConvert.DeserializeObject<string[]>(userListJson).Select(u => u.ToLowerInvariant()));
        //    DateTime timeStamp = DateTime.Parse(time);
        //    using (var context = new SimpleContext()){
        //    foreach(Message m in context.Messages.Include(u => u.Deliveries).Select(u => u.Deliveries).Where(d => d.Select(r => r.Reciever).ToArray().) ){

        //    }
        //}
        //}
    }
}
