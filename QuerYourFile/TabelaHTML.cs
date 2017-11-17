﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace QuerYourFile {
    public class TabelaHTML {

        public string ExportaTabelaHtml(DataTable dataTable) {
            if (dataTable != null) {
                StringBuilder html = new StringBuilder();
                html.Append("<div class='container'>");
                html.Append("<table>");
                foreach (DataColumn column in dataTable.Columns) {
                    html.Append("<th>");
                    html.Append(column.ColumnName);
                    html.Append("</th>");
                }

                html.Append("</tr></thead><tbody>");
                foreach (DataRow row in dataTable.Rows) {
                    html.Append("<tr>");
                    foreach (DataColumn column in dataTable.Columns) {
                        html.Append("<td>");
                        html.Append(row[column.ColumnName]);
                        html.Append("</td>");
                    }
                    html.Append("</tr>");
                }

                html.Append("</tbody></div></table></body></html>");
                return html.ToString();
            }
            else {
                return "";
            }
        }
    }
}