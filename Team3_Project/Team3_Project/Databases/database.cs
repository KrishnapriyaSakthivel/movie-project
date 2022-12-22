namespace Team3_Project.Databases {
	public abstract class database : superclass {
		private static System.String log = System.String.Empty;
		private static System.String error = System.String.Empty;
		private static readonly System.String connection_string;
		static database() {
			MySql.Data.MySqlClient.MySqlConnectionStringBuilder MySqlConnectionStringBuilder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder {
				Password = "" ,
				Server = "" ,
				SslMode = MySql.Data.MySqlClient.MySqlSslMode.None ,
				UserID = ""
			};
			connection_string = MySqlConnectionStringBuilder.ToString();
		}
		private readonly System.String schema;
		private readonly System.String table;
		protected database(System.String schema , System.String table) {
			this.schema = schema;
			this.table = table;
		}
		private static System.Data.DataTable run(System.String query) {
			log = query;
			System.Data.DataSet DataSet = new System.Data.DataSet();
			using (MySql.Data.MySqlClient.MySqlConnection MySqlConnection = new MySql.Data.MySqlClient.MySqlConnection(connection_string)) {
				using (MySql.Data.MySqlClient.MySqlCommand MySqlCommand = new MySql.Data.MySqlClient.MySqlCommand(query ?? System.String.Empty , MySqlConnection)) {
					using (MySql.Data.MySqlClient.MySqlDataAdapter MySqlDataAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(MySqlCommand)) {
						try {
							MySqlDataAdapter.Fill(DataSet);
						}
						catch (MySql.Data.MySqlClient.MySqlException MySqlException) {
							error = MySqlException.Message;
						}
					}
				}
			}
			return DataSet.Tables.Count > 0 ? DataSet.Tables[0] : new System.Data.DataTable();
		}
		protected abstract System.String[] columns();
		protected abstract type.abstraction[] values();
		protected System.String CALL(type.abstraction[] parameters) {
			System.Text.StringBuilder query = new System.Text.StringBuilder(255);
			query.Append(keyword.CALL);
			query.Append(unicode.SPACE);
			query.Append(this.TABLE());
			query.Append(unicode.SPACE);
			query.Append(unicode.LEFT_PARENTHESIS);
			query.Append(this.value_array(parameters));
			query.Append(unicode.RIGHT_PARENTHESIS);
			query.Append(unicode.SEMICOLON);
			return query.ToString();
		}
		protected System.String COLUMN(System.String[] columns) {
			System.String table = this.TABLE();
			System.Int32 index = 0;
			System.Int32 count = columns.Length;
			if (count > 0) {
				System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
				do {
					StringBuilder.Append(this.QUALIFY_IDENTIFIER(table , this.QUOTE_IDENTIFIER(columns[index])));
					index += 1;
					if (index >= count) {
						break;
					}
					StringBuilder.Append(keyword.COMMA_SEPARATED);
				}
				while (true);
				return StringBuilder.ToString();
			}
			return System.String.Empty;
		}

		protected System.String DELETE(System.String where = "") {
			System.Text.StringBuilder query = new System.Text.StringBuilder(255);
			query.Append(keyword.DELETE);
			query.Append(unicode.SPACE);
			query.Append(keyword.FROM);
			query.Append(unicode.SPACE);
			query.Append(this.TABLE());
			query.Append(this.WHERE(where));
			query.Append(unicode.SEMICOLON);
			return query.ToString();
		}

		protected System.String INSERT() {
			System.Text.StringBuilder query = new System.Text.StringBuilder(255);
			query.Append(keyword.INSERT);
			query.Append(unicode.SPACE);
			query.Append(keyword.INTO);
			query.Append(unicode.SPACE);
			query.Append(this.TABLE());
			query.Append(unicode.SPACE);
			query.Append(unicode.LEFT_PARENTHESIS);
			query.Append(this.COLUMN(this.columns()));
			query.Append(unicode.RIGHT_PARENTHESIS);
			query.Append(unicode.SPACE);
			query.Append(keyword.VALUES);
			query.Append(unicode.SPACE);
			query.Append(unicode.LEFT_PARENTHESIS);
			query.Append(this.VALUES());
			query.Append(unicode.RIGHT_PARENTHESIS);
			query.Append(unicode.SEMICOLON);
			return query.ToString();
		}
		protected System.String LIMIT(System.UInt32? limit = null) {
			if (limit != null) {
				System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
				StringBuilder.Append(unicode.SPACE);
				StringBuilder.Append(keyword.LIMIT);
				StringBuilder.Append(unicode.SPACE);
				StringBuilder.Append(limit);
				return StringBuilder.ToString();
			}
			return System.String.Empty;
		}
		protected System.String QUALIFY_IDENTIFIER(System.String QUALIFIER , System.String IDENTIFIER) {
			return System.String.Concat(QUALIFIER , unicode.FULL_STOP , IDENTIFIER);
		}
		protected System.String QUOTE_IDENTIFIER(System.String IDENTIFIER) {
			return System.String.Concat(unicode.GRAVE_ACCENT , IDENTIFIER , unicode.GRAVE_ACCENT);
		}
		protected System.String SCHEMA() {
			System.String schema = this.schema;
			return this.QUOTE_IDENTIFIER(schema);
		}
		protected System.String SELECT(System.String where = "" , System.UInt32? limit = null) {
			System.Text.StringBuilder query = new System.Text.StringBuilder(255);
			query.Append(keyword.SELECT);
			query.Append(unicode.SPACE);
			query.Append(this.COLUMN(this.columns()));
			query.Append(unicode.SPACE);
			query.Append(keyword.FROM);
			query.Append(unicode.SPACE);
			query.Append(this.TABLE());
			query.Append(this.WHERE(where));
			query.Append(this.LIMIT(limit));
			query.Append(unicode.SEMICOLON);
			return query.ToString();
		}
		protected System.String STRING_LITERAL(System.String STRING) {
			return System.String.Concat(unicode.APOSTROPHE , STRING , unicode.APOSTROPHE);
		}
		protected System.String TABLE() {
			System.String schema = this.SCHEMA();
			System.String table = this.table;
			return this.QUALIFY_IDENTIFIER(schema , this.QUOTE_IDENTIFIER(table));
		}
		protected System.String VALUES() {
			return this.value_array(this.values());
		}
		protected System.String WHERE(System.String where = "") {
			if (!System.String.IsNullOrEmpty(where)) {
				System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
				StringBuilder.Append(unicode.SPACE);
				StringBuilder.Append(keyword.WHERE);
				StringBuilder.Append(unicode.SPACE);
				StringBuilder.Append(where);
				return StringBuilder.ToString();
			}
			return System.String.Empty;
		}
		protected System.String value_array(type.abstraction[] array) {
			System.Int32 index = 0;
			System.Int32 count = array.Length;
			if (count > 0) {
				System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
				do {
					StringBuilder.Append(this.STRING_LITERAL(array[index].ToString()));
					index += 1;
					if (index >= count) {
						break;
					}
					StringBuilder.Append(keyword.COMMA_SEPARATED);
				}
				while (true);
				return StringBuilder.ToString();
			}
			return System.String.Empty;
		}
		protected static System.Collections.Generic.List<superclass> delete(constructor constructor , System.Object[] initialize) {
			System.String query;
			try {
				query = (System.String) initialize[0];
			}
			catch {
				throw new System.ArgumentException();
			}
			run(query);
			return new System.Collections.Generic.List<superclass>();
		}
		protected static System.Collections.Generic.List<superclass> insert(constructor constructor , System.Object[] initialize) {
			System.Collections.Generic.List<superclass> list = new System.Collections.Generic.List<superclass>();
			try {
				foreach (System.Object Object in initialize) {
					database database = (database) Object;
					list.Add(database);
				}
			}
			catch {
				throw new System.ArgumentException();
			}
			foreach (database item in list) {
				System.String query = item.INSERT();
				run(query);
			}
			return list;
		}
		protected static System.Collections.Generic.List<superclass> select(constructor constructor , System.Object[] initialize) {
			System.String query;
			try {
				query = (System.String) initialize[0];
			}
			catch {
				throw new System.ArgumentException();
			}
			System.Collections.Generic.List<superclass> list = new System.Collections.Generic.List<superclass>();
			System.Data.DataTable DataTable = run(query);
			foreach (System.Data.DataRow DataRow in DataTable.Rows) {
				database item = (database) constructor.Invoke();
				type.abstraction[] values = item.values();
				System.Int32 index = values.Length;
				while (index > 0) {
					index -= 1;
					System.Object Object = DataRow[index];
					values[index].cast(Object);
				}
				list.Add(item);
			}
			return list;
		}
	}
}