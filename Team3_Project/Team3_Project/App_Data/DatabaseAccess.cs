//TABLES:
//user: used for authentication
//title_basics:
public class DatabaseAccess {
	private readonly System.String conn = ""; //read only
	private readonly System.String conn2 = ""; //writable
	private void conn2_warning() {
		System.Console.WriteLine(this.conn2);
	}
	public System.Data.DataSet GetDataSet() //Example function to run a "GET" stored procedure
	{
		try {
			MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(this.conn2); //Make a new connection using our connection string
			System.Data.DataSet ds = new System.Data.DataSet();
			MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM user limit 1000;" , connection);
			//MySqlCommand cmd = new MySqlCommand("<stored procedure name>", connection);
			//cmd.CommandType = CommandType.StoredProcedure; //This tells it we are using a stored procedure instead of passing in a query of our own
			//This is an alternative that would accomplish the same thing as the lines above
			//cmd.CommandType = CommandType.StoredProcedure
			//cmd.Connection = connection
			//cmd.CommandText = "<stored procedure name>"
			//If the commandType is not specified as a stored procedure you can write a query in the CommandText field
			MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter(cmd);
			try {
				connection.Open();
				da.Fill(ds); //Whatever the stored procedure queries is dumped into our dataset
			}
			catch (System.Exception ex) {
				throw new System.ArgumentException(ex.Message);
			}
			finally {
				connection.Close(); //Make sure we always close the database connection when done with it
			}
			return ds;
		}
		catch (System.Exception ex) {
			throw new System.ArgumentException(ex.Message);
			//MessageBox.Show(ex.Message);
		}
	}
	public System.Data.DataSet InsertDataRow(System.String s , System.Int32 i)   //Example function that would insert a row into the table with values of s and i
	{
		try {
			MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(this.conn);     //Make a new connection using our connection string
			System.Data.DataSet ds = new System.Data.DataSet();
			MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("<stored procedure name>" , connection) {
				CommandType = System.Data.CommandType.StoredProcedure  //This tells it we are using a stored procedure instead of passing in a query of our own
			};
			cmd.Parameters.Add("@name" , MySql.Data.MySqlClient.MySqlDbType.VarChar , 100);  //Tells it we are passing in a parameter to the @name variable in the stored procedure
			cmd.Parameters[cmd.Parameters.Count - 1].Direction = System.Data.ParameterDirection.Input;  //Tells it that this is a value we are sending to the stored procedure
			cmd.Parameters[cmd.Parameters.Count - 1].Value = s;     //Tells it what value we are sending it
			cmd.Parameters.Add("@id" , MySql.Data.MySqlClient.MySqlDbType.Int32);
			cmd.Parameters[cmd.Parameters.Count - 1].Direction = System.Data.ParameterDirection.Input;
			cmd.Parameters[cmd.Parameters.Count - 1].Value = i;
			try {
				connection.Open();
				cmd.ExecuteNonQuery(); //Tells it we are not expecting the results of a query back
			}
			catch (System.Exception ex) {
				throw new System.ArgumentException(ex.Message);
			}
			finally {
				connection.Close();
			}
			return ds;
		}
		catch (System.Exception ex) {
			throw new System.ArgumentException(ex.Message);
			//MessageBox.Show(ex.Message);
		}
	}
	//Hash a string and return the result
	public System.String getHash(System.String s) {
		System.Security.Cryptography.HashAlgorithm algo = System.Security.Cryptography.SHA256.Create();
		System.Byte[] hash = algo.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s));
		return System.Convert.ToBase64String(hash);
	}
	//Insert a new user account with the username, email, and will hash the password and insert it
	public void insertAccount(System.String username , System.String email , System.String password) {
		try {
			MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(this.conn2); //Make a new connection using our connection string
			MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("INSERT INTO user (name, password, email) VALUES ('" + username + "', '"
				+ this.getHash(password) + "', '" + email + "');" , connection);
			try {
				connection.Open();
				cmd.ExecuteNonQuery();
			}
			catch (System.Exception ex) {
				throw new System.ArgumentException(ex.Message);
			}
			finally {
				connection.Close(); //Make sure we always close the database connection when done with it
			}
		}
		catch (System.Exception ex) {
			throw new System.ArgumentException(ex.Message);
		}
	}
	//This will return true if the password matches the email, false if it doesnt, and null if the email does not exist in the database
	public System.Nullable<System.Boolean> comparePass(System.String email , System.String password) {
		System.String hashedPass = "";
		try {
			MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(this.conn2); //Make a new connection using our connection string
			MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT password FROM user WHERE email = @email" , connection);
			cmd.Parameters.AddWithValue("@email" , email);
			try {
				connection.Open();
				MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read()) {
					hashedPass = reader.GetString(0);
				}
				reader.Close();
			}
			catch (System.Exception ex) {
				throw new System.ArgumentException(ex.Message);
			}
			finally {
				connection.Close(); //Make sure we always close the database connection when done with it
			}
		}
		catch (System.Exception ex) {
			throw new System.ArgumentException(ex.Message);
		}
		return hashedPass == "" ? null : (System.Boolean?) hashedPass.Equals(this.getHash(password));
	}
}