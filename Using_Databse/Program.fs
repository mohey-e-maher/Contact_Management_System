    open System
    open System.Windows.Forms
    open System.Data
    open MySql.Data.MySqlClient

    [<EntryPoint>]
    let main argv =
        // Main form
        let form = new Form(Text = "Contact Management System", Width = 890, Height = 330)

        // DataGridView to display data
        let dataGridView = new DataGridView(Width=350, Height = 200,Top = 20,Left=500)
        dataGridView.ReadOnly <- true
        dataGridView.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill
        //No5
        // Function to load data into DataGridView
        let loadData () =
            try
                let connectionString = "Server=localhost;Database=tester;User Id=root;Password=;"
                use connection = new MySqlConnection(connectionString)
                connection.Open()

                let query = "SELECT number, name, email FROM data_of_a7"
                use adapter = new MySqlDataAdapter(query, connection)
                let table = new DataTable()
                adapter.Fill(table) |> ignore
                dataGridView.DataSource <- table
            with
            | ex -> MessageBox.Show($"Error loading data: {ex.Message}") |> ignore

        // Refresh button
        let refreshButton = new Button(Text = "Refresh", Top = 230, Left = 620, Width = 100)
        refreshButton.Click.Add(fun _ ->
            loadData() // Reload data after adding
        )

        // Other UI elements
        let nameLabel = new Label(Text = "Name:", AutoSize = true, Top = 110, Left = 20)
        let nameTextBox = new TextBox(Width = 280, Top = 110, Left = 80)

        let numberLabel = new Label(Text = "Number:", AutoSize = true, Top = 150, Left = 20)
        let numberTextBox = new TextBox(Width = 280, Top = 150, Left = 80)

        let emailLabel = new Label(Text = "Email:", AutoSize = true, Top = 190, Left = 20)
        let emailTextBox = new TextBox(Width = 280, Top = 190, Left = 80)
        // No 1
        // Search by number
        let searchNLabel = new Label(Text = "Search by Number:", AutoSize = true, Top = 20, Left = 20)
        let searchNTextBox = new TextBox(Width = 210, Top = 20, Left = 150,MaxLength=11)
        let searchNumButton = new Button(Text = "Search", Top = 20, Left = 370, Width = 100)
  
        // Search by name
        let searchNameLabel = new Label(Text = "Search by Name:", AutoSize = true, Top = 45, Left = 20)
        let searchNameTextBox = new TextBox(Width = 210, Top = 45, Left = 150,MaxLength=11)
        let searchNameButton = new Button(Text = "Search", Top = 45, Left = 370, Width = 100)


    ////////////////////////////// action of search by number button
    
        searchNumButton.Click.Add(fun _ ->
            try
                let searchValue = searchNTextBox.Text
                if String.IsNullOrWhiteSpace(searchValue) then
                    MessageBox.Show("Please enter a number to search.") |> ignore
                else
                    let connectionString = "Server=localhost;Database=tester;User Id=root;Password=;"
                    use connection = new MySqlConnection(connectionString)
                    connection.Open()

                    let query = "SELECT name, number, email FROM data_of_a7 WHERE number = @number"
                    use command = new MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@number", Int32.Parse(searchValue)) |> ignore
                

                    use reader = command.ExecuteReader()

                    if reader.Read() then
                        nameTextBox.Text <- reader.GetString(0)
                        numberTextBox.Text <- reader.GetInt32(1).ToString()
                        emailTextBox.Text <- reader.GetString(2)
                    else
                        MessageBox.Show("this Number Not Exists") |> ignore

                    reader.Close()
                    use command = new MySqlDataAdapter(query, connection)
                    command.SelectCommand.Parameters.AddWithValue("@number", Int32.Parse(searchValue)) |> ignore
                    let table = new DataTable()
                    command.Fill(table) |> ignore

                    if table.Rows.Count > 0 then
                        dataGridView.DataSource <- table
            with
            | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
        )

    ////////////////////////////////// action of search by Name button
        searchNameButton.Click.Add(fun _ ->
            try
                let searchValue = searchNameTextBox.Text
                if String.IsNullOrWhiteSpace(searchValue) then
                    MessageBox.Show("Please enter The Name to search.") |> ignore
                else
                    let connectionString = "Server=localhost;Database=tester;User Id=root;Password=;"
                    use connection = new MySqlConnection(connectionString)
                    connection.Open()

                    let query = "SELECT name, number, email FROM data_of_a7 WHERE name = @name"
                    use command = new MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@name", searchValue) |> ignore

                    use reader = command.ExecuteReader()

                    if reader.Read() then
                        nameTextBox.Text <- reader.GetString(0)
                        numberTextBox.Text <- reader.GetInt32(1).ToString()
                        emailTextBox.Text <- reader.GetString(2)
                    else
                        MessageBox.Show("This Name Not Exists.") |> ignore
                    reader.Close()

                    use commandTable = new MySqlDataAdapter(query, connection)
                    commandTable.SelectCommand.Parameters.AddWithValue("@name", searchValue) |> ignore
                    let table = new DataTable()
                    commandTable.Fill(table) |> ignore

                    if table.Rows.Count > 0 then
                        dataGridView.DataSource <- table
                    reader.Close()
            with
            | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
        )
    
        //No2
        // Add button
        let addButton = new Button(Text = "Add", Top = 230, Left = 260, Width = 100)
        addButton.Click.Add(fun _ ->
            try
                let name = nameTextBox.Text
                let email = emailTextBox.Text
                let number = numberTextBox.Text

                if String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(number) then
                    MessageBox.Show("Please fill all fields.") |> ignore
                else
                    let connectionString = "Server=localhost;Database=tester;User Id=root;Password=;"
                    use connection = new MySqlConnection(connectionString)
                    connection.Open()

                    let query = "INSERT INTO data_of_a7 (name, number, email) VALUES (@name, @number, @email)"
                    use command = new MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@name", name) |> ignore
                    command.Parameters.AddWithValue("@number", Int32.Parse(number)) |> ignore
                    command.Parameters.AddWithValue("@email", email) |> ignore

                    let rowsAffected = command.ExecuteNonQuery()
                    if rowsAffected > 0 then
                        MessageBox.Show("Data added successfully! ") |> ignore
                        MessageBox.Show("Refresh the Table") |> ignore
                    else
                        MessageBox.Show("Failed to add data.") |> ignore
            with
            | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
        )

        //No3
        // Edit button
        let editButton = new Button(Text = "Edit", Top = 230, Left = 140, Width = 100)
        editButton.Click.Add(fun _ ->
            try
                let name = nameTextBox.Text
                let email = emailTextBox.Text
                let number = numberTextBox.Text

                if String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(number) then
                    MessageBox.Show("Please fill all fields to update.") |> ignore
                else
                    let connectionString = "Server=localhost;Database=tester;User Id=root;Password=;"
                    use connection = new MySqlConnection(connectionString)
                    connection.Open()

                    let query = "UPDATE data_of_a7 SET name = @name, email = @email WHERE number = @number"
                    use command = new MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@name", name) |> ignore
                    command.Parameters.AddWithValue("@number", Int32.Parse(number)) |> ignore
                    command.Parameters.AddWithValue("@email", email) |> ignore

                    let rowsAffected = command.ExecuteNonQuery()
                    if rowsAffected > 0 then
                        MessageBox.Show("Data updated successfully!") |> ignore
                        loadData() // Reload data after updating
                    else
                        MessageBox.Show("Failed to update data.") |> ignore
            with
            | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
        )
        //No4
        // Delete button
        let deleteButton = new Button(Text = "Delete", Top = 230, Left = 20, Width = 100)
        deleteButton.Click.Add(fun _ ->
            try
                let number = numberTextBox.Text

                if String.IsNullOrWhiteSpace(number) then
                    MessageBox.Show("Please enter a number to delete.") |> ignore
                else
                    let connectionString = "Server=localhost;Database=tester;User Id=root;Password=;"
                    use connection = new MySqlConnection(connectionString)
                    connection.Open()

                    let query = "DELETE FROM data_of_a7 WHERE number = @number"
                    use command = new MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@number", Int32.Parse(number)) |> ignore

                    let rowsAffected = command.ExecuteNonQuery()
                    nameTextBox.Text <- ""
                    numberTextBox.Text <- ""
                    emailTextBox.Text <- ""
                    searchNTextBox.Text <- ""
                    searchNameTextBox.Text <- ""

                    if rowsAffected > 0 then
                        MessageBox.Show("Data deleted successfully!") |> ignore
                        loadData() // Reload data after deleting
                    else
                        MessageBox.Show("Failed to delete data.") |> ignore
            with
            | ex -> MessageBox.Show($"Error: {ex.Message}") |> ignore
        )

        // Clear button
        let clearButton = new Button(Text = "Clear", Top = 110, Left = 370, Width = 100)
        clearButton.Click.Add(fun _ ->
            nameTextBox.Text <- ""
            numberTextBox.Text <- ""
            emailTextBox.Text <- ""
        )

        // Add controls to the form
        form.Controls.AddRange(
            [| dataGridView; refreshButton;
               searchNLabel; searchNTextBox; searchNumButton;
               searchNameLabel; searchNameTextBox; searchNameButton;
               nameLabel; nameTextBox;
               numberLabel; numberTextBox;
               emailLabel; emailTextBox;
               addButton; editButton; deleteButton; clearButton |]
        )

        // Initial data load
        loadData()

        // Run application
        Application.Run(form)
        0
