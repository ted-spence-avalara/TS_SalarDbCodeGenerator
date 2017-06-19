using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ====================================
// SalarDbCodeGenerator
// http://SalarDbCodeGenerator.codeplex.com
// Salar Khalilzadeh <salar2k@gmail.com>
// © 2012, All rights reserved
// 2012/07/06
// ====================================
namespace SalarDbCodeGenerator.Schema.Database
{
	/// <summary>
	/// An index that can have one or many columns
	/// </summary>
	public class DbIndex
	{
        /// <summary>
        /// Name of Index
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Name of ForeignKey
        /// </summary>
        public List<DbConstraintKey> Keys { get; set; }

        /// <summary>
        /// True if this is a primary key
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Is constraint key
        /// </summary>
        public bool IsUnique { get; set; }

        public override string ToString()
		{
            StringBuilder sb = new StringBuilder(IndexName);
            sb.Append(": ");
            foreach (var key in Keys) {
                sb.Append(key.ToString());
                sb.Append(", ");
            }
            if (Keys.Count > 0) {
                sb.Length -= 2;
            }
            return sb.ToString();
		}

        private static List<string> _existingKeyNames = new List<string>();
        private string _indexKeyName = null;
        public string GetIndexKeyName(DbTable table)
        {
            if (_indexKeyName == null) {

                // Construct friendly key name
                StringBuilder sb = new StringBuilder();
                foreach (var key in Keys) {
                    sb.Append(key.KeyColumn.FieldNameSchema);
                }
                _indexKeyName = sb.ToString();

                // See if we need to uniqueify it
                int id = 1;
                if (_existingKeyNames.Contains(table.TableName + "|" + _indexKeyName)) {
                    id++;
                    _indexKeyName = sb.ToString() + id.ToString();
                }
                _existingKeyNames.Add(table.TableName + "|" + _indexKeyName);
            }
            return _indexKeyName;
        }

        public string GetParameterList()
        {
            StringBuilder sb = new StringBuilder();
            bool first_column = true;
            foreach (var key in Keys) {

                // First value is not nullable
                if (first_column) {
                    sb.Append(key.KeyColumn.DataTypeDotNet);
                } else {
                    sb.Append(key.KeyColumn.DataTypeDotNet);
                    if (key.KeyColumn.DataTypeDotNet != "String") {
                        sb.Append("?");
                    }
                }
                sb.Append(" ");
                sb.Append(key.KeyColumn.FieldNameSchema);
                sb.Append(", ");
                first_column = false;
            }
            if (sb.Length > 0) sb.Length -= 2;
            return sb.ToString();
        }

        public string GetParameterAssignments()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in Keys) {
                sb.Append(key.KeyColumn.FieldNameSchema);
                sb.Append(" = ");
                sb.Append(key.KeyColumn.FieldNameSchema);
                sb.Append(", ");
            }
            if (sb.Length > 0) sb.Length -= 2;
            return sb.ToString();
        }

        public string GetMatchClause()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in Keys) {
                sb.Append("@");
                sb.Append(key.KeyColumn.FieldNameSchema);
                sb.Append(" = ");
                sb.Append(key.KeyColumn.FieldNameSchema);
                sb.Append(" AND ");
            }
            if (sb.Length > 0) sb.Length -= 5;
            return sb.ToString();
        }
    }
}
