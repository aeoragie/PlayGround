using System.Xml.Linq;

namespace Crawler.KFA
{
    /// <summary>
    /// Nexacro 플랫폼 XML 요청 생성 및 응답 파싱
    /// </summary>
    public static class NexacroHelper
    {
        private static readonly XNamespace Ns = "http://www.nexacroplatform.com/platform/dataset";

        /// <summary>
        /// 경기 상세 조회 요청 XML 생성
        /// </summary>
        public static string BuildMatchDetailRequest(
            string matchIdx, string singleIdx, string userId, string secret)
        {
            var stateValue = $"secret%3D{secret}%26returnUrl%3Dhttps%3A%2F%2Fwww.joinkfa.com";

            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(Ns + "Root",
                    new XElement(Ns + "Parameters",
                        NewParam("state", stateValue),
                        NewParam("GP_EMPL_ID", userId),
                        NewParam("GP_SYS_CD", "USER"),
                        NewParam("GP_MENU_ID", "WorkFrame"),
                        NewParam("GP_SVC_PATH", "/generate/MAP_04_001/SEARCH00.do"),
                        NewParam("GP_SERVICE_ID", "SEARCH00"),
                        NewParam("GP_AUTH_GROUP", "GE"),
                        NewParam("GP_LOG_YN", "")),
                    new XElement(Ns + "Dataset",
                        new XAttribute("id", "dsReqParam"),
                        new XElement(Ns + "ColumnInfo",
                            NewColumn("v_MATCH_IDX"),
                            NewColumn("v_SINGLE_IDX"),
                            NewColumn("v_USER_ID")),
                        new XElement(Ns + "Rows",
                            new XElement(Ns + "Row",
                                NewCol("v_MATCH_IDX", matchIdx),
                                NewCol("v_SINGLE_IDX", singleIdx),
                                NewCol("v_USER_ID", userId))))));

            return doc.Declaration + doc.ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Nexacro XML 응답 파싱 → Dataset별 Row 목록
        /// </summary>
        public static Dictionary<string, List<Dictionary<string, string>>> ParseResponse(string xml)
        {
            var result = new Dictionary<string, List<Dictionary<string, string>>>();
            var doc = XDocument.Parse(xml);
            var root = doc.Root;
            if (root == null)
            {
                return result;
            }

            // ErrorCode 확인
            var errorCode = root.Descendants(Ns + "Parameter")
                .FirstOrDefault(p => p.Attribute("id")?.Value == "ErrorCode")?.Value;
            if (errorCode != null && errorCode != "0")
            {
                var errorMsg = root.Descendants(Ns + "Parameter")
                    .FirstOrDefault(p => p.Attribute("id")?.Value == "ErrorMsg")?.Value ?? "";
                Console.Error.WriteLine($"[NEXACRO ERROR] Code={errorCode}, Msg={errorMsg}");
                return result;
            }

            foreach (var dataset in root.Elements(Ns + "Dataset"))
            {
                var datasetId = dataset.Attribute("id")?.Value ?? "";
                var rows = new List<Dictionary<string, string>>();

                foreach (var row in dataset.Element(Ns + "Rows")?.Elements(Ns + "Row") ?? [])
                {
                    var rowData = new Dictionary<string, string>();
                    foreach (var col in row.Elements(Ns + "Col"))
                    {
                        var colId = col.Attribute("id")?.Value ?? "";
                        rowData[colId] = col.Value;
                    }
                    rows.Add(rowData);
                }

                result[datasetId] = rows;
            }

            return result;
        }

        /// <summary>
        /// 특정 Dataset의 첫 번째 Row 반환
        /// </summary>
        public static Dictionary<string, string>? GetFirstRow(
            Dictionary<string, List<Dictionary<string, string>>> datasets, string datasetId)
        {
            if (datasets.TryGetValue(datasetId, out var rows) && rows.Count > 0)
            {
                return rows[0];
            }
            return null;
        }

        /// <summary>
        /// 특정 Dataset의 모든 Row 반환
        /// </summary>
        public static List<Dictionary<string, string>> GetRows(
            Dictionary<string, List<Dictionary<string, string>>> datasets, string datasetId)
        {
            return datasets.TryGetValue(datasetId, out var rows) ? rows : [];
        }

        /// <summary>
        /// Row에서 안전하게 값 추출
        /// </summary>
        public static string Get(Dictionary<string, string> row, string key)
        {
            return row.TryGetValue(key, out var value) ? value : "";
        }

        #region XML Element Builders

        private static XElement NewParam(string id, string value)
        {
            return new XElement(Ns + "Parameter", new XAttribute("id", id), value);
        }

        private static XElement NewColumn(string id)
        {
            return new XElement(Ns + "Column",
                new XAttribute("id", id),
                new XAttribute("type", "STRING"),
                new XAttribute("size", "256"));
        }

        private static XElement NewCol(string id, string value)
        {
            return new XElement(Ns + "Col", new XAttribute("id", id), value);
        }

        #endregion
    }
}
