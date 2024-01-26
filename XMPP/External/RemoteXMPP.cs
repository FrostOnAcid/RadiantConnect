﻿// ReSharper disable CheckNamespace

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using RadiantConnect.Network;

namespace RadiantConnect.XMPP
{
    internal record XMPPAuth(string OAuth, string Entitlement, string PASToken);

    internal class XMPPNet(Initiator initiator)
    {
        internal async Task<string> FetchPasToken()
        {
            return await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Get, "https://riot-geo.pas.si.riotgames.com", "/pas/v1/service/chat") ?? "";
        }

        internal async Task<(string,string)> FetchAffinity()
        {
            string? clientConfig = await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Get, "https://clientconfig.rpg.riotgames.com", "/api/v1/config/player?app=Riot%20Client");
            // I gotta test this to see output data
            return (string.Empty, clientConfig ?? "");
        }

        internal async Task<XMPPAuth> FetchTokens()
        {
            (string authorizationBearer, string jwt) = await initiator.ExternalSystem.Net.GetAuthorizationToken();
            return new XMPPAuth(authorizationBearer, jwt, await FetchPasToken());
        }

    }

    public class RemoteXMPP(Initiator initiator)
    {
        internal XMPPNet XmppNet = new(initiator);
        internal XMPPAuth XmppAuth = null!;
        internal string AffinityUrl = null!;
        internal string AffinityDomain = null!;

        internal NetworkStream ClientStream = null!;
        internal StreamReader ClientReader = null!;
        internal StreamWriter ClientWriter = null!;
        internal Socket ClientSocket = null!;

        public async Task InitiateConnection()
        {
            (AffinityUrl, AffinityDomain) = await XmppNet.FetchAffinity();
            XmppAuth = await XmppNet.FetchTokens();
            await Task.Run(ConnectToServerAsync);
        }

        internal async Task ConnectToServerAsync()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await ClientSocket.ConnectAsync(AffinityUrl, 5223);

            Debug.WriteLine($"Connected to {AffinityUrl}:5223");

            await HandleServerAsync(ClientSocket);
        }

        internal async Task SendAndWait(Socket client, [StringSyntax(StringSyntaxAttribute.Xml)] string xmlMessage, string waitFor)
        {
            await SendXmlMessage(xmlMessage);
            while (client.Connected)
            {
                string data = await ClientReader.ReadToEndAsync();
                Debug.WriteLine($"Auth Sending: {xmlMessage}\nReturned Data: {data}");
                if (data.Contains(waitFor))
                    break;
            }
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        internal async Task DoAuthSend(Socket client)
        {
            while (client.Connected)
            {
                try
                {
                    await SendAndWait(client, $"<?xml version=\"1.0\"?><stream:stream to=\"{AffinityDomain}.pvp.net\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\"/>", "X-Riot-RSO-PAS");
                    
                    await SendAndWait(client, $"<?xml version=\"1.0\"?><stream:stream to=\"${AffinityDomain}.pvp.net\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\"/><auth mechanism=\"X-Riot-RSO-PAS\" xmlns=\"urn:ietf:params:xml:ns:xmpp-sasl\"><rso_token>{XmppAuth.OAuth}</rso_token><pas_token>{XmppAuth.PASToken}</pas_token></auth>", "");
                    
                    await SendAndWait(client, $"<?xml version=\"1.0\"?><stream:stream to=\"{AffinityDomain}.pvp.net\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\"/>", "stream:features");

                    await SendAndWait(client, "<iq id=\"_xmpp_bind1\" type=\"set\"><bind xmlns=\"urn:ietf:params:xml:ns:xmpp-bind\"></bind></iq>", "");

                    await SendAndWait(client, "<iq id=\"_xmpp_session1\" type=\"set\"><session xmlns=\"urn:ietf:params:xml:ns:xmpp-session\"/></iq>", "");

                    await SendAndWait(client, $"<iq id=\"xmpp_entitlements_0\" type=\"set\"><entitlements xmlns=\"urn:riotgames:entitlements\"><token xmlns=\"\">{XmppAuth.Entitlement}</token></entitlements></iq>", "");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Auth Failure: {ex}");
                    break;
                }
            }
        }

        internal async Task HandleServerAsync(Socket client)
        {
            ClientStream = new NetworkStream(client);
            ClientReader = new StreamReader(ClientStream, Encoding.UTF8);
            ClientWriter = new StreamWriter(ClientStream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                await DoAuthSend(client);
                while (client.Connected)
                {
                    string? data = await ClientReader.ReadLineAsync();

                    if (data is null) continue;

                    Debug.WriteLine($"Received data from server: {data}");
                    await ClientWriter.WriteLineAsync($"Client sent: {data}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Remote XMPP Error: {ex}");
            }
            finally
            {
                ClientReader.Close();
                ClientWriter.Close();
                ClientStream.Close();
                client.Close();
            }
        }

        public async Task SendXmlMessage([StringSyntax(StringSyntaxAttribute.Xml)] string xmlMessage)
        {
            byte[] xmlBuffer = Encoding.UTF8.GetBytes(xmlMessage);
            await ClientStream.WriteAsync(xmlBuffer);
        }
    }
}
