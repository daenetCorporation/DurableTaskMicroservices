//  ----------------------------------------------------------------------------------
//  Copyright daenet Gesellschaft für Informationstechnologie mbH
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Globalization;

namespace Daenet.DurableTaskMicroservices.Common.Extensions
{
    public static class DataReaderExtensions
    {
        #region Public Methods
        /// <summary>
        /// Gets a value of an data reader.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static TResult GetValue<TResult>(this IDataReader reader, string column)
        {
            if (reader.IsDBNull(column) == true)
            {
                return default(TResult);
            }
            else
            {
                int pos = reader.GetOrdinal(column);

                if (typeof(TResult).IsGenericType &&
                    typeof(TResult).GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type innerType = typeof(TResult).GetGenericArguments()[0];
                    object obj = null;

                    if (innerType.IsEnum)
                    {
                        obj = Enum.Parse(innerType, reader.GetValue(pos).ToString());
                    }
                    else
                    {
                        obj = Convert.ChangeType(reader.GetValue(pos), innerType, CultureInfo.InvariantCulture);

                    }
                    NullableConverter e = new NullableConverter(typeof(TResult));
                    return (TResult)e.ConvertFrom(obj);
                }
                else if (typeof(TResult).IsEnum)
                {
                    return (TResult)Enum.Parse(typeof(TResult), reader.GetValue(pos).ToString());
                }
                //else
                return (TResult)Convert.ChangeType(reader.GetValue(pos), typeof(TResult), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets a value of an data reader.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></columnNo>
        public static TResult GetValue<TResult>(this IDataReader reader, int columnNo)
        {
            if (reader.IsDBNull(columnNo))
                return default(TResult);
            else
            {
                if (typeof(TResult).IsGenericType &&
                    typeof(TResult).GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type innerType = typeof(TResult).GetGenericArguments()[0];
                    object obj = Convert.ChangeType(reader.GetValue(columnNo), innerType, CultureInfo.InvariantCulture);
                    NullableConverter e = new NullableConverter(typeof(TResult));
                    return (TResult)e.ConvertFrom(obj);


                }
                //else
                return (TResult)Convert.ChangeType(reader.GetValue(columnNo), typeof(TResult), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets a value of an data reader.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static DateTime? GetUtcDateTime(this IDataReader reader, string column)
        {
            int pos = reader.GetOrdinal(column);

            if (reader.IsDBNull(pos))
                return null;
            else
            {
                return new DateTime(((DateTime)reader.GetDateTime(pos)).Ticks, DateTimeKind.Utc);
            }
        }



        public static bool IsDBNull(this IDataReader reader, string column)
        {
            if (reader.HasColumn(column))
            {
                int pos = reader.GetOrdinal(column);
                return reader.IsDBNull(pos);
            }
            else
            {
                return true;
            }
        }

        public static bool HasColumn(this IDataReader dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
        #endregion

    }
}
