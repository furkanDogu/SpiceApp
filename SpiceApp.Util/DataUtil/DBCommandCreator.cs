using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Util.DataUtil
{
    public class DBCommandCreator
    {
        public static void AddParameter(DbCommand command, string paramName, DbType dbDataType, ParameterDirection direction, object value)
        {
            // This utility func. helps adding parameters to CRUD ops.
            /*
                PARAMETERS: 
                -command => command to add parameters
                -paramName => 
                -DbType => type of the parameter
                -value => parameter value

                OUTPUT:
                -constructed query which is type of DbCommand
             
             */
            if (command == null)
                throw new ArgumentNullException("command", "The AddParameter's Command value is null.");

            try
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = paramName;
                parameter.DbType = dbDataType;
                parameter.Direction = direction;
                parameter.Value = value;
                command.Parameters.Add(parameter);
            }
            catch (Exception ex)
            {
                throw new Exception("DBCommandCreator::AddParameter::Error occured.", ex);
            }
        }

        public static string SELECT(string[] Fields, DBTableNames Target, string condition)
        {   // This utility func. helps building select queries.
            /*
                PARAMETERS: 
                -Fields => Name of the fields to be fetched from corresponding table
                -Target => Enum type of required table
                -condition => Any condition to be looked for in query

                OUTPUT:
                -constructed SELECT query
             
             */
            var query = new StringBuilder();
            query.Append("SELECT ");
            int counter = 0;
            foreach (string field in Fields)
            {
                query.Append(field);
                if (!(counter + 1 == Fields.Length))
                {
                    query.Append(", ");
                }
                counter++;
            }

            query.Append(" FROM ");
            query.Append(DBTableConverter.Tables[Target]);
            if (condition.Length != 0)
            {
                query.Append(" " + condition);
            }

            return query.ToString();
        }

        public static string INSERT(string[] Fields, DBTableNames Target)
        {
            // specifying which fields to insert
            var query = new StringBuilder();
            query.Append("INSERT " + DBTableConverter.Tables[Target] + " ( ");
            int counter = 0;
            foreach (string field in Fields)
            {
                query.Append(field);
                if (!(counter + 1 == Fields.Length))
                {
                    query.Append(", ");
                }
                counter++;
            }

            //concating paramater names to the query
            counter = 0;
            query.Append(" ) VALUES ( ");
            foreach (string field in Fields)
            {
                query.Append("@" + field);
                if (!(counter + 1 == Fields.Length))
                {
                    query.Append(", ");
                }
                counter++;
            }
            query.Append(" )");

            return query.ToString();
        }
        public static string EXEC(string[] Fields, string SpName)
        {
            var query = new StringBuilder();
            query.Append("EXEC "+SpName+" ");
            int counter = 0;
            foreach(string field in Fields)
            {
                query.Append("@" + field);
                if (!(counter + 1 == Fields.Length))
                {
                    query.Append(", ");
                }
                counter++;
            }
            return query.ToString();
        }

    }
}
