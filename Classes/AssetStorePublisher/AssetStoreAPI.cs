using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using HardCodeLab.RockTomate.Steps.AssetStorePublisher.Data;

namespace HardCodeLab.RockTomate.Steps.AssetStorePublisher
{
    [Serializable]
    public class AssetStoreAPI
    {
        private const string AssetStoreToolsVersion = "V4.1.0";
        private const string AnonymousXUnitySession = "26c4202eb475d02864b40827dfff11a14657aa41";
        private const string EndPoint = "https://kharma.unity3d.com";
        private const string LastUnitySessionKeyPref = "AssetStoreAPI_lastUnitySession";

        public Action SessionConnecting;
        public Action SessionConnected;
        public Action SessionConnectionFailed;
        public Action SessionDisconnected;

        public Action PackagesFetching;
        public Action PackagesFetched;

        public Action Uploading;
        public Action Uploaded;

        public string LastError { get; private set; }

        [NonSerialized]
        private Session session;
        public Session Session { get { return session; } }
        public bool IsConnected { get { return session != null && session.is_anonymous == false; } }

        [SerializeField]
        private Package[] packages;

        public bool HasPackages { get { return packages != null; } }
        public IEnumerable<Package> EachPackage
        {
            get
            {
                if (packages == null)
                    yield break;

                for (int i = 0; i < packages.Length; i++)
                {
                    yield return packages[i];
                }
            }
        }

        private HashSet<int> runningRequests = new HashSet<int>();

        public bool IsLogging()
        {
            lock (runningRequests)
            {
                string endPoint = EndPoint + "/login";
                return runningRequests.Contains(endPoint.GetHashCode());
            }
        }

        public void Login(string user, string pass, Action<HttpWebResponse, string> onCompleted = null)
        {
            string endPoint = EndPoint + "/login";
            string post = string.Format("user={0}&pass={1}&unityversion={2}&toolversion={3}&license_hash={4}&hardware_hash={5}", user, pass, Application.unityVersion, AssetStoreToolsVersion, InternalEditorUtility.GetAuthToken().Substring(0, 40), InternalEditorUtility.GetAuthToken().Substring(40, 40));
            HttpWebRequest request = CreateRequest(endPoint);
            byte[] data = Encoding.ASCII.GetBytes(post);

            if (SessionConnecting != null)
                SessionConnecting();

            request.Accept = "application/json";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HandleRequest(request, endPoint.GetHashCode(), OnLoginCompleted(onCompleted));
        }

        public void AutoLogin()
        {
            string lastUnitySession = EditorPrefs.GetString(LastUnitySessionKeyPref);
            if (string.IsNullOrEmpty(lastUnitySession) == false)
                LoginWithSession(lastUnitySession);
        }

        public void LoginWithSession(string unitySession, Action<HttpWebResponse, string> onCompleted = null)
        {
            string endPoint = string.Format("{0}/login?reuse_session={1}&unityversion={2}&toolversion={3}&xunitysession={4}", EndPoint, unitySession, Application.unityVersion, AssetStoreToolsVersion, AnonymousXUnitySession);
            HttpWebRequest request = CreateRequest(endPoint);

            request.Accept = "application/json";

            if (SessionConnecting != null)
                SessionConnecting();

            HandleRequest(request, (EndPoint + "/login").GetHashCode(), OnLoginCompleted(onCompleted));
        }

        private Action<HttpWebResponse, string> OnLoginCompleted(Action<HttpWebResponse, string> onCompleted)
        {
            return (response, result) =>
            {
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.LogError(result);
                    LastError = result;

                    if (SessionConnectionFailed != null)
                        SessionConnectionFailed();

                    if (onCompleted != null)
                        onCompleted(response, result);

                    return;
                }

                try
                {
                    Session session = new Session();

                    EditorJsonUtility.FromJsonOverwrite(result, session);

                    if (session.id == "0")
                    {
                        if (SessionConnectionFailed != null)
                            SessionConnectionFailed();

                        if (onCompleted != null)
                            onCompleted(response, result);

                        return;
                    }

                    this.session = session;
                    EditorPrefs.SetString(LastUnitySessionKeyPref, session.xunitysession);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);

                    if (SessionConnectionFailed != null)
                        SessionConnectionFailed();

                    if (onCompleted != null)
                        onCompleted(response, result);

                    session = null;

                    return;
                }

                if (SessionConnected != null)
                    SessionConnected();

