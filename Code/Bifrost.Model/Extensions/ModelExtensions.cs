using System;
using System.Collections.Generic;
using System.Linq;

namespace Bifrost.Model.Extensions
{
    public static class ModelExtensions
    {
        private const string DateFormat = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'fffffffZ";

        public static string ToJson(this IReadOnlyList<DatapointContainer> items)
        {
            var containersArray = new string[items.Count];
            for (int i = items.Count - 1; i >= 0; i--)
            {
                containersArray[i] = $"{ToJson(items[i].Dps)}";
            }

            return $"[{string.Join(",", containersArray)}]";
        }

        public static string ToJson(this IReadOnlyList<Datapoint> datapoints)
        {
            string[] datapointsArray = new string[datapoints.Count];

            for (int i = datapoints.Count - 1; i >= 0; i--)
            {
                datapointsArray[i] = ToJson(datapoints[i]);
            }

            return $"{{\"Dps\":[{string.Join(",", datapoints.Select(ToJson))}]}}";
        }

        public static string ToJson(this Datapoint datapoint)
        {
            return $"{{\"Id\":\"{datapoint.Id}\",\"CustomerId\":\"{datapoint.CustomerId}\",\"ContainerId\":\"{datapoint.ContainerId}\",\"Name\":\"{datapoint.Name}\",\"NumericValue\":{datapoint.NumericValue.ToString("#0'.'0")},\"StringValue\":\"{datapoint.StringValue}\",\"CollectedTimestamp\":\"{datapoint.CollectedTimestamp.ToString(DateFormat)}\",\"ReceivedTimestamp\":\"{datapoint.ReceivedTimestamp.ToString(DateFormat)}\",\"Tags\":[{ToJson(datapoint.Tags)}]}}";
        }

        public static string ToJson(this IReadOnlyList<Tag> tags)
        {
            string[] tagsArray = new string[tags.Count];
            for (int i = tags.Count - 1; i >= 0; i--)
            {
                tagsArray[i] = $"{{\"DatapointId\":\"{tags[i].DatapointId}\",\"CustomerId\":\"{tags[i].CustomerId}\",\"Key\":\"{tags[i].Key}\",\"Value\":\"{tags[i].Value}\",\"CollectedTimestamp\":\"{tags[i].CollectedTimestamp.ToString(DateFormat)}\",\"ReceivedTimestamp\":\"{tags[i].ReceivedTimestamp.ToString(DateFormat)}\"}}";
            }

            return string.Join(",", tagsArray);
        }


        public static IEnumerable<DatapointContainer> UpdateContainers(this IEnumerable<DatapointContainer> containers, Action<Datapoint> datapointAction = null)
        {
            foreach (var container in containers)
            {
                UpdateContainer(container, datapointAction);
            }

            return containers;
        }

        public static void UpdateContainer(this DatapointContainer container, Action<Datapoint> datapointAction = null)
        {
            var receivedTimestamp = DateTime.UtcNow;

            foreach (var datapoint in container.Dps)
            {
                datapoint.ReceivedTimestamp = receivedTimestamp;
                var hostName = "";
                var containerId = Guid.Empty;
                foreach (var tag in datapoint.Tags)
                {
                    tag.DatapointId = datapoint.Id;
                    tag.CustomerId = datapoint.CustomerId;
                    tag.CollectedTimestamp = datapoint.CollectedTimestamp;
                    tag.ReceivedTimestamp = datapoint.ReceivedTimestamp;

                    //this is to be slowly removed.. Bjarni 2016-03-09

                    if (tag.Key.Equals("CONTAINERID", StringComparison.OrdinalIgnoreCase))
                    {
                        containerId = Guid.Parse(tag.Value);
                    }

                    if (tag.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                    {
                        hostName = tag.Value;
                    }
                }
                if (datapoint.HostName == string.Empty)
                    datapoint.HostName = hostName;
                if (datapoint.ContainerId == Guid.Empty)
                {
                    //Log.Add($"CONTAINERID found in datapoint. We cant remote this stuff yet. Update clients for this customer { datapoint.CustomerId}");
                    datapoint.ContainerId = containerId;
                }

                datapointAction?.Invoke(datapoint);
            }
        }

        public const string UNKNOWN_HOST_NAME = "<UNKNOWN>";

        public static string GetHostName(this DatapointContainer datapointContainer)
        {
            if ((datapointContainer?.Dps == null) || (datapointContainer.Dps.Count == 0))
            {
                return string.Empty;
            }

            return GetHostName(datapointContainer.Dps[0]);
        }

        /// <summary>
        /// Returns a non-empty string. (If not found, UNKNOWN_HOST_NAME is returned -- never null or "".)
        /// </summary>
        public static string GetHostName(this Datapoint datapoint)
        {
            if (!string.IsNullOrEmpty(datapoint.HostName))
            {
                return datapoint.HostName;
            }

            Tag hostTag = datapoint.Tags.Find(t => t.Key.Equals("HOST"));
            return !string.IsNullOrEmpty(hostTag?.Value)
                ? hostTag.Value
                : UNKNOWN_HOST_NAME;
        }
    }
}