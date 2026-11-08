using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CP2077SaveEditor.Cloud
{
    internal sealed record DeviceCode(string UserCode, string VerificationUrl);

    internal sealed record CloudSlot(string Name, long Bytes, string LastModified);

    /// <summary>
    /// Talks to the CD PROJEKT RED cross-save cloud the same way the game does.
    /// The API is not publicly documented and may change without notice.
    /// The access token only ever lives in memory: it is never written to disk or logged.
    /// </summary>
    internal sealed class CdprCloudClient : IDisposable
    {
        private const string ClientId = "53652320436400298";
        private const string AuthBase = "https://accounts.cdprojektred.com/realms/cdp/protocol/openid-connect";
        private const string SavesBase = "https://game-saves.live.cdpred.services/v1";
        private const string SdkUserAgent = "REDGalaxySDK/1.149.23.0 (Windows) GOGGalaxyMultiplayerProtocol/2";

        private readonly HttpClient _http = new() { Timeout = TimeSpan.FromMinutes(5) };
        private string _token;
        private string _userId;
        private DateTimeOffset _expires;
        private HashSet<string> _objects = new();

        // The access token lasts about an hour, after that signing in is needed again
        public bool IsSignedIn => _token != null && DateTimeOffset.UtcNow < _expires - TimeSpan.FromMinutes(1);

        /// <summary>
        /// OAuth device flow: the user approves in the browser, so the password never reaches this app.
        /// </summary>
        public async Task SignInAsync(Action<DeviceCode> showCode, CancellationToken ct)
        {
            using var startReq = new HttpRequestMessage(HttpMethod.Post, $"{AuthBase}/auth/device")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["client_id"] = ClientId })
            };
            startReq.Headers.UserAgent.ParseAdd("CyberCAT-CloudSync");
            using var startResp = await _http.SendAsync(startReq, ct);
            startResp.EnsureSuccessStatusCode();
            using var start = JsonDocument.Parse(await startResp.Content.ReadAsStringAsync(ct));
            var root = start.RootElement;
            var deviceCode = root.GetProperty("device_code").GetString();
            var interval = Math.Max(1, root.TryGetProperty("interval", out var iv) ? iv.GetInt32() : 5);
            var expires = root.TryGetProperty("expires_in", out var ex) ? ex.GetInt32() : 600;
            showCode(new DeviceCode(root.GetProperty("user_code").GetString(), root.GetProperty("verification_uri_complete").GetString()));

            for (var waited = 0; waited < expires; waited += interval)
            {
                await Task.Delay(TimeSpan.FromSeconds(interval), ct);
                using var tokenResp = await _http.PostAsync($"{AuthBase}/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code",
                    ["device_code"] = deviceCode,
                    ["client_id"] = ClientId,
                }), ct);
                using var body = JsonDocument.Parse(await tokenResp.Content.ReadAsStringAsync(ct));
                if (tokenResp.IsSuccessStatusCode)
                {
                    _token = body.RootElement.GetProperty("access_token").GetString();
                    _userId = ReadSubject(_token);
                    _expires = ReadExpiry(_token);
                    return;
                }

                var error = body.RootElement.TryGetProperty("error", out var e) ? e.GetString() : "unknown";
                if (error == "slow_down")
                {
                    interval += 5;
                }
                else if (error != "authorization_pending")
                {
                    throw new InvalidOperationException($"Sign-in was rejected ({error}).");
                }
            }

            throw new TimeoutException("Sign-in was not approved in time.");
        }

        public async Task<List<CloudSlot>> ListSlotsAsync(CancellationToken ct)
        {
            var (bytes, _) = await GetAsync("?format=json", ct);
            using var doc = JsonDocument.Parse(Gunzip(bytes));
            var slots = new List<CloudSlot>();
            _objects = new HashSet<string>();
            foreach (var e in doc.RootElement.EnumerateArray())
            {
                var name = e.GetProperty("name").GetString() ?? "";
                _objects.Add(name);
                if (name.StartsWith("saves/") && name.EndsWith("/sav.dat"))
                {
                    slots.Add(new CloudSlot(
                        name["saves/".Length..^"/sav.dat".Length],
                        e.GetProperty("bytes").GetInt64(),
                        e.TryGetProperty("last_modified", out var lm) ? lm.GetString() ?? "" : ""));
                }
            }

            return slots.OrderByDescending(s => s.LastModified, StringComparer.Ordinal).ToList();
        }

        /// <summary>Downloads a slot into <paramref name="directory"/> (sav.dat, metadata.9.json, screenshot.png).</summary>
        public async Task<byte[]> DownloadSlotAsync(string slot, string directory, CancellationToken ct)
        {
            Directory.CreateDirectory(directory);
            var (sav, _) = await GetAsync($"/saves/{slot}/sav.dat", ct);
            var raw = Gunzip(sav);
            await File.WriteAllBytesAsync(Path.Combine(directory, "sav.dat"), raw, ct);

            // The editor can open a save without its metadata, so a missing metadata file is not fatal here
            try
            {
                var (meta, _) = await GetAsync($"/saves/{slot}/{MetadataName(slot)}", ct);
                await File.WriteAllBytesAsync(Path.Combine(directory, "metadata.9.json"), Gunzip(meta), ct);
            }
            catch (HttpRequestException)
            {
                var file = Path.Combine(directory, "metadata.9.json");
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            try
            {
                var (shot, _) = await GetAsync($"/saves/{slot}/screenshot.png", ct);
                await File.WriteAllBytesAsync(Path.Combine(directory, "screenshot.png"), shot, ct);
            }
            catch (HttpRequestException)
            {
                // The screenshot is optional
            }

            return raw;
        }

        /// <summary>
        /// Replaces sav.dat of a slot. The current cloud version is saved to <paramref name="backupDirectory"/> first,
        /// and the upload is read back and compared afterwards.
        /// </summary>
        public async Task UploadSlotAsync(string slot, byte[] newSave, byte[] fallbackMetadata, string backupDirectory, CancellationToken ct)
        {
            var (oldSav, savHeaders) = await GetAsync($"/saves/{slot}/sav.dat", ct);
            // The metadata normally comes from the cloud. If the slot lost it, fall back to the one of the open save
            string oldMetaText = null;
            HttpResponseHeaders metaHeaders = savHeaders;
            var oldRaw = Gunzip(oldSav);
            try
            {
                var (oldMeta, headers) = await GetAsync($"/saves/{slot}/{MetadataName(slot)}", ct);
                oldMetaText = Encoding.UTF8.GetString(Gunzip(oldMeta));
                metaHeaders = headers;
            }
            catch (HttpRequestException ex)
            {
                if (fallbackMetadata == null)
                {
                    var names = string.Join(", ", _objects.Where(o => o.StartsWith($"saves/{slot}/")).Select(o => o[$"saves/{slot}/".Length..]));
                    throw new InvalidOperationException($"The cloud slot has no readable metadata file ({ex.Message}) and the open save has none either. Files in this slot: {names}. Nothing was uploaded.");
                }
            }

            Directory.CreateDirectory(backupDirectory);
            await File.WriteAllBytesAsync(Path.Combine(backupDirectory, "sav.dat"), oldRaw, ct);
            if (oldMetaText != null)
            {
                await File.WriteAllTextAsync(Path.Combine(backupDirectory, "metadata.9.json"), oldMetaText, ct);
            }

            // The metadata records the size of sav.dat, keep it in step with the new file
            var baseMetaText = oldMetaText ?? Encoding.UTF8.GetString(fallbackMetadata);
            var newMetaText = System.Text.RegularExpressions.Regex.Replace(baseMetaText, @"""fileSize""\s*:\s*\d+", $"\"fileSize\": {newSave.Length}", System.Text.RegularExpressions.RegexOptions.None, TimeSpan.FromSeconds(2));

            await PutAsync($"/saves/{slot}/sav.dat", newSave, savHeaders, ct);
            await PutAsync($"/saves/{slot}/{MetadataName(slot)}", Encoding.UTF8.GetBytes(newMetaText), metaHeaders, ct);

            var (check, _) = await GetAsync($"/saves/{slot}/sav.dat", ct);
            if (!Gunzip(check).AsSpan().SequenceEqual(newSave))
            {
                throw new InvalidDataException("The upload could not be verified: the cloud copy differs from the uploaded save.");
            }
        }

        /// <summary>The metadata file of a slot, as the cloud lists it (the name carries a format version).</summary>
        private string MetadataName(string slot)
        {
            var preferred = $"saves/{slot}/metadata.9.json";
            if (_objects.Contains(preferred) || _objects.Count == 0)
            {
                return "metadata.9.json";
            }

            var found = _objects.FirstOrDefault(o => o.StartsWith($"saves/{slot}/metadata.") && o.EndsWith(".json"));
            return found == null ? "metadata.9.json" : found[$"saves/{slot}/".Length..];
        }

        private string Url(string rest) => $"{SavesBase}/{_userId}/{ClientId}{rest}";

        private async Task<(byte[] Body, HttpResponseHeaders Headers)> GetAsync(string rest, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, Url(rest));
            Prepare(req);
            using var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{(int)resp.StatusCode} {resp.ReasonPhrase} for {rest.Split('?')[0]}");
            }

            return (await resp.Content.ReadAsByteArrayAsync(ct), resp.Headers);
        }

        private async Task PutAsync(string rest, byte[] raw, HttpResponseHeaders objectMeta, CancellationToken ct)
        {
            using var ms = new MemoryStream();
            using (var gz = new GZipStream(ms, CompressionLevel.Optimal, true))
            {
                gz.Write(raw);
            }

            using var req = new HttpRequestMessage(HttpMethod.Put, Url(rest)) { Content = new ByteArrayContent(ms.ToArray()) };
            Prepare(req);
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            req.Content.Headers.ContentEncoding.Add("gzip");
            foreach (var h in objectMeta.Where(h => h.Key.StartsWith("x-object-meta-", StringComparison.OrdinalIgnoreCase)))
            {
                req.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }

            using var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{(int)resp.StatusCode} {resp.ReasonPhrase} for {rest}");
            }
        }

        private void Prepare(HttpRequestMessage req)
        {
            if (_token == null)
            {
                throw new InvalidOperationException("Not signed in.");
            }

            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            req.Headers.TryAddWithoutValidation("User-Agent", SdkUserAgent);
            req.Headers.TryAddWithoutValidation("Accept-Encoding", "identity, gzip, deflate");
        }

        private static DateTimeOffset ReadExpiry(string jwt)
        {
            var payload = jwt.Split('.')[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            using var doc = JsonDocument.Parse(Convert.FromBase64String(payload));
            return DateTimeOffset.FromUnixTimeSeconds(doc.RootElement.GetProperty("exp").GetInt64());
        }

        private static string ReadSubject(string jwt)
        {
            var payload = jwt.Split('.')[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            using var doc = JsonDocument.Parse(Convert.FromBase64String(payload));
            return doc.RootElement.GetProperty("sub").GetString();
        }

        private static byte[] Gunzip(byte[] data)
        {
            if (data.Length < 2 || data[0] != 0x1f || data[1] != 0x8b)
            {
                return data;
            }

            using var input = new MemoryStream(data);
            using var gz = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gz.CopyTo(output);
            return output.ToArray();
        }

        public void Dispose()
        {
            _token = null;
            _http.Dispose();
        }
    }
}