                if (onCompleted != null)
                    onCompleted(response, result);
            };
        }

        public void Logout()
        {
            session = null;
            packages = null;

            if (SessionDisconnected != null)
                SessionDisconnected();
        }

        public bool IsFetchingPackages()
        {
            lock (runningRequests)
            {
                return runningRequests.Contains("FetchPackages".GetHashCode());
            }
        }

        public void FetchPackages(Action<HttpWebResponse, string> onCompleted = null)
        {
            string endPoint = string.Format("{0}/api/asset-store-tools/metadata/0.json?unityversion={1}&toolversion={2}&xunitysession={3}", 
                EndPoint, Application.unityVersion, AssetStoreToolsVersion, session.xunitysession);

            var request = CreateRequest(endPoint);

            request.Accept = "application/json";
            request.Headers.Add("X-Unity-Session", session.xunitysession);

            if (PackagesFetching != null)
                PackagesFetching();

            HandleRequest(request, "FetchPackages".GetHashCode(), (response, result) =>
            {
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.LogError(result);
                    LastError = result;

                    if (SessionConnectionFailed != null)
                        SessionConnectionFailed();

                    if (onCompleted != null)
                        onCompleted(response, result);

                    return;
                }

                try
                {
                    string json = result;

                    // Convert JSON structure to allow Unity's json utility to handle it.
                    json = json.Replace("\"packages\":{\"", "\"packages\":[{\"packageId\":\"");
                    json = json.Replace("}},\"status\"", "}],\"status\"");
                    json = json.Replace("\"},\"", "\"},{\"packageId\":\"");
                    json = json.Replace(":{\"project_path\"", ",\"project_path\"");

                    PackageCollection packages = new PackageCollection();

                    EditorJsonUtility.FromJsonOverwrite(json, packages);

                    List<Package> list = new List<Package>(packages.packages);

                    list.Sort((a, b) => a.name.CompareTo(b.name));

                    this.packages = list.ToArray();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                finally
                {
                    if (PackagesFetched != null)
                        PackagesFetched();

                    if (onCompleted != null)
                        onCompleted(response, result);
                }
            });
        }

        public bool IsUploading(string versionId)
        {
            lock (runningRequests)
            {
                return runningRequests.Contains(versionId.GetHashCode());
            }
        }

        public void Upload(string versionId, string packagePath, string rootPath, string projectPath, string unityVersion, Action<HttpWebResponse, string> onCompleted = null)
        {
            string endPoint = EndPoint + "/api/asset-store-tools/package/"
                                                     + versionId + "/unitypackage.json?"
                                                     + "unityversion=" + unityVersion
                                                     + "&toolversion=" + AssetStoreToolsVersion
                                                     + "&xunitysession=" + session.xunitysession
                                                     + "&root_guid=" + AssetDatabase.AssetPathToGUID("Assets" + rootPath)
                                                     + "&root_path=" + Uri.EscapeDataString(rootPath)
                                                     + "&project_path=" + Uri.EscapeDataString(projectPath);
            HttpWebRequest request = CreateRequest(endPoint);

            request.Headers.Add("X-Unity-Session", session.xunitysession);
            request.Method = "PUT";
            request.AllowWriteStreamBuffering = false;
            request.Timeout = 36000000;
            request.KeepAlive = false;

            byte[] content = File.ReadAllBytes(packagePath);
            request.ContentLength = content.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(content, 0, content.Length);
            }

            HandleRequest(request, versionId.GetHashCode(), onCompleted);
        }

        private void Write(Stream stream, string a)
        {
            byte[] boundaryRNB = Encoding.UTF8.GetBytes(a);
            stream.Write(boundaryRNB, 0, boundaryRNB.Length);
        }

        private HttpWebRequest CreateRequest(string endpoint)
        {
            var request = (HttpWebRequest)WebRequest.Create(endpoint);
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            return request;
        }

        private void HandleRequest(HttpWebRequest request, int requestHash, Action<HttpWebResponse, string> onCompleted)
        {
            lock (runningRequests)
            {
                runningRequests.Add(requestHash);
            }

            Action asyncWebRequestInvoke = () =>
            {
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
                    {
                        string result = readStream.ReadToEnd();
                        EditorApplication.delayCall += () => onCompleted(response, result);
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;

                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                            session = null;
                        EditorApplication.delayCall += () => onCompleted(response, response.StatusDescription);
                    }
                    else
                    {
                        EditorApplication.delayCall += () => onCompleted(null, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    EditorApplication.delayCall += () => onCompleted(null, ex.Message);
                }
                finally
                {
                    EditorApplication.delayCall += () =>
                    {
                        lock (runningRequests)
                        {
                            runningRequests.Remove(requestHash);
                        }
                    };
                }
            };

            asyncWebRequestInvoke.BeginInvoke(iar => ((Action)iar.AsyncState).EndInvoke(iar), asyncWebRequestInvoke);
        }
    }
}